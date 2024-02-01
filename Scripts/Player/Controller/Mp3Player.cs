using System.Windows.Media;
using System;
using System.Windows.Controls;

namespace SkullMp3Player.Scripts.Player.Controller
{
    class Mp3Player
    {
        private MediaPlayer _player;
        private Timer _timer;
        private Action _onSoundEndAction;

        public string? CurrentMusic { get; private set; }
        public bool IsPlaying { get; private set; }

        public Mp3Player(Action onSoundEndAction, TextBlock textBlock, Slider musicPositionSlider)
        {
            _player = new MediaPlayer();
            _player.MediaOpened += OnSoundOpened;
            _player.MediaEnded += OnSoundEnd;

            _timer = new(textBlock, musicPositionSlider, _player);
            _onSoundEndAction = onSoundEndAction;
        }

        public void Play(string uri)
        {
            if (CurrentMusic == uri || string.IsNullOrEmpty(uri)) {
                return;
            }

            CurrentMusic = uri;
            _player.Open(new Uri(uri));
            Play();
        }

        public void Play()
        {
            IsPlaying = true;
            _player.Play();
        }

        public void Pause()
        {
            if (!IsPlaying) {
                Play();
                _timer.Start();
                return;
            }

            IsPlaying = false;
            _player.Pause();
            _timer.Stop();
        }

        public void Stop()
        {
            OnSoundEnd(this, EventArgs.Empty);
        }

        public void ChangeVolume(double volume)
        {
            _player.Volume = volume;
        }

        public void ChangeMusicPosition(double position)
        {
            _player.Position = TimeSpan.FromSeconds(position);
        }

        private void OnSoundOpened(object? sender, EventArgs e)
        {
            _timer.Start(_player.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
        }

        private void OnSoundEnd(object? sender, EventArgs e)
        {
            CurrentMusic = null;
            _player.Stop();
            _timer.Stop();
            _onSoundEndAction.Invoke();
        }
    }
}
