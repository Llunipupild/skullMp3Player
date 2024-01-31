using System.Windows.Media;
using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SkullMp3Player.Scripts.Player.Controller
{
    class Mp3Player
    {
        private MediaPlayer _player;
        private TextBlock _timerTextBlock;
        private DispatcherTimer _timer;

        private string _currentMusicTime = null!;
        private Action _onSoundEndAction;

        public string? CurrentMusic { get; private set; }
        public bool IsPlaying { get; private set; }

        public Mp3Player(Action onSoundEndAction, TextBlock textBlock)
        {
            _player = new MediaPlayer();
            _player.MediaOpened += OnSoundOpened;
            _player.MediaEnded += OnSoundEnd;

            _onSoundEndAction = onSoundEndAction;
            _timerTextBlock = textBlock;

            _timer = new() {
                Interval = TimeSpan.FromSeconds(0)
            };
            _timer.Tick += OnTimerIick;
        }

        public void Play(string uri)
        {
            if (CurrentMusic == uri) {
                return;
            }
            if (string.IsNullOrEmpty(uri)) {
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
            _player.Stop();
            _timer.Stop();
            _timerTextBlock.Text = string.Empty;
            OnSoundEnd(this, EventArgs.Empty);
        }

        public void ChangeVolume(double volume)
        {
            _player.Volume = volume;
        }

        private void OnSoundOpened(object? sender, EventArgs e)
        {
            _currentMusicTime = _player.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            _timer.Start();
        }

        private void OnSoundEnd(object? sender, EventArgs e)
        {
            CurrentMusic = null;
            _onSoundEndAction.Invoke();
        }

        private void OnTimerIick(object? sender, EventArgs e)
        {
            _timerTextBlock.Text = string.Format("{0} / {1}", _player.Position.ToString(@"mm\:ss"), _currentMusicTime);
        }
    }
}
