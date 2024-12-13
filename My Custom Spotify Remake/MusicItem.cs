using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace My_Custom_Spotify_Remake
{
    public class MusicItem : NotifyPropertyChanged
    {
        public string FilePath { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }

        private string mainNumber = string.Empty;
        private string numberInLikedList = string.Empty;
        private string duration = TimeSpan.Zero.ToString();
        private bool isLiked = false;
        private PlaybackState musicState=PlaybackState.Inactive;
        private Visibility musicVisibility = Visibility.Visible;

        #region Сonstructors
        public MusicItem()
        {

        }

        public MusicItem(string filePath, string title, string artist, string album)
        {
            FilePath = filePath;
            Title = title;
            Artist = artist;
            Album = album;
        }
        #endregion Сonstructors

        #region NotifyingPropertyChanged

        public string Duration
        {
            get { return duration; }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        public bool IsLiked
        {
            get { return isLiked; }
            set
            {
                if (isLiked != value)
                {
                    isLiked = value;
                    OnPropertyChanged(nameof(IsLiked));
                }
            }
        }

        public string MainNumber
        {
            get { return mainNumber; }
            set
            {
                if (mainNumber != value)
                {
                    mainNumber = value;
                    OnPropertyChanged(nameof(MainNumber));
                }
            }
        }
        public string NumberInLikedList
        {
            get { return numberInLikedList; }
            set
            {
                if (numberInLikedList != value)
                {
                    numberInLikedList = value;
                    OnPropertyChanged(nameof(NumberInLikedList));
                }
            }
        }

        public PlaybackState MusicState
        {
            get { return musicState; }
            set
            {
                if (musicState != value)
                {
                    musicState = value;
                    OnPropertyChanged(nameof(MusicState));
                }
            }
        }

        public Visibility MusicVisibility
        {
            get { return musicVisibility; }
            set
            {
                if (musicVisibility != value)
                {
                    musicVisibility = value;
                    OnPropertyChanged(nameof(MusicVisibility));
                }
            }
        }

        #endregion NotifyingPropertyChanged

    }
}
