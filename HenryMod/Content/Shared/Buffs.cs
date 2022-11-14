using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine.Networking;
using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.AddressableAssets;

namespace HANDMod.Content.Shared
{
    public class Buffs
    {
        public static BuffDef NemesisFocus;
        public static BuffDef Overclock;
        public static void Init()
        {
            if (!Buffs.Overclock)
            {
                Buffs.Overclock = Modules.Buffs.CreateBuffDef(
                    "HANDMod_Overclock",
                    false,
                    false,
                    false,
                    new Color(1.0f, 0.45f, 0f),
                    Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ShockNearby/bdTeslaField.asset").WaitForCompletion().iconSprite
                    );

                RecalculateStatsAPI.GetStatCoefficients += OverclockHook;
            }

            if (!Buffs.NemesisFocus)
            {
                Buffs.NemesisFocus = Modules.Buffs.CreateBuffDef(
                    "HANDMod_NemesisFocus",
                    false,
                    false,
                    false,
                    new Color(193f / 255f, 62f / 255f, 103f / 255f),
                    Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ShockNearby/bdTeslaField.asset").WaitForCompletion().iconSprite
                    );

                RecalculateStatsAPI.GetStatCoefficients += NemesisFocusHook;
            }
        }

        private static void OverclockHook(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(Buffs.Overclock))
            {
                args.attackSpeedMultAdd += 0.4f;
                args.moveSpeedMultAdd += 0.4f;
            }
        }

        private static void NemesisFocusHook(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(Buffs.NemesisFocus))
            {
                args.damageMultAdd += 0.5f;
                //args.moveSpeedReductionMultAdd += 0.3f;
            }
        }
    }
}
