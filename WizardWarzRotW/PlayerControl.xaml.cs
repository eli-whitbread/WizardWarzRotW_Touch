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
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        // ALL THE NEW STUFF
        private readonly Stopwatch _doubleTapStopwatch = new Stopwatch();
        private Point? _lastTapLocation;
        private int[,] PlayerPositions;
        public EventHandler DoubleTouchDown;
        Int32 checkCellColPos, checkCellRowPos;
        private int colCheckCur;
        private int rowCheckCur;
        // END NEW STUFF
        private GameTimer _playerControlTimer = null;
        Int32 _myTime, _count, _step = 0, _cellCount = 0;
        bool _canMove = false, _startMoving = false;
        public Point currentPOS;
        public Point lastClickPOS;
        public Point playerPosition;
        public Point relativePosition, localBombRelative;
        //public Grid localGameGrid = null;
        public Grid highlightLocalGrid = null;
        public Int32 tileSize, bombRadius = 3;
        public Int32[,] playerGridLocArray;
        public int playerStartPos;
        public Color playerColour = new Color();
        string playerImage;
        public GameBoard managerRef = null;
        public Image[,] gridCellsArray = null;

        //Animated Tile
        SpritesheetImage animPlayerTile;
        public BitmapImage facingRightImage, facingLeftImage;
        public bool facingRight;

        //public GameTimer playerTimerRef = null;
        //AudioManager playMusic = new AudioManager();

        public string playerState = null;
        public string playerName = null;

        //int p1PathCellCount = 0;
        public int playerX = 0;
        public int playerY = 0;

        float movementTimer = 0;

        //bool p1HasPath = false;
        //bool isP1Influenced = false;
        bool isTouched = false;

        Point curMousePos = new Point(0, 0);

        //public List<FrameworkElement> pathCells = new List<FrameworkElement>();
        List<Ellipse> pathHighlightTile = new List<Ellipse>();
        //public Rectangle[,] gridCellsArray = null;
        public Canvas gameCanRef = null;
        public LivesAndScore myLivesAndScore = null;
        public Powerups myPowerupRef = null;

        private Point LastTouchDown;

        public PlayerControl()
        {
            InitializeComponent();
            _playerControlTimer = GameTimer.ReturnTimerInstance();
            _playerControlTimer.processFrameEvent_TICK += _playerControlTimer_processFrameEvent_TICK;
            _myTime = 0;
            _count = 0;
            PlayerPositions = new int[20, 2];
            ResetPlayerPositionArray();
            gameCanRef = GameBoard.ReturnMainCanvas();
            //facingRightImage = new BitmapImage(new Uri("pack://application:,,,/Resources/ZombieHunter_SpriteSheet.png", UriKind.Absolute));
            //facingLeftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/ZombieHunter_SpriteSheet_facingLeft.png", UriKind.Absolute));
            facingRight = true;
            highlightLocalGrid = GameBoard.ReturnGameGrid();
            Loaded += PlayerControl_Loaded;
        }

        // ---------------------------------------------------------------------
        // ----------------------PLAYER TICK PROCESS -----------------------------------
        // ---------------------------------------------------------------------
        private void _playerControlTimer_processFrameEvent_TICK(object sender, EventArgs e)
        {
            //if(_cellCount >= 4 && _canMove == false && _startMoving == true)
            //{
            //    //_canMove = true;
            //    MoveThePlayer();
            //    //_cellCount = 0;
            //}

            _count++;

            if(_count >= 5)
            {
                _myTime++;
                _count = 0;

                if(_canMove == true)
                {
                    if(_step <= PlayerPositions.GetLength(0) )
                    {
                        MoveThePlayer();
                    }
                    else
                    {
                        ResetPlayerPositionArray();
                        //_myTime = 0;
                        //_step = 0;
                        //_cellCount = 0;
                        //_canMove = false;
                    }
                    
                }
                
            }

            
            
        }

        void ResetPlayerPositionArray()
        {
            
            for (int i = 0; i < PlayerPositions.GetLength(0); i++)
            {
                PlayerPositions[i, 0] = 0;
                PlayerPositions[i, 1] = 0;
            }
            checkCellColPos = 0;
            checkCellRowPos = 0;
            _cellCount = 0;
            _canMove = false;
            _myTime = 0;
            _step = 0;
            //_startMoving = false;
            
        }
        private Point lastTouchDownPoint;

        private void PlayerControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            animPlayerTile = new SpritesheetImage()
            {
                Source = facingRightImage,
                FrameMaxX = 4,
                FrameMaxY = 1,
                FrameRate = 30,
                Width = 64,
                Height = 64,
                PlaysRemaining = 10,
                LoopForever = true
            };
            animPlayerTile.AnimationComplete += (o, s) =>
            {
                myCanvas.Children.Remove(animPlayerTile);
            };

            Point centerPoint = new Point(this.ActualWidth / 2, this.ActualHeight / 2);

            //Grid.SetColumn(animPlayerTile, 0);
            //Grid.SetRow(animPlayerTile, 0);

            myCanvas.Children.Add(animPlayerTile);
            Canvas.SetTop(animPlayerTile, centerPoint.Y - 32);
            Canvas.SetLeft(animPlayerTile, centerPoint.X - 32);
            SetDirection();
            //animPlayerTile.PlaysRemaining = 10;
            //
            //Grid.SetColumn(pTile, 1);
            //Grid.SetRow(pTile, 1);
            //myGrid.Children.Add(pTile);

            //used for non-touch bomb testing
            //DropBomb();
        }

        private void UserControl_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            
            _startMoving = true;
            ResetPlayerPositionArray();
            if (IsDoubleTap(e))
            {

                Debug.WriteLine("Double Touch");
                OnDoubleTouchDown();
            }

            

            this.lastTouchDownPoint = e.GetTouchPoint((UIElement)sender).Position;

            Debug.WriteLine(string.Format("FIRST TOUCH: {0}", lastTouchDownPoint));
        }

        private void UserControl_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            //MoveThePlayer();
            _canMove = true;
            _startMoving = false;
            DeleteHighlight();
        }

        private void UserControl_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            FrameworkElement newPlayer = sender as FrameworkElement;
            if (newPlayer == null)
            {
                return;
            }

            //this.lastTouchDownPoint = e.GetTouchPoint(newPlayer).Position;

            //Debug.WriteLine(string.Format("Touch Point: {0}", this.lastTouchDownPoint));


            TouchPoint tp = e.GetTouchPoint(GameBoard.ReturnGameGrid());

            colCheckCur = (int)tp.Position.X / GameBoard.ReturnTileSize();
            rowCheckCur = (int)tp.Position.Y / GameBoard.ReturnTileSize(); ;
            //int colCheckPrev = PlayerPosition[]

            
            
            if (colCheckCur != checkCellColPos || rowCheckCur != checkCellRowPos)
            {

                for (int i = 0; i < PlayerPositions.GetLength(0); i++)
                {
                    if (PlayerPositions[i, 0] == 0 && CheckCanAddCell(colCheckCur, rowCheckCur))
                    {
                        if (GameBoard.curTileState[colCheckCur, rowCheckCur] == TileStates.Floor || GameBoard.curTileState[colCheckCur, rowCheckCur] == TileStates.Powerup)
                        {
                            Debug.WriteLine(String.Format("Added: {0}, {1}", ((int)tp.Position.X / GameBoard.ReturnTileSize()), ((int)tp.Position.Y / GameBoard.ReturnTileSize())));
                            PlayerPositions[i, 0] = colCheckCur;
                            PlayerPositions[i, 1] = rowCheckCur;

                            HighlightPathCalc(colCheckCur, rowCheckCur);

                            checkCellColPos = colCheckCur;
                            checkCellRowPos = rowCheckCur;

                            _startMoving = false;
                            _cellCount++;
                            //Debug.WriteLine(string.Format("Cells: {0}", _cellCount));

                            break;
                        }
                        else
                        {
                            //MoveThePlayer();
                            DeleteHighlight();
                            _canMove = true;
                            return;
                        }
                    }
                    }
                //}
            }
            
            else if (GameBoard.curTileState[colCheckCur, rowCheckCur] == TileStates.DestructibleWall)
            {
                _canMove = true;
            }

            //Debug.WriteLine(String.Format("{0}, {1}", ((int)tp.Position.X / 64), ((int)tp.Position.Y / 64)));

            newPlayer.CaptureTouch(e.TouchDevice);

            e.Handled = true;
        }

        private bool CheckCanAddCell(int Col, int Row)
        {
            // CHECKS WHETHER THE COORDINATES ARE VALID
            Debug.WriteLine(String.Format("CHECK CELLS: {0}, {1}", Col, Row));
            Debug.WriteLine(String.Format("OLD CELLS: {0}, {1}", checkCellColPos, checkCellRowPos));
            if (checkCellColPos == 0 || checkCellRowPos == 0)
            {
                return true;
            }
            else if(Col != checkCellColPos && Row != checkCellRowPos)
            {
                return false;
            }
            else if (Col > checkCellColPos + 1 || Col < checkCellColPos - 1)
            {
                return false;
            }
            else if (Row > checkCellRowPos + 1 || Row < checkCellRowPos - 1)
            {
                return false;
            }
            


            return true;
        }

        private void MoveThePlayer()
        {
            for (int i = _step; i < PlayerPositions.GetLength(0); i++ )
            {
                if (PlayerPositions[_step, 0] == 0)
                {
                    _myTime = 0;
                    _step = 0;
                    _canMove = false;
                    return;
                }

                Grid.SetColumn(this, PlayerPositions[_step, 0]);
                Grid.SetRow(this, PlayerPositions[_step, 1]);
               

                if (_step == PlayerPositions.GetLength(0))
                {
                    ResetPlayerPositionArray();
                    //_myTime = 0;
                    //_step = 0;
                    //_cellCount = 0;
                    //_canMove = false;
                    return;
                }

                GameBoard.ReturnGameGrid().Children.Remove(this);

                if (_step > 0)
                {
                    if (PlayerPositions[_step, 0] > PlayerPositions[_step - 1, 0])
                    {
                        SwitchSpriteDirection(true);
                    }
                    else if(PlayerPositions[_step, 0] < PlayerPositions[_step - 1, 0])
                    {
                        SwitchSpriteDirection(false);
                    }
                }
                // The Bombs class needs these variables
                playerX = PlayerPositions[_step, 0];
                playerY = PlayerPositions[_step, 1];
                //Console.WriteLine(string.Format("Player {0} postion: {1} {2}", playerName, playerX, playerY));

                GameBoard.ReturnGameGrid().Children.Add(this);

                // Scan tile for powerups
                PowerupPlayer();

                //if (GameBoard.curTileState[PlayerPositions[_step, 0], PlayerPositions[_step, 1]] == TileStates.Powerup)
                //{
                //    playerState = myPowerupRef.ReturnPowerup(PlayerPositions[_step, 0], PlayerPositions[_step, 1], GameBoard.ReturnGameGrid());

                //    if (playerState == "Lifeup")
                //    {
                //        myLivesAndScore.currentScore += 50;

                //        if (myLivesAndScore.playerLivesNumber <= 2)
                //            myLivesAndScore.playerLivesNumber += 1;

                //        playerState = null;
                //    }
                //}
                
                _step++;
                return;
            }

        }

        protected virtual void OnDoubleTouchDown()
        {
            DropBomb();
            if (DoubleTouchDown != null)
            {
                
                

                DoubleTouchDown(this, EventArgs.Empty);
            }
        }

        // Method that scans the tile the player is currently on for a powerup.
        public void PowerupPlayer()
        {
            string tempStateFlag;
            string previousStateFlag;

            // Check the tile the player is on for power ups
            // The last two conditions are to prevent players from being unable to pick up extra lives while they're holding onto a powerup.
            if (GameBoard.curTileState[playerX, playerY] == TileStates.Powerup && playerState == null ||
                playerState == "Superbomb" && GameBoard.powerupTileState[playerX, playerY] == PowerupTileStates.Lifeup ||
                playerState == "Shield" && GameBoard.powerupTileState[playerX, playerY] == PowerupTileStates.Lifeup)
            {
                // Set the previousStateFlag flag
                previousStateFlag = playerState;

                //MessageBox.Show("Scanning for powerups.");
                tempStateFlag = myPowerupRef.ReturnPowerup(playerX, playerY, GameBoard.ReturnGameGrid());
                
                //MessageBox.Show(string.Format("Player state: {0]", playerState));

                if (tempStateFlag == "Lifeup")
                {
                    tempStateFlag = null;
                    myLivesAndScore.playerLivesNumber += 1;

                    if (myLivesAndScore.playerLivesNumber >= 4)
                        myLivesAndScore.playerLivesNumber = 3;

                    myLivesAndScore.CalculateLives();

                    // Prevent the player from losing their current powerup due to collecting an extra life
                    playerState = previousStateFlag;
                }

                else
                {
                    playerState = tempStateFlag;
                    
                }

                Console.WriteLine("Player State: {0}", playerState);

                UpdatePlayerStatus(playerState);
            }
        }

        private void DropBomb()
        {
            //used for non-touch bomb testing
            //colCheckCur = 1;
           // rowCheckCur = 1;
            
            if (StaticCollections.CheckBombPosition(colCheckCur, rowCheckCur) == true)
            {
                Debug.WriteLine("Dropped Bomb!");

                Bombs fireBomb = new Bombs(GameBoard.ReturnGameGrid());
                fireBomb.managerRef = managerRef;
                fireBomb.myOwner = this;

                //localGameGrid.Children.Remove(playerTile);

                if (playerState == "Superbomb")
                    bombRadius = bombRadius * 2;

                fireBomb.InitialiseBomb(colCheckCur, rowCheckCur, bombRadius);
                //localGameGrid.Children.Add(playerTile);

                // Play Bomb Explode Sound (Should also play the tick sound here)
                //playMusic.playBombExplode(); - move to Bomb.cs

                //add bomb reference to bomb collection
                StaticCollections.AddBomb(fireBomb, colCheckCur, rowCheckCur);

                //MessageBox.Show(string.Format("Player state: {0}", playerState));
                if (playerState == "Superbomb")
                {
                    bombRadius = bombRadius / 2;
                    playerState = null;
                    UpdatePlayerStatus("null");
                }
            }
        }

        private bool IsDoubleTap(TouchEventArgs e)
        {
            Point currentTapPosition = e.GetTouchPoint(this).Position;
            bool tapsAreCloseInDistance = false;
            if (_lastTapLocation != null)
            {
                tapsAreCloseInDistance = GameBoard.GetDistanceBetweenPoints(currentTapPosition, (Point)_lastTapLocation) < 32;
            }
            _lastTapLocation = currentTapPosition;

            TimeSpan elapsed = _doubleTapStopwatch.Elapsed;
            _doubleTapStopwatch.Restart();
            bool tapsAreCloseInTime = (elapsed != TimeSpan.Zero && elapsed < TimeSpan.FromSeconds(0.7));

            if (tapsAreCloseInTime && tapsAreCloseInDistance)
            {
                _lastTapLocation = null;
            }

            return tapsAreCloseInDistance && tapsAreCloseInTime;

        }

        /// <summary>
        /// Adds a highlight to pressed path cell, as a visual feedback for player to know where they have moved. <para>Takes in the current touched Col and Row as ints</para>
        /// </summary>
        /// <param name="Col"></param>
        /// <param name="Row"></param>
        private void HighlightPathCalc(int Col, int Row)
        {
            Ellipse highlight = new Ellipse();
            Ellipse highlight2 = new Ellipse();

            // ADD HIGHLIGHT
            highlight.Height = GameBoard.ReturnTileSize() * 0.18f;
            highlight.Width = GameBoard.ReturnTileSize() * 0.36;
            highlight2.Height = GameBoard.ReturnTileSize() * 0.36;
            highlight2.Width = GameBoard.ReturnTileSize() * 0.18f;
            DetermineHighlightColour();
            highlight.Fill = new SolidColorBrush(playerColour);
            highlight.Fill.Opacity = 0.4f;
            highlight.IsHitTestVisible = false;
            pathHighlightTile.Add(highlight);

            highlight2.Fill = new SolidColorBrush(playerColour);
            highlight2.Fill.Opacity = 0.4f;
            highlight2.IsHitTestVisible = false;
            pathHighlightTile.Add(highlight2);

            Grid.SetColumn(highlight, Col);
            Grid.SetRow(highlight, Row);
            Grid.SetColumn(highlight2, Col);
            Grid.SetRow(highlight2, Row);
            highlightLocalGrid.Children.Add(highlight);
            highlightLocalGrid.Children.Add(highlight2);
        }

        /// <summary>
        /// Deletes added highlights, which were previously added. <para>Should be run any time the players touch path is invalidated / completed.</para>
        /// </summary>
        private void DeleteHighlight()
        {
            foreach (Ellipse highlight in pathHighlightTile)
            {
                highlightLocalGrid.Children.Remove(highlight);
            }

        }

        private void DetermineHighlightColour()
        {
            switch (playerName)
            {
                // PLAYER 1
                case ("Player 1"):
                    playerColour = Colors.Silver;
                    //Debug.WriteLine("Player 1");
                    break;
                // PLAYER 2
                case ("Player 2"):
                    //Debug.WriteLine("Player 2");
                    playerColour = Colors.Red;
                    break;
                // PLAYER 3
                case ("Player 3"):
                    playerColour = Colors.Blue;
                    break;
                // PLAYER 4
                case ("Player 4"):
                    playerColour = Colors.Yellow;
                    break;
                // PLAYER 5
                case ("Player 5"):
                    playerColour = (Color)ColorConverter.ConvertFromString("#FFAC02FB");
                    break;
                // PLAYER 6
                case ("Player 6"):
                    playerColour = Colors.Green;
                    break;
                default:

                    break;
            }
        }

        public void UpdatePlayerStatus(string status)
        {
            switch (status)
            {
                case ("Superbomb"):
                    myLivesAndScore.playerHomeTile.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/Bomb2.png", UriKind.Absolute)));
                    break;
                case ("Shield"):
                    myLivesAndScore.playerHomeTile.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/shield.png", UriKind.Absolute)));
                    break;
                case ("null"):
                    myLivesAndScore.playerHomeTile.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/Home0.png", UriKind.Absolute)));
                    break;
            }
        }

        public void SetPlayerSpritesheet()
        {
            switch (playerName)
            {
                // PLAYER 1
                case ("Player 1"):
                    facingRightImage = new BitmapImage(new Uri("pack://application:,,,/Resources/SpriteSheetSilver_Right.png", UriKind.Absolute));
                    facingLeftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/SpriteSheetSilver_Left.png", UriKind.Absolute));
                    break;
                // PLAYER 2
                case ("Player 2"):
                    facingRightImage = new BitmapImage(new Uri("pack://application:,,,/Resources/SpriteSheetRed_Right.png", UriKind.Absolute));
                    facingLeftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/SpriteSheetRed_Left.png", UriKind.Absolute));
                    break;
                // PLAYER 3
                case ("Player 3"):
                    facingRightImage = new BitmapImage(new Uri("pack://application:,,,/Resources/SpriteSheetBlue_Right.png", UriKind.Absolute));
                    facingLeftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/SpriteSheetBlue_Left.png", UriKind.Absolute));
                    break;
                // PLAYER 4
                case ("Player 4"):
                    facingRightImage = new BitmapImage(new Uri("pack://application:,,,/Resources/SpriteSheetYellow_Right.png", UriKind.Absolute));
                    facingLeftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/SpriteSheetYellow_Left.png", UriKind.Absolute));
                    break;
                // PLAYER 5
                case ("Player 5"):
                    facingRightImage = new BitmapImage(new Uri("pack://application:,,,/Resources/SpriteSheetPurple_Right.png", UriKind.Absolute));
                    facingLeftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/SpriteSheetPurple_Left.png", UriKind.Absolute));
                    break;
                // PLAYER 6
                case ("Player 6"):
                    facingRightImage = new BitmapImage(new Uri("pack://application:,,,/Resources/SpriteSheetGreen_Right.png", UriKind.Absolute));
                    facingLeftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/SpriteSheetGreen_Left.png", UriKind.Absolute));
                    break;
                default:

                    break;
            }
        }

        /// <summary>
        /// Change the player's sprite acording to the passed bool "facingRight". true = facingRight, false for left
        /// </summary>
        /// <param name="facingRight"></param>
        public void SwitchSpriteDirection(bool facingRight)
        {
            if(facingRight == false)
            {
                animPlayerTile.Source = facingLeftImage;
            }
            else
            {
                animPlayerTile.Source = facingRightImage;
            }
        }

        public void SetDirection()
        {
            
            switch (playerName)
            {
                
                case ("Player 1"):
                    
                    break;
                case ("Player 2"):
                    if (GameBoard.ReturnNumberOfPlayer() == 6)
                    {
                        // BECOME PLAYER 3
                        SwitchSpriteDirection(false);
                    }
                    else
                    {
                        // AM PLAYER 2
                        SwitchSpriteDirection(false);
                    }
                    break;
                case ("Player 3"):
                    if (GameBoard.ReturnNumberOfPlayer() == 6)
                    {
                        //AM PLAYER 5
                    }
                    else
                    {
                        // AM PLAYER 3
                        SwitchSpriteDirection(false);
                    }
                    break;
                case ("Player 4"):
                    if (GameBoard.ReturnNumberOfPlayer() == 6)
                    {
                        // AM PLAYER 6
                    }
                    else
                    {
                        // AM PLAYER 4
                    }
                    break;
                case ("Player 5"):

                    // Am PLAYER 2
                    SwitchSpriteDirection(false);
                    break;
                case ("Player 6"):

                    // Am PLAYER 4
                    SwitchSpriteDirection(false);
                    break;
            }
        
        }

    }
}
