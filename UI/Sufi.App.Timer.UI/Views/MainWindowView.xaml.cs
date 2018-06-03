using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Sufi.App.Timer.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : UserControl
    {
        private readonly DispatcherTimer _shutdownTimer;
        private readonly DispatcherTimer _countdownTimer;

        private DateTime _shutdownDate;

        public MainWindowView()
        {
            InitializeComponent();

            _shutdownTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _shutdownTimer.Tick += (sender, e) => RunShutdownCommand();

            _countdownTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _countdownTimer.Tick += (sender, e) => UpdateStatusText();
            _countdownTimer.Interval = TimeSpan.FromSeconds(1);
        }

        private void UpdateStatusText()
        {
            // Calculate remaining minutes.
            var timeSpan = _shutdownDate.Subtract(DateTime.Now);

            // Update the status text.
            textStatus.Text = $"System will shutdown in {timeSpan.Seconds} second(s)...";
        }

        private void OnButtonStartClicked(object sender, RoutedEventArgs e)
        {
            // Disable this button.
            var button = sender as Button;
            button.IsEnabled = false;

            // Convert minute into second.
            double minute = Convert.ToDouble(txtInput.Text);

            // Calculate shutdown time.
            _shutdownDate = DateTime.Now.AddMinutes(minute);

            _shutdownTimer.Interval = TimeSpan.FromMinutes(minute);
            _shutdownTimer.Start();

            _countdownTimer.Start();

            textStatus.Text = $"System will shutdown in {minute} minute(s)...";
        }

        private void RunShutdownCommand()
        {
            // Stop all timers.
            _countdownTimer.Stop();
            _shutdownTimer.Stop();

            // Start the shutdown process.
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("shutdown", "/s")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                },
            };

            Task.Run(() => process.Start());
        }

        private void OnButtonStopClicked(object sender, RoutedEventArgs e)
        {
            // Enable the button.
            btnStart.IsEnabled = true;

            // Stop all timer.
            _countdownTimer.Stop();
            _shutdownTimer.Stop();

            // Update the status text.
            textStatus.Text = $"Shutdown timer stopped.";
        }

        private void OnTextInputPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            var regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void OnTextInputPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                    e.CancelCommand();
            }
            else
                e.CancelCommand();
        }
    }
}
