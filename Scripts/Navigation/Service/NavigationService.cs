using System.Windows.Controls;
using System.Windows;

namespace SkullMp3Player.Scripts.Navigation.Service
{
    class NavigationService
    {
        private UserControl _currentUserControl;

        public NavigationService(UserControl currentUserControl) 
        { 
            _currentUserControl = currentUserControl;
        }

        public void NavigateTo(UserControl newUserControl)
        {
            _currentUserControl.Visibility = Visibility.Collapsed;
            _currentUserControl = newUserControl;
            _currentUserControl.Visibility = Visibility.Visible;
        }
    }
}
