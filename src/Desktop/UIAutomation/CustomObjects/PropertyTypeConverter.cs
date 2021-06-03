// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Interop.UIAutomationCore;
using System;

namespace Axe.Windows.Desktop.UIAutomation.CustomObjects
{
    internal static class PropertyTypeConverter
    {
        public static UIAutomationType FromPropertyType(PropertyType propertyType)
        {
            switch (propertyType)
            {
                case PropertyType.uiaBool: return UIAutomationType.UIAutomationType_Bool;
                case PropertyType.uiaDouble: return UIAutomationType.UIAutomationType_Double;
                case PropertyType.uiaElement: return UIAutomationType.UIAutomationType_Element;
                case PropertyType.uiaInt: return UIAutomationType.UIAutomationType_Int;
                case PropertyType.uiaPoint: return UIAutomationType.UIAutomationType_OutPoint;
                case PropertyType.uiaString: return UIAutomationType.UIAutomationType_OutString;
            }

            throw new ArgumentException("Unrecognized property type", nameof(propertyType));
        }
    }
}
