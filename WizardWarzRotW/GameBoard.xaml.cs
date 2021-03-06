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

    public enum PowerupTileStates
    {
        Empty,
        Superbomb,
        Shield,
        Lifeup
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
        public static PowerupTileStates[,] powerupTileState = null;

        static Random randomNumber = new Random();

        public int[,] innerWallPos;

        public int[,] destructibleWallPos;
        // ----------------------------- END GAME BOARD -------------------------------------------

        // ------------------------------ PUBLIC GAME DEPENDENCIES -----------------------------
        protected static Int32 tileSize = 54;
        private static GameTimer newGameTimer = null;
        double varRotTransform = 90;        
        protected static GameBoard gameBoardManager;
        protected static Canvas GameCanvasInstance;
        protected static List<PlayerControl> PlayerListRef;
        protected static Grid gameGrid;
        public RotateTransform trRot = null;
        protected static int noOfPlayers = 4;
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
        /// Returns a reference to the Game Grid. <para> The Game Grid is where all game play happens. </para>
        /// </summary>
        public static Grid ReturnGameGrid()
        {
            return gameGrid;

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
            //MessageBox.Show("Have been initialised!");
            GameDependencies();

            //MessageBox.Show(noOfPlayers.ToString());

            InitializeGameBoard();
            tempInitialisePlayers();
        }

        public void InitializeGameBoard()
        {
            // Initialise the gameboard reference
            gameBoardManager = this;

            curTileState = new TileStates[cols, rows];
            powerupTileState = new PowerupTileStates[cols, rows];
            panel2.Width = (cols + 2) * tileSize;
            panel2.Height = (rows + 2) * tileSize;
            Separator.Width = tileSize * 7;
            Separator.Height = tileSize;
            gameTimeText1.Width = tileSize * 2.296875f;
            gameTimeText1.Height = tileSize;
            gameTimeText1.FontSize = tileSize * 0.6f;
            gameTimeText2.Width = tileSize * 2.296875f;
            gameTimeText2.Height = tileSize;
            gameTimeText2.FontSize = tileSize * 0.6f;

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

                    // populate the secondary poweruptile grid with empty tiles.
                    powerupTileState[c, r] = PowerupTileStates.Empty;


                    //add a wall tile if along the grid extremes
                    if (InitialTilePlacementCheck(c, r, cols, rows) == true)
                    {
                        //flrTiles[c, r].Fill = new ImageBrush(new BitmapImage(new Uri(@".\Resources\Indesructable.png", UriKind.Relative)));
                        flrTiles[c, r].Source = new BitmapImage(new Uri("pack://application:,,,/Resources/StoneTile.png", UriKind.Absolute));
                        flrTiles[c, r].Stretch = Stretch.Fill;
                        curTileState[c, r] = TileStates.SolidWall;
                    }
                    //add destructible walls within the game grid
                    else if (DestructableWallPlacementCheck(c, r) == true)
                    {
                        //flrTiles[c, r].Fill = new ImageBrush(new BitmapImage(new Uri(@".\Resources\Destructible.png", UriKind.Relative)));
                        flrTiles[c, r].Source = new BitmapImage(new Uri("pack://application:,,,/Resources/DEstructible2.png", UriKind.Absolute));
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
                    flrTiles[c, r].Height = tileSize;
                    flrTiles[c, r].Width = tileSize;
                    Grid.SetColumn(flrTiles[c, r], c);
                    Grid.SetRow(flrTiles[c, r], r);


                    GameGridXAML.Children.Add(flrTiles[c, r]);
                }
            }

        }

        //Randomly assigns grid cell positions for "Destructible" wall tiles and adds them to an array
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

        
        /// <summary>
        /// Check if the passed grid column/row position ("c" and "r") should be assigned a SolidWall tile and state 
        /// </summary>
        /// <param name="c">The Grid column position</param>
        /// <param name="r">The Grid row position</param>
        /// <param name="colsLength">The grid's total column count</param>
        /// <param name="rowsLength">The grid's total row count</param>
        /// <returns>true or false</returns>
        public bool InitialTilePlacementCheck(Int32 c, Int32 r, Int32 colsLength, Int32 rowsLength)
        {
            innerWallPos = new Int32[(rows * cols), 2]; //this is waaay longer than required but don't have much choice

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
            if(newGameTimer != null)
            {
                newGameTimer.StopGameTimer();
            }

            if (newGameTimer == null)
            {
                newGameTimer = new GameTimer();

            }
            

            newGameTimer.StartGameTimer();

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

            gameGrid = GameGridXAML;
            initialiseGameBoardSize();

            playerControllers = new PlayerControl[noOfPlayers];
            playerLives = new LivesAndScore[noOfPlayers];
            //playerPositions = new int[noOfPlayers];
            

            StaticCollections _staticColections = new StaticCollections();
                        
            
        }

        private void tempInitialisePlayers()
        {
            initialisePlayerReferences();
            powerupRef = new Powerups();
            powerupRef.curGameGrid = GameGridXAML;
            powerupRef.InitialisePowerups();
        }

        private void initialiseGameBoardSize()
        {
            if (noOfPlayers == 4)
            {
                gameTimeText1.Margin = new Thickness(tileSize * 14.06, 0, -10, 0);
                gameTimeText2.Margin = new Thickness(tileSize * -3.9, 0, -10, 0);
                TopPanel.HorizontalAlignment = HorizontalAlignment.Center;
                BottomPanel.HorizontalAlignment = HorizontalAlignment.Center;
                BottomPanel.Margin = new Thickness(tileSize * 0.9375, 0, 0, 0);
            }
            else
            {
                gameTimeText1.Margin = new Thickness(tileSize * 6.25, 0, -10, 0);
                gameTimeText2.Margin = new Thickness(tileSize * -3.9, 0, -10, 0);

            }
        }

        public void initialisePlayerReferences()
        {         
            // ------------------------------- Initialise Player References ------------------------------------------------
            for (int i = 0; i <= noOfPlayers - 1; i++)
            {
                playerControllers[i] = new PlayerControl();
                playerLives[i] = new LivesAndScore();
                Grid.SetRow(playerControllers[i], 1);
                Grid.SetColumn(playerControllers[i], 1);
                
                playerControllers[i].managerRef = gameBoardManager;
                playerControllers[i].gridCellsArray = flrTiles;
                playerControllers[i].myLivesAndScore = playerLives[i];
                //playerControllers[i].initialisePlayerGridRef();
                playerControllers[i].myPowerupRef = new Powerups();
                playerControllers[i].playerName = "Player " + (i + 1).ToString();
                playerControllers[i].SetPlayerSpritesheet();

                

                // Set their position on the grid
                SetPlayerPosition(i);

                // --------------------------- Initialise All Players Lives and Score Controls -----------------------------
                initialisePlayerLivesAndScore(i);
                // Add the player to the list
                ListOfPlayers.Add(playerControllers[i]);
                GameGridXAML.Children.Add(playerControllers[i]);
                
                Canvas.SetZIndex(playerControllers[i],10);
            }

            // Set the reference
            PlayerListRef = ListOfPlayers;

            
        }

        //Set the start positions for each player on the game board
        public void SetPlayerPosition(int playerNumber)
        {
           
            switch(playerNumber)
            {
                case (0):
                    Grid.SetColumn(playerControllers[playerNumber], 1);
                    Grid.SetRow(playerControllers[playerNumber], 1);
                    break;
                case (1):
                    if(noOfPlayers == 6)
                    {
                        // BECOME PLAYER 3
                        Grid.SetColumn(playerControllers[playerNumber], 21);
                        Grid.SetRow(playerControllers[playerNumber], 6);
                        
                    }
                    else
                    {
                        // AM PLAYER 2
                        Grid.SetColumn(playerControllers[playerNumber], 21);
                        Grid.SetRow(playerControllers[playerNumber], 1);
                        
                    }
                    break;
                case (2):
                    if(noOfPlayers == 6)
                    {
                        //AM PLAYER 5
                        Grid.SetColumn(playerControllers[playerNumber], 1);
                        Grid.SetRow(playerControllers[playerNumber], 11);
                    }
                    else
                    {
                        // AM PLAYER 3
                        Grid.SetColumn(playerControllers[playerNumber], 21);
                        Grid.SetRow(playerControllers[playerNumber], 11);
                        
                    }                    
                    break;
                case (3):
                    if(noOfPlayers == 6)
                    {
                        // AM PLAYER 6
                        Grid.SetColumn(playerControllers[playerNumber], 1);
                        Grid.SetRow(playerControllers[playerNumber], 6);
                    }
                    else 
                    {
                        // AM PLAYER 4
                        Grid.SetColumn(playerControllers[playerNumber], 1);
                        Grid.SetRow(playerControllers[playerNumber], 11);
                    }                    
                    break;
                case (4):
                    Grid.SetColumn(playerControllers[playerNumber], 21);
                    Grid.SetRow(playerControllers[playerNumber], 1);
                    
                    break;
                case (5):
                    Grid.SetColumn(playerControllers[playerNumber], 21);
                    Grid.SetRow(playerControllers[playerNumber], 11);
                    
                    break;
            }
        }

        public void initialisePlayerLivesAndScore(int currentPlayer)
        {
            
            switch (currentPlayer)
            {
                case 0:
                    TopPanel.Height = tileSize;
                    varRotTransform = 180;
                    trRot = new RotateTransform(varRotTransform);
                    playerLives[currentPlayer].LayoutTransform = trRot;
                    TopPanel.Children.Add(playerLives[currentPlayer]);
                    break;

                case 1:
                    RightPanel.Height = tileSize * 6;
                    RightPanel.Width = tileSize;
                    varRotTransform = -90;
                    trRot = new RotateTransform(varRotTransform);
                    playerLives[currentPlayer].LayoutTransform = trRot;
                    RightPanel.Children.Add(playerLives[currentPlayer]);
                    break;

                case 2:
                    BottomPanel.Height = tileSize;
                    BottomPanel.Children.Add(playerLives[currentPlayer]);
                    break;

                case 3:
                    LeftPanel.Height = tileSize * 6;
                    LeftPanel.Width = tileSize;
                    varRotTransform = 90;
                    trRot = new RotateTransform(varRotTransform);
                    playerLives[currentPlayer].LayoutTransform = trRot;
                    LeftPanel.Children.Add(playerLives[currentPlayer]);
                    break;

                case 4:
                    TopPanel2.Height = tileSize;
                    varRotTransform = 180;
                    trRot = new RotateTransform(varRotTransform);
                    playerLives[currentPlayer].LayoutTransform = trRot;
                    TopPanel2.Children.Add(playerLives[currentPlayer]);
                    break;

                case 5:
                    BottomPanel2.Margin = new Thickness(0, tileSize * 12.96875, 0, 0);
                    BottomPanel2.Height = tileSize;
                    BottomPanel2.Children.Add(playerLives[currentPlayer]);
                    break;

                default:
                    //Debug.WriteLine("Nothing Happened!!");
                    break;
            }
        }

        public void CheckPlayersOnBoard()
        {
            
            //for (int i = 0; i < noOfPlayers; i++)
            for (int i = 0; i < playerControllers.Count(); i++)
            {
                if (playerControllers[i].myLivesAndScore.playerLivesNumber == 0 && GameGridXAML.Children.Contains(playerControllers[i]))
                {
                    

                    //playerControllers[i].myLivesAndScore.playerLivesNumber = -1;
                    //playerControllers[i].myLivesAndScore.currentScore -= 150;
                    GameGridXAML.Children.Remove(playerControllers[i]);
                    
                    noOfPlayers--;
                    //MessageBox.Show(string.Format("No Of Players {0}", noOfPlayers));
                    if (noOfPlayers <= 1)
                    {
                        //gameTimeMinutes = 0;
                        //gameTimeSeconds = 0;
                        foreach (PlayerControl pc in playerControllers)
                        {
                            pc.DestroyPlayer();
                        }
                        MainWindow.ReturnMainWindowInstance().ChangeGameState("end");
                    }
                }
            }
            //MessageBox.Show(string.Format("COUNT = {0}, No. = {1}", playerControllers.Count().ToString(), noOfPlayers.ToString()));
            
        }

        public void ChangeTimerText(int seconds, int minutes)
        {
            gameTimeText1.Content = minutes + ":" + seconds.ToString("D2");
            gameTimeText2.Content = gameTimeText1.Content;
        }

        public void ChangeTileImage(int PosX, int PosY)
        {
            flrTiles[PosX, PosY].Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Floor.png", UriKind.Absolute));
        }

        public void ChangeTileState(int PosX, int PosY, string tileState)
        {
            switch (tileState)
            {
                case ("Superbomb"):
                    curTileState[PosX, PosY] = TileStates.Powerup;
                    powerupTileState[PosX, PosY] = PowerupTileStates.Superbomb;
                    break;
                case ("Shield"):
                    curTileState[PosX, PosY] = TileStates.Powerup;
                    powerupTileState[PosX, PosY] = PowerupTileStates.Shield;
                    break;
                case ("Lifeup"):
                    curTileState[PosX, PosY] = TileStates.Powerup;
                    powerupTileState[PosX, PosY] = PowerupTileStates.Lifeup;
                    break;
            }
        }

        /// <summary>
        /// This function returns to the distance between two points. <para>Takes in two POINTS (A & B), and puts out the distance between them.</para>
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static double GetDistanceBetweenPoints(Point p, Point q)
        {
            double a = p.X - q.X;
            double b = p.Y - q.Y;
            double distance = Math.Sqrt(a * a + b * b);
            return distance;

        }

        
    }
}
