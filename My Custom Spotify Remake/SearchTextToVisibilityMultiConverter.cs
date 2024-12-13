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
    public class SearchTextToVisibilityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is string text && text!=null && values[1] is string textTitle && textTitle!=null)
            {
                char[]charList=text.ToCharArray();
                int matchCount = 0;

                for (int i = 0; i < Math.Min(text.Length, textTitle.Length); i++) // Менший розмір знаходим для коректного порівняння - результату
                {
                    if (matchCount != i)
                    {
                        return Visibility.Collapsed;
                    }
                    
                    if (charList[i] == textTitle.ElementAt(i))
                    {
                        matchCount++;
                    }
                }
                if (matchCount == Math.Min(text.Length, textTitle.Length))
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("SearchTextToVisibility ConvertBack not supported.");
        }

    }
}
