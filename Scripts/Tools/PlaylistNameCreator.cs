using System;

namespace SkullMp3Player.Scripts.Tools
{
    public static class PlaylistNameCreator
    {
        public static string GetNewPlaylistName(string playlistName, Type type)
        {
            return (playlistName + "//" + type.ToString()).ToLower();
        }
    }
}
