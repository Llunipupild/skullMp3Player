using SkullMp3Player.Scripts.Client.Controller;
using SkullMp3Player.Scripts.Parser.Base;
using SkullMp3Player.Scripts.Player.Music.Model;
using SkullMp3Player.Scripts.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkullMp3Player.Scripts.Parser.Hitmo
{
    class HitmoParser : WebsiteParser, IWebSiteParser
    {
        public const string HOST = "https://rus.hitmotop.com";
        public const string NEW_MUSIC_LINK = "https://rus.hitmotop.com/songs/new";
        public const string POPULAR_MUSIC_LINK = "https://rus.hitmotop.com/songs/top-today";

        private const string FIND_MUSIC_LINK = "https://rus.hitmotop.com/search?q=";
        private const string RANDOM_ARTIST_LINK = "https://rus.hitmotop.com/artist/";

        private const string PLAYLIST_START_LABEL = "<div class=\"track__play\">";
        private const string MUSIC_START_IMAGE_LABEL = "url(";
        private const string MUSIC_END_IMAGE_LABEL = "');";
        private const string MUSIC_LINK_START_LABEL = "ax href=\"";
        private const string MUSIC_LINK_END_LABEL = "\" class=\"track";
        private const string MUSIC_TITLE_START_LABEL = "title\">";
        private const string MUSIC_TITLE_END_LABEL = "</div>";
        private const string MUSIC_AUTHOR_START_LABEL = "desc\">"; 
        private const string MUSIC_AUTHOR_END_LABEL = "</div>";

        public string NewMusic => NEW_MUSIC_LINK;
        public string PopularMusic => POPULAR_MUSIC_LINK;

        public async Task<List<MusicModel>?> GetMusicAsync(string url)
        {
            string? response = await HttpController.SendGetRequest(url);
            if (string.IsNullOrEmpty(response)) {
                return null;
            }

            List<MusicModel> result = new();
            List<int> musicIndexes = GetAllSubstringsIndexes(response, PLAYLIST_START_LABEL);

            Tuple<List<int>, List<int>> tupleImagesIndexes = GetIndexes(response, musicIndexes, MUSIC_START_IMAGE_LABEL, MUSIC_END_IMAGE_LABEL);
            List<string> images = GetSubstringsByIndexes(response, tupleImagesIndexes.Item1, tupleImagesIndexes.Item2, 5);

            Tuple<List<int>, List<int>> tupleMusicIndexes = GetIndexes(response, tupleImagesIndexes.Item1, MUSIC_LINK_START_LABEL, MUSIC_LINK_END_LABEL);
            List<string> musics = GetSubstringsByIndexes(response, tupleMusicIndexes.Item1, tupleMusicIndexes.Item2, 9);

            Tuple<List<int>, List<int>> tupleTitleIndexes = GetIndexes(response, tupleImagesIndexes.Item1, MUSIC_TITLE_START_LABEL, MUSIC_TITLE_END_LABEL);
            List<string> titles = GetSubstringsByIndexes(response, tupleTitleIndexes.Item1, tupleTitleIndexes.Item2, 7);

            Tuple<List<int>, List<int>> tupleAuthorIndexes = GetIndexes(response, tupleTitleIndexes.Item1, MUSIC_AUTHOR_START_LABEL, MUSIC_AUTHOR_END_LABEL);
            List<string> authors = GetSubstringsByIndexes(response, tupleAuthorIndexes.Item1, tupleAuthorIndexes.Item2, 6);

            for (int i = 0; i < musics.Count; i++) {
                string title = titles[i];
                string author = authors[i];
                string image = images[i];
                string music = musics[i];

                MusicModel musicModel = new(music, title, author, image);
                result.Add(musicModel);
            }

            return result;
        }

        public async Task<List<MusicModel>?> GetRandomMusicAsync()
        {
            List<MusicModel> result = new();
            bool musicFind = false;
            bool responseIsNull = false;
            int countAttempts = 0;

            for (int i = 0; i < 3; i++) {
                musicFind = false;
                while (!musicFind && !responseIsNull) {
                    int randomNumber = GetRandomNumber();
                    string randomRequest = RANDOM_ARTIST_LINK + randomNumber;
                    string? response = await HttpController.SendGetRequest(randomRequest);
                    if (string.IsNullOrEmpty(response)) {
                        countAttempts++;
                        if (countAttempts == 3) {
                            responseIsNull = true;
                        }
                        continue;
                    }
                    if (response.Contains("Слушать и скачать музыку бесплатно: новинки Mp3 музыки - Hitmo")) {
                        continue;
                    }

                    MusicModel? music = GetRandomMusic(response);
                    if (music == null) {
                        continue;
                    }

                    result.Add(music);
                    musicFind = true; 
                }
            }

            return result;
        }

        public async Task<List<MusicModel>?> FindMusicAsync(string searchText)
        {
            return await GetMusicAsync(FIND_MUSIC_LINK + searchText);
        }

        private static MusicModel? GetRandomMusic(string response)
        {
            List<int> musicIndexes = GetAllSubstringsIndexes(response, PLAYLIST_START_LABEL);
            if (musicIndexes.IsNullOrEmpty()) {
                return null;
            }

            int randomMusicIndex = GetRandomNumber(0, musicIndexes.Count);

            Tuple<List<int>, List<int>> tupleImagesIndexes = GetIndexes(response, new List<int>() { musicIndexes[randomMusicIndex] }, MUSIC_START_IMAGE_LABEL, MUSIC_END_IMAGE_LABEL);
            List<string> images = GetSubstringsByIndexes(response, tupleImagesIndexes.Item1, tupleImagesIndexes.Item2, 5);

            Tuple<List<int>, List<int>> tupleMusicIndexes = GetIndexes(response, tupleImagesIndexes.Item1, MUSIC_LINK_START_LABEL, MUSIC_LINK_END_LABEL);
            List<string> musics = GetSubstringsByIndexes(response, tupleMusicIndexes.Item1, tupleMusicIndexes.Item2, 9);

            Tuple<List<int>, List<int>> tupleTitleIndexes = GetIndexes(response, tupleImagesIndexes.Item1, MUSIC_TITLE_START_LABEL, MUSIC_TITLE_END_LABEL);
            List<string> titles = GetSubstringsByIndexes(response, tupleTitleIndexes.Item1, tupleTitleIndexes.Item2, 7);

            Tuple<List<int>, List<int>> tupleAuthorIndexes = GetIndexes(response, tupleTitleIndexes.Item1, MUSIC_AUTHOR_START_LABEL, MUSIC_AUTHOR_END_LABEL);
            List<string> authors = GetSubstringsByIndexes(response, tupleAuthorIndexes.Item1, tupleAuthorIndexes.Item2, 6);

            return new MusicModel(musics[0], titles[0], authors[0], images[0]);
        }
    }
}
