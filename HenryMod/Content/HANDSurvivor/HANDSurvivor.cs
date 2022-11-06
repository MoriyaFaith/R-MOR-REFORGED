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
using R2API;
using System.Runtime.CompilerServices;

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

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texPortrait.png"),
            bodyColor = new Color(0.556862745f, 0.682352941f, 0.690196078f),

            crosshair = LegacyResourcesAPI.Load<GameObject>("prefabs/crosshair/simpledotcrosshair"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/robocratepod"),//RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")

            damage = 14f,
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

            //CameraTargetParams toolbotCamera = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/toolbotbody").GetComponent<CameraTargetParams>();
            CameraTargetParams cameraTargetParams = bodyPrefab.GetComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams.data.idealLocalCameraPos = new Vector3(0f, 1f, -11f);

            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;
            Transform fistHitboxTransform = childLocator.FindChild("FistHitbox");
            Prefabs.SetupHitbox(model, "FistHitbox", new Transform[] { fistHitboxTransform });

            /*AimAnimator aan = model.GetComponent<AimAnimator>();
            aan.yawRangeMin = -180f;
            aan.yawRangeMax = 180f;
            aan.fullYaw = true;*/

            Transform chargeHammerHitboxTransform = childLocator.FindChild("ChargeHammerHitbox");
            Prefabs.SetupHitbox(model, "ChargeHammerHitbox", new Transform[] { chargeHammerHitboxTransform });

            Transform hammerHitboxTransform = childLocator.FindChild("HammerHitbox");
            Prefabs.SetupHitbox(model, "HammerHitbox", new Transform[] { hammerHitboxTransform });

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
            bodyPrefab.AddComponent<HANDTargetingController>();
            bodyPrefab.AddComponent<DroneStockController>();
            bodyPrefab.AddComponent<DroneFollowerController>();
            bodyPrefab.AddComponent<HammerVisibilityController>();

            Content.HANDSurvivor.Buffs.Init();
            CreateHitEffects();
            EntityStates.HAND_Overclocked.Utility.BeginOverclock.jetEffectPrefab = BuildOverclockJets();
            EntityStates.HAND_Overclocked.Secondary.FireSlam.earthquakeEffectPrefab = CreateSlamEffect();
            EntityStates.HAND_Overclocked.Primary.SwingHammer.swingEffect = CreateSwingVFX("HANDMod_SwingHammerEffect", 1.5f * Vector3.one, Addressables.LoadAssetAsync<Material>("RoR2/Base/Lemurian/matLizardBiteTrail.mat").WaitForCompletion());
            EntityStates.HAND_Overclocked.Primary.SwingPunch.swingEffect = CreateSwingVFX("HANDMod_SwingPunchEffect", new Vector3 (0.4f, 2f, 1f), Addressables.LoadAssetAsync<Material>("RoR2/Base/Lemurian/matLizardBiteTrail.mat").WaitForCompletion());
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);

            SkillLocator sk = bodyPrefab.GetComponent<SkillLocator>();
            sk.passiveSkill.enabled = true;
            sk.passiveSkill.skillNameToken = HAND_PREFIX + "PASSIVE_NAME";
            sk.passiveSkill.skillDescriptionToken = HAND_PREFIX + "PASSIVE_DESC";
            sk.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPassive.png");

            InitializePrimarySkills();
            InitializeSecondarySkills();
            InitializeUtilitySkills();
            InitializeSpecialSkills();
        }

        private void InitializePrimarySkills()
        {
            SkillDef primarySkill = SkillDef.CreateInstance<SkillDef>();
            primarySkill.activationState = new SerializableEntityStateType(typeof(EntityStates.HAND_Overclocked.Primary.SwingPunch));
            primarySkill.skillNameToken = HAND_PREFIX + "PRIMARY_NAME";
            primarySkill.skillName = "SwingPunch";
            primarySkill.skillDescriptionToken = HAND_PREFIX + "PRIMARY_DESC";
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
            primarySkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryPunch.png");
            primarySkill.requiredStock = 1;
            primarySkill.stockToConsume = 1;
            primarySkill.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            FixScriptableObjectName(primarySkill);
            Modules.ContentPacks.skillDefs.Add(primarySkill);
            SkillDefs.PrimaryPunch = primarySkill;

            EntityStates.HAND_Overclocked.Primary.SwingPunch.punchVelocityCurve.preWrapMode = UnityEngine.WrapMode.ClampForever;
            EntityStates.HAND_Overclocked.Primary.SwingPunch.punchVelocityCurve.postWrapMode = UnityEngine.WrapMode.ClampForever;

            SkillDef primaryHammerSkill = SkillDef.CreateInstance<SkillDef>();
            primaryHammerSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.HAND_Overclocked.Primary.SwingHammer));
            primaryHammerSkill.skillNameToken = HAND_PREFIX + "PRIMARY_HAMMER_NAME";
            primaryHammerSkill.skillName = "SwingHammer";
            primaryHammerSkill.skillDescriptionToken = HAND_PREFIX + "PRIMARY_HAMMER_DESC";
            primaryHammerSkill.cancelSprintingOnActivation = false;
            primaryHammerSkill.canceledFromSprinting = false;
            primaryHammerSkill.baseRechargeInterval = 0f;
            primaryHammerSkill.baseMaxStock = 1;
            primaryHammerSkill.rechargeStock = 1;
            primaryHammerSkill.beginSkillCooldownOnSkillEnd = false;
            primaryHammerSkill.activationStateMachineName = "Weapon";
            primaryHammerSkill.interruptPriority = EntityStates.InterruptPriority.Any;
            primaryHammerSkill.isCombatSkill = true;
            primaryHammerSkill.mustKeyPress = false;
            primaryHammerSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryPunch.png");
            primaryHammerSkill.requiredStock = 1;
            primaryHammerSkill.stockToConsume = 1;
            primaryHammerSkill.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            FixScriptableObjectName(primaryHammerSkill);
            Modules.ContentPacks.skillDefs.Add(primaryHammerSkill);
            SkillDefs.PrimaryHammer = primaryHammerSkill;

            Skills.AddPrimarySkills(bodyPrefab, new SkillDef[] { primarySkill, primaryHammerSkill });
        }
        private void InitializeSecondarySkills()
        {
            SkillDef secondarySkill = SkillDef.CreateInstance<SkillDef>();
            secondarySkill.activationState = new SerializableEntityStateType(typeof(EntityStates.HAND_Overclocked.Secondary.ChargeSlam));
            secondarySkill.skillNameToken = HANDSurvivor.HAND_PREFIX + "SECONDARY_NAME";
            secondarySkill.skillName = "ChargeSlam";
            secondarySkill.skillDescriptionToken = HANDSurvivor.HAND_PREFIX + "SECONDARY_DESC";
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
            FixScriptableObjectName(secondarySkill);
            Modules.ContentPacks.skillDefs.Add(secondarySkill);

            Skills.AddSecondarySkills(bodyPrefab, new SkillDef[] { secondarySkill });
            SkillDefs.SecondaryChargeHammer = secondarySkill;

            InitializeScepterSkills();
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
            ovcSkill.keywordTokens = new string[] { };//HANDSurvivor.HAND_PREFIX + "KEYWORD_SPRINGY"
            FixScriptableObjectName(ovcSkill);
            Modules.ContentPacks.skillDefs.Add(ovcSkill);
            SkillDefs.UtilityOverclock = ovcSkill;

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
            ovcCancelDef.keywordTokens = new string[] { };
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
            SkillDefs.UtilityOverclockCancel = ovcCancelDef;

            EntityStates.HAND_Overclocked.Utility.BeginOverclock.overlayMaterial = UnityEngine.Material.Instantiate(LegacyResourcesAPI.Load<Material>("Materials/matWolfhatOverlay"));

            EntityStates.HAND_Overclocked.Utility.BeginOverclock.texGauge = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGauge.png");
            EntityStates.HAND_Overclocked.Utility.BeginOverclock.texGaugeArrow = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGaugeArrow.png");
            OverclockController.ovcDef = ovcSkill;

            SkillDef focusSkill = SkillDef.CreateInstance<SkillDef>();
            focusSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.HAND_Overclocked.Utility.BeginFocus));
            focusSkill.skillNameToken = HANDSurvivor.HAND_PREFIX + "UTILITY_NEMESIS_NAME";
            focusSkill.skillName = "BeginFocus";
            focusSkill.skillDescriptionToken = HANDSurvivor.HAND_PREFIX + "UTILITY_NEMESIS_DESC";
            focusSkill.isCombatSkill = false;
            focusSkill.cancelSprintingOnActivation = false;
            focusSkill.canceledFromSprinting = false;
            focusSkill.baseRechargeInterval = 7f;
            focusSkill.interruptPriority = EntityStates.InterruptPriority.Any;
            focusSkill.mustKeyPress = true;
            focusSkill.beginSkillCooldownOnSkillEnd = false;
            focusSkill.baseMaxStock = 1;
            focusSkill.fullRestockOnAssign = true;
            focusSkill.rechargeStock = 1;
            focusSkill.requiredStock = 1;
            focusSkill.stockToConsume = 1;
            focusSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityNemesis.png");
            focusSkill.activationStateMachineName = "Slide";
            focusSkill.keywordTokens = new string[] { };//HANDSurvivor.HAND_PREFIX + "KEYWORD_SPRINGY"
            FixScriptableObjectName(focusSkill);
            Modules.ContentPacks.skillDefs.Add(focusSkill);
            SkillDefs.UtilityFocus = focusSkill;

            EntityStates.HAND_Overclocked.Utility.BeginFocus.overlayMaterial = UnityEngine.Material.Instantiate(LegacyResourcesAPI.Load<Material>("Materials/matFullCrit"));

            EntityStates.HAND_Overclocked.Utility.BeginFocus.texGaugeNemesis = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGaugeNemesis.png");
            EntityStates.HAND_Overclocked.Utility.BeginFocus.texGaugeArrowNemesis = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGaugeArrowNemesis.png");


            Skills.AddUtilitySkills(bodyPrefab, new SkillDef[] { ovcSkill, focusSkill });
        }

        private void InitializeSpecialSkills()
        {
            DroneSetup.Init();

            Components.DroneProjectile.DroneDamageController.startSound = Assets.CreateNetworkSoundEventDef("Play_HOC_Drill");
            Components.DroneProjectile.DroneDamageController.hitSound = Assets.CreateNetworkSoundEventDef("Play_treeBot_m1_impact");

            EntityStateMachine stateMachine = bodyPrefab.AddComponent<EntityStateMachine>();
            stateMachine.customName = "DroneLauncher";
            stateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.BaseBodyAttachmentState));
            stateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.BaseBodyAttachmentState));
            NetworkStateMachine nsm = bodyPrefab.GetComponent<NetworkStateMachine>();
            nsm.stateMachines = nsm.stateMachines.Append(stateMachine).ToArray();

            //DroneSkillDef too restrictive, but it's there if it's needed.
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
            SkillDefs.SpecialDrone = droneSkill;

            Skills.AddSpecialSkills(bodyPrefab, new SkillDef[] { droneSkill });
        }

        private void InitializeScepterSkills()
        {
            SkillDef scepterSkill = SkillDef.CreateInstance<SkillDef>();
            scepterSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.HAND_Overclocked.Secondary.ChargeSlamScepter));
            scepterSkill.skillNameToken = HANDSurvivor.HAND_PREFIX + "SECONDARY_SCEPTER_NAME";
            scepterSkill.skillName = "ChargeSlamScepter";
            scepterSkill.skillDescriptionToken = HANDSurvivor.HAND_PREFIX + "SECONDARY_SCEPTER_DESC";
            scepterSkill.cancelSprintingOnActivation = SkillDefs.SecondaryChargeHammer.cancelSprintingOnActivation;
            scepterSkill.canceledFromSprinting = SkillDefs.SecondaryChargeHammer.canceledFromSprinting;
            scepterSkill.baseRechargeInterval = SkillDefs.SecondaryChargeHammer.baseRechargeInterval = 5f;
            scepterSkill.baseMaxStock = SkillDefs.SecondaryChargeHammer.baseMaxStock;
            scepterSkill.rechargeStock = SkillDefs.SecondaryChargeHammer.rechargeStock;
            scepterSkill.requiredStock = SkillDefs.SecondaryChargeHammer.requiredStock;
            scepterSkill.stockToConsume = SkillDefs.SecondaryChargeHammer.stockToConsume;
            scepterSkill.activationStateMachineName = SkillDefs.SecondaryChargeHammer.activationStateMachineName;
            scepterSkill.interruptPriority = SkillDefs.SecondaryChargeHammer.interruptPriority;
            scepterSkill.isCombatSkill = SkillDefs.SecondaryChargeHammer.isCombatSkill;
            scepterSkill.mustKeyPress = SkillDefs.SecondaryChargeHammer.mustKeyPress;
            scepterSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryScepter.png");
            scepterSkill.beginSkillCooldownOnSkillEnd = SkillDefs.SecondaryChargeHammer.beginSkillCooldownOnSkillEnd;
            scepterSkill.keywordTokens = SkillDefs.SecondaryChargeHammer.keywordTokens;
            FixScriptableObjectName(scepterSkill);
            Modules.ContentPacks.skillDefs.Add(scepterSkill);

            SkillDefs.SecondaryChargeHammerScepter = scepterSkill;

            if (HandPlugin.classicScepterLoaded) ClassicScepterCompat();
            if (HandPlugin.standaloneScepterLoaded) StandaloneScepterCompat();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ClassicScepterCompat()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(SkillDefs.SecondaryChargeHammerScepter, "HANDOverclockedBody", SkillSlot.Secondary, SkillDefs.SecondaryChargeHammer);
        }


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void StandaloneScepterCompat()
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(SkillDefs.SecondaryChargeHammerScepter, "HANDOverclockedBody", SkillSlot.Secondary, 0);
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
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Primary.SwingPunch));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Primary.SwingHammer));

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Secondary.ChargeSlam));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Secondary.FireSlam));

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Utility.BeginOverclock));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Utility.CancelOverclock));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Utility.BeginFocus));

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.Special.FireSeekingDrone));
        }
        private void FixScriptableObjectName(SkillDef sk)
        {
            (sk as ScriptableObject).name = sk.skillName;
        }
        private GameObject CreateSlamEffect()
        {
            GameObject slamImpactEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/ParentSlamEffect").InstantiateClone("HANDMod_SlamImpactEffect", false);

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
            //GameObject hitEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/ImpactToolbotDash").InstantiateClone("HANDMod_MeleeHitEffect");
            /*GameObject hitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFX.prefab").WaitForCompletion().InstantiateClone("HANDMod_MeleeHitEffect");
            EffectComponent ec = hitEffect.GetComponent<EffectComponent>();
            ec.soundName = "";
            //hitEffect.transform.localScale = 3f * Vector3.one;
            Modules.ContentPacks.effectDefs.Add(new EffectDef(hitEffect));
            EntityStates.HAND_Overclocked.Primary.SwingFist.hitEffect = hitEffect;*/


            /*NetworkSoundEventDef nse = Modules.Assets.CreateNetworkSoundEventDef("Play_MULT_shift_hit");
            EntityStates.HAND_Overclocked.Primary.SwingFist.networkHitSound = nse;
            EntityStates.HAND_Overclocked.Secondary.FireSlam.networkHitSound = nse;*/
        }

        private GameObject BuildOverclockJets()
        {
            GameObject jetObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoDashJets.prefab").WaitForCompletion().InstantiateClone("HANDMod_OverclockJetObject", false);

            ParticleSystemRenderer [] particles = jetObject.GetComponentsInChildren<ParticleSystemRenderer>();
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
                for (int i = 0; i< vfx.optionalLights.Length; i++)
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

        //Use these to check Vanilla values of things.
        public static void DumpEntityStateConfig(EntityStateConfiguration esc)
        {

            for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
            {
                if (esc.serializedFieldsCollection.serializedFields[i].fieldValue.objectValue)
                {
                    Debug.Log(esc.serializedFieldsCollection.serializedFields[i].fieldName + " - " + esc.serializedFieldsCollection.serializedFields[i].fieldValue.objectValue);
                }
                else
                {
                    Debug.Log(esc.serializedFieldsCollection.serializedFields[i].fieldName + " - " + esc.serializedFieldsCollection.serializedFields[i].fieldValue.stringValue);
                }
            }
        }
        public static void DumpEntityStateConfig(string entityStateName)
        {
            EntityStateConfiguration esc = LegacyResourcesAPI.Load<EntityStateConfiguration>("entitystateconfigurations/" + entityStateName);
            DumpEntityStateConfig(esc);
        }
    }
}
