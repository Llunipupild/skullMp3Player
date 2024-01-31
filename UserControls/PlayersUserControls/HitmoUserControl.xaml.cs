using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using SkullMp3Player.UserControls.PlayersUserControls.Interfaces;

namespace SkullMp3Player.UserControls
{
    /// <summary>
    /// Логика взаимодействия для HitmoUserControl.xaml
    /// </summary>
    public partial class HitmoUserControl : UserControl, IWebSitePlayerUserControl
    {
        public StackPanel MusicListStackPanel => MusicListPanel;
        public StackPanel PlaylistsStackPanel => PlaylistsPanel;
        public StackPanel RandomMusicListStackPanel => RandomMusicPanel;
        public TextBox FindMusicTextBox => FindMusicBox;
        public TextBlock FindMusicTextBlock => ResponseText;
        public string CurrentLoadedPlaylistName { get; set; } = null!;

        public event FindMusic? FindMusicEvent;
        public event RefreshRandomMusic? RefreshRandomMusicEvent;
        public delegate void FindMusic(string findMusicText);
        public delegate void RefreshRandomMusic();

        public HitmoUserControl()
        {
            InitializeComponent();
        }

        private void OnTextBoxValueChanged(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) {
                return;
            }

            FindMusicEvent?.Invoke(FindMusicTextBox.Text);
        }

        private void OnRefreshRandomMusicButtonClick(object sender, RoutedEventArgs e)
        {
            RefreshRandomMusicEvent?.Invoke();
        }
    }
}
