using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistanceSpeedrun
{
    public static class Timer
    {
        private static bool started = false;

        public static void Initialize()
        {

        }

        public static bool IsStarted()
        {
            return started;
        }

        public static void Pause()
        {
            Console.WriteLine("Timer.Pause");
        }

        public static void Start()
        {
            started = true;
            Console.WriteLine("Timer.Start");
        }

        public static void Stop()
        {
            started = false;
            Console.WriteLine("Timer.Stop");
        }

        public static void ShowResults()
        {

        }
    }
}
