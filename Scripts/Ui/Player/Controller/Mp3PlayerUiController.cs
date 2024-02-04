using SkullMp3Player.Scripts.Client.Controller;
using SkullMp3Player.Scripts.Player.Controller;
using SkullMp3Player.Scripts.Player.Playlists.Controller;
using SkullMp3Player.Scripts.Ui.MusicItems.Controller;
using SkullMp3Player.UserControls;
using System.Windows.Controls;
using System.Windows;
using SkullMp3Player.UserControls.Interface;
using MahApps.Metro.IconPacks;
using System.Windows.Controls.Primitives;

namespace SkullMp3Player.Scripts.Ui.Player.Controller
{
    public class Mp3PlayerUiController
    {
        private Mp3Player _mp3Player;
        private CurrentPlayingPlaylistController _currentPlayingPlaylistController;
        private MusicItemsController _musicItemsController;

        private PackIconMaterial _pauseButtonPackMaterial = null!;
        private TextBlock _currentPlayingMusicNameTextBlock;
        private Slider _volumeSlider;
        private bool _alreadyDownload;

        public IPlayerUserControl CurrentPlayerUserControl { get; set; }

        public Mp3PlayerUiController(Mp3Player mp3Player, CurrentPlayingPlaylistController currentPlayingPlaylistController,
            MusicItemsController musicItemsController, IPlayerUserControl playerUserControl, Slider volumeSlider, Slider positionSlider, TextBlock currentPlayingMusicName,
            Button pauseButton, Button prevPlayButton, Button nextPlayButton, Button volumeButton, Button downloadButton)
        {
            _mp3Player = mp3Player;
            _currentPlayingPlaylistController = currentPlayingPlaylistController;
            _musicItemsController = musicItemsController;
            _volumeSlider = volumeSlider;
            CurrentPlayerUserControl = playerUserControl;
            _currentPlayingMusicNameTextBlock = currentPlayingMusicName;
            _pauseButtonPackMaterial = (PackIconMaterial)pauseButton.Content;

            _mp3Player.EndSoundEvent += () => OnNextButtonClick(this, null!);

            pauseButton.Click += OnPauseButtonClick;
            prevPlayButton.Click += OnPrevButtonClick;
            nextPlayButton.Click += OnNextButtonClick;
            volumeButton.Click += OnVolumeButtonClick;
            downloadButton.Click += OnDownloadButtonClick;

            _volumeSlider.ValueChanged += OnVolumeSliderValueChanged;
            Thumb positionSliderThumb = GetThumb(positionSlider);
            positionSliderThumb.DragStarted += OnMusicPositionSliderDragStarted;
            positionSliderThumb.DragCompleted += OnMusicPositionSliderDragCompleted;
        }

        public void SetPlayButtonImage() => _pauseButtonPackMaterial.Kind = _mp3Player.IsPlaying ? PackIconMaterialKind.Pause : PackIconMaterialKind.Play;

        private void OnPauseButtonClick(object sender, RoutedEventArgs e)
        {
            if (_mp3Player.CurrentMusic == null) {
                return;
            }

            _mp3Player.Pause();
            SetPlayButtonImage();
        }

        private void OnPrevButtonClick(object sender, RoutedEventArgs e)
        {
            _currentPlayingPlaylistController.PlayPrev();
            _musicItemsController.SetCurrentPlayingMusicData(_currentPlayingPlaylistController.CurrentPlayingMusic!, CurrentPlayerUserControl);
            SetPlayButtonImage();
        }

        private void OnNextButtonClick(object sender, RoutedEventArgs e)
        {
            _musicItemsController.ClearCurrentPlayingMusicData();
            _currentPlayingPlaylistController.PlayNext();
            _musicItemsController.SetCurrentPlayingMusicData(_mp3Player.CurrentMusic!, CurrentPlayerUserControl);
            SetPlayButtonImage();
        }

        private void OnVolumeButtonClick(object sender, RoutedEventArgs e)
        {
            switch (_volumeSlider.Visibility)
            {
                case Visibility.Visible:
                    _volumeSlider.Visibility = Visibility.Collapsed;
                    return;
                case Visibility.Collapsed:
                    _volumeSlider.Visibility = Visibility.Visible;
                    return;
            }
        }

        private void OnMusicPositionSliderDragStarted(object sender, DragStartedEventArgs e)
        {
            _mp3Player.Pause();
        }

        private void OnMusicPositionSliderDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (sender is not Thumb thumb) {
                return;
            }
            if (thumb.TemplatedParent is not Slider slider) {
                return;
            }

            _mp3Player.ChangeMusicPosition(slider.Value);
            _mp3Player.Pause();
        }

        private void OnVolumeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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
            await HttpController.DownloadFile(_mp3Player.CurrentMusic, _currentPlayingMusicNameTextBlock.Text, ".mp3");
            _alreadyDownload = false;
        }

        private Thumb GetThumb(Slider slider)
        {
            return (slider.Template.FindName("PART_Track", slider) as Track)!.Thumb;
        }
    }
}
