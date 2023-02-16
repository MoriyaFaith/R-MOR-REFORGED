using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine.Networking;
using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.AddressableAssets;

namespace RMORMod.Content.Shared
{
    public class Buffs
    {
        public static BuffDef RMORFocus;
        public static BuffDef Fortify;
        public static BuffDef RMOROverclock;
        public static void Init()
        {
            if (!Buffs.RMOROverclock)
            {
                Buffs.RMOROverclock = Modules.Buffs.CreateBuffDef(
                    "RMORMod_Overclock",
                    false,
                    false,
                    false,
                    new Color(1.0f, 0.45f, 0f),
                    Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ShockNearby/bdTeslaField.asset").WaitForCompletion().iconSprite
                    );

                RecalculateStatsAPI.GetStatCoefficients += OverclockHook;
            }

            if (!Buffs.RMORFocus)
            {
                Buffs.RMORFocus = Modules.Buffs.CreateBuffDef(
                    "RMORMod_NemesisFocus",
                    false,
                    false,
                    false,
                    new Color(193f / 255f, 62f / 255f, 103f / 255f),
                    Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ShockNearby/bdTeslaField.asset").WaitForCompletion().iconSprite
                    );

                RecalculateStatsAPI.GetStatCoefficients += NemesisFocusHook;
            }

            if (!Buffs.Fortify)
            {
                Buffs.Fortify = Modules.Buffs.CreateBuffDef(
                    "RMORMod_Fortify",
                    false,
                    false,
                    false,
                    new Color(163f / 255f, 232f / 255f, 146f / 255f),
                    Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ShockNearby/bdTeslaField.asset").WaitForCompletion().iconSprite
                    );

                RecalculateStatsAPI.GetStatCoefficients += FortifyHook;
            }
        }

        private static void OverclockHook(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(Buffs.RMOROverclock))
            {
                args.attackSpeedMultAdd += 0.4f;
                args.moveSpeedMultAdd += 0.4f;
            }
        }

        private static void NemesisFocusHook(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(Buffs.RMORFocus))
            {
                args.damageMultAdd += 0.5f;
                args.moveSpeedReductionMultAdd += 0.3f;
                args.armorAdd += 50f;
            }
        }
        private static void FortifyHook(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(Buffs.Fortify))
            {
                args.armorAdd += 100f;
                args.baseRegenAdd += sender.maxHealth / 20;
            }
        }
    }
}
