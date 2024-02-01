using SkullMp3Player.Scripts.Player.Controller;
using SkullMp3Player.Scripts.Player.Music.Model;
using SkullMp3Player.Scripts.Player.Music.Repository;
using SkullMp3Player.Scripts.Player.Playlists.Model;
using SkullMp3Player.Scripts.Tools;
using System.Linq;

namespace SkullMp3Player.Scripts.Player.Playlists.Controller
{
    class CurrentPlayingPlaylistController
    {
        private Mp3Player _mp3Player;
        private MusicRepository _musicRepository;
        private int _currentMusicIndex = 0;

        public PlaylistModel CurrentPlayingPlaylist { get; set; } = null!;
        public string? CurrentPlayingMusic => _mp3Player.CurrentMusic;

        public CurrentPlayingPlaylistController(Mp3Player mp3Player, MusicRepository musicRepository)
        {
            _mp3Player = mp3Player;
            _musicRepository = musicRepository;
            CurrentPlayingPlaylist = new(new(), "");
        }

        public void Play(string playlistName, IWebSiteParser webSiteParser)
        {
            CurrentPlayingPlaylist = _musicRepository.GetPlaylistModel(webSiteParser, playlistName)!;
            Play(CurrentPlayingPlaylist.MusicModels.First().Link);
        }

        public void Play(string musicLink, string playlistName)
        {
            if (CurrentPlayingPlaylist == null || CurrentPlayingPlaylist.PlaylistName != playlistName) {
                CurrentPlayingPlaylist = _musicRepository.GetPlaylistModel(playlistName);
            }

            Play(musicLink);
        }

        public void Play(string musicLink, string playlistName, IWebSiteParser webSiteParser)
        {
            if (CurrentPlayingPlaylist == null || CurrentPlayingPlaylist.PlaylistName != PlaylistNameCreator.GetNewPlaylistName(playlistName, webSiteParser!.GetType())) {
                CurrentPlayingPlaylist = _musicRepository.GetPlaylistModel(webSiteParser, playlistName)!;
            }

            Play(musicLink);
        }

        public void Play(string musicLink)
        {
            UpdateCurrentMusicIndex(musicLink);
            _mp3Player.Play(musicLink);
        }

        public void PlayNext()
        {
            if (CurrentPlayingPlaylist == null) {
                return;
            }
            if (CurrentPlayingPlaylist.MusicModels.IsNullOrEmpty()) {
                return;
            }
            if (_currentMusicIndex + 1 >= CurrentPlayingPlaylist.MusicModels.Count) {
                _currentMusicIndex = -1;
            }

            _currentMusicIndex++;
            _mp3Player.Play(CurrentPlayingPlaylist.MusicModels[_currentMusicIndex].Link);
        }

        public void PlayPrev()
        {
            if (CurrentPlayingPlaylist == null) {
                return;
            }
            if (_currentMusicIndex - 1 < 0) {
                return;
            }

            _currentMusicIndex--;
            _mp3Player.Play(CurrentPlayingPlaylist.MusicModels[_currentMusicIndex].Link);
        }

        public void StopMusic()
        {
            _mp3Player.Stop();
        }

        public void RemovePlaylist()
        {
            CurrentPlayingPlaylist = null!;
            _mp3Player.Stop();
        }

        public void RemoveMusic(string musicLink)
        {
            foreach (MusicModel music in CurrentPlayingPlaylist.MusicModels) {
                if (music.Link != musicLink) {
                    continue;
                }

                CurrentPlayingPlaylist.MusicModels.Remove(music);
                return;
            }
        }

        public void UpdateCurrentMusicIndex(string? musicLink = null)
        {
            if (CurrentPlayingPlaylist == null) {
                return;
            }
            if (CurrentPlayingMusic == null) {
                return;
            }

            musicLink ??= CurrentPlayingMusic;
            _currentMusicIndex = CurrentPlayingPlaylist.MusicModels.IndexOf(CurrentPlayingPlaylist.MusicModels.First(x => x.Link == musicLink));
        }
    }
}
