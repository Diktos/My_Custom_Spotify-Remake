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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace My_Custom_Spotify_Remake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaCollection mediaCollection= new MediaCollection();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = mediaCollection;
        }

        #region Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mediaCollection.mediaElementController.SetMediaElement(mediaElement);
            string json = File.ReadAllText(Directory.GetCurrentDirectory()+ "\\Liked tracks.txt");
            if (mediaCollection.LikedItems == null)
            {
                mediaCollection.LikedItems = new ObservableCollection<MusicItem>();
            }

            mediaCollection.LikedItems = JsonConvert.DeserializeObject<ObservableCollection<MusicItem>>(json);
        }

        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem && listBoxItem.DataContext is MusicItem musicItem)
            {
                if (musicItem.MusicState == PlaybackState.Play)
                {
                    
                }
                else
                {
                    musicItem.MusicState = PlaybackState.Wait;
                }
            }
        }

        private void ListBoxItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem && listBoxItem.DataContext is MusicItem musicItem)
            {
                
                musicItem.MusicState = PlaybackState.Inactive;

                if (mediaCollection.mediaElementController.PlayerState == PlaybackState.Play 
                    && musicItem == mediaCollection.mediaElementController.CurrentPlayMusicItem)
                {
                    musicItem.MusicState = PlaybackState.Play;
                }
            }
        }

        #endregion Events

    }
}
