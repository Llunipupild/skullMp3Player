using System.Windows;
using System.Windows.Controls;

namespace SkullMp3Player.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Playlists.xaml
    /// </summary>
    public partial class PlaylistItemUserControl : UserControl
    {
        public const string NEW_MUSIC_PLAYLIST_NAME = "newMusic";
        public const string POPULAR_MUSIC_PLAYLIST_NAME = "popularMusic";

        public static readonly DependencyProperty PlaylistNameProperty = DependencyProperty.Register("PlaylistName", typeof(string), typeof(PlaylistItemUserControl));
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(PlaylistItemUserControl));
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(PlaylistItemUserControl));

        public PlaylistItemUserControl(string playlistName, string desription)
        {
            PlaylistName = playlistName;
            Description = desription;
            InitializeComponent();
        }

        public string PlaylistName
        {
            get => GetValue(PlaylistNameProperty).ToString()!;
            set => SetValue(PlaylistNameProperty, value);
        }

        public string Description
        {
            get => GetValue(DescriptionProperty).ToString()!;
            set => SetValue(DescriptionProperty, value);
        }

        public bool IsActive
        {
            get => (bool) GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }
    }
}
