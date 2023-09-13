// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ScanCollector
{
    internal interface IOptions
    {
        string OutputDirectory { get; }
        string ProcessName { get; }
        uint MillisecondsBetweenScans { get; }
        bool OverwriteOutputDirectory { get; }
    }
}
