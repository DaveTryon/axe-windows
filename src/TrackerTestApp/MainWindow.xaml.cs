// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Axe.Windows.Actions.Enums;
using Axe.Windows.Actions.Trackers;
using Axe.Windows.Core.Bases;
using Axe.Windows.Desktop.UIAutomation;
using Axe.Windows.Desktop.UIAutomation.TreeWalkers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;

namespace TrackerTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly object _lockObject = new object();
        private FocusTracker _focusTracker;
        private MouseTracker _mouseTracker;
        private bool _showUpdates = true;

        public MainWindow()
        {
            InitializeComponent();
            _focusTracker = new FocusTracker(OnElementSelectedByFocus)
            {
                Scope = SelectionScope.Element
            };
            _mouseTracker = new MouseTracker(OnElementSelectedByMouse)
            {
                Scope = SelectionScope.Element
            };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            lock (_lockObject)
            {
                _showUpdates = false;
                _mouseTracker.Stop();
                _mouseTracker.Dispose();
                _focusTracker.Stop();
                _focusTracker.Dispose();
                base.OnClosing(e);
            }
        }

        private void OnElementSelectedByMouse(A11yElement element)
        {
            OnElementSelected(element, "MOUSE");
        }

        private void OnElementSelectedByFocus(A11yElement element)
        {
            OnElementSelected(element, "FOCUS");
        }

        private void OnElementSelected(A11yElement element, string howSelected)
        {
            List<string> lines = new List<string>();

            lock (_lockObject)
            {
                DesktopElementAncestry anc = new DesktopElementAncestry(Axe.Windows.Core.Enums.TreeViewMode.Control, element, true);

                DesktopElement? de = element as DesktopElement;

                if (de == null)
                    return;

                Rectangle rc = de.BoundingRectangle;
                lines.Add($"Selected by {howSelected}");
                lines.Add($"name = {de.Name}");
                lines.Add($"bounding rectangle = {rc.Left}, {rc.Top}, {rc.Right}, {rc.Bottom}");
                lines.Add($"control type = {de.ControlTypeId}");
                lines.Add($"runtime id = {de.RuntimeId}");
                lines.Add($"automation id = {de.AutomationId}");
                lines.Add($"class name = {de.ClassName}");
                lines.Add($"process name = {de.ProcessName}");
                lines.Add($"isKeyboardFocusable = {de.IsKeyboardFocusable}");
                lines.Add($"ancestry levels = {anc.Items.Count}");

                int indent = 2;
                foreach (var i in anc.Items)
                {
                    string indentString = new string(' ', indent);
                    lines.Add($"{indentString}parent name = {i.Name}");
                    lines.Add($"{indentString}parent Automation ID = {i.AutomationId}");
                    lines.Add($"{indentString}parent Runtime ID = {i.RuntimeId}");
                    rc = de.BoundingRectangle;
                    lines.Add($"{indentString}parent bounding rectangle = {rc.Left}, {rc.Top}, {rc.Right}, {rc.Bottom}");
                    indent += 2;
                }
                lines.Add($"--------------------------------");
            }
            DispatchInsertSelectedElementInfo(lines);
        }

        private void DispatchInsertSelectedElementInfo(IEnumerable<string> lines)
        {
            Dispatcher.Invoke(() => InsertSelectedElementInfo(lines));
        }

        private void InsertSelectedElementInfo(IEnumerable<string> lines)
        {
            foreach (string line in lines)
            {
                if (_showUpdates)
                {
                    _elementList.Items.Add(line);
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            lock (_lockObject)
            {
                _elementList.Items.Clear();
            }
        }

        private void StartMouseTracking(object sender, RoutedEventArgs e)
        {
            _mouseTracker?.Start();
        }

        private void StopMouseTracking(object sender, RoutedEventArgs e)
        {
            _mouseTracker?.Start();
        }

        private void StartFocusTracking(object sender, RoutedEventArgs e)
        {
            _focusTracker?.Start();
        }

        private void StopFocusTracking(object sender, RoutedEventArgs e)
        {
            _focusTracker?.Start();
        }
    }
}
