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
using System.Windows.Threading;

namespace WizardWarzRotW
{
    /// <summary>
    /// Interaction logic for EndScreen.xaml
    /// </summary>
    public partial class EndScreen : UserControl
    {
        public float currentTick = 0;
        public int maxScore = 0;
        public string topPlayer;
        public double endCountdown = 10;
        int picCount = 0;
        int picWaitMove = 0;
        public DispatcherTimer endGameTimer = null;
        public GameBoard gbRef;

        public EndScreen()
        {
            InitializeComponent();

            // End game timer
            endGameTimer = new DispatcherTimer(DispatcherPriority.Render);
            endGameTimer.Interval = TimeSpan.FromSeconds(0.5);
            endGameTimer.Tick += new EventHandler(timer_Tick);
            endGameTimer.Start();

            RetrieveScores();
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            currentTick += 0.5f;
            endCountdown = 10 - currentTick;

            if (currentTick % 1 == 0)
            {
                Console.WriteLine("Current tick: {0}", currentTick);
                Countdown();
            }
            FlashTick();
        }

        public void Countdown()
        {
            // Count down to 0, then restart the game.
            endTimer.Content = endCountdown + " seconds.";
            if (endCountdown <= 0)
            {
                // Original method of restarting game:
                // ----------------------------------------------------------
                // Resets variables without creating a new instance of the game.

                //endGameTimer.Stop();

                //MainWindow mwRef = MainWindow.ReturnMainWindowInstance();
                //mwRef.MainCanvas.Children.Remove(this);

                //TitleScreen title = new TitleScreen();
                //title.MouseDown += mwRef.Title_MouseDown;
                //mwRef.MainCanvas.Children.Add(title);

                // New method of restarting game:
                // ----------------------------------------------------------
                // Closes the window, then launches a new one.
                endGameTimer.Stop();
                MainWindow.ChangeGameState("title");
                //MainWindow.GameRestart();
                //Application.Current.Shutdown();
            }
        }

        public void RetrieveScores()
        {
            gbRef = GameBoard.ReturnGameBoardInstance();

            Dictionary<string, int> unsortedPlayerStats = new Dictionary<string, int>();
            Dictionary<string, int> sortedPlayerStats = new Dictionary<string, int>();

            foreach (PlayerControl player in gbRef.playerControllers)
            {
                unsortedPlayerStats.Add(player.playerName, player.myLivesAndScore.currentScore);
                Console.WriteLine(string.Format("Player result: {0}, score: {1}", player.playerName, player.myLivesAndScore.currentScore));
            }

            // Use OrderBy method to sort dictionary by value, then add the sorted values to the second dictionary.
            foreach (var item in unsortedPlayerStats.OrderBy(i => i.Value))
            {
                //MessageBox.Show(string.Format("Dictionary key/value: {0}", item));
                Console.WriteLine(item);
                sortedPlayerStats.Add(item.Key, item.Value);
            }

            // The sorted dictionary should have the top-scoring player as the last entry.
            topPlayer = sortedPlayerStats.Keys.Last();
            Console.WriteLine("{0} is the winner!", topPlayer);
            Winner.Content = topPlayer;

            player1Score.Content = unsortedPlayerStats.Values.ElementAt(0);
            player2Score.Content = unsortedPlayerStats.Values.ElementAt(1);
            player3Score.Content = unsortedPlayerStats.Values.ElementAt(2);
            player4Score.Content = unsortedPlayerStats.Values.ElementAt(3);

            // If there are only 4 players, empty the last two groups of labels
            if (GameBoard.ReturnNumberOfPlayer() <= 4)
            {
                player5Score.Content = "";
                player5Label.Content = "";
                player6Score.Content = "";
                player6Label.Content = "";
            }

            else
            {
                player5Score.Content = unsortedPlayerStats.Values.ElementAt(4);
                player6Score.Content = unsortedPlayerStats.Values.ElementAt(5);
            }
        }

        private void FlashTick()
        {

            picWaitMove++;

            if (picCount <= 3)
            {
                FlashText();
            }
            else
            {
                picCount = 0;
                FlashText();
            }

            if (picWaitMove == 4)
            {

                picWaitMove = 0;
            }
        }

        private void FlashText()
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
    }
}
