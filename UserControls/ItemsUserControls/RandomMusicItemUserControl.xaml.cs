using SkullMp3Player.UserControls.ItemsUserControls.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SkullMp3Player.UserControls
{
    /// <summary>
    /// Логика взаимодействия для RandomMusicItemUserControl.xaml
    /// </summary>
    public partial class RandomMusicItemUserControl : UserControl, IItemUserControl
    {
        public const string RANDOM_PLAYLIST_NAME = "randomPlaylist";

        public static readonly DependencyProperty MusicNameProperty = DependencyProperty.Register("MusicName", typeof(string), typeof(RandomMusicItemUserControl));
        public static readonly DependencyProperty AuthorProperty = DependencyProperty.Register("Author", typeof(string), typeof(RandomMusicItemUserControl));
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(RandomMusicItemUserControl));
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(RandomMusicItemUserControl));

        public RandomMusicItemUserControl(string musicName, string author, string musicLink, ImageSource imageSource, bool isActive = false)
        {
            MusicName = musicName;
            Author = author;
            MusicLink = musicLink;
            Image = imageSource;
            IsActive = isActive;
            InitializeComponent();
        }

        public string MusicName
        {
            get => GetValue(MusicNameProperty).ToString()!;
            set => SetValue(MusicNameProperty, value);
        }

        public string Author
        {
            get => GetValue(AuthorProperty).ToString()!;
            set => SetValue(AuthorProperty, value);
        }

        public ImageSource Image
        {
            get => (ImageSource) GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public bool IsActive
        {
            get => (bool) GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public string MusicLink { get; } = null!;

        public string? PlaylistName => RANDOM_PLAYLIST_NAME;
    }
}
