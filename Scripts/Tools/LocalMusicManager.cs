using System.IO;

namespace SkullMp3Player.Scripts.Tools
{
    public static class LocalMusicManager
    {
        public static string[]? GetLocalMusic() 
        {
            return Directory.GetFiles(Mp3PlayerFolder.GetPlayerFolder());
        }
    }
}
