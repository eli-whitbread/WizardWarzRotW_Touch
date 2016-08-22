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
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
            //GlobalPlayerMainMenu = true;
        }

        /// <summary>
        /// Boolean that sets the game mode. (Four player = false. Six player = true.)
        /// </summary>
        public static bool GlobalPlayerMainMenu
        {
            get; set;
        }


        private void FourPlayerButton_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            //MessageBox.Show("4 player button pressed");
            Console.WriteLine("Touch down detected: Four player button");
            GlobalPlayerMainMenu = false;
            RunWizardWarz();
        }

        private void SixPlayerButton_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            //MessageBox.Show("6 player button pressed");
            Console.WriteLine("Touch down detected: Six player button");
            GlobalPlayerMainMenu = true;
            RunWizardWarz();
        }

        private void FourPlayerButton_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("4 player button pressed");
            Console.WriteLine("Mouse click detected: Four player button");
            GlobalPlayerMainMenu = false;
            RunWizardWarz();
        }

        private void SixPlayerButton_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("6 player button pressed");
            Console.WriteLine("Mouse click detected: Six player button");
            GlobalPlayerMainMenu = true;
            RunWizardWarz();
        }

        private void RunWizardWarz()
        {
            MainWindow.ReturnMainWindowInstance().ChangeGameState("game");

            //mainWinRef.newAudioManager.playMainMusic();
            //mainWinRef.GameStart();
        }        
    }
}
