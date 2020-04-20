// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Axe.Windows.Desktop.ColorContrastAnalyzer
{

    public class BitmapCollection : ImageCollection
    {
        private readonly SkiaSharp.SKBitmap bitmap;

        public BitmapCollection(SkiaSharp.SKBitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public override int NumColumns()
        {
            return bitmap.Width;
        }

        public override int NumRows()
        {
            return bitmap.Height;
        }

        public override Color GetColor(int row, int column)
        {
            SkiaSharp.SKColor color = bitmap.GetPixel(column, row);

            return new Color(color);
        }
    }
}
