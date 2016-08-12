﻿using System;
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
    public enum TileStates
    {
        Floor,
        SolidWall,
        DestructibleWall,
        Powerup
    }

    /// <summary>
    /// Interaction logic for GameBoard.xaml
    /// </summary>
    public partial class GameBoard : UserControl
    {
        // ----------------------------- GAME BOARD --------------------------------------------
        //public Grid gameGrid = null;

        public Image[,] flrTiles = null;
        Int32 rows = 13;
        Int32 cols = 23;
        Int32 numberOfDestructibleWalls = 60;
        public static TileStates[,] curTileState = null;

        static Random randomNumber = new Random();

        public int[,] innerWallPos;

        public int[,] destructibleWallPos;
        // ----------------------------- END GAME BOARD -------------------------------------------

        // ------------------------------ PUBLIC GAME DEPENDENCIES -----------------------------
        protected static Int32 tileSize = 64;
        private GameTimer gameTimerInstance = null;
        double varRotTransform = 90;        
        protected static GameBoard gameBoardManager;
        protected static Canvas GameCanvasInstance;
        protected static List<PlayerControl> PlayerListRef;        
        public RotateTransform trRot = null;
        protected static int noOfPlayers = 6;
        public PlayerControl[] playerControllers = null;
        public LivesAndScore[] playerLives;
        public Powerups powerupRef = null;
        //public Int32 playersOnBoard;
        public List<PlayerControl> ListOfPlayers = new List<PlayerControl>();
        protected static Int16 randomNo4PowerUps;        
        public int gameTimeSeconds = 00;
        public int gameTimeMinutes = 4;
        public double currentTick;
        // ------------------------------ END PUBLIC GAME DEPENDENCIES -----------------------------

        // ------------------------------ RETURN FUNCTIONS -----------------------------------------
        //Random number generator instance
        public RandomNumberGenerator RNG = new RandomNumberGenerator();


        /// <summary>
        /// Returns reference to MainWindow Canvas. <para> The Canvas exists above the game Grid, so will be rendered first.</para>
        /// </summary>
        public static Canvas ReturnMainCanvas()
        {
            return GameCanvasInstance;
        }

        

        /// <summary>
        /// Returns a reference to the GameBoardManager. <para> The GameBoard Manager holds tile/floor state information. </para>
        /// </summary>
        public static GameBoard ReturnGameBoardInstance()
        {
            return gameBoardManager;
        }

        /// <summary>
        /// Returns the number of players in the current game instance. <para> The game board size is different for both 4 or 6 players, wherein the latter is larger. If you use this reference, make sure what is placed on the board corresponds. </para>
        /// </summary>
        public static int ReturnNumberOfPlayer()
        {
            return noOfPlayers;
        }
        

        /// <summary>
        /// Returns the games set tile size. <para> Wizard Warz as default uses 64x64. </para>
        /// </summary>
        public static Int32 ReturnTileSize()
        {
            return tileSize;
        }

        /// <summary>
        /// Returns the globally callable player list - 1-4/ 1-6. Provides access to all players. <para> This list provides access to all players within the game - meaning other classes can interact with them as needed. </para>
        /// </summary>
        public static List<PlayerControl> ReturnPlayerList()
        {
            return PlayerListRef;
        }

        /// <summary>
        /// Returns the globally callable random Number - between 0 and 3. <para> This number is generated within GameWindow, each tick, thus providing some randomness. </para>
        /// </summary>
        public static Int16 ReturnRandomPowerUpNo()
        {
            return randomNo4PowerUps;
        }
        // ---------------------------------- END RETURN FUNCTIONS ---------------------------------------

        public GameBoard()
        {
            InitializeComponent();
            GameDependencies();

            InitializeGameBoard();
        }

        public void InitializeGameBoard()
        {

            curTileState = new TileStates[cols, rows];

            GridLengthConverter myGridLengthConverter = new GridLengthConverter();
            GridLength side = (GridLength)myGridLengthConverter.ConvertFromString("Auto");

            //Setup the grid Rows and Columns
            for (int i = 0; i < cols; i++)
            {
                GameGridXAML.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition());
                GameGridXAML.ColumnDefinitions[i].Width = side;
            }
            for (int x = 0; x < rows; x++)
            {
                GameGridXAML.RowDefinitions.Add(new System.Windows.Controls.RowDefinition());
                GameGridXAML.RowDefinitions[x].Height = side;
            }

            AssignDestructibleWallPositions();

            //create an empty Rectangle array
            //flrTiles = new Rectangle[cols, rows];
            flrTiles = new Image[cols, rows];

            //fill each element in the Rectangle array with an image "tile".
            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    //flrTiles[c, r] = new Rectangle();
                    flrTiles[c, r] = new Image();


                    //add a wall tile if along the grid extremes
                    if (InitialTilePlacementCheck(c, r, cols, rows) == true)
                    {
                        //flrTiles[c, r].Fill = new ImageBrush(new BitmapImage(new Uri(@".\Resources\Indesructable.png", UriKind.Relative)));
                        flrTiles[c, r].Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Indestructible.png", UriKind.Absolute));
                        flrTiles[c, r].Stretch = Stretch.Fill;
                        curTileState[c, r] = TileStates.SolidWall;
                    }
                    //add destructible walls within the game grid
                    else if (DestructableWallPlacementCheck(c, r) == true)
                    {
                        //flrTiles[c, r].Fill = new ImageBrush(new BitmapImage(new Uri(@".\Resources\Destructible.png", UriKind.Relative)));
                        flrTiles[c, r].Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Destructible.png", UriKind.Absolute));
                        flrTiles[c, r].Stretch = Stretch.Fill;
                        curTileState[c, r] = TileStates.DestructibleWall;
                    }
                    //otherwise add a floor tile
                    else
                    {
                        //flrTiles[c, r].Fill = new ImageBrush(new BitmapImage(new Uri(@".\Resources\Floor.png", UriKind.Relative)));
                        flrTiles[c, r].Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Floor.png", UriKind.Absolute));
                        flrTiles[c, r].Stretch = Stretch.Fill;
                        curTileState[c, r] = TileStates.Floor;
                    }
                    //
                    //inner solid and destrutable walls still required!!
                    //
                    flrTiles[c, r].Height = 64;
                    flrTiles[c, r].Width = 64;
                    Grid.SetColumn(flrTiles[c, r], c);
                    Grid.SetRow(flrTiles[c, r], r);


                    GameGridXAML.Children.Add(flrTiles[c, r]);
                }
            }

        }

        void AssignDestructibleWallPositions()
        {
            Int32 wallCount = 0;
            destructibleWallPos = new Int32[numberOfDestructibleWalls, 2];

            while (wallCount <= numberOfDestructibleWalls - 1)
            {

                Int32 rndColNum = randomNumber.Next(2, cols - 3);
                Int32 rndRowNum = randomNumber.Next(2, rows - 3);

                if ((rndColNum % 2 != 0) || (rndRowNum % 2 != 0))
                {

                    bool canPlaceTileHere = true;

                    for (int i = 0; i < destructibleWallPos.Length / 2; i++)
                    {
                        if (destructibleWallPos[i, 0] == rndColNum && destructibleWallPos[i, 1] == rndRowNum)
                        {
                            canPlaceTileHere = false;
                            break;
                        }

                    }

                    if (canPlaceTileHere == true)
                    {
                        destructibleWallPos[wallCount, 0] = rndColNum;
                        destructibleWallPos[wallCount, 1] = rndRowNum;

                        wallCount++;
                    }


                }
            }
        }

        public bool InitialTilePlacementCheck(Int32 c, Int32 r, Int32 colsLength, Int32 rowsLength)
        {
            innerWallPos = new Int32[(rows * cols), 2]; //this is waaay too long but don't have much choice

            Int32 innerWallCount = 0;

            if (r == 0 || r == rowsLength - 1 || c == 0 || c == colsLength - 1)
            {
                innerWallPos[innerWallCount, 0] = c;
                innerWallPos[innerWallCount, 1] = r;
                innerWallCount++;
                return true;
            }
            else if ((c % 2 == 0) && (r % 2 == 0))
            {
                innerWallPos[innerWallCount, 0] = c;
                innerWallPos[innerWallCount, 1] = r;
                innerWallCount++;
                return true;
            }

            return false;
        }

        public bool DestructableWallPlacementCheck(Int32 c, Int32 r)
        {
            for (int i = 0; i < destructibleWallPos.Length / 2; i++)
            {
                if (destructibleWallPos[i, 0] == c && destructibleWallPos[i, 1] == r)
                {
                    if (curTileState[c, r] == TileStates.DestructibleWall)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        

        private void GameDependencies()
        {

            if (gameTimerInstance == null)
            {
                gameTimerInstance = new GameTimer();

            }

            gameTimerInstance.StartGameTimer();

            // The number of players needs to be set before the game board is initialized
            if (MainMenu.GlobalPlayerMainMenu)
            {
                noOfPlayers = 6; 
                //playersOnBoard = 6; 
            }
            else
            {
                noOfPlayers = 4; 
                //playersOnBoard = 4; 
            }


            initialiseGameBoardSize();

            playerControllers = new PlayerControl[noOfPlayers];
            playerLives = new LivesAndScore[noOfPlayers];
            //playerPositions = new int[noOfPlayers];
            initialisePlayerReferences();

            StaticCollections _staticColections = new StaticCollections();

            powerupRef = new Powerups();
            powerupRef.curGameGrid = GameGridXAML;
            powerupRef.InitialisePowerups();


        //    // End timer
        //    gameTimerInstance.processFrameEvent_TICK += GameTimerInstance_processFrameEvent_TICK;
        //    endTimer = new DispatcherTimer(DispatcherPriority.Render);
        //    endTimer.Interval = TimeSpan.FromSeconds(0.5);
        //    endTimer.Tick += new EventHandler(timer_Tick);
        //    endTimer.Start();

        //}

        //private void GameTimerInstance_processFrameEvent_TICK(object sender, EventArgs e)
        //{
        //    currentTick += 0.5;

        //    if (currentTick % 1 == 0)
        //    {

        //        // Decrement the timer
        //        gameTimeSeconds -= 1;

        //        if (gameTimeSeconds <= -1)
        //        {
        //            gameTimeSeconds = 59;
        //            gameTimeMinutes -= 1;

        //            if (gameTimeMinutes <= -1)
        //            {
        //                //MessageBox.Show("Four minutes passed. End of game reached.");
        //                gameTimerInstance.gameLoopTimer.Stop();

        //                MainWindow mwRef = MainWindow.ReturnMainWindowInstance();
        //                //mwRef.GameEnd();
        //            }
        //        }

        //    }

        //    //provideAllPlayerPositions();
        //    CheckPlayersOnBoard();

        //    // "D2" = Standard Numeric Formatting. Ensures that the seconds will always be displayed in double digits.
        //    gameTimeText1.Content = gameTimeMinutes + ":" + gameTimeSeconds.ToString("D2");
        //    gameTimeText2.Content = gameTimeMinutes + ":" + gameTimeSeconds.ToString("D2");

        //    Random tempRandom = new Random();
        //    randomNo4PowerUps = (short)tempRandom.Next(0, 3);
        }

        //public void timer_Tick(object sender, EventArgs e)
        //{
        //    currentTick += 0.5;

        //    if (currentTick % 1 == 0)
        //    {

        //        // Decrement the timer
        //        gameTimeSeconds -= 1;

        //        if (gameTimeSeconds <= -1)
        //        {
        //            gameTimeSeconds = 59;
        //            gameTimeMinutes -= 1;

        //            if (gameTimeMinutes <= -1)
        //            {
        //                //MessageBox.Show("Four minutes passed. End of game reached.");
        //                gameTimerInstance.gameLoopTimer.Stop();

        //                MainWindow mwRef = MainWindow.ReturnMainWindowInstance();
        //                mwRef.GameEnd();
        //            }
        //        }

        //    }

        //    //provideAllPlayerPositions();
        //    CheckPlayersOnBoard();

        //    // "D2" = Standard Numeric Formatting. Ensures that the seconds will always be displayed in double digits.
        //    gameTimeText1.Content = gameTimeMinutes + ":" + gameTimeSeconds.ToString("D2");
        //    gameTimeText2.Content = gameTimeMinutes + ":" + gameTimeSeconds.ToString("D2");

        //    Random tempRandom = new Random();
        //    randomNo4PowerUps = (short)tempRandom.Next(0, 3);
        //}

        private void initialiseGameBoardSize()
        {
            if (noOfPlayers == 4)
            {
                gameTimeText1.Margin = new Thickness(900, 0, -10, 0);
                gameTimeText2.Margin = new Thickness(-252, 0, -10, 0);
                TopPanel.HorizontalAlignment = HorizontalAlignment.Center;
                BottomPanel.HorizontalAlignment = HorizontalAlignment.Center;
                BottomPanel.Margin = new Thickness(60, 0, 0, 0);
            }
            else
            {
                gameTimeText1.Margin = new Thickness(400, 0, -10, 0);
                gameTimeText2.Margin = new Thickness(-252, 0, -10, 0);

            }
        }

        public void initialisePlayerReferences()
        {
            // Set the reference
            PlayerListRef = ListOfPlayers;


            // ------------------------------- Initialise Player References ------------------------------------------------
            for (int i = 0; i <= noOfPlayers - 1; i++)
            {
                playerControllers[i] = new PlayerControl();
                playerLives[i] = new LivesAndScore();
                Grid.SetRow(playerControllers[i], 2);
                Grid.SetColumn(playerControllers[i], 2);
                //playerControllers[i].localGameGrid = MainGameGrid;
                //playerControllers[i].highlightLocalGrid = MainGameGrid;
                //playerControllers[i].managerRef = gameBoardManager;
                //playerControllers[i].gridCellsArray = gameBoardManager.flrTiles;
                playerControllers[i].myLivesAndScore = playerLives[i];
                //playerControllers[i].initialisePlayerGridRef();
                //playerControllers[i].myPowerupRef = new Powerup();
                playerControllers[i].playerName = "Player " + (i + 1).ToString();

                // Add the player to the list
                ListOfPlayers.Add(playerControllers[i]);

                // --------------------------- Initialise All Players Lives and Score Controls -----------------------------
                initialisePlayerLivesAndScore(i);
                GameGridXAML.Children.Add(playerControllers[i]);
            }

            // I had to hard code the player's starting positions, but they'll update properly whenever the player moves.
            for (int x = 0; x < noOfPlayers; x++)
            {
                playerControllers[0].playerPosition = new Point(64, 64);

                if (noOfPlayers == 4)
                {
                    playerControllers[1].playerPosition = new Point(1344, 64);
                    playerControllers[2].playerPosition = new Point(1344, 704);
                    playerControllers[3].playerPosition = new Point(64, 704);
                }
                else if (noOfPlayers == 6)
                {
                    playerControllers[1].playerPosition = new Point(1344, 384);
                    playerControllers[2].playerPosition = new Point(64, 704);
                    playerControllers[3].playerPosition = new Point(64, 384);
                    playerControllers[4].playerPosition = new Point(1344, 64);
                    playerControllers[5].playerPosition = new Point(1344, 704);
                }
            }
        }

        public void initialisePlayerLivesAndScore(int currentPlayer)
        {
            switch (currentPlayer + 1)
            {
                case 1:

                    varRotTransform = 180;
                    trRot = new RotateTransform(varRotTransform);
                    playerLives[currentPlayer].LayoutTransform = trRot;
                    TopPanel.Children.Add(playerLives[currentPlayer]);
                    break;

                case 2:
                    varRotTransform = -90;
                    trRot = new RotateTransform(varRotTransform);
                    playerLives[currentPlayer].LayoutTransform = trRot;
                    RightPanel.Children.Add(playerLives[currentPlayer]);
                    break;

                case 3:
                    BottomPanel.Children.Add(playerLives[currentPlayer]);
                    break;

                case 4:
                    varRotTransform = 90;
                    trRot = new RotateTransform(varRotTransform);
                    playerLives[currentPlayer].LayoutTransform = trRot;
                    LeftPanel.Children.Add(playerLives[currentPlayer]);
                    break;

                case 5:
                    varRotTransform = 180;
                    trRot = new RotateTransform(varRotTransform);
                    playerLives[currentPlayer].LayoutTransform = trRot;
                    TopPanel2.Children.Add(playerLives[currentPlayer]);
                    break;

                case 6:
                    BottomPanel2.Children.Add(playerLives[currentPlayer]);
                    break;

                default:
                    //Debug.WriteLine("Nothing Happened!!");
                    break;
            }
        }

        private void CheckPlayersOnBoard()
        {
            for (int i = 0; i < noOfPlayers; i++)
            {
                if (playerControllers[i].myLivesAndScore.playerLivesNumber == 0 && playerControllers[i] != null)
                {
                    GameGridXAML.Children.Remove(playerControllers[i]);

                    playerControllers[i].myLivesAndScore.playerLivesNumber = -1;
                    noOfPlayers--;

                    if (noOfPlayers <= 1)
                    {
                        gameTimeMinutes = 0;
                        gameTimeSeconds = 0;
                    }
                }
            }
        }

        public void ChangeTimerText(int seconds, int minutes)
        {
            gameTimeText1.Content = minutes + ":" + seconds.ToString("D2");
            gameTimeText2.Content = gameTimeText1.Content;
        }
    }
}
