using System;
using System.Drawing;
using System.Runtime.InteropServices;
using vatsys;

namespace VatsysRDF
{
    public class PositionConverter
    {
        private IntPtr radarWindowHandle;
        private Rectangle radarBounds;

        // Radar display bounds (geographic)
        private Coordinate topLeft;
        private Coordinate bottomRight;
        private double pixelsPerDegreeLat;
        private double pixelsPerDegreeLon;

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public PositionConverter()
        {
            UpdateRadarWindow();
        }

        public void UpdateRadarWindow()
        {
            try
            {
                // Try to find the VATSYS window
                radarWindowHandle = FindWindow(null, "vatSys");

                if (radarWindowHandle != IntPtr.Zero)
                {
                    if (GetWindowRect(radarWindowHandle, out RECT rect))
                    {
                        radarBounds = new Rectangle(
                            rect.Left,
                            rect.Top,
                            rect.Right - rect.Left,
                            rect.Bottom - rect.Top
                        );
                    }
                }

                // TODO: Get actual radar display bounds from VATSYS
                // For now, we'll use placeholder values
                // These would need to be retrieved from the VATSYS radar display
                UpdateGeographicBounds();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error updating radar window: {ex.Message}");
            }
        }

        private void UpdateGeographicBounds()
        {
            // TODO: This needs to be updated with actual VATSYS radar bounds
            // For now, using placeholder - this will need integration with VATSYS display API

            // Placeholder: Sydney area
            topLeft = new Coordinate(-32.0, 150.0);
            bottomRight = new Coordinate(-35.0, 152.0);

            if (radarBounds.Width > 0 && radarBounds.Height > 0)
            {
                double latSpan = Math.Abs(bottomRight.Latitude - topLeft.Latitude);
                double lonSpan = Math.Abs(bottomRight.Longitude - topLeft.Longitude);

                pixelsPerDegreeLat = radarBounds.Height / latSpan;
                pixelsPerDegreeLon = radarBounds.Width / lonSpan;
            }
        }

        public PointF ConvertToScreen(Coordinate geo)
        {
            if (geo == null || radarBounds.Width == 0 || radarBounds.Height == 0)
                return PointF.Empty;

            try
            {
                // Convert latitude/longitude to pixel coordinates
                double latDiff = topLeft.Latitude - geo.Latitude;
                double lonDiff = geo.Longitude - topLeft.Longitude;

                float x = radarBounds.X + (float)(lonDiff * pixelsPerDegreeLon);
                float y = radarBounds.Y + (float)(latDiff * pixelsPerDegreeLat);

                return new PointF(x, y);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error converting position: {ex.Message}");
                return PointF.Empty;
            }
        }

        public Rectangle GetRadarBounds()
        {
            return radarBounds;
        }

        public bool IsValidPosition(PointF point)
        {
            return radarBounds.Contains((int)point.X, (int)point.Y);
        }
    }
}
