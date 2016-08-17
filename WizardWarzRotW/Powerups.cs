using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WizardWarzRotW
{

    public enum PowerupTypes
    {
        Superbomb,
        Shield,
        Lifeup
    }

    public class Powerups
    {
        public static string pName;
        public int powerupCount;
        public int powerupType;
        public int gameTime;
        public static PowerupTypes[,] _powerupTile = null;
        protected static Powerups powerupRef = null;

        GameBoard _localGameBoard = null;
        public Grid curGameGrid = null;

        public int xPos;
        public int yPos;

        // Used to prevent the powerups from spawning on wall tiles
        bool xFlag = true;
        bool yFlag = true;

        public TileStates[,] curTileStates = GameBoard.curTileState;
        public PowerupTileStates[,] powerupTiles = GameBoard.powerupTileState;
        private RandomNumberGenerator RNG = new RandomNumberGenerator();

        public Powerups()
        {
            _localGameBoard = GameBoard.ReturnGameBoardInstance();

        }
        public void InitialisePowerups()
        {

        }

        public void Count()
        {
            powerupCount += 1;
            //Debug.WriteLine("Power up count: {0}", powerupCount);
            if (powerupCount >= 1)
            {
                powerupCount = 0;

                Random rand = new Random();
                xPos = rand.Next(1, 12);
                yPos = rand.Next(1, 12);

                if (_localGameBoard.innerWallPos != null && _localGameBoard.destructibleWallPos != null)
                {
                    TileCheck();
                }
            }
        }


        public void TileCheck()
        {
            xFlag = true;
            yFlag = true;

            //Console.WriteLine("xPos: {0}", xPos);
            //Console.WriteLine("yPos: {0}", yPos);

            // Compare x-coordinates
            for (int x = 0; x < _localGameBoard.innerWallPos.GetLength(0); x++)
            {
                if (xPos == _localGameBoard.innerWallPos[x, 0])
                    xFlag = false;
                if (yPos == _localGameBoard.innerWallPos[x, 1])
                    yFlag = false;
            }

            // Compare y-coordinates
            for (int y = 0; y < _localGameBoard.destructibleWallPos.GetLength(0); y++)
            {
                if (xPos == _localGameBoard.destructibleWallPos[y, 0])
                    xFlag = false;
                if (yPos == _localGameBoard.destructibleWallPos[y, 1])
                    yFlag = false;
            }

            // Check for existing powerups
            if (GameBoard.curTileState[xPos, yPos] == TileStates.Powerup)
            {
                xFlag = false;
                yFlag = false;
            }

            // Check for each player's positions
            // Method is only needed if we decide to have powerups spawn over time, rather than by destroying walls.
            for (int i = 0; i < GameBoard.ReturnNumberOfPlayer(); i++)
            {
                PlayerControl tempPlayer = GameBoard.ReturnPlayerList()[i];

                if (xPos == tempPlayer.playerX && yPos == tempPlayer.playerY)
                {
                    xFlag = false;
                    yFlag = false;
                }
            }

            if (xFlag || yFlag)
            {
                // The right-most number should be equal to the amount of powerups we've created 
                powerupType = GameBoard.ReturnRandomPowerUpNo();

                if (powerupType == 0)
                {
                    SpawnPowerup("Superbomb");
                    Console.WriteLine("Superbomb spawned");
                }
                else if (powerupType == 1)
                {
                    SpawnPowerup("Shield");
                    Console.WriteLine("Shield spawned");
                }
                else if (powerupType == 2)
                {
                    SpawnPowerup("Lifeup");
                    Console.WriteLine("Lfieup spawned");
                }
                else
                    Console.WriteLine("Powerup type error.");
            }

            else if (!xFlag && !yFlag)
                Console.WriteLine("Obstacle detected");

            // Just in case
            else
                Console.WriteLine("Unexpected outcome");
        }

        public void SpawnPowerup(string powerupName)
        {
            // Set general powerup properties
            Image powerupTile = new Image();

            powerupTile.Height = GameBoard.ReturnTileSize();
            powerupTile.Width = GameBoard.ReturnTileSize();
            Grid.SetColumn(powerupTile, xPos);
            Grid.SetRow(powerupTile, yPos);

            GameBoard.curTileState[xPos, yPos] = TileStates.Powerup;
            // Set properties unique to each powerup
            switch (powerupName)
            {
                case ("SuperBomb"):
                    pName = "Superbomb";
                    powerupTile.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Bomb2.png", UriKind.Absolute));
                    _localGameBoard.ChangeTileState(xPos, yPos, "Superbomb");
                    break;
                case ("Shield"):
                    pName = "Shield";
                    powerupTile.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/shield.png", UriKind.Absolute));
                    _localGameBoard.ChangeTileState(xPos, yPos, "Shield");
                    break;
                case ("Lifeup"):
                    pName = "Lifeup";
                    powerupTile.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/heart.png", UriKind.Absolute));
                    _localGameBoard.ChangeTileState(xPos, yPos, "Lifeup");
                    break;
            }

            curGameGrid.Children.Add(powerupTile);
        }


        // Spawn a powerup at a wall position
        public void WallSpawn(int PosX, int PosY, Grid GameGrid)
        {
            Image powerupTile = new Image();

            int rand = RNG.GenerateRandomNumber();
            //MessageBox.Show(string.Format("Random number: {0}", rand));

            // I believe that the rand number will always be between 1 and 255. So we divide 255 by 3 and see if 'rand' is equal to or less than one, two or three thirds of 255.\
            // Basically, this method of handling things should ensure an even chance of each power up spawning.
            if (rand <= 85)
            {
                pName = "Superbomb";
                powerupTile.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Bomb2.png", UriKind.Absolute));
                _localGameBoard.ChangeTileState(PosX, PosY, "Superbomb");
                //MessageBox.Show("Superbomb made");    
            }
            else if (rand >= 86 && rand <= 190)
            {
                pName = "Shield";
                powerupTile.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/shield.png", UriKind.Absolute));
                _localGameBoard.ChangeTileState(PosX, PosY, "Shield");
                //MessageBox.Show("Shield made");
            }
            else if (rand >= 191)
            {
                pName = "Lifeup";
                powerupTile.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/heart.png", UriKind.Absolute));
                _localGameBoard.ChangeTileState(PosX, PosY, "Lifeup");
                //MessageBox.Show("Lifeup made");
            }


            powerupTile.Height = GameBoard.ReturnTileSize();
            powerupTile.Width = GameBoard.ReturnTileSize();
            Grid.SetColumn(powerupTile, PosX);
            Grid.SetRow(powerupTile, PosY);
            GameBoard.curTileState[PosX, PosY] = TileStates.Powerup;
            GameGrid.Children.Add(powerupTile);
        }


        // Delete a single powerup, then return the name of it.
        // Used to empower players.
        public string ReturnPowerup(int col, int row, Grid GameGrid)
        {
            for (int i = 300; i < GameGrid.Children.Count; i++)
            {
                UIElement elem = GameGrid.Children[i];

                if (Grid.GetRow(elem) == row && Grid.GetColumn(elem) == col)
                {
                    //MessageBox.Show(string.Format("Child number: {0}. Total children: {1}", i, GameGrid.Children.Count));

                    if (GameBoard.curTileState[col, row] == TileStates.Powerup)
                    {
                        // Remove the powerup, then return its name.
                        GameBoard.curTileState[col, row] = TileStates.Floor;
                        GameGrid.Children.Remove(elem);

                        if (GameBoard.powerupTileState[col, row] == PowerupTileStates.Superbomb)
                        {
                            GameBoard.powerupTileState[col, row] = PowerupTileStates.Empty;
                            //MessageBox.Show("Superbomb!");
                            return "Superbomb";
                        }
                        else if (GameBoard.powerupTileState[col, row] == PowerupTileStates.Shield)
                        {
                            GameBoard.powerupTileState[col, row] = PowerupTileStates.Empty;
                            //MessageBox.Show("Shield!");
                            return "Shield";
                        }
                        else if (GameBoard.powerupTileState[col, row] == PowerupTileStates.Lifeup)
                        {
                            GameBoard.powerupTileState[col, row] = PowerupTileStates.Empty;
                            //MessageBox.Show("Lifeup!");
                            return "Lifeup";
                        }
                    }
                }
            }

            return null;
        }
    }
}
