using System;
using System.Runtime.InteropServices;

namespace DefaultNamespace
{
    public class Utility
    {
        private static Random r = new Random();
        
        public static float Rand(float max)
        {
            float ra = r.Next((int)max);
            return ra;
        }

        public static float Rand(float min, float max)
        {
            float ra = r.Next((int)(max - min));
            return ra + min;
        }

        public static float RandAngle(float startPoint = 10, int tolerance = 60)
        {
            float ra = r.Next(tolerance);
            double sign = r.NextDouble();
            float result = ra + startPoint;
            if (sign > 0.5)
                result *= -1;
            return result;
        }
    }
}