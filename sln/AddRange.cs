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
    public partial class AddRange : Form
    {
        private List<TrainerEvent> _events;
        public List<TrainerEvent> TrainerEvents => _events;
        public AddRange()
        {
            InitializeComponent();
            _events = new List<TrainerEvent>();
            fromNumeric.ValueChanged += SetupStepSign;
            toNumeric.ValueChanged += SetupStepSign;
        }

        
        private void SetupStepSign(object sender, EventArgs e)
        {
            var fromTransition = fromNumeric.Value > toNumeric.Value && stepNumeric.Maximum > 0;
            var toTransition = fromNumeric.Value < toNumeric.Value && stepNumeric.Maximum < 0;
            if (fromTransition || toTransition) {
                //Save values before overriding;
                var tempVal = stepNumeric.Value;
                var tempMin = stepNumeric.Minimum;
                
                stepNumeric.Minimum = -stepNumeric.Maximum;
                stepNumeric.Maximum = -tempMin;
                stepNumeric.Value = -tempVal;
            }

        }

        private void ParseRange(object sender, EventArgs e)
        {
            var seconds = maskedTextBox1.CoerceTime();
            if (!seconds.HasValue) { new ErrorMessages().InvalidDuration();return; }
            if(fromNumeric.Value == toNumeric.Value) { new ErrorMessages().InvalidBPM(); return; }

            _events = new List<TrainerEvent>();
            var ascending = fromNumeric.Value < toNumeric.Value;
            Func<int,bool> decision = (int pos) => ascending ? pos < toNumeric.Value + stepNumeric.Value : pos > toNumeric.Value + stepNumeric.Value;

            

            for(int i = (int)fromNumeric.Value; decision(i); i+= (int)stepNumeric.Value)
            {
                var evt = new TrainerEvent { BPM = i, Duration = seconds.Value, Message = richTextBox1.Text };
                _events.Add(evt);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
