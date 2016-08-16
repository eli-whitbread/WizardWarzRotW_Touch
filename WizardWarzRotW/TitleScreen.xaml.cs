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
    /// Interaction logic for TitleScreen.xaml
    /// </summary>
    public partial class TitleScreen : UserControl
    {
        //AudioManager titleScreenSound = new AudioManager();
        System.Windows.Threading.DispatcherTimer dTimer1 = new System.Windows.Threading.DispatcherTimer();
        public bool dTimerBool = false;
        int picCount = 0;
        int picWaitMove = 0;
        private static Random rnd = new Random();

        public MainWindow mwRef = null;
        //public HelpScreen tutorial = null;

        public TitleScreen()
        {
            InitializeComponent();
            dTimer1.Tick += DTimer1_Tick;
            dTimer1.Interval = new TimeSpan(0, 0, 0, 0, 150);
            dTimer1.Start();
            //titleScreenSound.playTitleSound();
        }

        private void TitleScreen_PreviewTouchDown(object sender, TouchEventArgs e)
        {

        }

        private void DTimer1_Tick(object sender, EventArgs e)
        {
            picWaitMove++;

            if (picCount <= 3)
            {
                flashText();
            }
            else
            {
                picCount = 0;
                flashText();
            }

            if (picWaitMove == 4)
            {
                //RandomiseWizards();
                picWaitMove = 0;
            }

        }

        private void flashText()
        {
            switch (picCount)
            {
                case 0:
                    wizardText.Source = new BitmapImage(new Uri("Resources/WizardWarzText2.png", UriKind.Relative));
                    picCount++;
                    break;

                case 1:
                    wizardText.Source = new BitmapImage(new Uri("Resources/WizardWarzText4.png", UriKind.Relative));
                    picCount++;
                    break;

                case 2:
                    wizardText.Source = new BitmapImage(new Uri("Resources/WizardWarzText3.png", UriKind.Relative));
                    picCount++;
                    break;

                case 3:
                    wizardText.Source = new BitmapImage(new Uri("Resources/WizardWarzText1.png", UriKind.Relative));
                    picCount++;
                    break;

                default:
                    wizardText.Source = new BitmapImage(new Uri("Resources/WizardWarzText.png", UriKind.Relative));
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
    }
}
