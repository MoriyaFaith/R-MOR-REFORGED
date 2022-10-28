using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using R2API;

namespace HANDMod.Content.HANDSurvivor
{
    public class Buffs
    {
        public static BuffDef Overclock;
        public static BuffDef DroneDebuff;

        public static void Init()
        {
            if (!Buffs.Overclock)
            {
                Buffs.Overclock = CreateBuffDef(
                    "HANDMod_Overclock",
                    false,
                    false,
                    false,
                    new Color(1.0f, 0.45f, 0f),
                    Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ShockNearby/bdTeslaField.asset").WaitForCompletion().iconSprite
                    );

                RecalculateStatsAPI.GetStatCoefficients += OverclockHook;
            }

            if (!Buffs.DroneDebuff)
            {
                Buffs.DroneDebuff = CreateBuffDef(
                    "HANDMod_DroneDebuff",
                    false,
                    false,
                    true,
                    new Color(0.556862745f, 0.682352941f, 0.690196078f),
                    Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Treebot/bdWeak.asset").WaitForCompletion().iconSprite
                    );
                RecalculateStatsAPI.GetStatCoefficients += DroneDebuffHook;
            }
        }

        private static void DroneDebuffHook(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(Buffs.DroneDebuff))
            {
                args.moveSpeedReductionMultAdd += 0.6f;
                args.damageMultAdd -= 0.3f;
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

        private static BuffDef CreateBuffDef(string name, bool canStack, bool isCooldown, bool isDebuff, Color color, Sprite iconSprite)
        {
            BuffDef bd = ScriptableObject.CreateInstance<BuffDef>();
            bd.name = name;
            bd.canStack = canStack;
            bd.isCooldown = isCooldown;
            bd.isDebuff = isDebuff;
            bd.buffColor = color;
            bd.iconSprite = iconSprite;

            HANDMod.Modules.ContentPacks.buffDefs.Add(bd);
            (bd as UnityEngine.Object).name = bd.name;
            return bd;
        }
    }
}
