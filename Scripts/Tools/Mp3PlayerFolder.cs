using System;

namespace SkullMp3Player.Scripts.Tools
{
    public static class Mp3PlayerFolder
    {
        public static string GetPlayerFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SkullMp3Player";
        }
    }
}
