using SkullMp3Player.Scripts.Player.Music.Model;
using SkullMp3Player.UserControls;
using System.Collections.Generic;
using SkullMp3Player.Scripts.Player.Music.Repository;
using System.Windows.Input;
using SkullMp3Player.Scripts.Ui.MusicItems.Controller;
using SkullMp3Player.UserControls.PlayersUserControls.Interfaces;

namespace SkullMp3Player.Scripts.Tools
{
    internal class InstallInitialMusicService
    {
        private MusicRepository _musicRepository;
        private MusicItemsController _musicItemController;

        public InstallInitialMusicService(MusicRepository musicRepository, MusicItemsController musicItemController) 
        {
            _musicRepository = musicRepository;
            _musicItemController = musicItemController;
        }

        public async void SetStartMusicOnUserControl(IWebSitePlayerUserControl playerUserControl, IWebSiteParser webSiteParser, MouseButtonEventHandler onPlaylistCLick)
        {
            string randomStartArtist = StartRandomArtist.GetRandomArtist();
            List<MusicModel>? musicModels = await webSiteParser.FindMusicAsync(randomStartArtist);
            if (!musicModels.IsNullOrEmpty()) {
                SetMusic(webSiteParser, playerUserControl, musicModels!, randomStartArtist);
            }

            List<MusicModel>? randomMusicModels = await webSiteParser.GetRandomMusicAsync();
            if (!randomMusicModels.IsNullOrEmpty()) {
                SetRandomMusic(webSiteParser, playerUserControl, randomMusicModels!);
            }
       
            List<MusicModel>? newMusicPlaylist = await webSiteParser.GetMusicAsync(webSiteParser.NewMusic);
            if (!newMusicPlaylist.IsNullOrEmpty()) {
                SetPlaylistMusic(webSiteParser, playerUserControl, newMusicPlaylist!, PlaylistItemUserControl.NEW_MUSIC_PLAYLIST_NAME, "новая музыка", onPlaylistCLick);
            }

            List<MusicModel>? popularMusicPlaylist = await webSiteParser.GetMusicAsync(webSiteParser.PopularMusic);
            if (!popularMusicPlaylist.IsNullOrEmpty()) {
                SetPlaylistMusic(webSiteParser, playerUserControl, popularMusicPlaylist!, PlaylistItemUserControl.POPULAR_MUSIC_PLAYLIST_NAME, "популярная музыка", onPlaylistCLick);
            }
        }
            
        private void SetMusic(IWebSiteParser webSiteParser, IWebSitePlayerUserControl playerUserControl, List<MusicModel> musicModels, string playlistName)
        {
            playerUserControl.FindMusicTextBlock!.Text = $"Музыка по запросу \"{playlistName}\"";
            _musicRepository.AddMusicToRepository(webSiteParser, musicModels!, playlistName);
            _musicItemController.AddRangeMusicItem(musicModels!, playerUserControl.MusicListStackPanel);
            playerUserControl.CurrentLoadedPlaylistName = playlistName;
        }

        private void SetRandomMusic(IWebSiteParser webSiteParser, IWebSitePlayerUserControl playerUserControl, List<MusicModel> musicModels)
        {
            _musicRepository.AddMusicToRepository(webSiteParser, musicModels, RandomMusicItemUserControl.RANDOM_PLAYLIST_NAME);
            _musicItemController.AddRandomMusicItem(musicModels!, playerUserControl.RandomMusicListStackPanel!);
        }

        private void SetPlaylistMusic(IWebSiteParser webSiteParser, IWebSitePlayerUserControl playerUserControl, List<MusicModel> musicModels, string playlistName, string playlistTitle, MouseButtonEventHandler onPlaylistCLick)
        {
            _musicRepository.AddMusicToRepository(webSiteParser, musicModels!, playlistName);
            PlaylistItemUserControl popularMusicplaylistItem = new(playlistName, playlistTitle);
            popularMusicplaylistItem.MouseLeftButtonDown += onPlaylistCLick;
            playerUserControl.PlaylistsStackPanel!.Children.Add(popularMusicplaylistItem);
        }
    }
}
