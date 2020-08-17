using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Metrotrainer
{
    public class Trainer
    {

        public EventHandler PlaylistInstanceChanged;
        public EventHandler TrainerStarted;
        public EventHandler TrainerStopped;
        public EventHandler<TrainerEvent?> TrainerEventChanged;
        public BindingList<TrainerEvent> Playlist { get; set; }

        private bool _running;
        private int _playlistIndex;        
        private int _innerClock;
        private int _timeStartEvent;
        

        public Trainer()
        {
            ClearPlaylist();
        }

        #region Properties
        public bool ValidPlaylistIndex => _playlistIndex >= 0 && _playlistIndex < Playlist.Count;
        public int PlaylistIndex => _playlistIndex;

        public float TrainNormalTime => CurrentEvent.HasValue ? (float)(_innerClock - _timeStartEvent) / (float)CurrentEvent.Value.Duration : 0;
        public float PlaylistNormalTime => Running? (float)_playlistIndex / ((float)Playlist.Count - 1) : 0;

        public TrainerEvent? CurrentEvent
        {
            get
            {
                if (ValidPlaylistIndex) return Playlist[_playlistIndex];
                else return null;
            }
        }

        public bool Running
        {
            get => _running;
            set
            {
                bool dif = _running != value;
                _running = value;
                if (dif && !value) TrainerStopped?.Invoke(this, null);
                else if (dif && value) TrainerStarted?.Invoke(this, null);
            }
        }
        #endregion

        #region IO
        public bool SavePlaylist(string path)
        {
            try 
            { 
                using (StreamWriter file = File.CreateText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, Playlist);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool LoadPlaylist(string path)
        {
            try
            {
                using (StreamReader file = File.OpenText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Playlist = (BindingList<TrainerEvent>)serializer.Deserialize(file,typeof(BindingList<TrainerEvent>));
                }
                OnPlaylistInstanceChange();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Events
        private void OnPlaylistInstanceChange()
        {
            PlaylistInstanceChanged?.Invoke(this, null);
            Running = false;
        }
        #endregion
        public void SkipTrain(int dir)
        {
            if (!Running) return;

            _playlistIndex = (_playlistIndex + dir).Mod(Playlist.Count);
            _timeStartEvent = _innerClock;
            TrainerEventChanged?.Invoke(this, CurrentEvent);
            Console.WriteLine("TrainerEvent Changed!");
        }
        public void Start()
        {
            if (Playlist.Count == 0) return;
            _playlistIndex = -1;
            Running = true;
            SkipTrain(1);
            
        }
        
        public void Stop()
        {
            if (!Running) return;
            Running = false;
        }

        internal void Update()
        {
            if (!ValidPlaylistIndex) Running = false;
            if (!Running) return;
            
            var duration = _innerClock - _timeStartEvent; 
            if (duration > Playlist[_playlistIndex].Duration)
                SkipTrain(1);

            _innerClock += 1;
        }

        internal void ClearPlaylist()
        {
            Playlist = new BindingList<TrainerEvent>();
            OnPlaylistInstanceChange();
        }
    }
}
