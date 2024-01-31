using System.Windows.Media;
using SkullMp3Player.Scripts.Tools;

namespace SkullMp3Player.Scripts.Player.Music.Model
{
    public class MusicModel
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Link { get; set; }
        public ImageSource Image { get; set; }

        public MusicModel(string link, string name, string author, string image)
        {
            Link = link;
            Name = name;
            Author = author;
            Image = ImageConverter.GetImageSource(image);
        }

        public MusicModel(string link, string name, string author, byte[] imageOnByte)
        {
            Link = link;
            Name = name;
            Author = author;
            Image = ImageConverter.GetImageSource(imageOnByte);
        }
    }
}
