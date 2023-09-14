// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ScanAnalyzer
{
    internal interface IOptions
    {
        string InputDirectory { get; }
        string OutputDirectory { get; }
        bool OverwriteOutputDirectory { get; }
    }
}
