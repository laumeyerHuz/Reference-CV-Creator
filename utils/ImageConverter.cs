using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Markup;


namespace ReferenceConfigurator.utils {

    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class ImageConverter : MarkupExtension, IValueConverter {
        private static ImageConverter _instance;

        public ImageConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            object result = null;
            var path = value as string;

            if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                using (var stream = File.OpenRead(path)) {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    result = image;
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return _instance ??= new ImageConverter();
        }
    }
}
