// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Attributes;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Types;
using System;
using UIAutomationClient;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Transform2 Control Pattern
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671291(v=vs.85).aspx
    /// </summary>
    public class TransformPattern2 : A11yPattern
    {
        IUIAutomationTransformPattern2 Pattern;

        public TransformPattern2(A11yElement e, IUIAutomationTransformPattern2 p) : base(e, PatternType.UIA_TransformPattern2Id)
        {
            Pattern = p;

            PopulateProperties();
        }

        private void PopulateProperties()
        {
#pragma warning disable CA2000 // Properties are disposed in A11yPattern.Dispose()
            this.Properties.Add(new A11yPatternProperty() { Name = "CanMove", Value = Convert.ToBoolean(this.Pattern.CurrentCanMove) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanResize", Value = Convert.ToBoolean(this.Pattern.CurrentCanResize) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanRotate", Value = Convert.ToBoolean(this.Pattern.CurrentCanRotate) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanZoom", Value = Convert.ToBoolean(this.Pattern.CurrentCanZoom) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanZoomLevel", Value = Convert.ToBoolean(this.Pattern.CurrentZoomLevel) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanZoomMaximum", Value = Convert.ToBoolean(this.Pattern.CurrentZoomMaximum) });
            this.Properties.Add(new A11yPatternProperty() { Name = "CanZoomMinimum", Value = Convert.ToBoolean(this.Pattern.CurrentZoomMinimum) });
#pragma warning restore CA2000 // Properties are disposed in A11yPattern.Dispose()
        }

        [PatternMethod(IsUIAction = true)]
        public void Zoom(double zoomValue)
        {
            this.Pattern.Zoom(zoomValue);
        }

        [PatternMethod(IsUIAction = true)]
        public void ZoomByUnit(ZoomUnit zu)
        {
            this.Pattern.ZoomByUnit(zu);
        }

        protected override void Dispose(bool disposing)
        {
            if (Pattern != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(Pattern);
                this.Pattern = null;
            }

            base.Dispose(disposing);
        }
    }
}
