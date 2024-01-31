using SkullMp3Player.Scripts.Player.Music.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkullMp3Player.Scripts
{
    interface IWebSiteParser
    {
        public string NewMusic { get; }
        public string PopularMusic { get; }
        public Task<List<MusicModel>?> GetMusicAsync(string url);
        public Task<List<MusicModel>?> GetRandomMusicAsync();
        public Task<List<MusicModel>?> FindMusicAsync(string searchText);
    }
}
