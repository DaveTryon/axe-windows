// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CommandLine;

namespace ScanCommon
{
    public static class CaseInsensitiveParser
    {
        public static Parser Create()
        {
            // CommandLineParser is case-sensitive by default (intentional choice by the code
            // owners for better compatibility with *nix platforms). This removes the case
            // sensitivity
            return new Parser((settings) =>
            {
                settings.CaseSensitive = false;
            });
        }
    }
}
