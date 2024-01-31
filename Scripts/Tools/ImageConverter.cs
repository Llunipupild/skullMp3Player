using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SkullMp3Player.Scripts.Tools
{
    static class ImageConverter
    {
        private static ImageSource _defaultImage = (ImageSource) Application.Current.Resources["localMusicIcon"];
        private static ImageSourceConverter _imageSourceConverter = new();

        public static ImageSource GetImageSource(string? image)
        {
            if (string.IsNullOrEmpty(image)) {
                return _defaultImage;
            }

            //если опять ошибка винды HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders и удалить папки Icache и ICookie
            try {
                return _imageSourceConverter.ConvertFromString(image) as ImageSource ?? _defaultImage;
            } catch (Exception) {
                return _defaultImage;
            }
        }

        public static ImageSource GetImageSource(byte[] imageOnByte)
        {
            MemoryStream stream = new(imageOnByte);
            BitmapImage image = new();

            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();

            return image ?? _defaultImage;
        }
    }
}
