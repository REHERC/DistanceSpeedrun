using Harmony;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using System;
using System.Reflection;

namespace DistanceSpeedrun
{
    public class Entry : IPlugin
    {
        public void Initialize(IManager manager, string ipcIdentifier)
        {
            try
            {
                Options.SetupDefaults();
                HarmonyInstance Harmony = HarmonyInstance.Create($"com.REHERC.Speedrunning");
                Harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Events.GameMode.Go.Subscribe((data) => {
                Timer.Start(true);
            });

            Events.RaceEnd.LocalCarHitFinish.Subscribe((data) => {
                Timer.Pause(true);
                
            });

            Events.Game.PauseToggled.Subscribe((data) => {
                if (data.paused_)
                    Timer.Pause(false);
                else if (Timex.ModeTime_ >= 0.0d && !G.Sys.GameManager_.RestartingLevel_)
                    Timer.Start(false);
            });
        }
    }
}
