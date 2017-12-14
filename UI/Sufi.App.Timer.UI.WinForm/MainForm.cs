using Sufi.App.Timer.UI.WinForm.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sufi.App.Timer.UI.WinForm
{
    public partial class MainForm : Form
    {
        private DateTime _shutdownDate;

        private const string ShutdownTimerStopped = "Shutdown timer stopped.";

        public MainForm()
        {
            InitializeComponent();

            lblStatus.Text = ShutdownTimerStopped;

            var timeList = new List<TimeSelectionModel>
            {
                new TimeSelectionModel{ Title = "10 seconds", Interval = 10000 },
                new TimeSelectionModel{ Title = "5 minutes", Interval = 300000 },
                new TimeSelectionModel{ Title = "15 minutes", Interval = 900000 },
                new TimeSelectionModel{ Title = "30 minutes", Interval = 1800000 },
                new TimeSelectionModel{ Title = "1 hour", Interval = 3600000 },
                new TimeSelectionModel{ Title = "2 hours", Interval = 7200000 },
            };

            cmbTime.Items.AddRange(timeList.ToArray());
        }

        #region Events
        private void OnButtonStartClicked(object sender, EventArgs e)
        {
            if (cmbTime.SelectedIndex < 0)
                return;
            
            timerStatus.Start();

            // Disable this button.
            btnStart.Enabled = false;

            // Enable the 'Stop' button.
            btnStop.Enabled = true;

            // Get selected item in the combo box.
            var selectedItem = cmbTime.SelectedItem as TimeSelectionModel;
            var timeSpan = TimeSpan.FromMilliseconds(selectedItem.Interval);

            // Calculate shutdown time.
            _shutdownDate = DateTime.Now.AddMilliseconds(selectedItem.Interval);

            timerShutdown.Interval = selectedItem.Interval;
            timerShutdown.Start();
        }

        private void OnButtonStopClicked(object sender, EventArgs e)
        {
            // Enable the 'Start' button.
            btnStart.Enabled = true;

            // Disable the 'Stop' button.
            btnStop.Enabled = false;

            // Stop all timer.
            timerStatus.Stop();
            timerShutdown.Stop();

            // Update the status text.
            lblStatus.Text = ShutdownTimerStopped;
        }

        private void OnTimerStatusTick(object sender, EventArgs e)
        {
            UpdateStatusText();
        }

        private void OnTimerShutdownTick(object sender, EventArgs e)
        {
            RunShutdownCommand();
        }

        private void OnMainFormResized(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void OnMainNotifyIconDoubleClicked(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
        }
        #endregion

        private void RunShutdownCommand()
        {
            // Stop all timers.
            timerStatus.Stop();
            timerShutdown.Stop();

            // Start the shutdown process.
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("shutdown", "/s /t 7200")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                },
            };

            Task.Run(() => process.Start());

            // Exit this application.
            Close();
        }

        private void UpdateStatusText()
        {
            // Calculate remaining minutes.
            var timeSpan = _shutdownDate.Subtract(DateTime.Now);

            var builder = new StringBuilder("System will shutdown in");

            if (timeSpan.Hours > 0)
            {
                builder.Append($" {timeSpan.Hours} hour(s)");
            }

            if (timeSpan.Minutes > 0)
            {
                builder.Append($" {timeSpan.Minutes} minute(s)");
            }

            if (timeSpan.Seconds > 0)
            {
                builder.Append($" {timeSpan.Seconds} second(s)");
            }

            builder.Append("...");

            // Update the status text.
            lblStatus.Text = builder.ToString();
        }
    }
}
