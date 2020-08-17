using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Metrotrainer
{
    public partial class AddEvent : Form
    {
        private TrainerEvent _event;

        public AddEvent()
        {
            InitializeComponent();
            beatPerMinuteUpDown.Minimum = 1;
            beatPerMinuteUpDown.Maximum = 1000;
        }


        public TrainerEvent TrainerEvent => _event;

    

        private void OnDone(object sender, EventArgs e)
        {

            var seconds = maskedTextBox1.CoerceTime();
            if(!seconds.HasValue){ new ErrorMessages().InvalidDuration(); return; }

            _event.Duration = seconds.Value;
            _event.BPM = (int)beatPerMinuteUpDown.Value;
            _event.Message = messageTextBox.Text;
            
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OnEnterMe(object sender, EventArgs e)
        {
            if(sender is MaskedTextBox)
            {
                BeginInvoke((Action)delegate { SetMaskedTextBoxSelectAll((MaskedTextBox)sender); });
            }
            else if(sender is NumericUpDown)
            {
                var obj = (NumericUpDown)sender;
                obj.Select(0, obj.Text.Length);
            }else if(sender is RichTextBox)
            {
                ((RichTextBox)sender).SelectAll();
            }
        }

        private void SetMaskedTextBoxSelectAll(MaskedTextBox txtbox)
        {
            txtbox.SelectAll();
        }

        private void messageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) OnDone(this, null);
        }
    }
}
