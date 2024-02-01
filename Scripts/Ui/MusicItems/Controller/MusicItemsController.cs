using SkullMp3Player.Scripts.Player.Music.Model;
using SkullMp3Player.UserControls.Interface;
using SkullMp3Player.UserControls;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System;
using SkullMp3Player.UserControls.ItemsUserControls.Interface;
using SkullMp3Player.UserControls.PlayersUserControls.Interfaces;

namespace SkullMp3Player.Scripts.Ui.MusicItems.Controller
{
    internal class MusicItemsController
    {
        private TextBlock _currentPlayingMusicName;
        private TextBlock _currentPlayingMusicAuthor;
        private ImageBrush _currentPlayingMusicImage;
        private IItemUserControl? _currentActiveMusicItem;

        public event ClickOnMusic ClickOnMusicEvent = null!;
        public event UpdateMusicItemPanel UpdateMusicItemPanelEvent = null!;
        public delegate void ClickOnMusic(string musicLink, string? playlistName = null);
        public delegate void UpdateMusicItemPanel();

        public MusicItemsController(TextBlock currentPlayingMusicName, TextBlock currentPlayingMusicAuthor, ImageBrush currentPlayingMusicImage)
        {
            _currentPlayingMusicName = currentPlayingMusicName;
            _currentPlayingMusicAuthor = currentPlayingMusicAuthor;
            _currentPlayingMusicImage = currentPlayingMusicImage;
        }

        public void AddRangeMusicItem(List<MusicModel> musicModels, StackPanel panel)
        {
            if (panel.Parent is ScrollViewer scrollViewer) {
                scrollViewer.ScrollToHome();
            }

            musicModels.ForEach(m => AddMusicItem(m, panel));
            UpdateMusicItemPanelEvent?.Invoke();
        }

        public void AddRandomMusicItem(List<MusicModel> musicModels, StackPanel panel)
        {
            foreach (MusicModel musicModel in musicModels) {
                RandomMusicItemUserControl randomMusicItem = new(musicModel.Name, musicModel.Author, musicModel.Link, musicModel.Image);
                randomMusicItem.MouseLeftButtonDown += OnMusicClick;
                panel.Children.Add(randomMusicItem);
            }
        }

        public void AddMusicItem(MusicModel musicModel, StackPanel panel)
        {
            IItemUserControl? existedItem = GetMusicItem(musicModel.Link, panel);
            if (existedItem != null) {
                return;
            }

            MusicItemUserControl musicItem = new(musicModel.Link, musicModel.Name, musicModel.Author, musicModel.Image);
            musicItem.MouseLeftButtonDown += OnMusicClick;
            panel.Children.Add(musicItem);
        }

        public void RemoveMusicItem(string musicLink, StackPanel panel)
        {
            IItemUserControl? item = GetMusicItem(musicLink, panel);
            if (item == null) {
                return;
            }

            panel.Children.Remove(item as UIElement);
        }

        public void SetCurrentPlayingMusicData(string musicLink, IPlayerUserControl playerUserControl)
        {
            IItemUserControl? itemUserControl = GetMusicItem(musicLink, playerUserControl.MusicListStackPanel);
            if (itemUserControl == null) {
                if (playerUserControl is not IWebSitePlayerUserControl webSitePlayerUserControl) {
                    return;
                }

                itemUserControl = GetMusicItem(musicLink, webSitePlayerUserControl.RandomMusicListStackPanel);
                if (itemUserControl == null) {
                    return;
                }
            }

            SetCurrentPlayingMusicData(itemUserControl);
        }

        public void SetCurrentPlayingMusicData(IItemUserControl item)
        {
            if (_currentActiveMusicItem != null) {
                _currentActiveMusicItem.IsActive = false;
            }

            _currentActiveMusicItem = item;
            _currentPlayingMusicName.Text = item.MusicName;
            _currentPlayingMusicAuthor.Text = item.Author;
            _currentPlayingMusicImage.ImageSource = item.Image;
            _currentActiveMusicItem.IsActive = true;
        }

        public void ClearCurrentPlayingMusicData()
        {
            if(_currentActiveMusicItem != null) {
                _currentActiveMusicItem.IsActive = false;
            }

            _currentActiveMusicItem = null;
            _currentPlayingMusicName.Text = string.Empty;
            _currentPlayingMusicAuthor.Text = string.Empty;
            _currentPlayingMusicImage.ImageSource = null;
        }

        public void ShufflePanelChildrens<T>(StackPanel panel, List<T> list)
        {
            Random random = new();
            int countChilds = panel.Children.Count;
            if (countChilds <= 0) {
                return;
            }

            for (int i = 0; i < countChilds; i++) {
                int randomNumber = random.Next(countChilds);
                object currentElement = panel.Children[i];
                T currentElementAtList = list[i]!;

                panel.Children.Remove((UIElement) currentElement);
                list.Remove(currentElementAtList);

                panel.Children.Insert(randomNumber, (UIElement) currentElement);
                list.Insert(randomNumber, currentElementAtList);
            }
        }

        public void ClearPanel(StackPanel panel) => panel.Children.Clear();

        private void OnMusicClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not IItemUserControl musicItem) {
                return;
            }

            ClickOnMusicEvent?.Invoke(musicItem.MusicLink, musicItem.PlaylistName);
        }

        private IItemUserControl? GetMusicItem(string musicLink, StackPanel panel)
        {
            foreach (object? children in panel.Children) {
                if (children is not IItemUserControl musicItem) {
                    continue;
                }

                if (musicItem.MusicLink == musicLink) {
                    return musicItem;
                }
            }

            return null;
        }
    }
}
