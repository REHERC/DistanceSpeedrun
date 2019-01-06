using Harmony;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace DistanceSpeedrun
{
    public class Entry : IPlugin
    {
        public void Initialize(IManager manager, string ipcIdentifier)
        {
            try
            {
                Options.SetupDefaults();
                Timer.Initialize();
                HarmonyInstance Harmony = HarmonyInstance.Create($"com.REHERC.Speedrunning");
                Harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Events.GameMode.Go.Subscribe((data) => {
                Timer.Start();
            });

            Events.RaceEnd.LocalCarHitFinish.Subscribe((data) => {
                Timer.Pause();
            });

            Events.Game.PauseToggled.Subscribe((data) => {
                if (data.paused_)
                    Timer.Pause();
                else
                    Timer.Start();
            });
        }
    }
}
