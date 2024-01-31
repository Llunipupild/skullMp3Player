using SkullMp3Player.UserControls.PlayersUserControls.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace SkullMp3Player.UserControls
{
    /// <summary>
    /// Логика взаимодействия для SuperMusicUserControl.xaml
    /// </summary>
    public partial class SuperMusicUserControl : UserControl, IWebSitePlayerUserControl
    {
        public StackPanel MusicListStackPanel => MusicList;
        public StackPanel PlaylistsStackPanel => PlaylistsPanel;
        public StackPanel RandomMusicListStackPanel => RandomMusicPanel;
        public TextBox FindMusicTextBox => FindMusicBox;
        public TextBlock FindMusicTextBlock => ResponseText;
        public string CurrentLoadedPlaylistName { get; set; } = null!;

        public event FindMusic? FindMusicEvent;
        public event RefreshRandomMusic? RefreshRandomMusicEvent;
        public delegate void FindMusic(string findMusicText);
        public delegate void RefreshRandomMusic();

        public SuperMusicUserControl()
        {
            InitializeComponent();
        }

        private void OnTextBoxValueChanged(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) {
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
