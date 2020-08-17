using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Metrotrainer
{
    public static class MaskedTextBoxExtension
    {
        public static int? CoerceTime(this MaskedTextBox me)
        {
            var minSecond = me.Text.Split(':');
            if (minSecond[0].InvalidString() || minSecond[1].InvalidString())return null;
            
            int minutes = int.Parse(minSecond[0]);
            int seconds = int.Parse(minSecond[1]);

            if (minutes >= 60 || seconds >= 60) return null;


            int totalSeconds = minutes * 60 + seconds;
            
            return totalSeconds;
        }
    }
}
