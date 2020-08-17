using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Metrotrainer
{


    public partial class MainForm : Form
    {
        private Trainer _trainer;
        private SpeechSynthesizer _synthesizer;
        public MainForm()
        {
            InitializeComponent();
            _trainer = new Trainer();
            _synthesizer = new SpeechSynthesizer();

            /*
             * Custom parameters:
                * TextToSpeech.Voice;
                * Beep duration;
                * Beep frequency;
             */

            //CUSTOM            
            TimerTrainer.Interval = 1000;
            TimerBeatPerMinute.Interval = 1000;

            //SUBSCRIPTIONS
            _trainer.PlaylistInstanceChanged += (object s, EventArgs e) => playlistBox.DataSource = _trainer.Playlist;
            _trainer.TrainerEventChanged += OnEventChange;
            _trainer.TrainerStopped += OnTrainerStop;
            _trainer.TrainerStarted += OnTrainerStart;
            playlistBox.DataSource = _trainer.Playlist;
        }

        #region Events

        private void OnEventChange(object sender, TrainerEvent? trainerEvent)
        {
            Console.WriteLine($"Event changed? {trainerEvent.HasValue}");
            if (trainerEvent.HasValue)
            {
                int interval = (int)(60F / (float)trainerEvent.Value.BPM * 1000F);
                if (interval != 0) TimerBeatPerMinute.Interval = interval;
                Console.WriteLine($"BPM has been set as {trainerEvent.Value.BPM}");

                if (!trainerEvent.Value.Message.InvalidString()) {
                    _synthesizer.SpeakAsync(trainerEvent.Value.Message);
                    Console.WriteLine($"Speaking async. Message = {trainerEvent.Value.Message}");
                }
            }
        }

        private void StartTrainer(object sender, EventArgs e){if (!ValidPlaylistWMessage()) return;_trainer.Start();}
        private void ToggleTrainer(object sender, EventArgs e)
        {
            if (!TimerTrainer.Enabled)
            {
                startTrainingButton.Text = StartButtonTexts.Started;
                TimerTrainer.Start();
                TimerBeatPerMinute.Start();
            }
            else
            {
                startTrainingButton.Text = StartButtonTexts.Continue;
                TimerTrainer.Stop();
                TimerBeatPerMinute.Stop();
            }
        }

        private void OnTrainerStart(object sender, EventArgs e)
        {
            Console.WriteLine("On Trainer Started");
            startTrainingButton.Text = StartButtonTexts.Started;
            startTrainingButton.Click -= StartTrainer;
            startTrainingButton.Click += ToggleTrainer;
            TimerTrainer.Start();
            TimerBeatPerMinute.Start();
            groupBox1.Enabled = true;
            groupBox2.Enabled = false;
        }
        private void OnTrainerStop(object sender, EventArgs e)
        {
            Console.WriteLine("On Trainer Stopped");
            startTrainingButton.Text = StartButtonTexts.NotStarted;
            startTrainingButton.Click -= ToggleTrainer;
            startTrainingButton.Click += StartTrainer;
            TimerTrainer.Stop();
            TimerBeatPerMinute.Stop();
            groupBox1.Enabled = false;
            groupBox2.Enabled = true;
        }
        private void ShowStats(object sender, EventArgs e)
        {
            if (!ValidPlaylistWMessage()) return;
            var stats = new PlaylistStatistics(_trainer.Playlist);


            MessageBox.Show(
            $@"Playlist Statistics:
            Average BPM: {stats.AverageBPM:0.##}
            Average Duration: {((int)stats.AverageDuration).ToTimeFormat():0.##}                
            Total Duration: {stats.TotalDuration.ToTimeFormat()}
            Number of Drills: {stats.NumberOfDrills}
            ", "Playlist Statistics");
        }

        private void PlaylistForward(object sender, EventArgs e) { _trainer.SkipTrain(1); playlistBox.SelectedIndex = _trainer.PlaylistIndex; }
        private void PlaylistBack(object sender, EventArgs e) { _trainer.SkipTrain(-1); playlistBox.SelectedIndex = _trainer.PlaylistIndex; }
        private void PlaylistStop(object sender, EventArgs e) => _trainer.Stop();
        private void PlaylistClear(object sender, EventArgs e) {if(!ValidPlaylistWMessage())return; _trainer.ClearPlaylist();}


        private void PlaylistAdd(object sender, EventArgs e)
        {
            var form = new AddEvent();
            var result = form.ShowDialog();
            if (result != DialogResult.No && result != DialogResult.Cancel && result != DialogResult.Abort)
            {
                _trainer.Playlist.Add(form.TrainerEvent);
                Console.WriteLine($"Successfully added {form.TrainerEvent}");
            }
        }
        private void AddRange(object sender, EventArgs e)
        {
            var form = new AddRange();
            var result = form.ShowDialog();
            if (result != DialogResult.No && result != DialogResult.Cancel && result != DialogResult.Abort)
            {
                foreach(var evt in form.TrainerEvents) _trainer.Playlist.Add(evt);
                Console.WriteLine($"Successfully added {form.TrainerEvents.Count}");
            }
        }

        private void PlaylistRemove(object sender, EventArgs e)
        {
            if (ValidPlaylistSelection())
            {
                Console.WriteLine($"Successfully removed {_trainer.Playlist[playlistBox.SelectedIndex]}");
                _trainer.Playlist.RemoveAt(playlistBox.SelectedIndex);
            }
        }

        private void PlaylistUp(object sender, EventArgs e)
        {
            if (ValidPlaylistSelection())
            {
                var newidx = playlistBox.SelectedIndex == 0 ? _trainer.Playlist.Count - 1 : playlistBox.SelectedIndex - 1;
                SwapPlaylist(playlistBox.SelectedIndex, newidx);
                playlistBox.SelectedIndex = newidx;
            }
        }

        private void PlaylistDown(object sender, EventArgs e)
        {
            if (ValidPlaylistSelection())
            {
                var newidx = playlistBox.SelectedIndex == _trainer.Playlist.Count - 1 ? 0 : playlistBox.SelectedIndex + 1;
                SwapPlaylist(playlistBox.SelectedIndex, newidx);
                playlistBox.SelectedIndex = newidx;
            }
        }

        private void PlaylistSave(object sender, EventArgs e)
        {
            var dig = new SaveFileDialog();
            DefaultFileDialog(dig);
            var res = dig.ShowDialog();
            if (res.Accept()) _trainer.SavePlaylist(dig.FileName);
            else new ErrorMessages().InvalidIO();
        }

        private void PlaylistLoad(object sender, EventArgs e)
        {

            var dig = new OpenFileDialog();
            DefaultFileDialog(dig);
            var res = dig.ShowDialog();
            if (res.Accept()) _trainer.LoadPlaylist(dig.FileName);
            else new ErrorMessages().InvalidIO();
        }

        #endregion





        private void TrainerTick(object sender, EventArgs e)
        {
            _trainer.Update();
            if(_trainer.Running)playlistBox.SelectedIndex = _trainer.PlaylistIndex;
            progressBar1.Value = ((int)(_trainer.TrainNormalTime * 100F)).Clamp(0,100) ;
            progressBar2.Value = ((int) (_trainer.PlaylistNormalTime * 100F)).Clamp(0,100);

            Console.WriteLine($"Train={_trainer.TrainNormalTime} Playlist={_trainer.PlaylistNormalTime}");
        }
        private async void MetronomeTick(object sender, EventArgs e)
        {
            Console.WriteLine("Metronome Tick");
            
            await Task.Run(() => Console.Beep(Properties.Settings.Default.BeepFrequency, Properties.Settings.Default.BeepDuration));
        }



        private void DefaultFileDialog(FileDialog fd)
        {
            fd.DefaultExt = "metro";
            fd.AddExtension = true;
            fd.Filter = "MetroTraining (*.metro) | *.metro";
            fd.FileName = "playlist";
        }

        private void SwapPlaylist(int a, int b)
        {
            var temp = _trainer.Playlist[a];
            _trainer.Playlist[a] = _trainer.Playlist[b];
            _trainer.Playlist[b] = temp;
        }

        private bool ValidPlaylistSelection()
        {
            return playlistBox.SelectedIndex != ListBox.NoMatches;
        }

        private bool ValidPlaylistWMessage()
        {
            if (_trainer.Playlist.Count <= 0) { new ErrorMessages().InvalidPlaylist(); return false; }
            return true;
        }

        public static class StartButtonTexts
        {
            public const string NotStarted = "Start ▶️";
            public const string Continue = "Continue ▶️";
            public const string Started = "Pause ⏸️";
        }
    }
}
