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
using System.Diagnostics;

namespace WizardWarzRotW
{
    /// <summary>
    /// Interaction logic for TitleScreen.xaml
    /// </summary>
    public partial class TitleScreen : UserControl
    {
        //AudioManager titleScreenSound = new AudioManager();
        System.Windows.Threading.DispatcherTimer dTimer1 = new System.Windows.Threading.DispatcherTimer();
        public bool dTimerBool = false;
        int picCount = 0;
        int picMoveCount = 0;
        private static Random rnd = new Random();

        public MainWindow mwRef = null;
        //public HelpScreen tutorial = null;

        public TitleScreen()
        {
            InitializeComponent();
            dTimer1.Tick += DTimer1_Tick;
            dTimer1.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dTimer1.Start();
            
            //titleScreenSound.playTitleSound();
        }

        private void DTimer1_Tick(object sender, EventArgs e)
        {
            if (picCount <= 3)
            {
                flashText();
            }
            else
            {                
                flashText();
            }
        }

        private void flashText()
        {
            switch (picCount)
            {
                case 0:                    
                    magicBall.Margin = new Thickness(120, 254, 0, 0);
                    wizardLeft.Source = new BitmapImage(new Uri("Resources/TitleSprites/SSB1.png", UriKind.Relative));
                    wizardRight.Source = new BitmapImage(new Uri("Resources/TitleSprites/SSR1.png", UriKind.Relative));
                    picCount++;
                    
                    break;

                case 1:
                    magicBall.Margin = new Thickness(190, 254, 0, 0);
                    wizardLeft.Source = new BitmapImage(new Uri("Resources/TitleSprites/SSB2.png", UriKind.Relative));
                    wizardRight.Source = new BitmapImage(new Uri("Resources/TitleSprites/SSR2.png", UriKind.Relative));
                    picCount++;
                    break;

                case 2:
                    magicBall.Margin = new Thickness(260, 254, 0, 0);
                    wizardLeft.Source = new BitmapImage(new Uri("Resources/TitleSprites/SSB3.png", UriKind.Relative));
                    wizardRight.Source = new BitmapImage(new Uri("Resources/TitleSprites/SSR3.png", UriKind.Relative));
                    Debug.WriteLine(picCount);
                    picCount++;
                    break;

                case 3:
                    magicBall.Margin = new Thickness(330, 254, 0, 0);
                    wizardLeft.Source = new BitmapImage(new Uri("Resources/TitleSprites/SSB4.png", UriKind.Relative));
                    wizardRight.Source = new BitmapImage(new Uri("Resources/Smoke.png", UriKind.Relative));
                    picCount = 0;
                    break;

                default:
                    picCount = 0;
                    break;
            }
        }

        private void RandomiseWizards()
        {
            Image[] wizards = { wizard1, wizard2, wizard3, wizard4 };

            for (int i = 0; i < 4; i++)
            {
                Int32 positionY = rnd.Next(200, (Int32)TitleGrid.Width - 200);
                Int32 positionX = rnd.Next(200, (Int32)TitleGrid.Height - 200);

                wizards[i].Margin = new Thickness(positionX, positionY, 0, 0);
            }
        }

        private void startText_MouseEnter(object sender, MouseEventArgs e)
        {
            startText.Source = new BitmapImage(new Uri("Resources/Start2.png", UriKind.Relative));
        }

        private void quitText_MouseEnter(object sender, MouseEventArgs e)
        {
            quitText.Source = new BitmapImage(new Uri("Resources/Quit2.png", UriKind.Relative));
        }

        private void startText_MouseLeave(object sender, MouseEventArgs e)
        {
            startText.Source = new BitmapImage(new Uri("Resources/Start.png", UriKind.Relative));
        }

        private void quitText_MouseLeave(object sender, MouseEventArgs e)
        {
            quitText.Source = new BitmapImage(new Uri("Resources/Quit.png", UriKind.Relative));
        }

        private void startText_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.ReturnMainWindowInstance().ChangeGameState("menu");
            Console.WriteLine("Detected mouse click.");
        }

        private void startText_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            MainWindow.ReturnMainWindowInstance().ChangeGameState("menu");
            Console.WriteLine("Detected touch down.");
        }

        private void quitText_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void quitText_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
