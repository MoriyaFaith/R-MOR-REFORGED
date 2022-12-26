using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using R2API;
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine.Networking;
using RoR2.Audio;
using RMORMod.Modules;

namespace RMORMod.Content.HANDSurvivor
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
                       "RMORMod_DronePassive",
                       true,
                       false,
                       false,
                       new Color(74f / 255f, 170f / 255f, 198f / 255f),
                       Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffSwarmArmor.png")
                       );
                R2API.RecalculateStatsAPI.GetStatCoefficients += RMORPassiveHook;
            }
        }
        private static void RMORPassiveHook(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            args.attackSpeedMultAdd += (sender.GetBuffCount(DronePassive) * 0.05f);
            //args.damageMultAdd += (sender.GetBuffCount(DronePassive) * 0.1f);
        }
    }
}
