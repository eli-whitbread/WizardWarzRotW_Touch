using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WizardWarzRotW
{
    /// <summary>
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        public int playerStartPos;
        public string playerName;
        public LivesAndScore myLivesAndScore = null;
        public Point playerPosition;
        //FrameworkElement element;

        private Point LastTouchDown;

        public PlayerControl()
        {
            InitializeComponent();
        }

        private void UserControl_PreviewTouchDown(object sender, TouchEventArgs e)
        {            
            LastTouchDown = e.GetTouchPoint(GameBoard.ReturnGameBoardInstance()).Position;
        }

        private void UserControl_PreviewTouchUp(object sender, TouchEventArgs e)
        {

        }

        private void UserControl_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            LastTouchDown = e.GetTouchPoint(GameBoard.ReturnGameBoardInstance()).Position;
        }
    }
}
