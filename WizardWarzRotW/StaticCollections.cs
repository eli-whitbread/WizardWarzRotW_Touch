using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardWarzRotW
{
    class StaticCollections
    {
        public Int32 maxBombsInLevel = 50;
        protected static Bombs[] levelBombInstances;
        public static Int32[,] levelBombGridPositions;

        public StaticCollections()
        {
            levelBombInstances = new Bombs[maxBombsInLevel];
            levelBombGridPositions = new Int32[levelBombInstances.Length, 2];
        }

        /// <summary>
        /// Add an instance of Bomb.cs to the LevelBombInstances Array 
        /// </summary>
        /// <param name="bombToAdd">Instance of Bomb.cs</param>
        /// <param name="colPos">Column position of Bomb.cs</param>
        /// <param name="rowPos">Row position of Bomb.cs</param>
        public static void AddBomb(Bombs bombToAdd, Int32 colPos, Int32 rowPos)
        {
            for (int i = 0; i < levelBombInstances.Length; i++)
            {
                if (levelBombInstances[i] == null)
                {
                    levelBombInstances[i] = bombToAdd;
                    levelBombGridPositions[i, 0] = colPos;
                    levelBombGridPositions[i, 1] = rowPos;
                    return;
                }
            }
        }

        /// <summary>
        /// Remove an instance of Bomb.cs to the LevelBombInstances Array
        /// </summary>
        /// <param name="bombToRemove">Instance of Bomb.cs</param>
        public static void RemoveBomb(Bombs bombToRemove)
        {
            for (int i = 0; i < levelBombInstances.Length; i++)
            {
                if (levelBombInstances[i] == bombToRemove)
                {
                    levelBombInstances[i] = null;
                    levelBombGridPositions[i, 0] = 0;
                    levelBombGridPositions[i, 1] = 0;
                    return;
                }
            }
        }

        //check for current bomb positions on the game grid - return false if a bomb alread exists at the passed position (column, row)
        /// <summary>
        /// Check for current bomb positions on the game grid <para>return false if a bomb alread exists at the passed position (column, row)</para>
        /// </summary>
        /// <param name="colPos">Column position of the instance</param>
        /// <param name="rowPos">Row position of the instance</param>
        /// <returns></returns>
        public static bool CheckBombPosition(Int32 colPos, Int32 rowPos)
        {
            for (int i = 0; i < levelBombGridPositions.GetLength(0); i++)
            {
                if (levelBombGridPositions[i, 0] == colPos && levelBombGridPositions[i, 1] == rowPos)
                {

                    return false;
                }

            }
            return true;
        }

    }
}


