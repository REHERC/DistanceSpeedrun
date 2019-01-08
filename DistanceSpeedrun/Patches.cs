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
                            overheat = $"0{overheat}";
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
            static bool Prefix(FinalCountdownWidget __instance)
            {
                if (!Options.Get<bool>("Enabled")) return true;
                TextMesh mesh = __instance.gameObject.GetComponent<TextMesh>();

                float duration = 1.5f;

                float lerp = Extensions.Map((float)Global.HUDAnimate.Elapsed.TotalSeconds,0, duration, 0,1);

                Color color = Color.Lerp(Timer.GetColor(), Color.white, lerp);

                mesh.color = color;

                bool freezetimer = Global.HUDAnimate.Elapsed.TotalSeconds < duration;
                if (!freezetimer)
                    mesh.text = Timer.GetTime();
                return false;
            }
        }
        
        [HarmonyPatch(typeof(LevelGridMenu), "Start")]
        public class Menu__Show__Patch
        {
            static void Postfix()
            {
                if (Timer.GetSeconds() <= 0) return;
                Timer.ShowResults();
            }
        }

        [HarmonyPatch(typeof(CarCheckpointHitListener), "OnCarEventCheckpointHit")]
        public class CarCheckpointHitListener__OnCarEventCheckpointHit__Patch
        {
            static void Postfix(CarCheckpointHitListener __instance, ref CheckpointHit.Data data)
            {
                if (data.silent_) return;
                Global.ColorFlag = !Global.ColorFlag;
                Global.HUDAnimate = Stopwatch.StartNew();
            }
        }

        [HarmonyPatch(typeof(TriggerCooldownLogic), "OnTriggerEnter")]
        public class TriggerCooldownLogic__OnTriggerEnter__Patch
        {
            static void Postfix(TriggerCooldownLogic __instance, ref Collider other)
            {
                if (!GUtils.IsRelevantLocalCar(other)) return;
                Global.ColorFlag = !Global.ColorFlag;
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

        [HarmonyPatch(typeof(PowerPosterLogic), "Update")]
        public class PowerPosterLogic__Update_Patch
        {
            static void Postfix(PowerPosterLogic __instance)
            {
                if (!Options.Get<bool>("Enabled")) return;
                TextMesh mesh = __instance.gameObject.GetComponentInChildren<TextMesh>();
                if (mesh != null)
                    mesh.text = Timer.GetTime();
            }
        }

        [HarmonyPatch(typeof(AdventureSpecialIntro), "Start")]
        [HarmonyPatch(typeof(LostToEchoesIntroCutscene), "Start")]
        public class Campaign__Intro__Patch
        {
            static void Postfix()
            {
                Timer.Start(true);
            }
        }
    }
}
