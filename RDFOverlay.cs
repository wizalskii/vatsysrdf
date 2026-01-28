using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VatsysRDF
{
    public class RDFOverlay : Form
    {
        private readonly TransmissionTracker tracker;
        private readonly RDFSettings settings;
        private readonly Timer refreshTimer;

        public RDFOverlay(TransmissionTracker tracker, RDFSettings settings)
        {
            this.tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

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

            // Set up refresh timer
            refreshTimer = new Timer
            {
                Interval = 100 // 100ms = 10 FPS
            };
            refreshTimer.Tick += (s, e) => Invalidate();
            refreshTimer.Start();
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

            var transmissions = tracker.GetActiveTransmissions();
            if (transmissions.Count == 0)
                return;

            // Determine color based on number of concurrent transmissions
            Color penColor = transmissions.Count > 1 ? settings.ConcurrentTxColor : settings.SingleTxColor;

            using (var pen = new Pen(penColor, settings.PenWidth))
            {
                foreach (var tx in transmissions)
                {
                    DrawTransmissionCircle(g, pen, tx);
                }
            }
        }

        private void DrawTransmissionCircle(Graphics g, Pen pen, TransmissionData tx)
        {
            if (tx.ScreenPosition.IsEmpty)
                return;

            try
            {
                float radius = settings.CircleRadius;
                float x = tx.ScreenPosition.X - radius;
                float y = tx.ScreenPosition.Y - radius;
                float diameter = radius * 2;

                g.DrawEllipse(pen, x, y, diameter, diameter);

                // Optionally draw crosshair at center
                //float crossSize = 5;
                //g.DrawLine(pen, tx.ScreenPosition.X - crossSize, tx.ScreenPosition.Y, tx.ScreenPosition.X + crossSize, tx.ScreenPosition.Y);
                //g.DrawLine(pen, tx.ScreenPosition.X, tx.ScreenPosition.Y - crossSize, tx.ScreenPosition.X, tx.ScreenPosition.Y + crossSize);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error drawing circle: {ex.Message}");
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
            if (disposing)
            {
                refreshTimer?.Stop();
                refreshTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
