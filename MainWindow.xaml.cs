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
using SkullMp3Player.Scripts.Ui.Player.Controller;
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
        private Mp3PlayerUiController _mp3PlayerUiController = null!;
        private CurrentPlayingPlaylistController _currentPlayingPlaylistController = null!;

        private WebsiteMusicController _websiteMusicController = null!;
        private LocalMusicController _localMusicController = null!;

        private HitmoParser _hitmoParser = null!;
        private SuperMusicParser _superMusicParser = null!;

        private IWebSiteParser? _currentParser;
        private IPlayerUserControl _currentPlayerUserControl = null!;
        private MusicItemsController _musicItemsController = null!;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Init()
        {
            _mp3Player = new(CurrentPlayingMusicTime, MusicPositionSlider);
            _musicRepository = new();
            _currentPlayingPlaylistController = new(_mp3Player, _musicRepository);
            _navigationService = new(LocalMusicUserControl);

            _musicItemsController = new(CurrentPlayingMusicName, CurrentPlayingMusicAuthor, CurrentPlayingMusicImage);
            _websiteMusicController = new(_musicRepository, _musicItemsController, _currentPlayingPlaylistController);
            _localMusicController = new(_musicRepository, _musicItemsController, _currentPlayingPlaylistController, LocalMusicUserControl);

            _currentPlayerUserControl = LocalMusicUserControl;
            _currentParser = null;
            HitmoButton.IsEnabled = false;
            SuperMusicButton.IsEnabled = false;

            _mp3PlayerUiController = new(_mp3Player, _currentPlayingPlaylistController, _musicItemsController, _currentPlayerUserControl, VolumeSlider, MusicPositionSlider, CurrentPlayingMusicName, PauseButton, PrevMusicButton, NextMusicButton, VolumeButton, DownloadButton);
            _websiteMusicController.UpdatePlayButtonImageEvent += _mp3PlayerUiController.SetPlayButtonImage;

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

        public void OnMusicClick(string musicLink, string? playlistName = null)
        {
            playlistName ??= _currentPlayerUserControl.CurrentLoadedPlaylistName;
            _musicItemsController.SetCurrentPlayingMusicData(musicLink, _currentPlayerUserControl);
            if (_currentParser == null) {
                _currentPlayingPlaylistController.Play(musicLink, playlistName);
            } else {
                _currentPlayingPlaylistController.Play(musicLink, playlistName, _currentParser);
            }

            _mp3PlayerUiController.SetPlayButtonImage();
        }

        private void OnCloseAppButtonCLick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void OnHideAppButtonCLick(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void OnLocalMusicButtonClick(object sender, RoutedEventArgs e) => NavigateTo(LocalMusicUserControl, null);

        private void OnHitmoMusicButtonClick(object sender, RoutedEventArgs e) => NavigateTo(HitmoUserControl, _hitmoParser);

        private void OnSuperMusicButtonClick(object sender, RoutedEventArgs e) => NavigateTo(SuperMusicUserControl, _superMusicParser);

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
                _musicRepository.AddParser(_hitmoParser);
                HitmoButton.IsEnabled = true;
                installInitialMusicService.SetStartMusicOnUserControl(HitmoUserControl, _hitmoParser, _websiteMusicController.Playlist);
            }

            if (await HttpController.HasConnection(SuperMusicParser.HOST)) {
                _superMusicParser = new();
                _musicRepository.AddParser(_superMusicParser);
                SuperMusicButton.IsEnabled = true;
                installInitialMusicService.SetStartMusicOnUserControl(SuperMusicUserControl, _superMusicParser, _websiteMusicController.Playlist);
            }
        }

        private void NavigateTo(IPlayerUserControl userControl, IWebSiteParser? webSiteParser)
        {
            _navigationService.NavigateTo((UserControl) userControl);
            _currentParser = webSiteParser;
            _currentPlayerUserControl = userControl;
            _mp3PlayerUiController.CurrentPlayerUserControl = _currentPlayerUserControl;

            if (webSiteParser == null) {
                return;
            }

            _websiteMusicController.CurrentWebSiteParser = _currentParser!;
            _websiteMusicController.CurrentPlayerUserControl = (_currentPlayerUserControl as IWebSitePlayerUserControl)!;
        }
    }
}