// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;

namespace ScanCommon.A11yTestComparison
{
    public class A11yTestFileContent
    {
        public string ElementTree { get; }
        public string Screenshot { get; }

        public A11yTestFileContent(string elementTree, string screenshot)
        {
            ElementTree = elementTree;
            Screenshot = screenshot;
        }

        public DiffFlags Diff(A11yTestFileContent other)
        {
            DiffFlags diff = DiffFlags.Identical;

            if (ElementTree != other.ElementTree)
            {
                diff |= DiffFlags.DifferenElementTree;
            }

            if (Screenshot != other.Screenshot)
            {
                diff |= DiffFlags.DifferentScreenshot;
            }

            return diff;
        }

        public static A11yTestFileContent Create(string a11yTestFile)
        {
            using (FileStream str = File.Open(a11yTestFile, FileMode.Open, FileAccess.Read))
            using (Package package = ZipPackage.Open(str, FileMode.Open, FileAccess.Read))
            {
                string elementTree;
                string screenshot;
                var parts = package.GetParts();
                using (Stream elementsPart = (from p in parts where p.Uri.OriginalString == "/el.snapshot" select p.GetStream()).First())
                using (StreamReader reader = new StreamReader(elementsPart))
                {
                    elementTree = reader.ReadToEnd();
                }

                using (Stream screenshotPart = (from p in parts where p.Uri.OriginalString == "/scshot.png" select p.GetStream()).First())
                using (BinaryReader reader = new BinaryReader(screenshotPart))
                {
                    byte[] screenshotArray = reader.ReadBytes((int)screenshotPart.Length);
                    screenshot = Convert.ToBase64String(screenshotArray);
                }

                return new A11yTestFileContent(elementTree, screenshot);
            }
        }
    }
}
