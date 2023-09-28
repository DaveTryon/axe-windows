// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace ScanAnalyzer
{
    internal class FileCollection
    {
        public IReadOnlyList<string> FileList { get; }

        private FileCollection(IReadOnlyList<string> fileList)
        {
            FileList = fileList;
        }

        public static FileCollection Create(string dataDirectory)
        {
            List<string> fileList = new List<string>();
            foreach (string file in Directory.EnumerateFiles(dataDirectory, "*.a11ytest"))
            {
                fileList.Add(file);
            }

            fileList.Sort(StringComparer.OrdinalIgnoreCase);

            return new FileCollection(fileList);
        }
    }
}
