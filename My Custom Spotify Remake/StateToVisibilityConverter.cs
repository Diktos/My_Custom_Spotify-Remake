using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace My_Custom_Spotify_Remake
{
    public class StateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != DependencyProperty.UnsetValue && value is PlaybackState state && (state==PlaybackState.Play || state==PlaybackState.Pause || state==PlaybackState.Wait))
            {
               return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("StateToVisibilityConverter ConvertBack not supported.");
        }

    }
}
