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
        public Grid localGameGrid = null;
        public Grid highlightLocalGrid = null;
        public Int32 tileSize, bombRadius = 3;
        public Int32[,] playerGridLocArray;
        public int playerStartPos;
        public Color playerColour = new Color();
        string playerImage;
        public GameBoard managerRef = null;
        public Image[,] gridCellsArray = null;

        //Animated Tile
        //SpritesheetImage animPlayerTile;
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

        public List<FrameworkElement> pathCells = new List<FrameworkElement>();
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
            facingRight = true;
            gameCanRef = GameBoard.ReturnMainCanvas();
            facingRightImage = new BitmapImage(new Uri("pack://application:,,,/Resources/ZombieHunter_SpriteSheet.png", UriKind.Absolute));
            facingLeftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/ZombieHunter_SpriteSheet_facingLeft.png", UriKind.Absolute));
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
            
            SpritesheetImage animPlayerTile = new SpritesheetImage()
            {
                Source = facingRightImage,
                FrameMaxX = 5,
                FrameMaxY = 2,
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
            //animPlayerTile.PlaysRemaining = 10;
            //
            //Grid.SetColumn(pTile, 1);
            //Grid.SetRow(pTile, 1);
            //myGrid.Children.Add(pTile);


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

        public void UpdatePlayerStatus(string pStatus)
        {

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
                GameBoard.ReturnGameGrid().Children.Add(this);
                _step++;
                return;
            }
                //if (_myTime <= PlayerPositions.GetLength(0))
                //{
                //    // NOT HAPPENING FOR EVERY PART OF THE ARRAY, AS NOT RUNNING EACH 60 Frames
                //    if (PlayerPositions[_myTime, 0] != 0)
                //    {
                //        Grid.SetColumn(this, PlayerPositions[_myTime, 0]);
                //        Grid.SetRow(this, PlayerPositions[_myTime, 1]);
                //    }

                //    if (_myTime == PlayerPositions.GetLength(0))
                //    {
                //        _myTime = 0;
                //        _canMove = false;
                //        return;
                //    }

                //    GameBoard.ReturnGameGrid().Children.Remove(this);
                //    GameBoard.ReturnGameGrid().Children.Add(this);

                //}
                //else
                //{
                //    _canMove = false;
                //}
            //for (int i = _myTime; i < PlayerPositions.GetLength(0);)
            //{
            //    if (PlayerPositions[i, 0] != 0)
            //    {
            //        Grid.SetColumn(this, PlayerPositions[i, 0]);
            //        Grid.SetRow(this, PlayerPositions[i, 1]);
            //    }
            //    else
            //    {
            //        break;
            //    }
            //    GameBoard.ReturnGameGrid().Children.Remove(this);
            //    GameBoard.ReturnGameGrid().Children.Add(this);
            //    return;
            //}
            



        }

        protected virtual void OnDoubleTouchDown()
        {
            DropBomb();
            if (DoubleTouchDown != null)
            {
                
                

                DoubleTouchDown(this, EventArgs.Empty);
            }
        }

        private void DropBomb()
        {
            if (StaticCollections.CheckBombPosition(colCheckCur, rowCheckCur) == true)
            {
                //Debug.WriteLine("Dropped Bomb!");

                Bombs fireBomb = new Bombs(GameBoard.ReturnGameGrid());
                fireBomb.managerRef = managerRef;
                fireBomb.myOwner = this;

                //localGameGrid.Children.Remove(playerTile);

                if (playerState == "Superbomb")
                    bombRadius += 3;

                fireBomb.InitialiseBomb(colCheckCur, rowCheckCur, bombRadius);
                //localGameGrid.Children.Add(playerTile);

                // Play Bomb Explode Sound (Should also play the tick sound here)
                //playMusic.playBombExplode(); - move to Bomb.cs

                //add bomb reference to bomb collection
                StaticCollections.AddBomb(fireBomb, colCheckCur, rowCheckCur);

                //MessageBox.Show(string.Format("Player state: {0}", playerState));
                if (playerState == "Superbomb")
                {
                    bombRadius = 3;
                    playerState = null;

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
    }
}
