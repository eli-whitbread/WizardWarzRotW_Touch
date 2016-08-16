using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
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
    public enum GameStates
    {
        Title,
        MainMenu,
        Game,
        EndScreen
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameStates currentGameState = GameStates.Title;
        protected static GameStates changeState;
        protected static Canvas GameCanvasRef;

        public static TitleScreen title;
        public static MainMenu mainmenu;
        public static GameBoard game;
        public static EndScreen end;

        public static bool GlobalAudio1
        {
            get; set;
        }

        public MainWindow()
        {
            // Initialize static references
            changeState = currentGameState;
            GameCanvasRef = GameCANVAS;

            InitializeComponent();
            // Initialise Audio
            GlobalAudio1 = true;
            AudioMan.audioOn = GlobalAudio1;
            AudioMan.playWizardOne();

            //// Initialize each instance of each screen when the game loads.
            //title = new TitleScreen();
            //mainmenu = new MainMenu();
            //game = new GameBoard();
            //end = new EndScreen();

            title = new TitleScreen();
            GameCANVAS.Children.Add(title);
        }
              

        public static void ChangeGameState(string GameState)
        {
            switch (GameState)
            {
                case ("title"):
                    changeState = GameStates.Title;
                    title = new TitleScreen();

                    if (end != null)
                        GameCanvasRef.Children.Remove(end);

                    GameCanvasRef.Children.Add(title);
                    break;

                case ("mainmenu"):
                    changeState = GameStates.MainMenu;
                    mainmenu = new MainMenu();
                    GameCanvasRef.Children.Remove(title);
                    GameCanvasRef.Children.Add(mainmenu);
                    break;

                case ("game"):
                    changeState = GameStates.Game;
                    game = new GameBoard();
                    GameCanvasRef.Children.Remove(mainmenu);
                    GameCanvasRef.Children.Add(game);
                    break;

                case ("endscreen"):
                    changeState = GameStates.EndScreen;
                    end = new EndScreen();
                    GameCanvasRef.Children.Remove(game);
                    GameCanvasRef.Children.Add(end);
                    break;
            }

            Console.WriteLine("Current game state: {0}", changeState);
        }
    }
}
