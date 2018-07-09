using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufi.App.Timer.UI.Models
{
    public class DurationModel
    {
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }

        public DurationModel(string title, TimeSpan duration)
        {
            Title = title;
            Duration = duration;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
