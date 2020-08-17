using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metrotrainer
{
    public struct TrainerEvent
    {
        public int BPM { get; set; }
        public int Duration { get; set; }
        public string Message { get; set; }


        public override string ToString()
        {
            var msg = Message.InvalidString() ? "" : $"Message[{Message.LimitLength(10)}]";
            return $"Duration[{Duration.ToTimeFormat()}] {msg} BPM[{BPM,3:D3}]";
        }
    }
}
