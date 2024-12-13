using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace My_Custom_Spotify_Remake
{
    public class MediaElementController : NotifyPropertyChanged
    {
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private MediaElement mediaElement;
        private string fileName = string.Empty;
        private int fileLengthInSeconds = 0;
        private int currentPositionInSeconds = 0;
        private PlaybackState playerState=PlaybackState.Inactive;
        private MusicItem currentPlayMusicItem=new MusicItem();
        private bool isRepeating = false;
        private bool isShuffling = false;
        private string currentTimeForViewing = "00:00";
        private string musicLenghtString = "00:00";

        #region Commands
        public ICommand OpenFileCommand { get; set; }
        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand PrevCommand { get; set; }
        public ICommand ShuffleCommand { get; set; }
        public ICommand RepeatCommand { get; set; }

        #endregion Commands

        public MediaElementController()
        {
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            dispatcherTimer.Tick += DispatcherTimer_Tick;

            PlayCommand = new RelayCommand<MusicItem>(Play);
            PauseCommand = new RelayCommand(Pause);
            NextCommand = new RelayCommand(NextMusic);
            PrevCommand = new RelayCommand(PrevMusic);
            ShuffleCommand = new RelayCommand(ShuffleMusic);
            RepeatCommand = new RelayCommand(RepeatMusic);
        }

        #region OperationWithMediafile
        public void SetStandartProperties()
        {
            FileName = string.Empty;
            FileLengthInSeconds = 0;
            CurrentPositionInSeconds = 0;
            PlayerState=PlaybackState.Inactive;
            CurrentTimeForViewing = "00:00";
            MusicLenghtString = "00:00";
            CurrentPlayMusicItem = new MusicItem();
        }

        public int MusicFileLengthInSeconds()
        {
            return (int)mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
        }

        public void SetMediaElement(MediaElement mediaElement)
        {
            this.mediaElement = mediaElement;
            mediaElement.MediaOpened += MediaElement_MediaOpened;
            mediaElement.MediaEnded += MediaElement_MediaEnded;
        }

        #endregion OperationWithMediafile

        #region MediaFilePreperating
        public void SetPlaybackSource()
        {
            if (!string.IsNullOrEmpty(CurrentPlayMusicItem.FilePath) && System.IO.File.Exists(CurrentPlayMusicItem.FilePath))
            {
                FileName = System.IO.Path.GetFileNameWithoutExtension(CurrentPlayMusicItem.FilePath);
                mediaElement.Source = new Uri(CurrentPlayMusicItem.FilePath);
            }
        }

        #endregion OpeningMediaFileAndPreperating

        #region Gamemode
        public void Play(MusicItem musicItem)
        {
            if (CurrentPlayMusicItem != musicItem)
            {
                CurrentPlayMusicItem.MusicState = PlaybackState.Inactive; // Попередній елемент, змінюєм йому кнопку на hidden
                PlayerState= PlaybackState.Inactive;
            }

            CurrentPlayMusicItem = musicItem;
            
            if (PlayerState == PlaybackState.Inactive || mediaElement.Source.LocalPath != CurrentPlayMusicItem.FilePath) // Якщо плеер не грає або змінили пісню(шлях)
            {
                SetPlaybackSource();
            }

            if (PlayerState == PlaybackState.Play && CurrentPlayMusicItem.MusicState==PlaybackState.Play)
            {
                Pause();
            }

            else
            {
                if (!string.IsNullOrEmpty(CurrentPlayMusicItem.FilePath) && File.Exists(CurrentPlayMusicItem.FilePath))
                {
                    mediaElement.Play();
                    dispatcherTimer.Start();
                    PlayerState = PlaybackState.Play;
                    CurrentPlayMusicItem.MusicState = PlaybackState.Play;
                }
            }
        }

        public void Pause()
        {
            CurrentPlayMusicItem.MusicState = PlaybackState.Pause;
            mediaElement.Pause();
            dispatcherTimer.Stop();
            PlayerState = PlaybackState.Pause;
        }

        public void Stop()
        {
            mediaElement.Stop();
            dispatcherTimer.Stop();
        }

        public void NextMusic()
        {
            if (DataTransfer.ListItemsTemporaryView != null && DataTransfer.ListItemsTemporaryView.Count > 0)
            {
                int nextMusicItemIndex = DataTransfer.ListItemsTemporaryView.IndexOf(CurrentPlayMusicItem) + 1;
                PlayerState = PlaybackState.Inactive;
                if (nextMusicItemIndex >= DataTransfer.ListItemsTemporaryView.Count)
                {
                    Play(DataTransfer.ListItemsTemporaryView.ElementAt(0));
                }
                else
                {
                    Play(DataTransfer.ListItemsTemporaryView.ElementAt(nextMusicItemIndex));
                }
            }
        }

        public void PrevMusic()
        {
            if(DataTransfer.ListItemsTemporaryView != null && DataTransfer.ListItemsTemporaryView.Count > 0)
            {
                int prevMusicItemIndex = DataTransfer.ListItemsTemporaryView.IndexOf(CurrentPlayMusicItem) - 1;
                PlayerState = PlaybackState.Inactive;

                if (prevMusicItemIndex >= 0)
                {
                    Play(DataTransfer.ListItemsTemporaryView.ElementAt(prevMusicItemIndex));
                }
                else
                {
                    Play(DataTransfer.ListItemsTemporaryView.ElementAt(DataTransfer.ListItemsTemporaryView.Count - 1));
                }
            }
        }

        public void ShuffleMusic()
        {
            IsShuffling = !IsShuffling;
        }

        public void RepeatMusic()
        {
            IsRepeating = !IsRepeating;
        }

        #endregion Gamemode

        #region AllEvents
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            CurrentPositionInSeconds = (int)mediaElement.Position.TotalSeconds;
            TimeSpan timeSpan = TimeSpan.FromSeconds(CurrentPositionInSeconds);
            CurrentTimeForViewing = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }

        private void MediaElement_MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            FileLengthInSeconds = (int)mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            TimeSpan timeSpan = TimeSpan.FromSeconds(FileLengthInSeconds);
            MusicLenghtString = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }

        private void MediaElement_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            if(IsRepeating && IsShuffling) // Помилка
            {
                System.Windows.MessageBox.Show("Repeat and Stop modes cannot be activated in together!", "Error. Chooce something one", MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

            if(IsRepeating) // Повторення пісні поки кнопка вкл
            {
                PlayerState = PlaybackState.Inactive;
                Play(CurrentPlayMusicItem);
            }

            else if (IsShuffling) // Рандомна пісня
            {
                PlayerState = PlaybackState.Inactive;
                Random random = new Random();
                int randomMusicIndex = random.Next(0, DataTransfer.ListItemsTemporaryView.Count);
                Play(DataTransfer.ListItemsTemporaryView.ElementAt(randomMusicIndex));
            }

            if(!IsRepeating && !IsShuffling)
            {
                NextMusic(); // Після завершення гратиме автоматично наступна музика
            }
        }

        #endregion AllEvents

        #region NotifyingPropertyChanged
        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        public int FileLengthInSeconds
        {
            get { return fileLengthInSeconds; }
            set
            {
                if (fileLengthInSeconds != value)
                {
                    fileLengthInSeconds = value;
                    OnPropertyChanged(nameof(FileLengthInSeconds));
                }
            }
        }

        public int CurrentPositionInSeconds
        {
            get { return currentPositionInSeconds; }
            set
            {
                if (currentPositionInSeconds != value)
                {
                    currentPositionInSeconds = value;
                    OnPropertyChanged(nameof(CurrentPositionInSeconds));
                }
            }
        }

        public PlaybackState PlayerState
        {
            get { return playerState; }
            set
            {
                if (playerState != value)
                {
                    playerState = value;
                    OnPropertyChanged(nameof(PlayerState));
                }
            }
        }

        public MusicItem CurrentPlayMusicItem
        {
            get { return currentPlayMusicItem; }
            set
            {
                if (currentPlayMusicItem != value)
                {
                    currentPlayMusicItem = value;
                    OnPropertyChanged(nameof(CurrentPlayMusicItem));
                }
            }
        }

        public bool IsRepeating
        {
            get { return isRepeating; }
            set
            {
                if (isRepeating != value)
                {
                    isRepeating = value;
                    OnPropertyChanged(nameof(IsRepeating));
                }
            }
        }

        public bool IsShuffling
        {
            get { return isShuffling; }
            set
            {
                if (isShuffling != value)
                {
                    isShuffling = value;
                    OnPropertyChanged(nameof(IsShuffling));
                }
            }
        }

        public string CurrentTimeForViewing
        {
            get { return currentTimeForViewing; }
            set
            {
                if (currentTimeForViewing != value)
                {
                    currentTimeForViewing = value;
                    OnPropertyChanged(nameof(CurrentTimeForViewing));
                }
            }
        }

        public string MusicLenghtString
        {
            get { return musicLenghtString; }
            set
            {
                if (musicLenghtString != value)
                {
                    musicLenghtString = value;
                    OnPropertyChanged(nameof(MusicLenghtString));
                }
            }
        }

        #endregion

    }
}
