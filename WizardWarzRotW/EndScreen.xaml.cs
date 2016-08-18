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
        protected static EndScreen endScreenInstance;

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

            //RetrieveScores();

            // initialize reference
            endScreenInstance = this;
        }

        public static EndScreen ReturnEndScreenInstance()
        {
            return endScreenInstance;
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            if (MainWindow.ReturnMainWindowInstance().currentGameState == GameStates.EndScreen)
            {
                currentTick += 0.5f;
                endCountdown = 10 - currentTick;

                if (currentTick % 1 == 0)
                {
                    Console.WriteLine("Current tick: {0}", currentTick);
                    Countdown();
                }
                
            }
        }

        public void Countdown()
        {
            // Count down to 0, then restart the game.
            endTimer.Content = endCountdown + " seconds.";
            if (endCountdown <= 0)
            {
                // Final method of restarding game: Use ChangeGameState function.
                endGameTimer.Stop();
                MainWindow.ReturnMainWindowInstance().ChangeGameState("title");
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
        
    }
}
