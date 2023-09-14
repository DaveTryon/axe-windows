// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ScanCommon.A11yTestComparison
{
    public class FileNameAndUniqueId
    {
        public string FileName { get; }
        public int UniqueId { get; }

        public FileNameAndUniqueId(string fileName, int uniqueId)
        {
            FileName = fileName;
            UniqueId = uniqueId;
        }

        public override string ToString()
        {
            return $"{FileName} ({UniqueId})";
        }
    }
}
