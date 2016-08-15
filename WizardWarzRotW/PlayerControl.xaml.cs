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
        //public PlayerUserControl playerTileAnimOverlay;
        //public Rectangle playerTile;
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
            facingRight = true;
            gameCanRef = GameBoard.ReturnMainCanvas();
            facingRightImage = new BitmapImage(new Uri("pack://application:,,,/Resources/ZombieHunter_SpriteSheet.png", UriKind.Absolute));
            facingLeftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/ZombieHunter_SpriteSheet_facingLeft.png", UriKind.Absolute));
            Loaded += PlayerControl_Loaded;
        }

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
            LastTouchDown = e.GetTouchPoint(GameBoard.ReturnGameBoardInstance()).Position;
        }

        private void UserControl_PreviewTouchUp(object sender, TouchEventArgs e)
        {

        }

        private void UserControl_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            LastTouchDown = e.GetTouchPoint(GameBoard.ReturnGameBoardInstance()).Position;
        }

        public void UpdatePlayerStatus(string pStatus)
        {

        }
    }
}
