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
        public ErrorDictionary ErrorDictionary { get; }

        public A11yTestFileContent(string elementTree, ErrorDictionary errorDictionary, string screenshot)
        {
            ElementTree = elementTree;
            ErrorDictionary = errorDictionary;
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
                ErrorDictionary errorDictionary;
                var parts = package.GetParts();
                using (Stream elementsPart = (from p in parts where p.Uri.OriginalString == "/el.snapshot" select p.GetStream()).First())
                using (BinaryReader reader = new BinaryReader(elementsPart))
                {
                    byte[] elementsData = reader.ReadBytes((int)elementsPart.Length);

                    using (MemoryStream memoryStream = new MemoryStream(elementsData))
                    using (StreamReader textReader = new StreamReader(memoryStream))
                    {
                        elementTree = textReader.ReadToEnd();
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        errorDictionary = ErrorDictionary.CreateFromStream(memoryStream, a11yTestFile);
                    }
                }

                using (Stream screenshotPart = (from p in parts where p.Uri.OriginalString == "/scshot.png" select p.GetStream()).First())
                using (BinaryReader reader = new BinaryReader(screenshotPart))
                {
                    byte[] screenshotArray = reader.ReadBytes((int)screenshotPart.Length);
                    screenshot = Convert.ToBase64String(screenshotArray);
                }

                return new A11yTestFileContent(elementTree, errorDictionary, screenshot);
            }
        }
    }
}
