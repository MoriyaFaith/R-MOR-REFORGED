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
using EntityStates;
using System.Linq;
using System.Runtime.CompilerServices;

using HANDMod.Content.HANDSurvivor.Components.Body;
using HANDMod.Content.Shared.Components.Body;
using HANDMod.Content.HANDSurvivor.CharacterUnlock;
using HAND_Overclocked.Content.Shared.Components.Body;

namespace HANDMod.Content.RMORSurvivor
{
    internal class RMORSurvivor : SurvivorBase
    {
        public static bool enabled = false;
        public const string RMOR_PREFIX = HandPlugin.DEVELOPER_PREFIX + "_RMOR_BODY_";
        public override string survivorTokenPrefix => RMOR_PREFIX;

        public override UnlockableDef characterUnlockableDef => null;

        public override string bodyName => "RMOR";
        public override string cachedName => "RMOR";

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "RMORBody",
            bodyNameToken = HandPlugin.DEVELOPER_PREFIX + "_RMOR_BODY_NAME",
            subtitleNameToken = HandPlugin.DEVELOPER_PREFIX + "_RMOR_BODY_SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texRMORPortraitOld.png"),
            bodyColor = new Color(0.556862745f, 0.682352941f, 0.690196078f),

            crosshair = LegacyResourcesAPI.Load<GameObject>("prefabs/crosshair/simpledotcrosshair"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/robocratepod"),//RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")

            damage = 12f,
            maxHealth = 140f,
            healthRegen = 0.5f,
            armor = 20f,

            jumpCount = 1,

            sortPosition = Config.sortPosition
        };

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();

            CharacterMotor cm = bodyPrefab.GetComponent<CharacterMotor>();
            cm.mass = 300f;

            DroneFollowerController.Initialize();
            HammerVisibilityController.Initialize();

            CharacterBody cb = bodyPrefab.GetComponent<CharacterBody>();
            cb.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Mechanical;

            SfxLocator sfx = bodyPrefab.GetComponent<SfxLocator>();
            sfx.landingSound = "play_char_land";
            sfx.fallDamageSound = "Play_MULT_shift_hit";

            //CameraTargetParams toolbotCamera = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/toolbotbody").GetComponent<CameraTargetParams>();
            CameraTargetParams cameraTargetParams = bodyPrefab.GetComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams.data.idealLocalCameraPos = new Vector3(0f, 1f, -11f);

            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();

            //Transform hammerTransform = childLocator.FindChild("HanDHammer");
            //hammerTransform.gameObject.SetActive(false);

            GameObject model = childLocator.gameObject;
            //Transform fistHitboxTransform = childLocator.FindChild("FistHitbox");
            //Prefabs.SetupHitbox(model, "FistHitbox", new Transform[] { fistHitboxTransform });

            AimAnimator aan = model.GetComponent<AimAnimator>();
            aan.yawRangeMin = -180f;
            aan.yawRangeMax = 180f;
            aan.fullYaw = true;

            //Transform chargeHammerHitboxTransform = childLocator.FindChild("ChargeHammerHitbox");
            //Prefabs.SetupHitbox(model, "ChargeHammerHitbox", new Transform[] { chargeHammerHitboxTransform });

            //Transform hammerHitboxTransform = childLocator.FindChild("HammerHitbox");
            //Prefabs.SetupHitbox(model, "HammerHitbox", new Transform[] { hammerHitboxTransform });

            LoopSoundWhileCharacterMoving ls = bodyPrefab.AddComponent<LoopSoundWhileCharacterMoving>();
            ls.startSoundName = "Play_MULT_move_loop";
            ls.stopSoundName = "Stop_MULT_move_loop";
            ls.applyScale = true;
            ls.disableWhileSprinting = false;
            ls.minSpeed = 3f;
            ls.requireGrounded = false;

            RegisterStates();
            bodyPrefab.AddComponent<HANDNetworkComponent>();
            bodyPrefab.AddComponent<OverclockController>();
            //bodyPrefab.AddComponent<HANDTargetingController>(); RMOR doesn't use this
            bodyPrefab.AddComponent<DroneStockController>();
            bodyPrefab.AddComponent<DroneFollowerController>();
            //bodyPrefab.AddComponent<HammerVisibilityController>();


            Content.HANDSurvivor.Buffs.Init();

            BrokenJanitorInteractable.Initialize();
            if (Modules.Config.allowPlayerRepair)
            {
                bodyPrefab.AddComponent<CreateRepairOnDeath>();
            }
        }

        //TODO: REPLACE
        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] {
            new CustomRendererInfo {
                childName = "HANDMesh",
            },
        };

        public override Type characterMainState => typeof(EntityStates.HAND_Overclocked.HANDMainState);

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);

            SkillLocator sk = bodyPrefab.GetComponent<SkillLocator>();
            sk.passiveSkill.enabled = true;
            sk.passiveSkill.skillNameToken = RMOR_PREFIX + "PASSIVE_NAME";
            sk.passiveSkill.skillDescriptionToken = RMOR_PREFIX + "PASSIVE_DESC";
            sk.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPassive.png");

            InitializePrimarySkills();
            InitializeSecondarySkills();
            InitializeUtilitySkills();
            InitializeSpecialSkills();
        }

        private void InitializePrimarySkills()
        {
            SkillDef primarySkill = SkillDef.CreateInstance<SkillDef>();
            primarySkill.activationState = new SerializableEntityStateType(typeof(EntityStates.RMOR.Primary.RMORRocket));
            primarySkill.skillNameToken = RMOR_PREFIX + "PRIMARY_NAME";
            primarySkill.skillName = "SwingPunch";
            primarySkill.skillDescriptionToken = RMOR_PREFIX + "PRIMARY_DESC";
            primarySkill.cancelSprintingOnActivation = false;
            primarySkill.canceledFromSprinting = false;
            primarySkill.baseRechargeInterval = 0f;
            primarySkill.baseMaxStock = 1;
            primarySkill.rechargeStock = 1;
            primarySkill.beginSkillCooldownOnSkillEnd = false;
            primarySkill.activationStateMachineName = "Weapon";
            primarySkill.interruptPriority = EntityStates.InterruptPriority.Any;
            primarySkill.isCombatSkill = true;
            primarySkill.mustKeyPress = false;
            primarySkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRMORPrimary.png");
            primarySkill.requiredStock = 1;
            primarySkill.stockToConsume = 1;
            primarySkill.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            Modules.Skills.FixScriptableObjectName(primarySkill);
            Modules.ContentPacks.skillDefs.Add(primarySkill);
            Skilldefs.PrimaryCannon = primarySkill;

            SkillFamily primarySkillFamily = bodyPrefab.GetComponent<SkillLocator>().primary.skillFamily;
            Skills.AddSkillToFamily(primarySkillFamily, primarySkill);

        }
        private void InitializeSecondarySkills()
        {
            SkillDef secondarySkill = SkillDef.CreateInstance<SkillDef>();
            secondarySkill.activationState = new SerializableEntityStateType(typeof(EntityStates.RMOR.Secondary.ChargeCannon));
            secondarySkill.skillNameToken = RMOR_PREFIX + "SECONDARY_NAME";
            secondarySkill.skillName = "ChargeSlam";
            secondarySkill.skillDescriptionToken = RMOR_PREFIX + "SECONDARY_DESC";
            secondarySkill.cancelSprintingOnActivation = false;
            secondarySkill.canceledFromSprinting = false;
            secondarySkill.baseRechargeInterval = 5f;
            secondarySkill.baseMaxStock = 1;
            secondarySkill.rechargeStock = 1;
            secondarySkill.requiredStock = 1;
            secondarySkill.stockToConsume = 1;
            secondarySkill.activationStateMachineName = "Weapon";
            secondarySkill.interruptPriority = EntityStates.InterruptPriority.Skill;
            secondarySkill.isCombatSkill = true;
            secondarySkill.mustKeyPress = false;
            secondarySkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondary.png");
            secondarySkill.beginSkillCooldownOnSkillEnd = true;
            secondarySkill.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            Modules.Skills.FixScriptableObjectName(secondarySkill);
            Modules.ContentPacks.skillDefs.Add(secondarySkill);

            SkillDef secondaryCannonSkill = SkillDef.CreateInstance<SkillDef>();
            secondaryCannonSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.HAND_Overclocked.Secondary.ChargeSlam));
            secondaryCannonSkill.skillNameToken = RMOR_PREFIX + "PRIMARY_HAMMER_NAME";
            secondaryCannonSkill.skillName = "SwingHammer";
            secondaryCannonSkill.skillDescriptionToken = RMOR_PREFIX + "PRIMARY_HAMMER_DESC";
            secondaryCannonSkill.cancelSprintingOnActivation = false;
            secondaryCannonSkill.canceledFromSprinting = false;
            secondaryCannonSkill.baseRechargeInterval = 0f;
            secondaryCannonSkill.baseMaxStock = 1;
            secondaryCannonSkill.rechargeStock = 1;
            secondaryCannonSkill.beginSkillCooldownOnSkillEnd = false;
            secondaryCannonSkill.activationStateMachineName = "Weapon";
            secondaryCannonSkill.interruptPriority = EntityStates.InterruptPriority.Any;
            secondaryCannonSkill.isCombatSkill = true;
            secondaryCannonSkill.mustKeyPress = false;
            secondaryCannonSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryHammer.png");
            secondaryCannonSkill.requiredStock = 1;
            secondaryCannonSkill.stockToConsume = 1;
            secondaryCannonSkill.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            Modules.Skills.FixScriptableObjectName(secondaryCannonSkill);
            Modules.ContentPacks.skillDefs.Add(secondaryCannonSkill);
            Skilldefs.PrimaryMissile = secondaryCannonSkill;

            UnlockableDef primaryHammerUnlock = ScriptableObject.CreateInstance<UnlockableDef>();
            primaryHammerUnlock.cachedName = "Skills.HANDOverclocked.HammerPrimary";
            primaryHammerUnlock.nameToken = "ACHIEVEMENT_MOFFEINHANDOVERCLOCKEDHAMMERPRIMARYUNLOCK_NAME";
            primaryHammerUnlock.achievementIcon = secondaryCannonSkill.icon;
            Modules.ContentPacks.unlockableDefs.Add(primaryHammerUnlock);

            SkillFamily secondarySkillFamily = bodyPrefab.GetComponent<SkillLocator>().secondary.skillFamily;
            Skilldefs.SecondaryChargeHammer = secondarySkill;

            Skills.AddSkillToFamily(secondarySkillFamily, secondarySkill);
            Skills.AddSkillToFamily(secondarySkillFamily, secondaryCannonSkill, Modules.Config.forceUnlock ? null : primaryHammerUnlock);

            InitializeScepterSkills();
        }
        private void InitializeUtilitySkills()
        {
            Skills.AddUtilitySkills(bodyPrefab, new SkillDef[] { Shared.SkillDefs.UtilityOverclock, Shared.SkillDefs.UtilityFocus });
        }
        private void InitializeSpecialSkills()
        {
            HANDSurvivor.DroneSetup.Init();

            HANDSurvivor.Components.DroneProjectile.DroneDamageController.startSound = Assets.CreateNetworkSoundEventDef("Play_HOC_Drill");
            HANDSurvivor.Components.DroneProjectile.DroneDamageController.hitSound = Assets.CreateNetworkSoundEventDef("Play_treeBot_m1_impact");

            EntityStateMachine stateMachine = bodyPrefab.AddComponent<EntityStateMachine>();
            stateMachine.customName = "DroneLauncher";
            stateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.BaseBodyAttachmentState));
            stateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.BaseBodyAttachmentState));
            NetworkStateMachine nsm = bodyPrefab.GetComponent<NetworkStateMachine>();
            nsm.stateMachines = nsm.stateMachines.Append(stateMachine).ToArray();

            //DroneSkillDef too restrictive, but it's there if it's needed.
            SkillDef droneSkill = SkillDef.CreateInstance<SkillDef>();
            droneSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.RMOR.Special.LockOn));
            droneSkill.skillNameToken = RMORSurvivor.RMOR_PREFIX + "SPECIAL_NAME";
            droneSkill.skillName = "MissileDrones";
            droneSkill.skillDescriptionToken = RMORSurvivor.RMOR_PREFIX + "SPECIAL_DESC";
            droneSkill.isCombatSkill = true;
            droneSkill.cancelSprintingOnActivation = false;
            droneSkill.canceledFromSprinting = false;
            droneSkill.baseRechargeInterval = 20f;
            droneSkill.interruptPriority = EntityStates.InterruptPriority.Any;
            droneSkill.mustKeyPress = false;
            droneSkill.beginSkillCooldownOnSkillEnd = true;
            droneSkill.baseMaxStock = 10;
            droneSkill.fullRestockOnAssign = false;
            droneSkill.rechargeStock = 1;
            droneSkill.requiredStock = 1;
            droneSkill.stockToConsume =0;
            droneSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecial.png");
            droneSkill.activationStateMachineName = "DroneLauncher";
            droneSkill.keywordTokens = new string[] { };
            Modules.Skills.FixScriptableObjectName(droneSkill);
            Modules.ContentPacks.skillDefs.Add(droneSkill);
            Skilldefs.SpecialMissile = droneSkill;

            Skills.AddSpecialSkills(bodyPrefab, new SkillDef[] { droneSkill });
        }
        private void InitializeScepterSkills()
        {
            SkillDef scepterSkill = SkillDef.CreateInstance<SkillDef>();
            scepterSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.HAND_Overclocked.Secondary.ChargeSlamScepter));
            scepterSkill.skillNameToken = RMOR_PREFIX + "SECONDARY_SCEPTER_NAME";
            scepterSkill.skillName = "ChargeSlamScepter";
            scepterSkill.skillDescriptionToken = RMOR_PREFIX + "SECONDARY_SCEPTER_DESC";
            scepterSkill.cancelSprintingOnActivation = Skilldefs.SecondaryChargeHammer.cancelSprintingOnActivation;
            scepterSkill.canceledFromSprinting = Skilldefs.SecondaryChargeHammer.canceledFromSprinting;
            scepterSkill.baseRechargeInterval = Skilldefs.SecondaryChargeHammer.baseRechargeInterval = 5f;
            scepterSkill.baseMaxStock = Skilldefs.SecondaryChargeHammer.baseMaxStock;
            scepterSkill.rechargeStock = Skilldefs.SecondaryChargeHammer.rechargeStock;
            scepterSkill.requiredStock = Skilldefs.SecondaryChargeHammer.requiredStock;
            scepterSkill.stockToConsume = Skilldefs.SecondaryChargeHammer.stockToConsume;
            scepterSkill.activationStateMachineName = Skilldefs.SecondaryChargeHammer.activationStateMachineName;
            scepterSkill.interruptPriority = Skilldefs.SecondaryChargeHammer.interruptPriority;
            scepterSkill.isCombatSkill = Skilldefs.SecondaryChargeHammer.isCombatSkill;
            scepterSkill.mustKeyPress = Skilldefs.SecondaryChargeHammer.mustKeyPress;
            scepterSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryScepter.png");
            scepterSkill.beginSkillCooldownOnSkillEnd = Skilldefs.SecondaryChargeHammer.beginSkillCooldownOnSkillEnd;
            scepterSkill.keywordTokens = Skilldefs.SecondaryChargeHammer.keywordTokens;
            Modules.Skills.FixScriptableObjectName(scepterSkill);
            Modules.ContentPacks.skillDefs.Add(scepterSkill);

            Skilldefs.SecondaryChargeHammerScepter = scepterSkill;

            if (HandPlugin.ScepterClassicLoaded) ClassicScepterCompat();
            if (HandPlugin.ScepterStandaloneLoaded) StandaloneScepterCompat();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ClassicScepterCompat()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(Skilldefs.SecondaryChargeHammerScepter, "RMORBody", SkillSlot.Secondary, Skilldefs.SecondaryChargeHammer);
        }


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void StandaloneScepterCompat()
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skilldefs.SecondaryChargeHammerScepter, "RMORBody", SkillSlot.Secondary, 0);
        }
        private void RegisterStates()
        {

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.HANDMainState));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Emotes.Sit));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Emotes.Spin));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Emotes.MenuPose));

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Primary.SwingPunch));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Primary.SwingHammer));

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Secondary.ChargeSlam));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Secondary.FireSlam));

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Secondary.ChargeSlamScepter));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Secondary.FireSlamScepter));

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Utility.BeginOverclock));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Utility.CancelOverclock));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Utility.BeginFocus));

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Special.FireSeekingDrone));
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
    }
}
