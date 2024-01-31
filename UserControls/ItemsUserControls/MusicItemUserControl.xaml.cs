using SkullMp3Player.UserControls.ItemsUserControls.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SkullMp3Player.UserControls
{
    /// <summary>
    /// Логика взаимодействия для SongItem.xaml
    /// </summary>
    public partial class MusicItemUserControl : UserControl, IItemUserControl
    {
        public static readonly DependencyProperty MusicNameProperty = DependencyProperty.Register("MusicName", typeof(string), typeof(MusicItemUserControl));
        public static readonly DependencyProperty AuthorProperty = DependencyProperty.Register("Author", typeof(string), typeof(MusicItemUserControl));
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(MusicItemUserControl));
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(MusicItemUserControl));

        public MusicItemUserControl(string musicLink, string name, string author, ImageSource image, bool isActive = false)
        {
            MusicLink = musicLink;
            Author = author;
            MusicName = name;
            Image = image;
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

        public string? PlaylistName => null;
    }
}
