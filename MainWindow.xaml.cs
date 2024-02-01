using MahApps.Metro.IconPacks;
using SkullMp3Player.Scripts;
using SkullMp3Player.Scripts.Client.Controller;
using SkullMp3Player.Scripts.Navigation.Service;
using SkullMp3Player.Scripts.Parser.Hitmo;
using SkullMp3Player.Scripts.Parser.SuperMusic;
using SkullMp3Player.Scripts.Player.Controller;
using SkullMp3Player.Scripts.Player.Music.LocalMusic.Controller;
using SkullMp3Player.Scripts.Player.Music.Repository;
using SkullMp3Player.Scripts.Player.Music.WebsiteMusic.Controller;
using SkullMp3Player.Scripts.Player.Playlists.Controller;
using SkullMp3Player.Scripts.Tools;
using SkullMp3Player.Scripts.Ui.MusicItems.Controller;
using SkullMp3Player.UserControls;
using SkullMp3Player.UserControls.Interface;
using SkullMp3Player.UserControls.PlayersUserControls.Interfaces;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SkullMp3Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NavigationService _navigationService = null!;
        private MusicRepository _musicRepository = null!;
        private Mp3Player _mp3Player = null!;
        private CurrentPlayingPlaylistController _currentPlayingPlaylistController = null!;

        private WebsiteMusicController _websiteMusicController = null!;
        private LocalMusicController _localMusicController = null!;

        private HitmoParser _hitmoParser = null!;
        private SuperMusicParser _superMusicParser = null!;

        private IWebSiteParser? _currentParser;
        private IPlayerUserControl _currentPlayerUserControl = null!;
        private PackIconMaterial _pauseButtonPackMaterial = null!;
        private MusicItemsController _musicItemsController = null!;

        private bool _alreadyDownload;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Init()
        {
            _mp3Player = new Mp3Player(() => OnNextButtonClick(this, null!), CurrentPlayingMusicTime);
            _musicRepository = new();
            _currentPlayingPlaylistController = new(_mp3Player, _musicRepository);
            _navigationService = new(LocalMusicUserControl);

            _musicItemsController = new(CurrentPlayingMusicName, CurrentPlayingMusicAuthor, CurrentPlayingMusicImage);
            _websiteMusicController = new(_musicRepository, _musicItemsController, _currentPlayingPlaylistController);
            _localMusicController = new(_musicRepository, _musicItemsController, _currentPlayingPlaylistController, LocalMusicUserControl);
            
            _currentPlayerUserControl = LocalMusicUserControl;
            _pauseButtonPackMaterial = (PackIconMaterial) PauseButton.Content;
            _currentParser = null;
            HitmoButton.IsEnabled = false;
            SuperMusicButton.IsEnabled = false;

            LocalMusicUserControl.AddMusicEvent += _localMusicController.AddMusic;
            LocalMusicUserControl.RemoveMusicEvent += _localMusicController.RemoveMusic;
            LocalMusicUserControl.ShuffleMusicEvent += _localMusicController.ShuffleMusic;

            HitmoUserControl.FindMusicEvent += _websiteMusicController.FindMusicAsync;
            HitmoUserControl.RefreshRandomMusicEvent += _websiteMusicController.RefreshRandomMusicAsync;
            SuperMusicUserControl.FindMusicEvent += _websiteMusicController.FindMusicAsync;
            SuperMusicUserControl.RefreshRandomMusicEvent += _websiteMusicController.RefreshRandomMusicAsync;

            _musicItemsController.ClickOnMusicEvent += OnMusicClick;
            _mp3Player.ChangeVolume(VolumeSlider.Value);
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Init();
            AddLocalMusicFromPlayerFolder();
            MakeTestConnectionsAsync();
        }

        private void OnBorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) {
                return;
            }

            DragMove();
        }

        private void OnCloseAppButtonCLick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void OnHideAppButtonCLick(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void OnLocalMusicButtonClick(object sender, RoutedEventArgs e) => NavigateTo(LocalMusicUserControl, null);

        private void OnHitmoMusicButtonClick(object sender, RoutedEventArgs e) => NavigateTo(HitmoUserControl, _hitmoParser);

        private void OnSuperMusicButtonClick(object sender, RoutedEventArgs e) => NavigateTo(SuperMusicUserControl, _superMusicParser);

        private void OnMusicClick(string musicLink, string? playlistName = null)
        {
            playlistName ??= _currentPlayerUserControl.CurrentLoadedPlaylistName;
            _musicItemsController.SetCurrentPlayingMusicData(musicLink, _currentPlayerUserControl);
            _currentPlayingPlaylistController.Play(musicLink, playlistName, _currentParser);
            SetPlayButtonImage();
        }

        private void OnPauseButtonClick(object sender, RoutedEventArgs e)
        {
            _mp3Player.Pause();
            SetPlayButtonImage();
        }

        private void OnPrevButtonClick(object sender, RoutedEventArgs e)
        {
            _currentPlayingPlaylistController.PlayPrev();
            _musicItemsController.SetCurrentPlayingMusicData(_mp3Player.CurrentMusic!, _currentPlayerUserControl);
            SetPlayButtonImage();
        }

        private void OnNextButtonClick(object sender, RoutedEventArgs e)
        {
            _currentPlayingPlaylistController.PlayNext();
            _musicItemsController.SetCurrentPlayingMusicData(_mp3Player.CurrentMusic!, _currentPlayerUserControl);
            SetPlayButtonImage();
        }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is not Slider slider) {
                return;
            }
            if (_mp3Player == null) {
                return;
            }

            _mp3Player.ChangeVolume(slider.Value);
        }

        private async void OnDownloadButtonClick(object sender, RoutedEventArgs e)
        {
            if (_alreadyDownload) {
                return;
            }
            if (_mp3Player.CurrentMusic == null) {
                return;
            }
            if (_currentPlayingPlaylistController.CurrentPlayingPlaylist.PlaylistName == LocalMusicUserControl.LOCAL_MUSIC_PLAYLIST_NAME) {
                return;
            }

            _alreadyDownload = true;
            await HttpController.DownloadFile(_mp3Player.CurrentMusic, CurrentPlayingMusicName.Text, ".mp3");
            _alreadyDownload = false;
        }

        private void AddLocalMusicFromPlayerFolder()
        {
            Directory.CreateDirectory(Mp3PlayerFolder.GetPlayerFolder());
            LocalMusicUserControl.AddMusicFromPlayerFolder();
        }

        private async void MakeTestConnectionsAsync()
        {
            InstallInitialMusicService installInitialMusicService = new(_musicRepository, _musicItemsController);
            if (await HttpController.HasConnection(HitmoParser.HOST)) {
                _hitmoParser = new();
                HitmoButton.IsEnabled = true;
                installInitialMusicService.SetStartMusicOnUserControl(HitmoUserControl, _hitmoParser, _websiteMusicController.Playlist);
            }

            if (await HttpController.HasConnection(SuperMusicParser.HOST)) {
                _superMusicParser = new();
                SuperMusicButton.IsEnabled = true;
                installInitialMusicService.SetStartMusicOnUserControl(SuperMusicUserControl, _superMusicParser, _websiteMusicController.Playlist);
            }
        }

        private void NavigateTo(IPlayerUserControl userControl, IWebSiteParser? webSiteParser)
        {
            _navigationService.NavigateTo((UserControl) userControl);
            _currentParser = webSiteParser;
            _currentPlayerUserControl = userControl;

            if (webSiteParser == null) {
                return;
            }
            _websiteMusicController.CurrentWebSiteParser = _currentParser!;
            _websiteMusicController.CurrentPlayerUserControl = (_currentPlayerUserControl as IWebSitePlayerUserControl)!;
        }

        private void SetPlayButtonImage() => _pauseButtonPackMaterial.Kind = _mp3Player.IsPlaying ? PackIconMaterialKind.Pause : PackIconMaterialKind.Play;
    }
}