// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Actions.Attributes;
using Axe.Windows.Actions.Enums;
using Axe.Windows.Desktop.Utility;
using Axe.Windows.Telemetry;
using System;
using System.Drawing;

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
            // For now, just return a yellow background
            canvas.DrawColor(new SkiaSharp.SKColor(255, 255, 0));
        }
    }
}
