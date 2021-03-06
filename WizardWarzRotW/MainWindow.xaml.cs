﻿
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
using System;

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
        public GameStates currentGameState = GameStates.Title;

        protected static MainWindow MainWindowReference;

        public TitleScreen title;
        public MainMenu menu;
        public GameBoard game;
        public EndScreen end;

        public static bool GlobalAudio1
        {
            get; set;
        }

        /// <summary>
        /// Returns the instance of the Main Window. Used primarily to change between game states.
        /// </summary>
        /// <returns></returns>
        public static MainWindow ReturnMainWindowInstance()
        {
            return MainWindowReference;
        }

        public MainWindow()
        {
            InitializeComponent();
            // Initialise Audio
            GlobalAudio1 = true;
            AudioMan.audioOn = GlobalAudio1;
            AudioMan.playWizardOne();

            MainWindowReference = this;

            // Initialize each instance of each screen when the game loads.
            title = TitleScreenInstance;
            menu = MainMenuInstance;
            //game = GameInstance;
            //end = EndScreenInstance;

            // Reveal the title screen.
            ChangeGameState("title");
        }

        /// <summary>
        /// Method that changes the state of the game. Will only work if the game states are cycled through in order.
        /// <para> The four states of the game that can be passed are (in order): "title", "menu", "game", "end". </para>
        /// </summary>
        /// <param name="GameState"></param>
        public void ChangeGameState(string GameState)
        {
            switch (GameState)
            {
                case ("title"):
                    TitleScreenInstance.Visibility = Visibility.Visible;
                    AudioMan.titleOrMain = false;
                    currentGameState = GameStates.Title;

                    if (end != null)
                    {
                        if (end.Visibility == Visibility.Visible)
                        {
                            end.Visibility = Visibility.Hidden;
                            game = null;
                            end = null;
                        }
                    }

                    AudioMan.StopTrack();
                    AudioMan.playWizardOne();
                    title.Visibility = Visibility.Visible;
                    break;

                case ("menu"):
                    currentGameState = GameStates.MainMenu;                    
                    title.Visibility = Visibility.Hidden;
                    menu.Visibility = Visibility.Visible;
                    break;

                case ("game"):
                    if (game == null)
                    {
                        game = null;

                        game = new GameBoard();
                        GameCANVAS.Children.Add(game);
                    }
                    
                     
                    currentGameState = GameStates.Game;

                    menu.Visibility = Visibility.Hidden;
                    game.Visibility = Visibility.Visible;

                    AudioMan.StopTrack();
                    AudioMan.titleOrMain = true;
                    AudioMan.playMainMusic();

                    // Set the timer (plus text) and start it. Format: seconds, minutes
                    GameBoard.ReturnGameBoardInstance().ChangeTimerText(59, 3);
                    GameTimer.ReturnTimerInstance().StartGameTimer();
                    GameTimer.ReturnTimerInstance().GameTimeSeconds = 59;
                    GameTimer.ReturnTimerInstance().GameTimeMinutes = 3;
                    GameTimer.ReturnTimerInstance().currentTick = 0;
                    break;

                case ("end"):
                    currentGameState = GameStates.EndScreen;
                    game.Visibility = Visibility.Collapsed;

                    if (end == null)
                    {
                        end = null;

                        end = new EndScreen();
                        GameCANVAS.Children.Add(end);
                        
                        GameBoard.ReturnGameGrid().Children.Clear();
                        GameBoard.ReturnPlayerList().Clear();
                    }
                                
                    end.Visibility = Visibility.Visible;

                    // Set the end timer, timer text, and start the timer.
                    EndScreen.ReturnEndScreenInstance().currentTick = 0;
                    EndScreen.ReturnEndScreenInstance().endCountdown = 10;
                    EndScreen.ReturnEndScreenInstance().endGameTimer.Start();
                    EndScreen.ReturnEndScreenInstance().endTimer.Content = (EndScreen.ReturnEndScreenInstance().endCountdown + " seconds.");
                    break;
            }
            //MessageBox.Show(GameCANVAS.Children.Count.ToString());
            Console.WriteLine("Current game state: {0}", currentGameState);
        }
    }
}
