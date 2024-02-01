using SkullMp3Player.Scripts.Player.Music.Model;
using SkullMp3Player.Scripts.Player.Music.Repository;
using SkullMp3Player.Scripts.Player.Playlists.Controller;
using SkullMp3Player.Scripts.Ui.MusicItems.Controller;
using SkullMp3Player.UserControls;
using System.Collections.Generic;

namespace SkullMp3Player.Scripts.Player.Music.LocalMusic.Controller
{
    internal class LocalMusicController
    {
        private MusicRepository _musicRepository;
        private MusicItemsController _musicItemsController;
        private CurrentPlayingPlaylistController _currentPlayingPlaylistController;
        private LocalMusicUserControl _localMusicUserControl;

        public LocalMusicController(MusicRepository musicRepository, MusicItemsController musicItemsController, CurrentPlayingPlaylistController currentPlayingPlaylistController, LocalMusicUserControl localMusicUserControl) 
        {
            _musicRepository = musicRepository;
            _musicItemsController = musicItemsController;
            _currentPlayingPlaylistController = currentPlayingPlaylistController;
            _localMusicUserControl = localMusicUserControl;
        }

        public void AddMusic(List<MusicModel> musicModels)
        {
            _localMusicUserControl.CurrentLoadedPlaylistName = LocalMusicUserControl.LOCAL_MUSIC_PLAYLIST_NAME;
            _musicRepository.AddLocalMusic(musicModels);
            _musicItemsController.AddRangeMusicItem(musicModels, _localMusicUserControl.MusicListStackPanel);
        }

        public void RemoveMusic()
        {
            if (_currentPlayingPlaylistController.CurrentPlayingMusic == null) {
                return;
            }
            if (_currentPlayingPlaylistController.CurrentPlayingPlaylist.PlaylistName != LocalMusicUserControl.LOCAL_MUSIC_PLAYLIST_NAME) {
                return;
            }

            _currentPlayingPlaylistController.RemoveMusic(_currentPlayingPlaylistController.CurrentPlayingMusic);
            _musicRepository.RemoveLocalMusic(_currentPlayingPlaylistController.CurrentPlayingMusic);
            _musicItemsController.RemoveMusicItem(_currentPlayingPlaylistController.CurrentPlayingMusic, _localMusicUserControl.MusicListStackPanel);
            _localMusicUserControl.RemoveOnAddedMusic(_currentPlayingPlaylistController.CurrentPlayingMusic);
            _currentPlayingPlaylistController.StopMusic();
            _currentPlayingPlaylistController.PlayPrev();
            _musicItemsController.SetCurrentPlayingMusicData(_currentPlayingPlaylistController.CurrentPlayingMusic, _localMusicUserControl);

            if (_currentPlayingPlaylistController.CurrentPlayingPlaylist.MusicModels.Count == 0) {
                _musicItemsController.ClearCurrentPlayingMusicData();
            }
        }

        public void ShuffleMusic()
        {
            _musicItemsController.ShufflePanelChildrens(_localMusicUserControl.MusicListStackPanel, _musicRepository.LocalMusicPlaylist.MusicModels);
            _currentPlayingPlaylistController.UpdateCurrentMusicIndex();
        }
    }
}
