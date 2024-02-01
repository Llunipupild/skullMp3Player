using SkullMp3Player.Scripts.Player.Music.Model;
using SkullMp3Player.Scripts.Player.Playlists.Model;
using SkullMp3Player.Scripts.Tools;
using SkullMp3Player.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkullMp3Player.Scripts.Player.Music.Repository
{
    class MusicRepository
    {
        private Dictionary<IWebSiteParser, List<PlaylistModel>> _cachedMusicModels;

        public PlaylistModel LocalMusicPlaylist { get; }

        public MusicRepository()
        {
            _cachedMusicModels = new();
            LocalMusicPlaylist = new(new List<MusicModel>(), LocalMusicUserControl.LOCAL_MUSIC_PLAYLIST_NAME);
        }

        public void AddParser(IWebSiteParser webSiteParser)
        {
            if (!_cachedMusicModels.ContainsKey(webSiteParser)) {
                _cachedMusicModels.Add(webSiteParser, new List<PlaylistModel>());
            }
        }

        public void AddMusicToRepository(IWebSiteParser webSiteParser, List<MusicModel> musicModels, string playlistName)
        {
            PlaylistModel playlistModel = new(musicModels, PlaylistNameCreator.GetNewPlaylistName(playlistName, webSiteParser.GetType()));
            _cachedMusicModels[webSiteParser].Add(playlistModel);
        }

        public void RemoveMusic(IWebSiteParser webSiteParser, string playlistName)
        {
            if (!_cachedMusicModels.ContainsKey(webSiteParser)) {
                return;
            }

            playlistName = PlaylistNameCreator.GetNewPlaylistName(playlistName, webSiteParser.GetType());
            PlaylistModel? playlistModel = _cachedMusicModels[webSiteParser].FirstOrDefault(pm => pm.PlaylistName == playlistName);
            if (playlistModel == null) {
                return;
            }

            _cachedMusicModels[webSiteParser].Remove(playlistModel);
        }

        public void AddLocalMusic(List<MusicModel> musicModels)
        {
            musicModels.ForEach(LocalMusicPlaylist.MusicModels.Add);
        }

        public void RemoveLocalMusic(string musicLink)
        {
            MusicModel? musicModel = LocalMusicPlaylist.MusicModels.FirstOrDefault(m => m.Link == musicLink);
            if (musicModel == null) {
                return;
            }

            LocalMusicPlaylist.MusicModels.Remove(musicModel);
        }

        public PlaylistModel GetPlaylistModel(string playlistName)
        {
            if (playlistName == LocalMusicUserControl.LOCAL_MUSIC_PLAYLIST_NAME) {
                return LocalMusicPlaylist;
            }

            throw new NullReferenceException($"{playlistName} unknown playlist");
        }

        public PlaylistModel? GetPlaylistModel(IWebSiteParser webSiteParser, string playlistName)
        {
            if (!_cachedMusicModels.ContainsKey(webSiteParser)) {
                throw new NullReferenceException($"{webSiteParser.GetType()} not cached");
            }

            playlistName = PlaylistNameCreator.GetNewPlaylistName(playlistName, webSiteParser.GetType());
            return _cachedMusicModels[webSiteParser].FirstOrDefault(pm => pm.PlaylistName == playlistName);
        }
    }
}

