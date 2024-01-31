using SkullMp3Player.Scripts.Client.Controller;
using SkullMp3Player.Scripts.Parser.Base;
using SkullMp3Player.Scripts.Player.Music.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkullMp3Player.Scripts.Parser.SuperMusic
{
    internal class SuperMusicParser : WebsiteParser, IWebSiteParser
    {
        private readonly List<char> _alphabet = new() {
            'А','Б','В','Г','Д','Е','Ж','З','И','К',
            'Л','М','Н','О','П','Р','С','Т','У','Ф',
            'Х','Ц','Ч','Ш','Щ','Э','Ю','Я','1','2',
            '3','4','5','6','7','8','9','A','B','C',
            'D','E','F','G','H','I','J','K','L','M',
            'N','O','P','Q','R','S','T','U','V','W',
            'X', 'Y', 'Z'
        };

        public const string HOST = "https://wow19.supermusic.me";
        public const string NEW_MUSIC_LINK = "https://wow19.supermusic.me/lastnews/";
        public const string POPULAR_FOR_MONTHS = "https://wow19.supermusic.me/month.html";
        public const string POPULAR_FOR_HALF_YEAR = "https://wow19.supermusic.me/6_months.html";
        public const string POPULAR_FOR_YEAR = "https://wow19.supermusic.me/year.html";

        private const string FIND_MUSIC_LINK = "https://wow19.supermusic.me/search?story=";
        private const string RANDOM_ARTIST_LINK = "https://wow19.supermusic.me/singer/";
        private const string ARTIST_LINK = "https://wow19.supermusic.me/";

        private const string PLAYLIST_START_LABEL = "class=\"popular-play\"";
        private const string MUSIC_LINK_START_LABEL = "url=";
        private const string MUSIC_LINK_END_LABEL = "\" class=";
        private const string MUSIC_AUTHOR_START_LABEL = "composition";
        private const string MUSIC_AUTHORE_END_LABEL = "</a>";
        private const string MUSIC_TITLE_START_LABEL = "author";
        private const string MUSIC_TITLE_END_LABEL = "</a>";
        private const string HTML = ".html";

        public string NewMusic => NEW_MUSIC_LINK;
        public string PopularMusic => POPULAR_FOR_MONTHS;

        public async Task<List<MusicModel>?> GetMusicAsync(string url)
        {
            string? response = await HttpController.SendGetRequest(url);
            if (string.IsNullOrEmpty(response)) {
                return null;
            }

            List<MusicModel> result = new();
            List<int> musicIndexes = GetAllSubstringsIndexes(response, PLAYLIST_START_LABEL);

            Tuple<List<int>, List<int>> tupleMusicIndexes = GetIndexes(response, musicIndexes, MUSIC_LINK_START_LABEL, MUSIC_LINK_END_LABEL);
            List<string> musics = GetSubstringsByIndexes(response, tupleMusicIndexes.Item1, tupleMusicIndexes.Item2, 5);

            Tuple<List<int>, List<int>> tupleAuthorsIndexes = GetIndexes(response, tupleMusicIndexes.Item1, MUSIC_AUTHOR_START_LABEL, MUSIC_AUTHORE_END_LABEL);
            List<string> authors = GetSubstringsByIndexes(response, tupleAuthorsIndexes.Item1, tupleAuthorsIndexes.Item2, 13);

            Tuple<List<int>, List<int>> tupleTitleIndexes = GetIndexes(response, tupleAuthorsIndexes.Item1, MUSIC_TITLE_START_LABEL, MUSIC_TITLE_END_LABEL);
            List<string> titles = GetSubstringsByIndexes(response, tupleTitleIndexes.Item1, tupleTitleIndexes.Item2, 8);

            for (int i = 0; i < titles.Count; i++) {
                string title = titles[i];
                string author = authors[i];
                string music = musics[i];

                MusicModel musicModel = new(music, title, author, string.Empty);
                result.Add(musicModel);
            }

            return result;
        }

        public async Task<List<MusicModel>?> GetRandomMusicAsync()
        {
            List<MusicModel> result = new();
            for (int i = 0; i < 3; i++) {
                char randomLetter = _alphabet[GetRandomNumber(0, _alphabet.Count)];
                string randomArtistRequest = RANDOM_ARTIST_LINK + randomLetter + HTML;

                string? randomArtistResponse = await HttpController.SendGetRequest(randomArtistRequest);
                if (string.IsNullOrEmpty(randomArtistResponse)) {
                    return null;
                }

                string randomArtist = GetRandomArtist(randomArtistResponse);
                string? randomMusicResponse = await HttpController.SendGetRequest(ARTIST_LINK + randomArtist);
                if (string.IsNullOrEmpty(randomMusicResponse)) {
                    return null;
                }

                List<int> musicIndexes = GetAllSubstringsIndexes(randomMusicResponse, PLAYLIST_START_LABEL);
                int randomMusicIndex = GetRandomNumber(0, musicIndexes.Count);

                Tuple<List<int>, List<int>> tupleMusicIndexes = GetIndexes(randomMusicResponse, new List<int> { musicIndexes[randomMusicIndex] }, MUSIC_LINK_START_LABEL, MUSIC_LINK_END_LABEL);
                List<string> musics = GetSubstringsByIndexes(randomMusicResponse, tupleMusicIndexes.Item1, tupleMusicIndexes.Item2, 5);

                Tuple<List<int>, List<int>> tupleAuthorsIndexes = GetIndexes(randomMusicResponse, tupleMusicIndexes.Item1, MUSIC_AUTHOR_START_LABEL, MUSIC_AUTHORE_END_LABEL);
                List<string> authors = GetSubstringsByIndexes(randomMusicResponse, tupleAuthorsIndexes.Item1, tupleAuthorsIndexes.Item2, 13);

                Tuple<List<int>, List<int>> tupleTitleIndexes = GetIndexes(randomMusicResponse, tupleAuthorsIndexes.Item1, MUSIC_TITLE_START_LABEL, MUSIC_TITLE_END_LABEL);
                List<string> titles = GetSubstringsByIndexes(randomMusicResponse, tupleTitleIndexes.Item1, tupleTitleIndexes.Item2, 8);

                result.Add(new MusicModel(musics[0], titles[0], authors[0], string.Empty));
            }

            return result;
        }

        public async Task<List<MusicModel>?> FindMusicAsync(string searchText)
        {
            return await GetMusicAsync(FIND_MUSIC_LINK + searchText);
        }

        private string GetRandomArtist(string randomArtistResponse)
        {
            List<int> artistsListIndexes = GetAllSubstringsIndexes(randomArtistResponse, "list-artists");
            int randomArtistListIndex = artistsListIndexes[GetRandomNumber(0, artistsListIndexes.Count)];

            List<int> allArtistsIndexes = GetAllSubstringsIndexes(randomArtistResponse, "<li>", randomArtistListIndex);
            int randomArtistStartIndex = allArtistsIndexes[GetRandomNumber(0, allArtistsIndexes.Count)];
            int randomArtistEndIndex = randomArtistResponse.IndexOf(HTML, randomArtistStartIndex);
            return GetSubstringsByIndexes(randomArtistResponse, new List<int>() { randomArtistStartIndex }, new List<int>() { randomArtistEndIndex }, 13)[0] + HTML;
        }
    }
}
