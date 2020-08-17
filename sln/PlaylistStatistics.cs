using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metrotrainer
{
    public class PlaylistStatistics
    {
        IList<TrainerEvent> Playlist { get; set; }
        public PlaylistStatistics(IList<TrainerEvent> playlist) =>Playlist = playlist;
        
        public float AverageDuration => (float)Playlist.Average(x => x.Duration);
        public int TotalDuration => Playlist.Sum(x => x.Duration);
        public int NumberOfDrills => Playlist.Count;
        public float AverageBPM => (float)Playlist.Average(x => x.BPM);
        

        
        
        
    }
}
