using System.Windows.Controls;

namespace SkullMp3Player.UserControls.Interface
{
    public interface IPlayerUserControl
    {
        public StackPanel MusicListStackPanel { get; }
        public string CurrentLoadedPlaylistName { get; set; }
    }
}
