using Events.Car;
using Harmony;
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DistanceSpeedrun
{
    class Patches
    {
        [HarmonyPatch(typeof(CarScreenLogic), "Awake")]
        public class CarScreenLogic__Awake__Patch
        {
            static void Postfix(CarScreenLogic __instance)
            {
                if (!Options.Get<bool>("Enabled")) return;
                __instance.gameObject.AddComponent<CarScreenLogic__Script>();
            }

            public class CarScreenLogic__Script : MonoBehaviour
            {
                private CarScreenLogic carscreen;

                public void Start()
                {
                    this.carscreen = gameObject.GetComponent<CarScreenLogic>();
                }

                void Update()
                {
                    if (carscreen != null && carscreen.isActiveAndEnabled)
                    {
                        carscreen.ShowCountdown();
                        carscreen.StopScrensaver();
                        carscreen.minimap_.SetActive(false);
                        carscreen.timeWidget_.IsVisible_ = true;
                        carscreen.timeWidget_.gameObject.SetActive(true);
                        carscreen.timeWidget_.State_ = TimeWidget.State.ConstantString;

                        string overheat = Mathf.Round(Global.Overheat * 100).ToString();
                        while (overheat.Length != 3)
                        {
                            overheat = $"0{overheat}";
                        }
                        overheat = $"{overheat} %";

                        float treshold = 0.7f;
                        float lerp = Global.Overheat > treshold ? Mathf.Clamp(Extensions.Map(Global.Overheat, treshold, 1, 0, 0.8f),0, 1) : 0;

                        Color color = Color.Lerp(Color.white, Color.red, lerp);
                        color *= 0.95f;

                        carscreen.timeWidget_.SetTimeTextToString(overheat, color, 1);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CarLogic), "Update")]
        public class CarLogic__Update__Patch
        {
            static void Postfix(CarLogic __instance)
            {
                Global.Overheat = __instance.Heat_;
            }
        }

        [HarmonyPatch(typeof(CarScreenLogic), "GlitchCarScreen")]
        public class CarScreenLogic__GlitchCarScreen__Patch
        {
            static bool Prefix()
            {
                if (!Options.Get<bool>("Enabled")) return true;
                return false;
            }
        }
        
        [HarmonyPatch(typeof(FinalCountdownWidget), "UpdateLogic")]
        public class FinalCountdownWidget__UpdateLogic__Patch
        {
            static void Postfix(FinalCountdownWidget __instance)
            {
                if (!Options.Get<bool>("Enabled")) return;
                TextMesh mesh = __instance.gameObject.GetComponent<TextMesh>();

                float duration = 0.75f;

                float lerp = Extensions.Map(Mathf.Clamp((float)Global.HUDAnimate.Elapsed.TotalSeconds,0, duration),0, duration, 1,0);

                Color color = Color.Lerp(Color.white, Global.HUDCheckpointColor, lerp);

                mesh.color = color;
            }
        }

        [HarmonyPatch(typeof(MainMenuLogic), "Awake")]
        public class MainMenuLogic__Awake__Patch
        {
            static void Postfix(MainMenuLogic __instance)
            {
                if (SceneManager.GetActiveScene().name != "MainMenu") return;
                Timer.Stop();
                Timer.ShowResults();
            }
        }

        [HarmonyPatch(typeof(CarCheckpointHitListener), "OnCarEventCheckpointHit")]
        public class CarCheckpointHitListener__OnCarEventCheckpointHit__Patch
        {
            static void Postfix(CarCheckpointHitListener __instance, ref CheckpointHit.Data data)
            {
                if (data.silent_) return;
                Global.HUDAnimate = Stopwatch.StartNew();
            }
        }

        [HarmonyPatch(typeof(GeneralMenu), "InitializeVirtual")]
        public class __InitializeVirtual__Patch
        {
            static void Postfix(GeneralMenu __instance)
            {
                if (SceneManager.GetActiveScene().name != "MainMenu") return;
                void OnEnabledChanged(bool value)
                {
                    Options.Set("Enabled", value);
                }
                __instance.CallPrivateMethod("TweakBool", "SPEEDRUN MODE", Options.Get("Enabled"), new Action<bool>(OnEnabledChanged), (string)null);
            }
        }
    }
}
