using BepInEx;
using HANDMod.Content.HANDSurvivor;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace HANDMod
{
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

        private void Awake()
        {
            instance = this;

            Log.Init(Logger);
            Modules.Assets.Initialize(); // load assets and read config
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            new LanguageTokens();
            // survivor initialization
            //new MyCharacter().Initialize();

            Content.DamageTypes.Initialize();
            new HANDSurvivor().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();
        }
    }
}