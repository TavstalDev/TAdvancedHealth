using System;

namespace Tavstal.TAdvancedHealth.Managers
{
    internal class MathHelper
    {
        private static Random random;
        private static object syncObj = new object();

        public static int GenerateRandomNumber(int min, int max)
        {
            lock (syncObj)
            {
                if (random == null)
                    random = new Random(); // Or exception...
                return random.Next(min, max);
            }
        }

        public static int GenerateRandomNumber(int max)
        {
            lock (syncObj)
            {
                if (random == null)
                    random = new Random(); // Or exception...
                return random.Next(0, max);
            }
        }

        public static double GenerateRandomNumber(double min, double max)
        {
            lock (syncObj)
            {
                if (random == null)
                    random = new Random(); // Or exception...
                return (random.NextDouble() * Math.Abs(max - min)) + min;
            }
        }

        public static double GenerateRandomNumber(double max)
        {
            lock (syncObj)
            {
                if (random == null)
                    random = new Random(); // Or exception...
                return (random.NextDouble() * Math.Abs(max));
            }
        }
    }
}
