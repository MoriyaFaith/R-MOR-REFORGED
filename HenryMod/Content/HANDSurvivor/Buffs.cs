using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using R2API;
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine.Networking;
using RoR2.Audio;
using HANDMod.Modules;

namespace HANDMod.Content.HANDSurvivor
{
    public class Buffs
    {
        private static NetworkSoundEventDef platingSound = LegacyResourcesAPI.Load<NetworkSoundEventDef>("NetworkSoundEventDefs/nseArmorPlateBlock");
        public static BuffDef DronePassive;

        public static void Init()
        {
            if (!Buffs.DronePassive)
            {
                Buffs.DronePassive = Modules.Buffs.CreateBuffDef(
                       "HANDMod_DronePassive",
                       true,
                       false,
                       false,
                       new Color(74f / 255f, 170f / 255f, 198f / 255f),
                       Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffSwarmArmor.png")
                       );
                On.RoR2.HealthComponent.TakeDamage += HANDPassiveHook;
                R2API.RecalculateStatsAPI.GetStatCoefficients += RMORPassiveHook;
            }
        }

        private static void HANDPassiveHook(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active && BodyCatalog.GetBodyName(self.body.bodyIndex).Equals("HANDOverclockedBody"))
            {
                if (!damageInfo.damageType.HasFlag(DamageType.BypassArmor) && self.body)
                {
                    int buffCount = self.body.GetBuffCount(Buffs.DronePassive);
                    if (buffCount > 0 && damageInfo.damage > 1f)
                    {
                        float totalDamageReduction = buffCount * 0.003f * self.fullCombinedHealth;
                        damageInfo.damage = Mathf.Max(1f, damageInfo.damage - totalDamageReduction);
                        EntitySoundManager.EmitSoundServer(platingSound.index, self.gameObject);
                    }
                }
            }
            orig(self, damageInfo);
        }
        private static void RMORPassiveHook(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if(BodyCatalog.GetBodyName(sender.bodyIndex).Equals("RMORBody"))
                args.attackSpeedMultAdd += (sender.GetBuffCount(DronePassive) * 0.1f);
        }
    }
}
