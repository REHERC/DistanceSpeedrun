using System.Diagnostics;
using UnityEngine;

namespace DistanceSpeedrun
{
    public static class Global
    {
        public static Stopwatch HUDAnimate = Stopwatch.StartNew();
        public static Color HUD_Green = new Color(0.115f, 0.588f, 0.331f, 1.0f);
        public static Color HUD_Red = new Color(0.588f, 0.115f, 0.115f, 1.0f);

        public static bool ColorFlag = true;

        public static float Overheat = 0.0f;
    }
}
