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
    /// Interaction logic for LivesAndScore.xaml
    /// </summary>
    public partial class LivesAndScore : UserControl
    {
        public Int32 playerLivesNumber;
        public Rectangle playerHomeTile;
        
        public Rectangle playerLivesTile;
        public Label playerScore;
        public int currentScore;
        
        public int tileSizeLocal;
        //AudioManager playMusic = new AudioManager();

        public LivesAndScore()
        {
            InitializeComponent();

            tileSizeLocal = GameBoard.ReturnTileSize();

            //--------------- Player Lives (HAS TO BE CHANGED HERE)--------------------------
            playerLivesNumber = playerLivesNumber + 3;

            //--------------- Player Score (HAS TO BE CHANGED HERE)--------------------------
            currentScore = 0;

            // -------------- INITIALISATION of PLAYER STATS----------------------------------------
            initialiseHomeBases();
            initialiseLives();
            initialiseScore();
        }

        /// <summary>
        /// Initialises a home base. <para> Can be used by any number of players, though needs player controllers set up before hand, and in conjunction. Sets the same initial home base position for all players. </para>
        /// </summary>
        public void initialiseHomeBases()
        {
            //--------------------------------------------| Initialise Player Home Base |-------------------------------------------         
            playerHomeTile = new Rectangle();
            playerHomeTile.Fill = new ImageBrush(new BitmapImage(new Uri(@".\Resources\Home0.png", UriKind.Relative)));
            playerHomeTile.Height = tileSizeLocal;
            playerHomeTile.Width = tileSizeLocal;

            // --------------- Set position, within the local grid (livesGrid) of this element ------------------------------
            Grid.SetRow(playerHomeTile, 0);
            Grid.SetColumn(playerHomeTile, 0);


            // ------------------ Add element to the grid, according to the above -------------------------------------
            livesGrid.Children.Add(playerHomeTile);



        }

        /// <summary>
        /// Sets the initial amount of lives a player has. 
        /// </summary>
        public void initialiseLives()
        {
            //-----------------------------------------------------| Initialise Lives|-------------------------------------------
            // --------------------- Add hearts to grid, depending on number of lives--------------
            for (int i = 1; i <= playerLivesNumber; i++)
            {
                playerLivesTile = new Rectangle();

                playerLivesTile.Fill = new ImageBrush(new BitmapImage(new Uri(@".\Resources\heart.png", UriKind.Relative)));

                playerLivesTile.Height = tileSizeLocal;
                playerLivesTile.Width = tileSizeLocal;

                // --------------- Set position, within the local grid (livesGrid) of this element ------------------------------
                Grid.SetRow(playerLivesTile, 0);
                Grid.SetColumn(playerLivesTile, i);

                livesGrid.Children.Add(playerLivesTile);

            }

        }

        /// <summary>
        /// Reduces a players lives by the passed in number. <para> Needs to be associated with a player, but allows the players lives to be changed at will. Though always negatively. </para>
        /// </summary>
        public void ReduceLives(int count)
        {

            // REDUCE LIVES FUNCTION
            playerLivesNumber -= count;

            CalculateLives();
            //Debug.WriteLine("Player lives reduced!");
        }

        /// <summary>
        /// Handles the graphics of reducing lives. <para>Is auto called as party of ReduceLives.</para>
        /// </summary>
        public void CalculateLives()
        {
            // ---------------------- Remove ALL hearts from grid----------------------------
            try { livesGrid.Children.RemoveAt(3); }
            catch
            {//throw;
                //MessageBox.Show("Nothing at index: " + 3);
            }
            try { livesGrid.Children.RemoveAt(2); }
            catch
            {//throw;
                //MessageBox.Show("Nothing at index: " + 2);
            }
            try { livesGrid.Children.RemoveAt(1); }
            catch
            {//throw;
                //MessageBox.Show("Nothing at index: " + 1);
            }

            //playMusic.playPickupBomb();
            if (playerLivesNumber <= 0)
            {
                //Debug.WriteLine(string.Format("Sorry Player {0}, you are out of lives, and cannot respawn..", /*playerID*/ 1));

            }
            else if (playerLivesNumber > 0)
            {
                initialiseLives();
            }

        }

        /// <summary>
        /// Sets up each players score initially. <para> Is called as part of calculate lives function. This function redraws the score after each time it is called. </para>
        /// </summary>
        public void initialiseScore()
        {
            //------------------------------------------------------------------------------------------------------------------
            //--------------------------------------------| Initialise Player Score |-------------------------------------------           
            //------------------------------------------------------------------------------------------------------------------

            playerScore = new Label();

            playerScore.Content = currentScore.ToString();
            playerScore.FontSize = 32;
            playerScore.Foreground = new SolidColorBrush(Colors.Black);
            playerScore.Width = 128;
            playerScore.Height = GameBoard.ReturnTileSize();

            // --------------- Set position, within the local grid (scoreGrid) of this element --------------------------------

            Grid.SetRow(playerScore, 0);
            Grid.SetColumn(playerScore, 1);

            scoreGrid.Children.Add(playerScore);
            //-----------------------------------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------------

        }

        /// <summary>
        /// Handy score changing function, used in conjunction with player controller. <para> Takes in two parameters - the first is the number by which the score is change, and the second as to whether it is positive (true) or negative (false). </para>
        /// </summary>
        public void ChangeScore(int scoreUpOrDown, bool Up)
        {
            string info;
            if (Up)
            {
                currentScore += scoreUpOrDown; info = " + " + scoreUpOrDown.ToString(); 
                //Debug.WriteLine("Player score changed by:" + info); 
            }
            else if (!Up)
            {
                currentScore -= scoreUpOrDown; info = " - " + scoreUpOrDown.ToString(); 
                //Debug.WriteLine("Player score changed by:" + info);
            }

            CalculateScore();
        }

        /// <summary>
        /// Is Auto-called as part of Change Score. <para> Removes the score from the grid, so it can then be redrawn in intialise score. </para>
        /// </summary>
        public void CalculateScore()
        {
            try
            {
                // ------------- Remove Score from Grid --------------------------------------------------------------------------
                scoreGrid.Children.RemoveAt(0);
            }
            catch
            {

                //throw;
            }

            // ----------------- Run initialise Score method again, to populate grid with new score information ------------------
            //playMusic.playEnemyAttack();

            initialiseScore();

        }
    }
}
