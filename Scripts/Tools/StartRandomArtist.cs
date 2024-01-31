using System;
using System.Collections.Generic;

namespace SkullMp3Player.Scripts.Tools
{
    static class StartRandomArtist
    {
        private static List<string> _artists = new(){
                "Ария", "Король и шут", "Horus", 
                "Atl", "Amatory", "Англия", 
                "Murda Killa", "Грот", "Mr kitty", 
                "Linkin park", "Slipknot"
         };

        public static string GetRandomArtist()
        {
            return _artists[Random.Shared.Next(0, _artists.Count)];
        }
    }
}
