using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;

namespace WizardWarzRotW
{
    class GameTimer
    {
        public DispatcherTimer gameLoopTimer;
        protected static GameTimer gameTimerInstance;
        private int fps = 60;
        public int currentTick = 0;

        public int GameTimeSeconds = 59;
        public int GameTimeMinutes = 4;
        private GameBoard gameBoardInstance;

        /// <summary>
        /// Event Flag for the games Render Tick Event. <para> Anything using this tick will be added to the Rendering tick thread (not an extra thread). </para>
        /// </summary> 
        public event EventHandler renderFrameEvent_TICK;

        /// <summary>
        /// Event Flag for the games Process Tick Event. <para> Anything using this tick will be added to the Processing tick thread (not an extra thread). </para>
        /// </summary>
        public event EventHandler processFrameEvent_TICK;

        /// <summary>
        /// Returns a references to the Game Timer. <para> This provides a way to easily reference the game timer (once, instead of several instances), within any class. </para>
        /// </summary>
        public static GameTimer ReturnTimerInstance()
        {
            return gameTimerInstance;
        }

        // ---------------------------------------------------------------------
        // ----------------------INITIALISE DispatchTimer-----------------------
        // ---------------------------------------------------------------------
        public GameTimer()
        {
            gameLoopTimer = new DispatcherTimer(DispatcherPriority.Render);
            gameTimerInstance = this;
            gameLoopTimer.Interval = TimeSpan.FromMilliseconds(1000/fps);
            gameLoopTimer.Tick += new EventHandler(timer_Tick);
            gameBoardInstance = GameBoard.ReturnGameBoardInstance();
        }

    
        public void StartGameTimer()
        {
            gameLoopTimer.Start();
        }

        public void StopGameTimer()
        {
            gameLoopTimer.Stop();
        }


        // ---------------------------------------------------------------------
        // ---------------------- TICK EVENT -----------------------------------
        // ---------------------------------------------------------------------
        public void timer_Tick(object sender, EventArgs e)
        {
            if (MainWindow.ReturnMainWindowInstance().currentGameState == GameStates.Game)
            {
                currentTick += 1;
                if (currentTick % 60 == 0)
                {
                    GameTimeSeconds -= 1;
                    Console.WriteLine("Full second tick");

                    if (GameTimeSeconds <= -1)
                    {
                        GameTimeMinutes -= 1;
                        if (GameTimeMinutes <= 0 && GameTimeSeconds <= 0)
                        {
                            gameLoopTimer.Stop();
                            MainWindow.ReturnMainWindowInstance().ChangeGameState("end");
                        }
                        GameTimeSeconds = 59;
                    }

                    GameBoard.ReturnGameBoardInstance().ChangeTimerText(GameTimeSeconds, GameTimeMinutes);

                }
                // ---------------------------------------------------------------------
                // ----------------------TICK EVENT FOR PROCESSING ---------------------
                // ---------------------------------------------------------------------             
                if (processFrameEvent_TICK != null)
                {
                    processFrameEvent_TICK.Invoke(this, e);

                }

                // ---------------------------------------------------------------------
                // ----------------------TICK EVENT FOR RENDERING-----------------------
                // ---------------------------------------------------------------------            
                if (renderFrameEvent_TICK != null)
                {
                    renderFrameEvent_TICK.Invoke(this, e);
                }
            }     
        }

    }
}