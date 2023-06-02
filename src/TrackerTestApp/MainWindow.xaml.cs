using Axe.Windows.Actions.Enums;
using Axe.Windows.Actions.Trackers;
using Axe.Windows.Core.Bases;
using Axe.Windows.Desktop.UIAutomation;
using Axe.Windows.Desktop.UIAutomation.TreeWalkers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace TrackerTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly object _lockObject = new object();
        private FocusTracker? _focusTracker = null;
        private bool _showUpdates = true;

        public MainWindow()
        {
            InitializeComponent();
            StartTracker();
        }

        private void StartTracker()
        {
            _focusTracker = new FocusTracker(OnElementSelected)
            {
                Scope = SelectionScope.Element
            };
            _focusTracker.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            lock (_lockObject)
            {
                _showUpdates = false;
                _focusTracker?.Stop();
                _focusTracker?.Dispose();
                base.OnClosing(e);
            }
        }

        private void OnElementSelected(A11yElement element)
        {
            List<string> lines = new List<string>();

            lock (_lockObject)
            {
                DesktopElementAncestry anc = new DesktopElementAncestry(Axe.Windows.Core.Enums.TreeViewMode.Control, element, true);

                DesktopElement? de = element as DesktopElement;
                lines.Add($"name = {de.Name}");
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
    }
}
