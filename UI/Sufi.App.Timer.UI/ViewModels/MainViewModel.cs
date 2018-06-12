using Sufi.App.Timer.UI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Sufi.App.Timer.UI.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly DispatcherTimer _shutdownTimer;
        private readonly DispatcherTimer _countdownTimer;

        private string _selectedInterval;
        private string _status;
        private DateTime _shutdownDate;

        public MainViewModel()
        {
            _shutdownTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _shutdownTimer.Tick += (sender, e) => RunShutdownCommand();

            _countdownTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _countdownTimer.Tick += (sender, e) => UpdateStatusText();
            _countdownTimer.Interval = TimeSpan.FromMilliseconds(500);

            // Default value.
            Status = "Shutdown timer stopped.";
        }

        public IEnumerable<string> Options { get; } = new List<string>() { "30 seconds", "1 minute", "5 minutes", "1 hour" };

        public string SelectedInterval
        {
            get { return _selectedInterval; }
            set
            {
                _selectedInterval = value;
                RaisePropertyChangedEvent("SelectedInterval");
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChangedEvent("Status");
            }
        }

        public ICommand StartButtonCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    // Convert minute into second.
                    double minute = Convert.ToDouble(SelectedInterval.Split(' ')[0]);

                    // Calculate shutdown time.
                    _shutdownDate = DateTime.Now.AddMinutes(minute);

                    _shutdownTimer.Interval = TimeSpan.FromMinutes(minute);
                    _shutdownTimer.Start();

                    _countdownTimer.Start();

                    Status = $"System will shutdown in {minute} minute(s)...";
                }, 
                () => 
                {
                    // Disable the button whenever the timer is active.
                    return !_shutdownTimer.IsEnabled;
                });
            }
        }

        public ICommand StopButtonCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    // Enable the button.
                    //btnStart.IsEnabled = true;

                    // Stop all timer.
                    _countdownTimer.Stop();
                    _shutdownTimer.Stop();

                    // Update the status text.
                    Status = $"Shutdown timer stopped.";
                },
                () =>
                {
                    return true;
                });
            }
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

        private void UpdateStatusText()
        {
            // Calculate remaining minutes.
            var timeSpan = _shutdownDate.Subtract(DateTime.Now);

            // Update the status text.
            Status = $"System will shutdown in {timeSpan.Minutes} minute(s) and {timeSpan.Seconds} second(s)...";
        }
    }
}
