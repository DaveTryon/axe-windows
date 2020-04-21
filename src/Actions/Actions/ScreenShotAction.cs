// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Actions.Attributes;
using Axe.Windows.Actions.Enums;
using Axe.Windows.Desktop.Utility;
using Axe.Windows.Telemetry;
using Axe.Windows.Win32;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Axe.Windows.Actions
{
    /// <summary>
    /// Class SelectionAction
    /// this class is to select unelement via focus or keyboard
    /// </summary>
    [InteractionLevel(UxInteractionLevel.NoUxInteraction)]
    public static class ScreenShotAction
    {
        // unit test hooks
        internal static Func<DataManager> GetDataManager = () => DataManager.GetDefaultInstance();
        internal static Func<int, int, SkiaSharp.SKBitmap> CreateBitmap = (width, height) => new SkiaSharp.SKBitmap(width, height);
        internal static readonly Action<SkiaSharp.SKCanvas, int, int, Size> DefaultCopyFromScreen = TempCopyFromScreen;
        internal static Action<SkiaSharp.SKCanvas, int, int, Size> CopyFromScreen = DefaultCopyFromScreen;

        /// <summary>
        /// Take a screenshot of the given element's parent window, if it has one
        ///     returns null if the bounding rectangle is 0-sized
        /// </summary>
        /// <param name="ecId">Element Context Id</param>
        public static void CaptureScreenShot(Guid ecId)
        {
            try
            {
                var ec = GetDataManager().GetElementContext(ecId);
                var el = ec.Element;

                var win = el.GetParentWindow();
                el = win ?? el;
                var rect = el.BoundingRectangle;
                if (rect.IsEmpty)
                {
                    return; // no capture. 
                }

                SkiaSharp.SKBitmap bmp = CreateBitmap(rect.Width, rect.Height);
                using (SkiaSharp.SKCanvas canvas = new SkiaSharp.SKCanvas(bmp))
                {
                    CopyFromScreen(canvas, rect.X, rect.Y, rect.Size);
                }
                ec.DataContext.Screenshot = bmp;
                ec.DataContext.ScreenshotElementId = el.UniqueId;
            }
            catch(TypeInitializationException e)
            {
                e.ReportException();
                // silently ignore. since it happens only on WCOS.
                // in this case, the results file will be loaded with yellow box.
            }
        }

        private static void TempCopyFromScreen(SkiaSharp.SKCanvas canvas, int x, int y, Size size)
        {
            IntPtr sourceDC = IntPtr.Zero;
            IntPtr targetDC = IntPtr.Zero;
            IntPtr compatibleBitmapHandle = IntPtr.Zero;

            int width = size.Width;
            int height = size.Height;
            int yText = 10;

            // create the final SkiaSharp image
            SkiaSharp.SKImage image = SkiaSharp.SKImage.Create(new SkiaSharp.SKImageInfo(width, height));
            SkiaSharp.SKPixmap pixmap = image.PeekPixels();

            try
            {
                DrawText(canvas, "Getting Source DC", ref yText);
                // gets the main desktop and all open windows
                sourceDC = NativeMethods.GetDC(NativeMethods.GetDesktopWindow());
                DrawText(canvas, string.Format("Source DC = {0}", sourceDC), ref yText);
                DrawText(canvas, "Getting Target DC", ref yText);
                targetDC = NativeMethods.CreateCompatibleDC(sourceDC);
                DrawText(canvas, string.Format("Target DC = {0}", targetDC), ref yText);

                // create a bitmap compatible with our target DC
                DrawText(canvas, "Creating Compatible Bitmap", ref yText);
                compatibleBitmapHandle = NativeMethods.CreateCompatibleBitmap(sourceDC, width, height);
                DrawText(canvas, string.Format("Compatible Bitmap Handle = {0}", compatibleBitmapHandle), ref yText);

                // gets the bitmap into the target device context
                DrawText(canvas, "Selecting the Bitmap", ref yText);
                var oldBmp = NativeMethods.SelectObject(targetDC, compatibleBitmapHandle);
                DrawText(canvas, string.Format("oldBmp = {0}", oldBmp), ref yText);

                // copy from source to destination
                DrawText(canvas, "Calling BitBlt", ref yText);
                var b = NativeMethods.BitBlt(targetDC, 0, 0, width, height, sourceDC, x, y, Win32.TernaryRasterOperations.SRCCOPY);
                DrawText(canvas, string.Format("BitBlt returned = {0}", b), ref yText);

                // create the info structure
                BITMAPINFOHEADER bmi = new BITMAPINFOHEADER
                {
                    biPlanes = 1,
                    biBitCount = 32,
                    biWidth = width,
                    biHeight = -height,
                    biCompression = BitmapCompressionMode.BI_RGB,
                };

                // read the raw pixels into the pixmap for the image
                DrawText(canvas, "Calling GetDIBits", ref yText);
                var t = NativeMethods.GetDIBits(targetDC, compatibleBitmapHandle, 0, height, pixmap.GetPixels(), bmi, Win32.DIB_Color_Mode.DIB_RGB_COLORS);
                DrawText(canvas, string.Format("GetDIBits returned = {0}", t), ref yText);

                int xMiddle = x + width / 2;
                int yMiddle = y + height / 2;
                DrawText(canvas, string.Format("Calling GetPixel({0},{1})", xMiddle, yMiddle), ref yText);
                var z = NativeMethods.GetPixel(sourceDC, xMiddle, yMiddle);
                DrawText(canvas, string.Format("GetPixel returned = {0}", z), ref yText);

                DrawText(canvas, "Calling GetDeviceCaps", ref yText);
                var z2 = NativeMethods.GetDeviceCaps(sourceDC, Win32Constants.RASTERCAPS);
                DrawText(canvas, string.Format("GetDeviceCaps returned = {0}", z2), ref yText);

                DrawText(canvas, "Calling DrawImage", ref yText);
                canvas.DrawImage(image, 0, 0);
            }
            catch (Exception e)
            {
                string data = e.ToString();
                DrawText(canvas, data, ref yText);
            }
            finally
            {
                NativeMethods.DeleteObject(compatibleBitmapHandle);

                NativeMethods.ReleaseDC(IntPtr.Zero, sourceDC);
                NativeMethods.ReleaseDC(IntPtr.Zero, targetDC);
            }
        }

        private static void DrawText(SkiaSharp.SKCanvas canvas, string text, ref int y)
        {
            SkiaSharp.SKPoint point = new SkiaSharp.SKPoint(0, y);
            canvas.DrawText(text, point, new SkiaSharp.SKPaint { Color = new SkiaSharp.SKColor(0,0,0)});
            y += 20;
        }
    }
}
