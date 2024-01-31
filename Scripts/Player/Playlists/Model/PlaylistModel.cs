using SkullMp3Player.Scripts.Player.Music.Model;
using System.Collections.Generic;

namespace SkullMp3Player.Scripts.Player.Playlists.Model
{
    public class PlaylistModel
    {
        public List<MusicModel> MusicModels { get; }
        public string PlaylistName { get; }

        public PlaylistModel(List<MusicModel> musicModels, string playlistName)
        {
            MusicModels = musicModels;
            PlaylistName = playlistName;
        }
    }
}
