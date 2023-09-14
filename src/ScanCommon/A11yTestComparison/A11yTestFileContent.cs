// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Axe.Windows.Desktop.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;

namespace ScanCommon.A11yTestComparison
{
    public class A11yTestFileContent
    {
        private const int BufferSize = 0x1000;

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
                PackagePartCollection parts = package.GetParts();
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

        public static void CopyFileWithUpdatedMetadata(string inputA11yTestFile, string outputA11yTestFile, int selectedId)
        {
            using (FileStream inputStream = File.Open(inputA11yTestFile, FileMode.Open, FileAccess.Read))
            using (Package inputPackage = ZipPackage.Open(inputStream, FileMode.Open, FileAccess.Read))
            {
                PackagePartCollection inputPackageParts = inputPackage.GetParts();

                using (FileStream outputStream = File.Open(outputA11yTestFile, FileMode.Create))
                using (Package outputPackage = ZipPackage.Open(outputStream, FileMode.Create))
                {
                    foreach (var inputPart in inputPackageParts)
                    {
                        CopyAdjustedPackagePart(inputPart, outputPackage, selectedId);
                    }
                }
            }
        }

        private static void CopyAdjustedPackagePart(PackagePart inputPart, Package outputPackage, int selectedId)
        {
            Uri partUri = inputPart.Uri;
            PackagePart outputPart = outputPackage.CreatePart(partUri, inputPart.ContentType, inputPart.CompressionOption);
            CopyStream(GetAdjustedInputStream(inputPart, selectedId), outputPart.GetStream());
        }

        private static Stream GetAdjustedInputStream(PackagePart inputPart, int selectedId)
        {
            if (inputPart.Uri.OriginalString == "/metadata.json")
            {
                using (Stream metadataPart = inputPart.GetStream())
                using (StreamReader reader = new StreamReader(metadataPart))
                {
                    string jsonMeta = reader.ReadToEnd();
                    SnapshotMetaInfo? metaInfo = JsonConvert.DeserializeObject<SnapshotMetaInfo>(jsonMeta);
                    metaInfo.Mode = A11yFileMode.Inspect;
                    metaInfo.SelectedItems = new List<int> { selectedId };
                    jsonMeta = JsonConvert.SerializeObject(metaInfo, Formatting.Indented);
                    return new MemoryStream(Encoding.UTF8.GetBytes(jsonMeta));
                }
            }
            else
            {
                return inputPart.GetStream();
            }
        }

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[BufferSize];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }   
    }
}
