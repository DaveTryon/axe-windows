// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Win32;
using System.Drawing;

namespace Axe.Windows.Actions
{
    internal static class PrivacyExtensions
    {
        /// <summary>
        /// Given the root to an element tree, synthesize a "yellow box" bitmap to describe it
        /// </summary>
        /// <param name="rootElement">The root element for the tree</param>
        /// <returns>A bitmap representing the element tee (normalized to having the app window at location (0,0)</returns>
        internal static SkiaSharp.SKBitmap SynthesizeBitmapFromElements(this IA11yElement rootElement)
        {
            const int penWidth = 4;  // This is the same width as our highlighter

            if (rootElement == null) return null;

            Rectangle rootBoundingRectangle = rootElement.BoundingRectangle;

            // Synthesize the bitmap
            bool isHighContrast = HighContrast.Create().IsOn;
            Color brushColor = isHighContrast ? SystemColors.Window : Color.DarkGray;
            Color penColor = isHighContrast ? SystemColors.WindowFrame : Color.Yellow;
            SkiaSharp.SKBitmap bitmap = new SkiaSharp.SKBitmap(rootBoundingRectangle.Width, rootBoundingRectangle.Height);

            SkiaSharp.SKPaint pen = new SkiaSharp.SKPaint
            {
                Color = GetSKColor(penColor),
                StrokeWidth = penWidth,
                StrokeCap = SkiaSharp.SKStrokeCap.Butt,
            };

            using (SkiaSharp.SKCanvas canvas = new SkiaSharp.SKCanvas(bitmap))
            {
                canvas.DrawColor(GetSKColor(brushColor));
                canvas.Translate(-rootBoundingRectangle.Left, -rootBoundingRectangle.Top);
                AddBoundingRectsRecursively(canvas, pen, rootElement);
            }

            return bitmap;
        }

        /// <summary>
        /// Add the bounding rect for this element to the Graphics surface
        /// </summary>
        /// <param name="canvas">Canvas to manipulate</param>
        /// <param name="skPaint">The SKPaint to use (cached for performance)</param>
        /// <param name="element">The element to add--its children will also be added</param>
        private static void AddBoundingRectsRecursively(SkiaSharp.SKCanvas canvas, SkiaSharp.SKPaint skPaint, IA11yElement element)
        {
            canvas.DrawRect(element.BoundingRectangle.X, element.BoundingRectangle.Y,
                element.BoundingRectangle.Width, element.BoundingRectangle.Height,
                skPaint);

            if (element.Children != null)
            {
                foreach (var child in element.Children)
                {
                    AddBoundingRectsRecursively(canvas, skPaint, child);
                }
            }
        }

        private static SkiaSharp.SKColor GetSKColor(System.Drawing.Color color)
        {
            return new SkiaSharp.SKColor(color.R, color.G, color.B);
        }
    }
}
