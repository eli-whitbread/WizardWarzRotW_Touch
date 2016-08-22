using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace WizardWarzRotW
{
    public partial class RandomNumberGenerator
    {
        public int powerupID;
        public void main(string[] args)
        {
            GenerateRandomNumber();
        }

        /// <summary>
        /// Generates a random integer between 1 and 255.
        /// </summary>
        /// <returns></returns>
        public int GenerateRandomNumber()
        {
            byte[] bytes1 = new byte[100];
            byte[] bytes2 = new byte[100];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] rndBytes = new byte[4];
            rng.GetBytes(rndBytes);
            int rand = BitConverter.ToInt32(rndBytes, 0);

            RNGCryptoServiceProvider rng2 = new RNGCryptoServiceProvider();
            byte[] rndBytes2 = new byte[4];
            rng2.GetBytes(rndBytes2);
            int rand2 = BitConverter.ToInt32(rndBytes2, 0);

            Random rnd1 = new Random(rand);
            Random rnd2 = new Random(rand2);

            rnd1.NextBytes(bytes1);
            rnd2.NextBytes(bytes2);

            //Console.WriteLine("First Series:");
            for (int ctr = bytes1.GetLowerBound(0);
                 ctr <= bytes1.GetUpperBound(0);
                 ctr++)
            {
                //Console.Write("{0, 5}", bytes1[ctr]);

                //if (ctr == 1)
                //    return bytes1[ctr];

                //if ((ctr + 1) % 10 == 0)
                //Console.WriteLine();
            }
            return bytes1[0];
        }

    }
}
