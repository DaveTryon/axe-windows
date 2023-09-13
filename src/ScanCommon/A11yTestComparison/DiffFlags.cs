// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace ScanCommon.A11yTestComparison
{
    [Flags]
    public enum DiffFlags
    {
        Identical = 0,
        DifferenElementTree = 1,
        DifferentScreenshot = 2,

        All = 3,
    }
}
