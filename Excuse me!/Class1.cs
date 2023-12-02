using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace Excuse_me
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class ExcuseMe : BaseUnityPlugin
    {
        private const string modGUID = "Hexnet.lethalcompany.excuseme";
        private const string modName = "Excuse me!";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        void Awake()
        {
            harmony.PatchAll();

            Debug.Log("[EM]: Excuse me! loaded successfully!");
        }
    }
}

namespace Excuse_me.Patches
{
    [HarmonyPatch]
    internal class BellDingerFixes
    {
        private static InteractTrigger BellTrigger;

        [HarmonyPatch(typeof(StartOfRound))]
        internal class SceneFixes
        {
            [HarmonyPatch("SceneManager_OnLoadComplete1")]
            [HarmonyPostfix]
            public static void InitializePatch(ulong clientId, string sceneName)
            {
                if (sceneName != "CompanyBuilding")
                {
                    BellTrigger = null;
                    return;
                }

                GameObject TriggerObject = UnityEngine.GameObject.Find("BellDinger/Trigger");

                if (TriggerObject)
                {
                    BellTrigger = TriggerObject.GetComponent<InteractTrigger>();
                }
            }
        }

        [HarmonyPatch(typeof(InteractTrigger))]
        internal class BellFixes
        {
            [HarmonyPatch("Interact")]
            [HarmonyPostfix]
            public static void ResetBell()
            {
                if (BellTrigger != null && BellTrigger.interactCooldown)
                {
                    BellTrigger.interactCooldown = false;
                }
            }
        }
    }
}
