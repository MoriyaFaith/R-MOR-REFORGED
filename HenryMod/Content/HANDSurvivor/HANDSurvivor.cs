using HANDMod.Modules.Survivors;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using RoR2.Skills;
using HANDMod.Modules.Characters;
using HANDMod.Modules;
using System;
using HANDMod;
using System.Collections.Generic;
using HANDMod.Content.HANDSurvivor.Components.Body;
using EntityStates;
using System.Linq;

namespace HANDMod.Content.HANDSurvivor
{
    internal class HANDSurvivor : SurvivorBase
    {
        public const string HAND_PREFIX = HandPlugin.DEVELOPER_PREFIX + "_HAND_BODY_";
        public override string survivorTokenPrefix => HAND_PREFIX;

        public override UnlockableDef characterUnlockableDef => null;

        public override string bodyName => "HANDOverclocked";

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "HANDOverclockedBody",
            bodyNameToken = HandPlugin.DEVELOPER_PREFIX + "_HAND_BODY_NAME",
            subtitleNameToken = HandPlugin.DEVELOPER_PREFIX + "_HAND_BODY_SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texPortraitOld.png"),
            bodyColor = new Color(0.556862745f, 0.682352941f, 0.690196078f),

            crosshair = LegacyResourcesAPI.Load<GameObject>("prefabs/crosshair/simpledotcrosshair"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/robocratepod"),//RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")

            maxHealth = 160f,
            healthRegen = 2.5f,
            armor = 0f,

            jumpCount = 1
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] { 
            new CustomRendererInfo {
                childName = "HanDHammer",
            }, 
            new CustomRendererInfo {
                childName = "HANDMesh",
            },
        };

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();

            CharacterBody cb = bodyPrefab.GetComponent<CharacterBody>();
            cb.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Mechanical;

            SfxLocator sfx = bodyPrefab.GetComponent<SfxLocator>();
            sfx.landingSound = "play_char_land";
            sfx.fallDamageSound = "Play_MULT_shift_hit";

            CameraTargetParams cameraTargetParams = bodyPrefab.GetComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/toolbotbody").GetComponent<CameraTargetParams>().cameraParams;
            cameraTargetParams.cameraParams.data.idealLocalCameraPos = new Vector3(0f, 1f, -11f);

            RegisterStates();
            bodyPrefab.AddComponent<HANDNetworkComponent>();
            bodyPrefab.AddComponent<OverclockController>();
            bodyPrefab.AddComponent<TargetingController>();
            bodyPrefab.AddComponent<DroneStockController>();
            bodyPrefab.AddComponent<DroneFollowerController>();

            Content.HANDSurvivor.Buffs.Init();
        }
        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = HandPlugin.DEVELOPER_PREFIX;

            SkillDef nevermore = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Heretic/HereticDefaultAbility.asset").WaitForCompletion();

            Skills.AddPrimarySkills(bodyPrefab, new SkillDef[] { nevermore });

            Skills.AddSecondarySkills(bodyPrefab, new SkillDef[] { nevermore });
            
            InitializeUtilitySkills();
            InitializeSpecialSkills();
        }

        private void InitializeUtilitySkills()
        {

            Skills.AddUtilitySkills(bodyPrefab, new SkillDef[] {});
            SkillDef ovcSkill = SkillDef.CreateInstance<SkillDef>();
            ovcSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.HAND_Overclocked.Utility.BeginOverclock));
            ovcSkill.skillNameToken = HANDSurvivor.HAND_PREFIX + "UTILITY_NAME";
            ovcSkill.skillName = "BeginOverclock";
            ovcSkill.skillDescriptionToken = HANDSurvivor.HAND_PREFIX + "UTILITY_DESC";
            ovcSkill.isCombatSkill = false;
            ovcSkill.cancelSprintingOnActivation = false;
            ovcSkill.canceledFromSprinting = false;
            ovcSkill.baseRechargeInterval = 7f;
            ovcSkill.interruptPriority = EntityStates.InterruptPriority.Any;
            ovcSkill.mustKeyPress = true;
            ovcSkill.beginSkillCooldownOnSkillEnd = false;
            ovcSkill.baseMaxStock = 1;
            ovcSkill.fullRestockOnAssign = true;
            ovcSkill.rechargeStock = 1;
            ovcSkill.requiredStock = 1;
            ovcSkill.stockToConsume = 1;
            ovcSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityOverclock.png");
            ovcSkill.activationStateMachineName = "Slide";
            ovcSkill.keywordTokens = new string[] { HANDSurvivor.HAND_PREFIX + "KEYWORD_SPRINGY" };
            FixScriptableObjectName(ovcSkill);
            Modules.ContentPacks.skillDefs.Add(ovcSkill);

            SkillDef ovcCancelDef = SkillDef.CreateInstance<SkillDef>();
            ovcCancelDef.activationState = new SerializableEntityStateType(typeof(EntityStates.HAND_Overclocked.Utility.CancelOverclock));
            ovcCancelDef.activationStateMachineName = "Slide";
            ovcCancelDef.baseMaxStock = 1;
            ovcCancelDef.baseRechargeInterval = 7f;
            ovcCancelDef.beginSkillCooldownOnSkillEnd = true;
            ovcCancelDef.canceledFromSprinting = false;
            ovcCancelDef.dontAllowPastMaxStocks = true;
            ovcCancelDef.forceSprintDuringState = false;
            ovcCancelDef.fullRestockOnAssign = true;
            ovcCancelDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityOverclockCancel.png");
            ovcCancelDef.interruptPriority = InterruptPriority.Skill;
            ovcCancelDef.isCombatSkill = false;
            ovcCancelDef.keywordTokens = new string[] { HANDSurvivor.HAND_PREFIX + "KEYWORD_SPRINGY" };
            ovcCancelDef.mustKeyPress = true;
            ovcCancelDef.cancelSprintingOnActivation = false;
            ovcCancelDef.rechargeStock = 1;
            ovcCancelDef.requiredStock = 0;
            ovcCancelDef.skillName = "CancelOverclock";
            ovcCancelDef.skillNameToken = HANDSurvivor.HAND_PREFIX + "UTILITY_CANCEL_NAME";
            ovcCancelDef.skillDescriptionToken = HANDSurvivor.HAND_PREFIX + "UTILITY_CANCEL_DESC";
            ovcCancelDef.stockToConsume = 0;
            FixScriptableObjectName(ovcCancelDef);
            Modules.ContentPacks.skillDefs.Add(ovcCancelDef);
            EntityStates.HAND_Overclocked.Utility.BeginOverclock.cancelSkillDef = ovcCancelDef;

            OverclockController.texGauge = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGauge.png");
            OverclockController.texGaugeArrow = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGaugeArrow.png");
            OverclockController.ovcDef = ovcSkill;

            Skills.AddUtilitySkills(bodyPrefab, new SkillDef[] { ovcSkill });
        }

        private void InitializeSpecialSkills()
        {
            DroneSetup.Init();

            EntityStateMachine stateMachine = bodyPrefab.AddComponent<EntityStateMachine>();
            stateMachine.customName = "DroneLauncher";
            stateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.BaseBodyAttachmentState));
            stateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.BaseBodyAttachmentState));
            NetworkStateMachine nsm = bodyPrefab.GetComponent<NetworkStateMachine>();
            nsm.stateMachines = nsm.stateMachines.Append(stateMachine).ToArray();

            SkillDef droneSkill = SkillDef.CreateInstance<SkillDef>();
            droneSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.HAND_Overclocked.Special.FireSeekingDrone));
            droneSkill.skillNameToken = HANDSurvivor.HAND_PREFIX + "SPECIAL_NAME";
            droneSkill.skillName = "Drones";
            droneSkill.skillDescriptionToken = HANDSurvivor.HAND_PREFIX + "SPECIAL_DESC";
            droneSkill.isCombatSkill = true;
            droneSkill.cancelSprintingOnActivation = false;
            droneSkill.canceledFromSprinting = false;
            droneSkill.baseRechargeInterval = 10f;
            droneSkill.interruptPriority = EntityStates.InterruptPriority.Any;
            droneSkill.mustKeyPress = false;
            droneSkill.beginSkillCooldownOnSkillEnd = true;
            droneSkill.baseMaxStock = 10;
            droneSkill.fullRestockOnAssign = false;
            droneSkill.rechargeStock = 1;
            droneSkill.requiredStock = 1;
            droneSkill.stockToConsume = 1;
            droneSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecial.png");
            droneSkill.activationStateMachineName = "DroneLauncher";
            droneSkill.keywordTokens = new string[] { };
            FixScriptableObjectName(droneSkill);
            Modules.ContentPacks.skillDefs.Add(droneSkill);

            Skills.AddSpecialSkills(bodyPrefab, new SkillDef[] { droneSkill });
        }

        public override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(HAND_PREFIX + "DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererinfos,
                model);
            skins.Add(defaultSkin);
            #endregion

            skinController.skins = skins.ToArray();
        }

        private void RegisterStates()
        {
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Utility.BeginOverclock));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Utility.CancelOverclock));

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Special.FireSeekingDrone));
        }
        private void FixScriptableObjectName(SkillDef sk)
        {
            (sk as ScriptableObject).name = sk.skillName;
        }
    }
}
