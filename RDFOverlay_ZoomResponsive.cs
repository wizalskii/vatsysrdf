using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using vatsys;

namespace VatsysRDF
{
    public class RDFOverlay : Form
    {
        private readonly RDFSettings settings;
        private readonly ConcurrentDictionary<string, DateTime> highlightedAircraft;
        private readonly ConcurrentDictionary<string, Coordinate> aircraftPositions;

        public RDFOverlay(RDFSettings settings,
            ConcurrentDictionary<string, DateTime> highlightedAircraft,
            ConcurrentDictionary<string, Coordinate> aircraftPositions)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.highlightedAircraft = highlightedAircraft ?? throw new ArgumentNullException(nameof(highlightedAircraft));
            this.aircraftPositions = aircraftPositions ?? throw new ArgumentNullException(nameof(aircraftPositions));

            // Configure the form
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            BackColor = Color.Magenta; // Transparency key
            TransparencyKey = Color.Magenta;
            Opacity = 1.0;
            StartPosition = FormStartPosition.Manual;

            // Enable double buffering to reduce flicker
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT - makes window click-through
                cp.ExStyle |= 0x00080000; // WS_EX_LAYERED - required for transparency
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!settings.Enabled)
                return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (highlightedAircraft.Count == 0)
                return;

            // Determine color based on number of concurrent transmissions
            Color penColor = highlightedAircraft.Count > 1 ? settings.ConcurrentTxColor : settings.SingleTxColor;

            using (var pen = new Pen(penColor, settings.PenWidth))
            {
                foreach (var kvp in highlightedAircraft)
                {
                    string callsign = kvp.Key;
                    if (aircraftPositions.TryGetValue(callsign, out Coordinate coord))
                    {
                        DrawTransmissionCircle(g, pen, coord);
                    }
                }
            }
        }

        private void DrawTransmissionCircle(Graphics g, Pen pen, Coordinate coord)
        {
            try
            {
                // Get main ASD window for coordinate conversion
                if (MMI.FPASD == null)
                    return;

                // Convert coordinate to screen position
                Point screenPos = MMI.FPASD.GetPaintPoint(coord);

                // Calculate zoom-responsive radius
                float radius = CalculateZoomResponsiveRadius(coord);

                float x = screenPos.X - radius;
                float y = screenPos.Y - radius;
                float diameter = radius * 2;

                g.DrawEllipse(pen, x, y, diameter, diameter);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error drawing circle: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculate radius in pixels based on current zoom level
        /// The radius scales so it represents a constant distance (in nautical miles) on the map
        /// </summary>
        private float CalculateZoomResponsiveRadius(Coordinate centerCoord)
        {
            try
            {
                if (MMI.FPASD == null)
                    return settings.CircleRadius; // Fallback to fixed radius

                // Define a radius in nautical miles (configurable)
                double radiusNM = 5.0; // 5 NM ring

                // Create a second point offset by the radius in NM
                // For simplicity, offset in latitude (1 degree latitude ≈ 60 NM)
                double offsetDegrees = radiusNM / 60.0;
                Coordinate offsetCoord = new Coordinate(
                    centerCoord.LatitudeDegrees + offsetDegrees,
                    centerCoord.LongitudeDegrees
                );

                // Convert both points to screen coordinates
                Point centerScreen = MMI.FPASD.GetPaintPoint(centerCoord);
                Point offsetScreen = MMI.FPASD.GetPaintPoint(offsetCoord);

                // Calculate screen distance (this is the zoom-responsive radius)
                double dx = offsetScreen.X - centerScreen.X;
                double dy = offsetScreen.Y - centerScreen.Y;
                double radiusPixels = Math.Sqrt(dx * dx + dy * dy);

                // Clamp radius to reasonable bounds (min 10px, max 200px)
                radiusPixels = Math.Max(10, Math.Min(200, radiusPixels));

                return (float)radiusPixels;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error calculating zoom radius: {ex}");
                return settings.CircleRadius; // Fallback
            }
        }

        public void UpdatePosition(Rectangle bounds)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Rectangle>(UpdatePosition), bounds);
                return;
            }

            Location = new Point(bounds.X, bounds.Y);
            Size = new Size(bounds.Width, bounds.Height);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
