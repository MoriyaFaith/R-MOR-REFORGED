using HANDMod.Modules.Survivors;
using UnityEngine;
using RoR2;
using HANDMod.Modules.Characters;
using HANDMod.Modules;
using System;
using System.Collections.Generic;
using RoR2.Skills;
using UnityEngine.AddressableAssets;
using R2API;
using HANDMod.Content.RMORSurvivor.Components.Body;

namespace HANDMod.Content.RMORSurvivor
{
    internal class RMORSurvivor : SurvivorBase
    {
        public static bool enabled = false;
        public const string RMOR_PREFIX = HandPlugin.DEVELOPER_PREFIX + "_RMOR_BODY_";
        public override string survivorTokenPrefix => RMOR_PREFIX;

        public override UnlockableDef characterUnlockableDef => null;

        public override string bodyName => "RMOR";

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "RMORBody",
            bodyNameToken = HandPlugin.DEVELOPER_PREFIX + "_RMOR_BODY_NAME",
            subtitleNameToken = HandPlugin.DEVELOPER_PREFIX + "_RMOR_BODY_SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texPortrait.png"),
            bodyColor = new Color(0.556862745f, 0.682352941f, 0.690196078f),

            //crosshair = LegacyResourcesAPI.Load<GameObject>("prefabs/crosshair/simpledotcrosshair"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/robocratepod"),//RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")

            damage = 14f,
            maxHealth = 140f,
            healthRegen = 1f,
            armor = 0f,

            jumpCount = 1
        };

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            bodyPrefab.AddComponent<HANDMod.Content.Shared.Components.Body.OverclockController >();

            RMORTargetingController.enemyIndicatorPrefab = CreateEnemyIndicator();
            RMORTargetingController tc = bodyPrefab.AddComponent<RMORTargetingController>();
        }

        //TODO: REPLACE
        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] {
            new CustomRendererInfo {
                childName = "HanDHammer",
            },
            new CustomRendererInfo {
                childName = "HANDMesh",
            },
        };

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            InitializeUtilitySkills();
        }

        private void InitializeUtilitySkills()
        {
            Skills.AddUtilitySkills(bodyPrefab, new SkillDef[] { Shared.SkillDefs.UtilityOverclock, Shared.SkillDefs.UtilityFocus });
        }

        private GameObject CreateEnemyIndicator()
        {
            GameObject indicator = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiMissileTrackingIndicator.prefab").WaitForCompletion().InstantiateClone("HANDMod_RMOR_EnemyIndicator", false);
            SpriteRenderer[] sr = indicator.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer s in sr)
            {
                if (s.name == "Base Core")
                {
                    s.color = new Color(255f / 255f, 107f / 255f, 119f/255f);
                    break;
                }
            }
            return indicator;
        }

        private GameObject CreateRocketProjectile()
        {
            GameObject projectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/PaladinRocket.prefab").WaitForCompletion().InstantiateClone("HANDMod_RMOR_Rocket", true);
            Modules.ContentPacks.projectilePrefabs.Add(projectile);
            return projectile;
        }
    }
}
