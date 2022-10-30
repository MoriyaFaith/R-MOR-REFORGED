using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using R2API;
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine.Networking;
using RoR2.Audio;

namespace HANDMod.Content.HANDSurvivor
{
    public class Buffs
    {
        public static BuffDef NemesisFocus;
        public static BuffDef Overclock;
        public static BuffDef DroneDebuff;
        public static BuffDef DronePassive;

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

                IL.RoR2.CharacterModel.UpdateOverlays += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    c.GotoNext(
                         x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "AttackSpeedOnCrit")
                        );
                    c.Index += 2;
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<bool, CharacterModel, bool>>((hasBuff, self) =>
                    {
                        return hasBuff || self.body.HasBuff(Buffs.Overclock);
                    });
                };
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

            if (!Buffs.NemesisFocus)
            {
                Buffs.NemesisFocus = CreateBuffDef(
                    "HANDMod_NemesisFocus",
                    false,
                    false,
                    false,
                    new Color(193f / 255f, 62f / 255f, 103f/255f),
                    Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ShockNearby/bdTeslaField.asset").WaitForCompletion().iconSprite
                    );

                RecalculateStatsAPI.GetStatCoefficients += NemesisFocusHook;
                IL.RoR2.CharacterModel.UpdateOverlays += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    c.GotoNext(
                         x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "FullCrit")
                        );
                    c.Index += 2;
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<bool, CharacterModel, bool>>((hasBuff, self) =>
                    {
                        return hasBuff || (self.body.HasBuff(Buffs.NemesisFocus));
                    });
                };
            }

            if (!Buffs.DronePassive)
            {
                Buffs.DronePassive = CreateBuffDef(
                       "HANDMod_DronePassive",
                       true,
                       false,
                       false,
                       new Color(74f / 255f, 170f / 255f, 198f / 255f),
                       Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdSmallArmorBoost.asset").WaitForCompletion().iconSprite
                       );
                On.RoR2.HealthComponent.TakeDamage += DronePassiveHook;
            }
        }

        private static NetworkSoundEventDef platingSound = LegacyResourcesAPI.Load<NetworkSoundEventDef>("NetworkSoundEventDefs/nseArmorPlateBlock");
        private static void DronePassiveHook(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active)
            {
                if (!damageInfo.damageType.HasFlag(DamageType.BypassArmor) && self.body)
                {
                    int buffCount = self.body.GetBuffCount(Buffs.DronePassive);
                    if (buffCount > 0 && damageInfo.damage > 1f)
                    {
                        float totalDamageReduction = buffCount * 0.005f * self.fullCombinedHealth;
                        damageInfo.damage = Mathf.Max(1f, damageInfo.damage - totalDamageReduction);
                        EntitySoundManager.EmitSoundServer(platingSound.index, self.gameObject);
                    }
                }
            }
            orig(self, damageInfo);
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

        private static void NemesisFocusHook(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(Buffs.NemesisFocus))
            {
                args.damageMultAdd += 0.5f;
                args.moveSpeedReductionMultAdd += 0.5f;
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
