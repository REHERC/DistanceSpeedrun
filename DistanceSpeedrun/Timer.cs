using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace DistanceSpeedrun
{
    public static class Timer
    {
        private static bool started = false;
        private static double totaltime = 0;
        public static Stopwatch timer;
        public static double levelstarttime;
        private static int notificationcount = 0;

        public static bool IsStarted()
        {
            return started;
        }

        public static void Pause(bool levelEND)
        {
            if (!started) return;
            if (levelEND)
                LevelStats();
            timer.Stop();
            totaltime += timer.Elapsed.TotalSeconds;
            timer = new Stopwatch();
        }

        private static void LevelStats()
        {
            NotificationBox.Notification n = new NotificationBox.Notification($"{G.Sys.GameManager_.LevelName_}", $"{Timer.GetTime(true, 3)}", NotificationBox.NotificationType.Campaign, "WorldTraveller");
            NotificationBox.Show(n, false);
        }

        public static void Start(bool levelGO)
        {
            notificationcount = 0;
            if (levelGO && !started)
                levelstarttime = GetSeconds();
            timer = Stopwatch.StartNew();
            started = true;
        }

        public static void Stop()
        {
            if (!started) return;
            Reset();
            started = false;
        }

        public static string GetTime(bool getleveltime = false, int decimals = 2, bool fullformat = false)
        {
            if (!started || timer == null || timer is null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('0', decimals);
                return $"00:00.{sb.ToString()}";
            }

            double leveltime = GetSeconds() - levelstarttime;
            TimeSpan value = TimeSpan.FromSeconds(getleveltime ? leveltime : GetSeconds());
            return fullformat ? TimeHHMMSSMS(value, decimals) : value.Hours > 0 ? TimeHHMMSS(value) : TimeMMSSMS(value, decimals);
            
            //return @"¯\_(ツ)_/¯";
        }

        private static string TimeHHMMSS(TimeSpan value)
        {
            if (value.TotalMilliseconds < 0) return "00:00:00";

            string hours = value.Hours.ToString();
            string minutes = value.Minutes.ToString();
            string seconds = value.Seconds.ToString();

            while (hours.Length < 2) hours = $"0{hours}";
            while (minutes.Length < 2) minutes = $"0{minutes}";
            while (seconds.Length < 2) seconds = $"0{seconds}";

            return $"{hours}:{minutes}:{seconds}";
        }

        private static string TimeMMSSMS(TimeSpan value, int decimals)
        {
            if (value.TotalMilliseconds < 0) return "00:00:00";
            
            string minutes = value.Minutes.ToString();
            string seconds = value.Seconds.ToString();
            string miliseconds = value.Milliseconds.ToString();
            
            while (minutes.Length < 2) minutes = $"0{minutes}";
            while (seconds.Length < 2) seconds = $"0{seconds}";
            while (miliseconds.Length < decimals) miliseconds = $"{miliseconds}0";

            miliseconds = miliseconds.Substring(0, decimals);

            return $"{minutes}:{seconds}.{miliseconds}";
        }

        private static string TimeHHMMSSMS(TimeSpan value, int decimals)
        {
            if (value.TotalMilliseconds < 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('0', decimals);
                return $"00:00:00.{sb.ToString()}";
            }

            string hours = value.Hours.ToString();
            string minutes = value.Minutes.ToString();
            string seconds = value.Seconds.ToString();
            string miliseconds = value.Milliseconds.ToString();

            while (hours.Length < 2) hours = $"0{hours}";
            while (minutes.Length < 2) minutes = $"0{minutes}";
            while (seconds.Length < 2) seconds = $"0{seconds}";
            while (miliseconds.Length < decimals) miliseconds = $"{miliseconds}0";

            miliseconds = miliseconds.Substring(0, decimals);

            return $"{hours}:{minutes}:{seconds}.{miliseconds}";
        }

        public static double GetSeconds()
        {
            if (timer is null || timer == null)
                return 0;
            return timer.Elapsed.TotalSeconds + totaltime;
        }

        public static void ShowResults()
        {
            if (timer is null || timer == null || notificationcount > 0)
            {
                Stop();
                return;
            }
            NotificationBox.Notification n = new NotificationBox.Notification("Run results", $"{Timer.GetTime(false, 3, true)}", NotificationBox.NotificationType.Campaign, "MeetYourRival");
            NotificationBox.Show(n, true);
            notificationcount += 1;

            Stop();
            return;
            string message = $"Run time: {Timer.GetTime()}";

            G.Sys.MenuPanelManager_.ShowYesNo(message, "SEEDRUN MODE", () => {
                Console.WriteLine("TEST");
            });
        }

        public static Color GetColor()
        {
            return Global.HUD_Green;
            return Global.ColorFlag ? Global.HUD_Green : Global.HUD_Red;
        }

        public static void Reset()
        {
            totaltime = 0.0d;
            timer = null;
        }
    }
}
