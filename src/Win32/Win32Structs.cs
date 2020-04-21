// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.InteropServices;

namespace Axe.Windows.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    struct HighContrastData
    {
        public UInt32 Size;
        public Int32 Flags;

        // changing the following type to string will cause .Net Core to crash during the unit tests
        public IntPtr DefaultScheme;
    } // struct

    [StructLayout(LayoutKind.Sequential)]
    public class BITMAPINFOHEADER
    {
        public int biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public BitmapCompressionMode biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader = new BITMAPINFOHEADER();
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public byte[] bmiColors;
    }
} // namespace
