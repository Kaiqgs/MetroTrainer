using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Metrotrainer
{
    public static class DialogResultExtension
    {
        public static bool Accept(this DialogResult me)
        {
            return me == DialogResult.OK || me == DialogResult.Yes;
        }
    }
}
