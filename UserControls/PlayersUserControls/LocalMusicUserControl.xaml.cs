using Microsoft.Win32;
using SkullMp3Player.Scripts.Player.Music.Model;
using SkullMp3Player.UserControls.PlayersUserControls.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TagLib;

namespace SkullMp3Player.UserControls
{
    /// <summary>
    /// Логика взаимодействия для LocalMusicUserControl.xaml
    /// </summary>
    public partial class LocalMusicUserControl : UserControl, ILocalMusicPlayerUserControl
    {
        public const string LOCAL_MUSIC_PLAYLIST_NAME = "localMusic";

        public StackPanel MusicListStackPanel => MusicList;
        public string CurrentLoadedPlaylistName { get; set; } = null!;

        public event AddMusic? AddMusicEvent;
        public event RemoveMusic? RemoveMusicEvent;
        public event ShuffleMusic? ShuffleMusicEvent;
        public delegate void AddMusic(List<MusicModel> music);
        public delegate void RemoveMusic();
        public delegate void ShuffleMusic();

        private const string UNKNOWN = "???";
        private static List<string> _addedMusic = new();

        public LocalMusicUserControl()
        {
            InitializeComponent();
        }

        public void RemoveOnAddedMusic(string musiclink)
        {
            if (!_addedMusic.Contains(musiclink)) {
                return;
            }

            _addedMusic.Remove(musiclink);
        }

        private void OnSelectMusicButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new() {
                Multiselect = true,
                Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == false) {
                return;
            }

            AddMusicToPlayer(openFileDialog.FileNames);
        }

        private void OnMusicDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
                return;
            }

            AddMusicToPlayer((string[]) e.Data.GetData(DataFormats.FileDrop));
        }

        private void OnRemoveMusicButtonClick(object sender, RoutedEventArgs e)
        {
            RemoveMusicEvent?.Invoke();
        }

        private void OnShuffleButtonClick(object sender, RoutedEventArgs e)
        {
            ShuffleMusicEvent?.Invoke();
        }

        private void AddMusicToPlayer(string[] mp3files)
        {
            AddMusicEvent?.Invoke(GetMusicModels(RemoveAlreadyAddedMusic(mp3files)));
        }

        private List<MusicModel> GetMusicModels(List<string> mp3Files)
        {
            List<MusicModel> result = new();
            foreach (string mp3File in mp3Files) {
                File taglibfile = File.Create(mp3File);
                string title = taglibfile.Tag.Title ?? UNKNOWN;
                string author = taglibfile.Tag.Performers.FirstOrDefault() ?? UNKNOWN;
                IPicture? picture = taglibfile.Tag.Pictures.FirstOrDefault();
                
                MusicModel musicModel = picture == null ? new MusicModel(mp3File, title, author, string.Empty) : new MusicModel(mp3File, title, author, picture.Data.Data);
                result.Add(musicModel);
                _addedMusic.Add(mp3File);
            }

            return result;
        }

        private List<string> RemoveAlreadyAddedMusic(string[] mp3files)
        {
            List<string> music = mp3files.ToList();
            return music.Where(m => !_addedMusic.Contains(m)).ToList();
        }
    }
}
