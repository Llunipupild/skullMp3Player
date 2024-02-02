using System.Windows.Media;

namespace SkullMp3Player.UserControls.ItemsUserControls.Interface
{
    public interface IItemUserControl
    {
        public string MusicLink { get; }
        public string MusicName { get; }
        public string Author { get; }
        public string? PlaylistName { get; }
        public bool IsActive { get; set; }
        public ImageSource Image { get; }
    }
}
