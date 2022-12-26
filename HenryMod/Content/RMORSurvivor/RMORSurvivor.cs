using RMORMod.Modules.Survivors;
using UnityEngine;
using RoR2;
using RMORMod.Modules.Characters;
using RMORMod.Modules;
using System;
using System.Collections.Generic;
using RoR2.Skills;
using UnityEngine.AddressableAssets;
using R2API;
using RMORMod.Content.RMORSurvivor.Components.Body;
using EntityStates;
using System.Linq;
using System.Runtime.CompilerServices;

using RMORMod.Content.HANDSurvivor.Components.Body;
using RMORMod.Content.Shared.Components.Body;
using RMORMod.Content.HANDSurvivor.CharacterUnlock;
using RMORMod.Content.HANDSurvivor;

namespace RMORMod.Content.RMORSurvivor
{
    internal class RMORSurvivor : SurvivorBase
    {
        public static bool enabled = false;
        public const string RMOR_PREFIX = HandPlugin.DEVELOPER_PREFIX + "_RMOR_BODY_";
        public override string survivorTokenPrefix => RMOR_PREFIX;
        public override ItemDisplaysBase itemDisplays => new RMOR_Reforged.Content.RMORSurvivor.RMORItemDisplays();
        public override UnlockableDef characterUnlockableDef => CreateUnlockableDef();
        private static UnlockableDef survivorUnlock;
        public override string bodyName => "RMOR";
        public override string cachedName => "RMOR";

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "RMORBody",
            bodyNameToken = HandPlugin.DEVELOPER_PREFIX + "_RMOR_BODY_NAME",
            subtitleNameToken = HandPlugin.DEVELOPER_PREFIX + "_RMOR_BODY_SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texRMORPortrait.png"),
            bodyColor = new Color(0.556862745f, 0.682352941f, 0.690196078f),

            crosshair = LegacyResourcesAPI.Load<GameObject>("prefabs/crosshair/simpledotcrosshair"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/robocratepod"),//RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")
            
            damage = 12f,
            damageGrowth = 12f * 0.2f,

            maxHealth = 120f,
            healthGrowth = 120f * 0.3f,

            healthRegen = 0.5f,
            regenGrowth = 0.5f * 0.1f,

            armor = 50f,
            armorGrowth = 0f,

            jumpCount = 1,

            sortPosition = Config.sortPosition
        };
        private static UnlockableDef CreateUnlockableDef()
        {
            if (!survivorUnlock)
            {
                survivorUnlock = ScriptableObject.CreateInstance<UnlockableDef>();
                survivorUnlock.cachedName = "Characters.RMOR";
                survivorUnlock.nameToken = "ACHIEVEMENT_MORIYARMORSURVIVORUNLOCK_NAME";
                survivorUnlock.hidden = true;
                survivorUnlock.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texRMORPortrait.png");
                Modules.ContentPacks.unlockableDefs.Add(survivorUnlock);
            }

            if (Modules.Config.forceUnlock) return null;
            return survivorUnlock;
        }

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
            Transform bladeHitboxTransform = childLocator.FindChild("BladeHitbox");
            Prefabs.SetupHitbox(model, "BladeHitbox", new Transform[] { bladeHitboxTransform }); ;
            Transform stabHitboxTransform = childLocator.FindChild("StabHitbox");
            Prefabs.SetupHitbox(model, "StabHitbox", new Transform[] { stabHitboxTransform }); ;

            AimAnimator aan = model.GetComponent<AimAnimator>();
            aan.yawRangeMin = -180f;
            aan.yawRangeMax = 180f;
            aan.fullYaw = true;

            Material matDefault = Addressables.LoadAssetAsync<Material>("RoR2/Base/Lemurian/matLizardBiteTrail.mat").WaitForCompletion();
            EntityStates.RMOR.Primary.SwingStab.swingEffect = CreateSwingVFX("RMORMod_SwingPunchEffect", new Vector3(0.25f, 2f, 0.7f), matDefault);

            Material matFocus = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            EntityStates.RMOR.Primary.SwingStab.swingEffectFocus = CreateSwingVFX("RMORMod_SwingPunchFocusEffect", new Vector3(0.25f, 2f, 0.7f), matFocus);

            LoopSoundWhileCharacterMoving ls = bodyPrefab.AddComponent<LoopSoundWhileCharacterMoving>();
            ls.startSoundName = "Play_MULT_move_loop";
            ls.stopSoundName = "Stop_MULT_move_loop";
            ls.applyScale = true;
            ls.disableWhileSprinting = false;
            ls.minSpeed = 3f;
            ls.requireGrounded = false;

            RegisterStates();
            bodyPrefab.AddComponent<RMORNetworkComponent>();
            bodyPrefab.AddComponent<OverclockController>();
            bodyPrefab.AddComponent<RMORTargetingController>();
            bodyPrefab.AddComponent<DroneStockController>();
            bodyPrefab.AddComponent<DroneFollowerController>();
            //bodyPrefab.AddComponent<HammerVisibilityController>();


            Content.HANDSurvivor.Buffs.Init();

            CreateHitEffects();
            EntityStates.RMOR.Utility.BeginOverclock.jetEffectPrefab = BuildOverclockJets();
            EntityStates.RMOR.Secondary.FireSlam.earthquakeEffectPrefab = CreateSlamEffect();

            BrokenJanitorInteractable.Initialize();
            if (Modules.Config.allowPlayerRepair)
            {
                bodyPrefab.AddComponent<CreateRepairOnDeath>();
            }
        }

        //TODO: REPLACE
        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] {
            new CustomRendererInfo {
                childName = "RMORBody",
                material = Materials.CreateHopooMaterial("matRMORDefault"),
            },
            new CustomRendererInfo {
                childName = "Drone",
                material = Materials.CreateHopooMaterial("matRMORDrone"),
            },
        };

        public override Type characterMainState => typeof(EntityStates.HAND_Junked.HANDMainState);

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
            Modules.Skills.FixScriptableObjectName(primarySkill);
            Modules.ContentPacks.skillDefs.Add(primarySkill);
            Skilldefs.PrimaryCannon = primarySkill;

            SkillDef primaryStabSkill = SkillDef.CreateInstance<SkillDef>();
            primaryStabSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.RMOR.Primary.SwingStab));
            primaryStabSkill.skillNameToken = RMOR_PREFIX + "PRIMARY_BLADE_NAME";
            primaryStabSkill.skillName = "SwingPunch";
            primaryStabSkill.skillDescriptionToken = RMOR_PREFIX + "PRIMARY_BLADE_DESC";
            primaryStabSkill.cancelSprintingOnActivation = false;
            primaryStabSkill.canceledFromSprinting = false;
            primaryStabSkill.baseRechargeInterval = 0f;
            primaryStabSkill.baseMaxStock = 1;
            primaryStabSkill.rechargeStock = 1;
            primaryStabSkill.beginSkillCooldownOnSkillEnd = false;
            primaryStabSkill.activationStateMachineName = "Weapon";
            primaryStabSkill.interruptPriority = EntityStates.InterruptPriority.Any;
            primaryStabSkill.isCombatSkill = true;
            primaryStabSkill.mustKeyPress = false;
            primaryStabSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryPunch.png");
            primaryStabSkill.requiredStock = 1;
            primaryStabSkill.stockToConsume = 1;
            primaryStabSkill.keywordTokens = new string[] { "KEYWORD_MORIYARMOR_BLEEDING" };
            Modules.Skills.FixScriptableObjectName(primaryStabSkill);
            Modules.ContentPacks.skillDefs.Add(primaryStabSkill);
            Skilldefs.PrimaryStab = primaryStabSkill;

            SkillFamily primarySkillFamily = bodyPrefab.GetComponent<SkillLocator>().primary.skillFamily;
            Skills.AddSkillToFamily(primarySkillFamily, primarySkill);
            Skills.AddSkillToFamily(primarySkillFamily, primaryStabSkill);

        }
        private void InitializeSecondarySkills()
        {
            SkillDef secondarySkill = SkillDef.CreateInstance<SkillDef>();
            secondarySkill.activationState = new SerializableEntityStateType(typeof(EntityStates.RMOR.Secondary.ChargeCannon));
            secondarySkill.skillNameToken = RMOR_PREFIX + "SECONDARY_NAME";
            secondarySkill.skillName = "ChargeCannon";
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
            secondarySkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRMORSecondary.png");
            secondarySkill.beginSkillCooldownOnSkillEnd = true;
            Modules.Skills.FixScriptableObjectName(secondarySkill);
            Modules.ContentPacks.skillDefs.Add(secondarySkill);
            Skilldefs.SecondaryChargeCannon = secondarySkill;

            SkillDef secondaryHammerSkill = SkillDef.CreateInstance<SkillDef>();
            secondaryHammerSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.RMOR.Secondary.ChargeSlam));
            secondaryHammerSkill.skillNameToken = RMOR_PREFIX + "ALTSECONDARY_NAME";
            secondaryHammerSkill.skillName = "ChargeHammer";
            secondaryHammerSkill.skillDescriptionToken = RMOR_PREFIX + "ALTSECONDARY_DESC";
            secondaryHammerSkill.cancelSprintingOnActivation = false;
            secondaryHammerSkill.canceledFromSprinting = false;
            secondaryHammerSkill.baseRechargeInterval = 5f;
            secondaryHammerSkill.baseMaxStock = 1;
            secondaryHammerSkill.rechargeStock = 1;
            secondaryHammerSkill.requiredStock = 1;
            secondaryHammerSkill.stockToConsume = 1;
            secondaryHammerSkill.activationStateMachineName = "Weapon";
            secondaryHammerSkill.interruptPriority = EntityStates.InterruptPriority.Skill;
            secondaryHammerSkill.isCombatSkill = true;
            secondaryHammerSkill.mustKeyPress = false;
            secondaryHammerSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondary.png");
            secondaryHammerSkill.beginSkillCooldownOnSkillEnd = true;
            secondaryHammerSkill.keywordTokens = new string[] { "KEYWORD_MORIYARMOR_BLEEDING" };
            Modules.Skills.FixScriptableObjectName(secondaryHammerSkill);
            Modules.ContentPacks.skillDefs.Add(secondaryHammerSkill);
            Skilldefs.SecondaryChargeHammer = secondaryHammerSkill;

            SkillFamily secondarySkillFamily = bodyPrefab.GetComponent<SkillLocator>().secondary.skillFamily;
            Skilldefs.SecondaryChargeHammer = secondaryHammerSkill;

            Skills.AddSkillToFamily(secondarySkillFamily, secondarySkill);
            Skills.AddSkillToFamily(secondarySkillFamily, secondaryHammerSkill); //not working right now

            InitializeScepterSkills();
        }
        private void InitializeUtilitySkills()
        {
            UnlockableDef ovcUnlock = ScriptableObject.CreateInstance<UnlockableDef>();
            ovcUnlock.cachedName = "Skills.RMOR.Overclock";
            ovcUnlock.nameToken = "ACHIEVEMENT_MORIYARMOROVERCLOCKUNLOCK_NAME";
            ovcUnlock.achievementIcon = Shared.SkillDefs.UtilityOverclock.icon;
            Modules.ContentPacks.unlockableDefs.Add(ovcUnlock);

            SkillFamily utilityFamily = bodyPrefab.GetComponent<SkillLocator>().utility.skillFamily;
            Skills.AddSkillToFamily(utilityFamily, Shared.SkillDefs.UtilityFortify);

            Skills.AddSkillToFamily(utilityFamily, Shared.SkillDefs.UtilityOverclock, Modules.Config.forceUnlock ? null : ovcUnlock);
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
            droneSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.RMOR.Special.FireSeekingDrone));
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
            droneSkill.stockToConsume = 1;
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
            scepterSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.RMOR.Secondary.ChargeCannonScepter));
            scepterSkill.skillNameToken = RMOR_PREFIX + "SECONDARY_SCEPTER_NAME";
            scepterSkill.skillName = "ChargeCannonScepter";
            scepterSkill.skillDescriptionToken = RMOR_PREFIX + "SECONDARY_SCEPTER_DESC";
            scepterSkill.cancelSprintingOnActivation = Skilldefs.SecondaryChargeCannon.cancelSprintingOnActivation;
            scepterSkill.canceledFromSprinting = Skilldefs.SecondaryChargeCannon.canceledFromSprinting;
            scepterSkill.baseRechargeInterval = Skilldefs.SecondaryChargeCannon.baseRechargeInterval = 5f;
            scepterSkill.baseMaxStock = Skilldefs.SecondaryChargeCannon.baseMaxStock;
            scepterSkill.rechargeStock = Skilldefs.SecondaryChargeCannon.rechargeStock;
            scepterSkill.requiredStock = Skilldefs.SecondaryChargeCannon.requiredStock;
            scepterSkill.stockToConsume = Skilldefs.SecondaryChargeCannon.stockToConsume;
            scepterSkill.activationStateMachineName = Skilldefs.SecondaryChargeCannon.activationStateMachineName;
            scepterSkill.interruptPriority = Skilldefs.SecondaryChargeCannon.interruptPriority;
            scepterSkill.isCombatSkill = Skilldefs.SecondaryChargeCannon.isCombatSkill;
            scepterSkill.mustKeyPress = Skilldefs.SecondaryChargeCannon.mustKeyPress;
            scepterSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRMORSecondaryScepter.png");
            scepterSkill.beginSkillCooldownOnSkillEnd = Skilldefs.SecondaryChargeCannon.beginSkillCooldownOnSkillEnd;
            scepterSkill.keywordTokens = Skilldefs.SecondaryChargeCannon.keywordTokens;
            Modules.Skills.FixScriptableObjectName(scepterSkill);
            Modules.ContentPacks.skillDefs.Add(scepterSkill);

            Skilldefs.SecondaryChargeCannonScepter = scepterSkill;

            SkillDef scepterHammerSkill = SkillDef.CreateInstance<SkillDef>();
            scepterHammerSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.RMOR.Secondary.ChargeSlamScepter));
            scepterHammerSkill.skillNameToken = RMOR_PREFIX + "ALTSECONDARY_SCEPTER_NAME";
            scepterHammerSkill.skillName = "ChargeSlamScepter";
            scepterHammerSkill.skillDescriptionToken = RMOR_PREFIX + "ALTSECONDARY_SCEPTER_DESC";
            scepterHammerSkill.cancelSprintingOnActivation = Skilldefs.SecondaryChargeHammer.cancelSprintingOnActivation;
            scepterHammerSkill.canceledFromSprinting = Skilldefs.SecondaryChargeHammer.canceledFromSprinting;
            scepterHammerSkill.baseRechargeInterval = Skilldefs.SecondaryChargeHammer.baseRechargeInterval = 5f;
            scepterHammerSkill.baseMaxStock = Skilldefs.SecondaryChargeHammer.baseMaxStock;
            scepterHammerSkill.rechargeStock = Skilldefs.SecondaryChargeHammer.rechargeStock;
            scepterHammerSkill.requiredStock = Skilldefs.SecondaryChargeHammer.requiredStock;
            scepterHammerSkill.stockToConsume = Skilldefs.SecondaryChargeHammer.stockToConsume;
            scepterHammerSkill.activationStateMachineName = Skilldefs.SecondaryChargeHammer.activationStateMachineName;
            scepterHammerSkill.interruptPriority = Skilldefs.SecondaryChargeHammer.interruptPriority;
            scepterHammerSkill.isCombatSkill = Skilldefs.SecondaryChargeHammer.isCombatSkill;
            scepterHammerSkill.mustKeyPress = Skilldefs.SecondaryChargeHammer.mustKeyPress;
            scepterHammerSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryScepter.png");
            scepterHammerSkill.beginSkillCooldownOnSkillEnd = Skilldefs.SecondaryChargeHammer.beginSkillCooldownOnSkillEnd;
            scepterHammerSkill.keywordTokens = Skilldefs.SecondaryChargeHammer.keywordTokens;
            Modules.Skills.FixScriptableObjectName(scepterHammerSkill);
            Modules.ContentPacks.skillDefs.Add(scepterHammerSkill);

            Skilldefs.SecondaryChargeHammerScepter = scepterHammerSkill;

            if (HandPlugin.ScepterClassicLoaded) ClassicScepterCompat();
            if (HandPlugin.ScepterStandaloneLoaded) StandaloneScepterCompat();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ClassicScepterCompat()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(Skilldefs.SecondaryChargeCannonScepter, "RMORBody", SkillSlot.Secondary, Skilldefs.SecondaryChargeCannon);
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(Skilldefs.SecondaryChargeHammerScepter, "RMORBody", SkillSlot.Secondary, Skilldefs.SecondaryChargeHammer);
        }


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void StandaloneScepterCompat()
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skilldefs.SecondaryChargeCannonScepter, "RMORBody", SkillSlot.Secondary, 0);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skilldefs.SecondaryChargeHammerScepter, "RMORBody", SkillSlot.Secondary, 1);
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
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef("DEFAULT_SKIN",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texRMORSkinIconDefault"),
                defaultRendererinfos,
                model);

            skins.Add(defaultSkin);

            //materials are the default materials
            #endregion

            /*
            #region MasterySkin

            //creating a new skindef as we did before
            Sprite masteryIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texHANDSkinIconMastery");
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(RMOR_PREFIX + "MASTERY_SKIN_NAME",
                masteryIcon,
                defaultRendererinfos,
                model,
                //masterySkinUnlockableDef
                );

            masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos,
                "meshHanDMastery_Body",
                "meshDroneMastery_Body");

            masterySkin.rendererInfos[0].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHANDMastery");

            UnlockableDef masteryUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            masteryUnlockableDef.cachedName = "Skins.RMOR.Mastery";
            masteryUnlockableDef.nameToken = "ACHIEVEMENT_MORIYARMORCLEARGAMEMONSOON_NAME";
            masteryUnlockableDef.achievementIcon = masteryIcon;
            Modules.ContentPacks.unlockableDefs.Add(masteryUnlockableDef);
            masterySkin.unlockableDef = masteryUnlockableDef;

            skins.Add(masterySkin);

            #endregion
            */

            skinController.skins = skins.ToArray();

            On.RoR2.ProjectileGhostReplacementManager.Init += ProjectileGhostReplacementManager_Init;
        }

        private void ProjectileGhostReplacementManager_Init(On.RoR2.ProjectileGhostReplacementManager.orig_Init orig)
        {
            ModelSkinController skinController = bodyPrefab.GetComponentInChildren<ModelSkinController>();
            for (int i = 1; i < skinController.skins.Length; i++)
            {
                SkinDef skin = skinController.skins[i];

                if (DoesSkinHaveDroneReplacement(skin))
                {
                    skin.projectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[]
                    {
                        new SkinDef.ProjectileGhostReplacement
                        {
                            projectilePrefab = EntityStates.HAND_Junked.Special.FireSeekingDrone.projectilePrefab,
                            projectileGhostReplacementPrefab = CreateProjectileGhostReplacementPrefab(skin),
                        }
                    };
                }
            }

            orig();
        }


        public static bool DoesSkinHaveDroneReplacement(SkinDef skin)
        {

            SkinDef defaultSkin = RMORSurvivor.instance.bodyPrefab.GetComponentInChildren<ModelSkinController>().skins[0];

            for (int i = 0; i < skin.meshReplacements.Length; i++)
            {
                if (skin.meshReplacements[i].renderer == defaultSkin.rendererInfos[2].renderer ||
                   skin.meshReplacements[i].renderer == defaultSkin.rendererInfos[3].renderer)
                {
                    return true;
                }
            }
            for (int i = 0; i < skin.rendererInfos.Length; i++)
            {
                if (skin.rendererInfos[i].renderer == defaultSkin.rendererInfos[2].renderer ||
                    skin.rendererInfos[i].renderer == defaultSkin.rendererInfos[3].renderer)
                {
                    return true;
                }
            }
            return false;
        }


        public static GameObject CreateProjectileGhostReplacementPrefab(SkinDef skin)
        {
            GameObject ghostReplacement = PrefabAPI.InstantiateClone(DroneSetup.droneProjectileGhost, skin.nameToken + "DroneGhostReplacement", false);
            SkinnedMeshRenderer ghostDroneRenderer = ghostReplacement.GetComponent<ChildLocator>().FindChildComponent<SkinnedMeshRenderer>("Drone");
            MeshRenderer ghostSawRenderer = ghostReplacement.GetComponent<ChildLocator>().FindChildComponent<MeshRenderer>("Saw");

            SkinDef defaultSkin = RMORSurvivor.instance.bodyPrefab.GetComponentInChildren<ModelSkinController>().skins[0];

            CharacterModel.RendererInfo defaultRendererInfoDrone = defaultSkin.rendererInfos[2];
            CharacterModel.RendererInfo defaultRendererInfoSaw = defaultSkin.rendererInfos[3];

            for (int i = 0; i < skin.rendererInfos.Length; i++)
            {
                if (skin.rendererInfos[i].renderer == defaultRendererInfoDrone.renderer)
                {
                    ghostDroneRenderer.material = skin.rendererInfos[i].defaultMaterial;
                }
                if (skin.rendererInfos[i].renderer == defaultRendererInfoSaw.renderer)
                {
                    ghostSawRenderer.material = skin.rendererInfos[i].defaultMaterial;
                }
            }

            for (int i = 0; i < skin.meshReplacements.Length; i++)
            {
                if (skin.meshReplacements[i].renderer == defaultRendererInfoDrone.renderer)
                {
                    ghostDroneRenderer.sharedMesh = skin.meshReplacements[i].mesh;
                }
                if (skin.meshReplacements[i].renderer == defaultRendererInfoSaw.renderer)
                {
                    ghostSawRenderer.GetComponent<MeshFilter>().mesh = skin.meshReplacements[i].mesh;
                }
            }

            return ghostReplacement;
        }

        private void RegisterStates()
        {

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Junked.HANDMainState));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Junked.Emotes.Sit));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Junked.Emotes.Spin));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Junked.Emotes.MenuPose));

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.RMOR.Primary.RMORRocket));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Junked.Primary.SwingHammer));

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.RMOR.Secondary.ChargeSlam));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.RMOR.Secondary.FireSlam));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.RMOR.Secondary.ChargeCannon));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.RMOR.Secondary.FireCannon));

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.RMOR.Secondary.ChargeSlamScepter));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.RMOR.Secondary.FireSlamScepter));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.RMOR.Secondary.ChargeCannonScepter));

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.RMOR.Utility.BeginOverclock));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.RMOR.Utility.CancelOverclock));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.RMOR.Utility.BeginFocus));

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.RMOR.Special.LockOn));
        }
        private GameObject CreateSwingVFX(string name, Vector3 scale, Material material)
        {
            GameObject swingTrail = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/handslamtrail").InstantiateClone(name, false);
            UnityEngine.Object.Destroy(swingTrail.GetComponent<ShakeEmitter>());

            Transform swingTrailTransform = swingTrail.transform.Find("SlamTrail");
            swingTrailTransform.localScale = scale;

            ParticleSystemRenderer renderer = swingTrailTransform.GetComponent<ParticleSystemRenderer>();

            Material swingTrailMat = material;
            if (renderer)
            {
                renderer.material = swingTrailMat;
            }

            Modules.ContentPacks.effectDefs.Add(new EffectDef(swingTrail));

            return swingTrail;
        }

        private GameObject CreateSlamEffect()
        {
            GameObject slamImpactEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/ParentSlamEffect").InstantiateClone("RMORMod_SlamImpactEffect", false);

            var particleParent = slamImpactEffect.transform.Find("Particles");
            var debris = particleParent.Find("Debris, 3D");
            var debris2 = particleParent.Find("Debris");
            var sphere = particleParent.Find("Nova Sphere");

            debris.gameObject.SetActive(false);
            debris2.gameObject.SetActive(false);
            sphere.gameObject.SetActive(false);

            slamImpactEffect.GetComponent<EffectComponent>().soundName = "";
            //Play_parent_attack1_slam

            Modules.ContentPacks.effectDefs.Add(new EffectDef(slamImpactEffect));

            return slamImpactEffect;
        }

        private void CreateHitEffects()
        {
            GameObject hitEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/ImpactToolbotDash").InstantiateClone("RMORMod_HitEffect", false);
            EffectComponent ec = hitEffect.GetComponent<EffectComponent>();
            ec.soundName = "Play_MULT_shift_hit";
            Modules.ContentPacks.effectDefs.Add(new EffectDef(hitEffect));
            EntityStates.HAND_Junked.Primary.SwingStab.hitEffect = hitEffect;
            EntityStates.HAND_Junked.Primary.SwingHammer.hitEffect = hitEffect;
            EntityStates.HAND_Junked.Secondary.FireSlam.hitEffect = hitEffect;


            /*NetworkSoundEventDef nse = Modules.Assets.CreateNetworkSoundEventDef("Play_MULT_shift_hit");
            EntityStates.HAND_Junked.Primary.SwingFist.networkHitSound = nse;
            EntityStates.HAND_Junked.Secondary.FireSlam.networkHitSound = nse;*/
        }

        private GameObject BuildOverclockJets()
        {
            GameObject jetObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoDashJets.prefab").WaitForCompletion().InstantiateClone("RMORMod_OverclockJetObject", false);

            ParticleSystemRenderer[] particles = jetObject.GetComponentsInChildren<ParticleSystemRenderer>();
            foreach (ParticleSystemRenderer p in particles)
            {
                //[Info   : Unity Log] Jet
                //[Info   : Unity Log] Ring
                //[Info   : Unity Log] Distortion
                //[Info   : Unity Log] Sparks
                //[Info   : Unity Log] Flare

                string name = p.name;
                if (name != "Jet")
                {
                    UnityEngine.Object.Destroy(p);
                }
            }

            VFXAttributes vfx = jetObject.GetComponent<VFXAttributes>();
            if (vfx)
            {
                for (int i = 0; i < vfx.optionalLights.Length; i++)
                {
                    vfx.optionalLights[i].enabled = false;
                }
            }

            DestroyOnTimer dot = jetObject.GetComponent<DestroyOnTimer>();
            dot.duration = 0.2f;//0.3f vanilla

            Light[] lights = jetObject.GetComponentsInChildren<Light>();
            foreach (Light light in lights)
            {
                light.enabled = false;
            }

            //Does not have EffectComponent, no need to register.
            return jetObject;
        }

        private GameObject CreateEnemyIndicator()
        {
            GameObject indicator = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiMissileTrackingIndicator.prefab").WaitForCompletion().InstantiateClone("RMORMod_RMOR_EnemyIndicator", false);
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
