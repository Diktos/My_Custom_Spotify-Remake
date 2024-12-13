using Id3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace My_Custom_Spotify_Remake
{
    public enum PlaybackState
    {
        Inactive,
        Play,
        Pause,
        Wait
    }

    public class MediaCollection : NotifyPropertyChanged
    {
        public MediaElementController mediaElementController { get; set; }

        private ObservableCollection<MusicItem> items = new ObservableCollection<MusicItem>();
        private ObservableCollection<MusicItem> likedItems = new ObservableCollection<MusicItem>();
        private string searchRequest = string.Empty;
        private bool textIsEmpty = true;

        #region Commands
        public ICommand OpenOrAddFilesCommand { get; }
        public ICommand LikeOperationCommand { get; set; }
        public ICommand RemoveItemCommand { get; set; }

        #endregion Commands

        public MediaCollection()
        {
            mediaElementController = new MediaElementController();
            OpenOrAddFilesCommand = new RelayCommand<string>(OpenFiles);
            LikeOperationCommand = new RelayCommand<MusicItem>(AddOrRemoveItemInLikes);
            RemoveItemCommand = new RelayCommand<MusicItem>(RemoveItemInMainList);
        }

        #region Search
        private void SearchProcess()
        {
            if (string.IsNullOrEmpty(SearchRequest))
            {
                Items.ToList().ForEach(item => item.MusicVisibility = Visibility.Visible);
            }
            else
            {
                foreach (MusicItem item in Items)
                {
                    if (item.Title.Contains(SearchRequest))
                    {
                        item.MusicVisibility = Visibility.Visible;
                    }
                    else
                    {
                        item.MusicVisibility = Visibility.Collapsed;
                    }
                }
            }
            
        }

        #endregion Search

        #region UpdateNumbersOnList
        private void UpdateItemsNumbersInMainList()
        {
            // Оновлення індексів для відображення у основному списку 
            for (int i = 0; i < Items.Count;i++)
            {
                Items[i].MainNumber = $"{i+1}.";
            }
        }

        private void UpdateItemsNumbersInLikedList()
        {
            // Оновлення індексів для відображення у списку likes 
            for (int i = 0; i < LikedItems.Count; i++)
            {
                LikedItems[i].NumberInLikedList = $"{i + 1}.";
            }
        }

        #endregion UpdateNumbersOnList

        #region OperationsWithLists
        private void AddOrRemoveItemInLikes(MusicItem musicItem)
        {
            if (musicItem.IsLiked)
            {
                LikedItems.Remove(musicItem);
                musicItem.IsLiked=!musicItem.IsLiked;
            }
            else
            {
                LikedItems.Add(musicItem);
                musicItem.IsLiked = !musicItem.IsLiked;
            }
            UpdateItemsNumbersInLikedList();
           
            string json = JsonConvert.SerializeObject(LikedItems);
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\Liked tracks.txt", json);
        }

        private void RemoveItemInMainList(MusicItem musicItem)
        {
            if (musicItem != null)
            {
                if (musicItem.FilePath == mediaElementController.CurrentPlayMusicItem.FilePath)
                {
                    mediaElementController.Stop();
                    mediaElementController.SetStandartProperties();
                }
                Items.Remove(musicItem);
                UpdateItemsNumbersInMainList();
                DataTransfer.ListItemsTemporaryView = Items;
            }
        }

        #endregion OperationsWithLists

        #region OperationWithOpeningAndCreating
        private void OpenFiles(string isProcessAddFiles)
        {
            if (!bool.Parse(isProcessAddFiles))
            {
                mediaElementController.Stop();
                Items.Clear();
            }
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Music"; // Default file name
            dialog.DefaultExt = ".mp3"; // Default file extension
            dialog.Filter = "Music files (.mp3)|*.mp3"; // Filter files by extension
            dialog.Multiselect = true;

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                List<string> files = dialog.FileNames.ToList();
                CreateMusicItems(files);
                DataTransfer.ListItemsTemporaryView = Items;
            }
        }

        private void CreateMusicItems(List<string> filesPath)
        {
            foreach (string path in filesPath)
            {
                MusicItem musicItem = new MusicItem() { FilePath = path };
                ExtractTagLibSharpTag(musicItem);
                Items.Add(musicItem);
            }
            UpdateItemsNumbersInMainList();
        }

        private void ExtractTagLibSharpTag(MusicItem musicItem)
        {
            string unknownTag = "Unknown";

            try
            {
                if (TagLib.File.Create(musicItem.FilePath) is TagLib.File file && file.Tag is TagLib.Tag tag)
                {
                    musicItem.Title = !string.IsNullOrEmpty(tag.Title) ? tag.Title : unknownTag;
                    string artists = string.Join(", ", tag.Performers ?? new string[0]);
                    musicItem.Artist = !string.IsNullOrEmpty(artists) ? artists : unknownTag;
                    musicItem.Album = !string.IsNullOrEmpty(tag.Album) ? tag.Album : unknownTag;
                    string genres = string.Join(", ", tag.Performers ?? new string[0]);
                    musicItem.Genre = !string.IsNullOrEmpty(genres) ? genres : unknownTag;

                    int lengthInSeconds = (int)file.Properties.Duration.TotalSeconds;
                    TimeSpan timeSpan = TimeSpan.FromSeconds(lengthInSeconds);
                    musicItem.Duration = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
                }
                else
                {
                    musicItem.Title = unknownTag;
                    musicItem.Artist = unknownTag;
                    musicItem.Album = unknownTag;
                }
            }
            catch
            {
                MessageBox.Show("The program cannot run this mediafile, it may be corrupted!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        } 

        #endregion OperationWithOpeningAndCreating

        #region NotifyingPropertyChanged
        public ObservableCollection<MusicItem> Items
        {
            get { return items; }
            set
            {
                if (items != value)
                {
                    items = value;
                    OnPropertyChanged(nameof(Items));
                }
            }
        }
        public ObservableCollection<MusicItem> LikedItems
        {
            get { return likedItems; }
            set
            {
                if (likedItems != value)
                {
                    likedItems = value;
                    OnPropertyChanged(nameof(LikedItems));
                }
            }
        }

        public string SearchRequest
        {
            get { return searchRequest; }
            set
            {
                if (searchRequest != value)
                {
                    searchRequest = value;
                    OnPropertyChanged(nameof(SearchRequest));

                    TextIsEmpty = (SearchRequest == string.Empty) ? true : false;
                    SearchProcess();
                }
            }
        }

        public bool TextIsEmpty
        {
            get { return textIsEmpty; }
            set
            {
                if (textIsEmpty != value)
                {
                    textIsEmpty = value;
                    OnPropertyChanged(nameof(TextIsEmpty));
                }
            }
        }

        #endregion NotifyingPropertyChanged

    }
}
