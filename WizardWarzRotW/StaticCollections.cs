using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardWarzRotW
{
    class StaticCollections
    {
        public Int32 maxBombsInLevel = 10;
        protected static Bombs[] levelBombInstances;
        public static Int32[,] levelBombGridPositions;

        public StaticCollections()
        {
            levelBombInstances = new Bombs[maxBombsInLevel];
            levelBombGridPositions = new Int32[levelBombInstances.Length, 2];
        }

        //no longer needed - changed to event system
        //public static void SendBombUpdate()
        //{
        //    for (int i = 0; i < levelBombInstances.Length; i++)
        //    {
        //        if (levelBombInstances[i] != null)
        //        {
        //            levelBombInstances[i].BombTickUpdate();

        //        }

        //    }
        //}

        //add a bomb to the collection
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

        //remove a bomb from the collection
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


