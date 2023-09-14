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
        public ErrorAggregator ErrorAggregator { get; }

        public A11yTestFileContent(string elementTree, ErrorAggregator errorAggregator, string screenshot)
        {
            ElementTree = elementTree;
            ErrorAggregator = errorAggregator;
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
                ErrorAggregator errorDictionary;
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
                        errorDictionary = ErrorAggregator.CreateFromStream(memoryStream, a11yTestFile);
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

        //public static void CopyFileWithUpdatedMetadata(string inputA11yTestFile, string outputA11yTestFile, string runtimeIdToTarget)
        //{
        //    using (FileStream str = File.Open(inputA11yTestFile, FileMode.Open, FileAccess.Read))
        //    using (Package package = ZipPackage.Open(str, FileMode.Open, FileAccess.Read))
        //    {
        //        string elementTree;
        //        string screenshot;
        //        ErrorAggregator errorDictionary;
        //        var parts = package.GetParts();
        //        using (Stream elementsPart = (from p in parts where p.Uri.OriginalString == "/el.snapshot" select p.GetStream()).First())
        //        using (BinaryReader reader = new BinaryReader(elementsPart))
        //        {
        //            byte[] elementsData = reader.ReadBytes((int)elementsPart.Length);

        //            using (MemoryStream memoryStream = new MemoryStream(elementsData))
        //            using (StreamReader textReader = new StreamReader(memoryStream))
        //            {
        //                elementTree = textReader.ReadToEnd();
        //                memoryStream.Seek(0, SeekOrigin.Begin);
        //                errorDictionary = ErrorAggregator.CreateFromStream(memoryStream, a11yTestFile);
        //            }
        //        }

        //        using (Stream screenshotPart = (from p in parts where p.Uri.OriginalString == "/scshot.png" select p.GetStream()).First())
        //        using (BinaryReader reader = new BinaryReader(screenshotPart))
        //        {
        //            byte[] screenshotArray = reader.ReadBytes((int)screenshotPart.Length);
        //            screenshot = Convert.ToBase64String(screenshotArray);
        //        }

        //        return new A11yTestFileContent(elementTree, errorDictionary, screenshot);
        //    }

        //}
    }
}
