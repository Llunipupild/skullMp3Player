using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SkullMp3Player.Scripts.Player
{
    public class Timer
    {
        private DispatcherTimer _timer;
        private MediaPlayer _player;
        private TextBlock _timerTextBlock;
        private Slider _musicPositionSlider;

        private string _currentMusicTime = null!;

        public Timer(TextBlock timerTextBlock, Slider musicPositionSlider, MediaPlayer mediaPlayer)
        {
            _timer = new();
            _timer.Interval = TimeSpan.FromSeconds(0);
            _timer.Tick += OnTimerIick;

            _timerTextBlock = timerTextBlock;
            _musicPositionSlider = musicPositionSlider;
            _player = mediaPlayer;
        }

        public void Start(string musicTime)
        {
            _currentMusicTime = musicTime;
            _musicPositionSlider.Maximum = _player.NaturalDuration.TimeSpan.TotalSeconds;
            Start();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _timerTextBlock.Text = string.Empty;
        }

        private void OnTimerIick(object? sender, EventArgs e)
        {
            _timerTextBlock.Text = string.Format("{0} / {1}", _player.Position.ToString(@"mm\:ss"), _currentMusicTime);
            _musicPositionSlider.Value = _player.Position.TotalSeconds;
        }
    }
}
