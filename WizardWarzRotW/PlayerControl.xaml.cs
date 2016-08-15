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
        public Rectangle[,] gridCellsArray = null;
        public Canvas gameCanRef = null;
        public LivesAndScore myLivesAndScore = null;
        public Powerups myPowerupRef = null;

        private Point LastTouchDown;

        public PlayerControl()
        {
            InitializeComponent();
            PlayerPositions = new int[20, 2];
            ResetPlayerPositionArray();
            facingRight = true;
            gameCanRef = GameBoard.ReturnMainCanvas();
            facingRightImage = new BitmapImage(new Uri("pack://application:,,,/Resources/ZombieHunter_SpriteSheet.png", UriKind.Absolute));
            facingLeftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/ZombieHunter_SpriteSheet_facingLeft.png", UriKind.Absolute));
            Loaded += PlayerControl_Loaded;
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
            if (IsDoubleTap(e))
            {
                Debug.WriteLine("Double Touch");
                OnDoubleTouchDown();
            }

            ResetPlayerPositionArray();

            this.lastTouchDownPoint = e.GetTouchPoint((UIElement)sender).Position;

            Debug.WriteLine(string.Format("FIRST TOUCH: {0}", lastTouchDownPoint));
        }

        private void UserControl_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            MoveThePlayer();
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

            colCheckCur = (int)tp.Position.X / 64;
            rowCheckCur = (int)tp.Position.Y / 64;
            //int colCheckPrev = PlayerPosition[]

            if (colCheckCur != checkCellColPos || rowCheckCur != checkCellRowPos)
            {
                for (int i = 0; i < PlayerPositions.GetLength(0); i++)
                {
                    if (PlayerPositions[i, 0] == 0)
                    {
                        if (GameBoard.curTileState[colCheckCur, rowCheckCur] == TileStates.Floor)
                        {
                            PlayerPositions[i, 0] = colCheckCur;
                            PlayerPositions[i, 1] = rowCheckCur;

                            checkCellColPos = colCheckCur;
                            checkCellRowPos = rowCheckCur;

                            break;
                        }
                        else
                        {
                            MoveThePlayer();
                            return;
                        }
                    }
                }

            }
            else if (GameBoard.curTileState[colCheckCur, rowCheckCur] == TileStates.DestructibleWall)
            {
                MoveThePlayer();

            }

            Debug.WriteLine(String.Format("{0}, {1}", ((int)tp.Position.X / 64), ((int)tp.Position.Y / 64)));

            newPlayer.CaptureTouch(e.TouchDevice);

            e.Handled = true;
        }

        public void UpdatePlayerStatus(string pStatus)
        {

        }

        private void MoveThePlayer()
        {


            for (int i = 0; i < PlayerPositions.GetLength(0); i++)
            {
                if (PlayerPositions[i, 0] != 0)
                {
                    Grid.SetColumn(this, PlayerPositions[i, 0]);
                    Grid.SetRow(this, PlayerPositions[i, 1]);
                }
                else
                {
                    break;
                }
                GameBoard.ReturnGameGrid().Children.Remove(this);
                GameBoard.ReturnGameGrid().Children.Add(this);
            }



        }

        protected virtual void OnDoubleTouchDown()
        {
            if (DoubleTouchDown != null)
            {

                DoubleTouchDown(this, EventArgs.Empty);
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
