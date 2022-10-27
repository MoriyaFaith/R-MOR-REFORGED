using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;

namespace HANDMod.Content.HAND
{
    public class Buffs
    {
        public static BuffDef Overclock;

        public static void Init()
        {
            if (!Buffs.Overclock)
            {
                Overclock = CreateBuffDef(
                    "HANDMod_Overclock",
                    false,
                    false,
                    false,
                    new Color(1.0f, 0.45f, 0f),
                    Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ShockNearby/bdTeslaField.asset").WaitForCompletion().iconSprite
                    );

                R2API.RecalculateStatsAPI.GetStatCoefficients += OverclockHook;
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
