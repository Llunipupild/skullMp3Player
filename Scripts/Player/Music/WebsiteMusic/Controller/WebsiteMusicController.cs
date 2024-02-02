using SkullMp3Player.Scripts.Player.Music.Model;
using SkullMp3Player.Scripts.Player.Music.Repository;
using SkullMp3Player.Scripts.Player.Playlists.Controller;
using SkullMp3Player.Scripts.Ui.MusicItems.Controller;
using SkullMp3Player.UserControls;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading.Tasks;
using SkullMp3Player.Scripts.Player.Playlists.Model;
using SkullMp3Player.Scripts.Tools;
using SkullMp3Player.UserControls.PlayersUserControls.Interfaces;

namespace SkullMp3Player.Scripts.Player.Music.WebsiteMusic.Controller
{
    class WebsiteMusicController
    {
        private MusicRepository _musicRepository;
        private MusicItemsController _musicItemsController;
        private CurrentPlayingPlaylistController _currentPlayingPlaylistController;

        private bool _alreadyRefresh;
        private bool _alreadyLooking;
        private PlaylistItemUserControl? _pressedPlaylistItem;

        public IWebSitePlayerUserControl CurrentPlayerUserControl { get; set; } = null!;
        public IWebSiteParser CurrentWebSiteParser { get; set; } = null!;

        public event UpdatePlayButtonImage UpdatePlayButtonImageEvent = null!;
        public delegate void UpdatePlayButtonImage();

        public WebsiteMusicController(MusicRepository musicRepository, MusicItemsController musicItemsController, CurrentPlayingPlaylistController currentPlayingPlaylistController)
        {
            _musicRepository = musicRepository;
            _musicItemsController = musicItemsController;
            _currentPlayingPlaylistController = currentPlayingPlaylistController;
        }

        public void Playlist(object sender, MouseButtonEventArgs e)
        {
            if (sender is not PlaylistItemUserControl playlistItem) {
                return;
            }

            CurrentPlayerUserControl.FindMusicTextBlock!.Visibility = System.Windows.Visibility.Collapsed;
            CurrentPlayerUserControl.CurrentLoadedPlaylistName = playlistItem.PlaylistName;
            PlaylistModel plalyistModel = _musicRepository.GetPlaylistModel(CurrentWebSiteParser, CurrentPlayerUserControl.CurrentLoadedPlaylistName)!;
            playlistItem.IsActive = true;

            _musicItemsController.ClearPanel(CurrentPlayerUserControl.MusicListStackPanel);
            _musicItemsController.ClearCurrentPlayingMusicData();
            _musicItemsController.AddRangeMusicItem(plalyistModel.MusicModels, CurrentPlayerUserControl.MusicListStackPanel);
            _currentPlayingPlaylistController.Play(playlistItem.PlaylistName, CurrentWebSiteParser!);
            _musicItemsController.SetCurrentPlayingMusicData(_currentPlayingPlaylistController.CurrentPlayingMusic!, CurrentPlayerUserControl);
            _musicItemsController.UpdateMusicItemPanelEvent += OnUpdateMusicItemPanel;
            _pressedPlaylistItem = playlistItem;

            UpdatePlayButtonImageEvent?.Invoke();
        }

        public async void FindMusicAsync(string findMusicText)
        {
            if (_alreadyLooking) {
                return;
            }

            //может одновременно несколько поисков музыки по сайтам. поэтому загоняем в переменные
            _alreadyLooking = true;
            IWebSiteParser webSiteParser = CurrentWebSiteParser;
            IWebSitePlayerUserControl playerUserControl = CurrentPlayerUserControl;
            List<MusicModel>? musicModels = null;

            PlaylistModel? playlistModel = _musicRepository.GetPlaylistModel(webSiteParser!, findMusicText);
            if (playlistModel == null) {
                musicModels = await FindMusicModelsAsync(findMusicText);
                if (musicModels.IsNullOrEmpty()) {
                    _alreadyLooking = false;
                    return;
                }
            }

            musicModels ??= playlistModel!.MusicModels;
            playerUserControl.CurrentLoadedPlaylistName = findMusicText;
            playerUserControl.FindMusicTextBlock!.Visibility = System.Windows.Visibility.Visible;
            playerUserControl.FindMusicTextBlock!.Text = $"Музыка по запросу \"{findMusicText}\"";
            playerUserControl.FindMusicTextBox!.Text = string.Empty;
            _musicItemsController.ClearPanel(playerUserControl.MusicListStackPanel);
            _musicItemsController.AddRangeMusicItem(musicModels, playerUserControl.MusicListStackPanel);
            _currentPlayingPlaylistController.StopMusic();
            _alreadyLooking = false;
        }

        public async void RefreshRandomMusicAsync()
        {
            if (_alreadyRefresh) {
                return;
            }

            _alreadyRefresh = true;
            IWebSiteParser webSiteParser = CurrentWebSiteParser;
            IWebSitePlayerUserControl playerUserControl = CurrentPlayerUserControl;

            List<MusicModel>? musicModels = await webSiteParser.GetRandomMusicAsync();
            if (musicModels.IsNullOrEmpty()) {
                _alreadyRefresh = false;
                return;
            }

            if (_currentPlayingPlaylistController.CurrentPlayingPlaylist.PlaylistName == PlaylistNameCreator.GetNewPlaylistName(RandomMusicItemUserControl.RANDOM_PLAYLIST_NAME, webSiteParser.GetType())) {
                _currentPlayingPlaylistController.RemovePlaylist();
                _musicItemsController.ClearCurrentPlayingMusicData();
            }

            _musicItemsController.ClearPanel(playerUserControl.RandomMusicListStackPanel!);
            _musicRepository.RemoveMusic(webSiteParser, RandomMusicItemUserControl.RANDOM_PLAYLIST_NAME);
            _musicItemsController.AddRandomMusicItem(musicModels!, playerUserControl.RandomMusicListStackPanel!);
            _musicRepository.AddMusicToRepository(webSiteParser, musicModels!, RandomMusicItemUserControl.RANDOM_PLAYLIST_NAME);
            _alreadyRefresh = false;
        }

        private void OnUpdateMusicItemPanel()
        {
            if (_pressedPlaylistItem != null) {
                _pressedPlaylistItem.IsActive = false;
                _pressedPlaylistItem = null;
            }

            _musicItemsController.UpdateMusicItemPanelEvent -= OnUpdateMusicItemPanel;
        }

        private async Task<List<MusicModel>?> FindMusicModelsAsync(string findMusicText)
        {
            List<MusicModel>? findedMusicModels = await CurrentWebSiteParser.FindMusicAsync(findMusicText);
            if (findedMusicModels.IsNullOrEmpty()) {
                CurrentPlayerUserControl.FindMusicTextBlock!.Visibility = System.Windows.Visibility.Visible;
                CurrentPlayerUserControl.FindMusicTextBox!.Text = string.Empty;
                CurrentPlayerUserControl.FindMusicTextBlock!.Text = $"По запросу {findMusicText} ничего не найдено";
                return null;
            }

            _musicRepository.AddMusicToRepository(CurrentWebSiteParser, findedMusicModels!, findMusicText);
            return findedMusicModels;
        }
    }
}
