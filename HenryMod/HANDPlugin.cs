using BepInEx;
using HANDMod.Content.HANDSurvivor;
using HANDMod.Content.RMORSurvivor;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace HANDMod
{
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "UnlockableAPI",
        "RecalculateStatsAPI",
        "DamageAPI"
    })]

    public class HandPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.EnforcerGang.HANDOverclocked";
        public const string MODNAME = "HAN-D Overclocked";
        public const string MODVERSION = "1.0.0";

        public const string DEVELOPER_PREFIX = "MOFFEIN";

        public static HandPlugin instance;
        public static bool ScepterStandaloneLoaded = false;
        public static bool ScepterClassicLoaded = false;
        public static bool EmoteAPILoaded = false;
        public static bool ArenaPluginLoaded = false;
        public static bool ArenaModeActive = false;
        public static bool RiskOfOptionsLoaded = false;

        private void Awake()
        {
            instance = this;

            CheckDependencies();
            Modules.Config.ReadConfig();

            Log.Init(Logger);
            Modules.Assets.Initialize(); // load assets and read config
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            new LanguageTokens();
            // survivor initialization
            //new MyCharacter().Initialize();

            new Content.Shared.SharedContent();
            Content.DamageTypes.Initialize();

            new HANDSurvivor().Initialize();
            if (RMORSurvivor.enabled) new RMORSurvivor().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            if (EmoteAPILoaded) EmoteAPICompat();
            if (ArenaPluginLoaded)
            {
                Stage.onStageStartGlobal += SetArena;
            }
        }

        private void CheckDependencies()
        {
            ScepterStandaloneLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            ScepterClassicLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems");
            EmoteAPILoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI");
            ArenaPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Kingpinush.KingKombatArena");
            RiskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
        }


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetArena(Stage obj)
        {
            HandPlugin.ArenaModeActive = NS_KingKombatArena.KingKombatArenaMainPlugin.s_GAME_MODE_ACTIVE;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void EmoteAPICompat()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                foreach (var item in SurvivorCatalog.allSurvivorDefs)
                {
                    if (item.bodyPrefab.name == "HANDOverclockedBody")
                    {
                        var skele = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("animHANDEmote.prefab");
                        EmotesAPI.CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele);
                        skele.GetComponentInChildren<BoneMapper>().scale = 1.5f;
                    }
                }
            };
        }
    }
}