using Sufi.App.Timer.UI.Framework;
using Sufi.App.Timer.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Sufi.App.Timer.UI.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly DispatcherTimer _shutdownTimer;
        private readonly DispatcherTimer _countdownTimer;

        private readonly DelegateCommand _startButtonCommand;

        private DurationModel _selectedInterval;
        private string _status;
        private DateTime _shutdownDate;

        public MainViewModel()
        {
            _shutdownTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _shutdownTimer.Tick += (sender, e) => RunShutdownCommand();

            _countdownTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _countdownTimer.Tick += (sender, e) => UpdateStatusText();
            _countdownTimer.Interval = TimeSpan.FromMilliseconds(1000);

            _startButtonCommand = new DelegateCommand((sender) =>
            {
                // Calculate shutdown time.
                _shutdownDate = DateTime.Now.Add(_selectedInterval.Duration);

                _shutdownTimer.Interval = _selectedInterval.Duration;
                _shutdownTimer.Start();

                _countdownTimer.Start();

                Status = $"Shutdown countdown started...";
            },
            () =>
            {
                // Disable the button whenever the timer is active.
                return !_shutdownTimer.IsEnabled && !string.IsNullOrEmpty(SelectedInterval);
            });

            // Default value.
            Status = "Shutdown timer stopped.";
        }

        public IEnumerable<DurationModel> Options { get; } = new List<DurationModel>()
        {
            new DurationModel("30 seconds", TimeSpan.FromSeconds(30)),
            new DurationModel("1 minute", TimeSpan.FromMinutes(1)),
            new DurationModel("5 minutes", TimeSpan.FromMinutes(5)),
            new DurationModel("1 hour", TimeSpan.FromHours(1)),
        };

        public string SelectedInterval
        {
            get { return _selectedInterval?.Title; }
            set
            {
                _selectedInterval = Options.First(x => x.Title == value);
                RaisePropertyChangedEvent("SelectedInterval");
                _startButtonCommand.RaiseCanExecuteChanged();
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

        public ICommand StartButtonCommand => _startButtonCommand;

        public ICommand StopButtonCommand
        {
            get
            {
                return new DelegateCommand((sender) =>
                {
                    // Stop all timer.
                    _countdownTimer.Stop();
                    _shutdownTimer.Stop();

                    // Update the status text.
                    Status = $"Shutdown timer stopped.";
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

            // Exit this application.
            Application.Current.Shutdown();
        }

        private void UpdateStatusText()
        {
            // Calculate remaining minutes.
            var timeSpan = _shutdownDate.Subtract(DateTime.Now);

            // Update the status text.
            Status = $"System will shutdown in {timeSpan.Hours.ToString().PadLeft(2, '0')}:{timeSpan.Minutes.ToString().PadLeft(2, '0')}:{timeSpan.Seconds.ToString().PadLeft(2, '0')}...";
        }
    }
}
