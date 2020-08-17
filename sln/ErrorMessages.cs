using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Metrotrainer
{
    public class ErrorMessages
    {
        public void InvalidDuration()
        {
            MessageBox.Show("Invalid duration for this operation", "Duration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void InvalidBPM()
        {
            MessageBox.Show("Invalid BPM for this operation", "BPM Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void InvalidPlaylist()
        {
            MessageBox.Show("Invalid Playlist for this operation", "Playlist Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void InvalidIO()
        {
            MessageBox.Show("Invalid Input/Output for this operation", "IO Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }
}
