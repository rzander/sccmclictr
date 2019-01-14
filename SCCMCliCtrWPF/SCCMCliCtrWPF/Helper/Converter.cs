using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClientCenter.Helper
{
    public class BooleanToImageConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">Has to contain uris to two images seperated by | </param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Boolean bval = false;
            Boolean.TryParse(value.ToString(), out bval);

            String[] Images = ((String)parameter).Split('|');

            if(bval)
                if(Images[0] == "")
                    return BitmapImage.Create(
                                                2,
                                                2,
                                                96,
                                                96,
                                                PixelFormats.Indexed1,
                                                new BitmapPalette(new List<Color> { Colors.Transparent }),
                                                new byte[] { 0, 0, 0, 0 },
                                                1);
                else
                    return new BitmapImage(new Uri(Images[0]));
            else
                if (Images.Length == 1 || Images[1] == "")
                    return BitmapImage.Create(
                                            2,
                                            2,
                                            96,
                                            96,
                                            PixelFormats.Indexed1,
                                            new BitmapPalette(new List<Color> { Colors.Transparent }),
                                            new byte[] { 0, 0, 0, 0 },
                                            1);
                else
                    return new BitmapImage(new Uri(Images[1]));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
