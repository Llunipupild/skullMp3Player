using SkullMp3Player.UserControls.Interface;
using System.Windows.Controls;

namespace SkullMp3Player.UserControls.PlayersUserControls.Interfaces
{
    public interface IWebSitePlayerUserControl : IPlayerUserControl
    {
        public StackPanel PlaylistsStackPanel { get; }
        public StackPanel RandomMusicListStackPanel { get; }
        public TextBox FindMusicTextBox { get; }
        public TextBlock FindMusicTextBlock { get; }
    }
}
