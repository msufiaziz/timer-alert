using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufi.App.Timer.UI.WinForm.Models
{
    class TimeSelectionModel
    {
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the interval value (in miliseconds).
        /// </summary>
        public int Interval { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
