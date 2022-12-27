using EntityStates;
using RMORMod.Content.Shared;
using RMORMod.Modules;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RMORMod.Content.Shared
{
    internal class SharedContent
    {
        public SharedContent()
        {
            Buffs.Init();
            InitializeSkills();
        }

        private void InitializeSkills()
        {
            InitializeUtilitySkills();
        }

        private void InitializeUtilitySkills()
        {
            SkillDef ovcSkill = SkillDef.CreateInstance<SkillDef>();
            ovcSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.RMOR.Utility.BeginOverclock));
            ovcSkill.skillNameToken = RMORMod.Content.RMORSurvivor.RMORSurvivor.RMOR_PREFIX + "UTILITY_NAME";
            ovcSkill.skillName = "BeginOverclock";
            ovcSkill.skillDescriptionToken = RMORMod.Content.RMORSurvivor.RMORSurvivor.RMOR_PREFIX + "UTILITY_DESC";
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
            ovcSkill.keywordTokens = new string[] { "KEYWORD_MORIYAHANDOVERCLOCKED_BOOST" };//HANDSurvivor.HAND_PREFIX + "KEYWORD_SPRINGY"
            Modules.Skills.FixScriptableObjectName(ovcSkill);
            Modules.ContentPacks.skillDefs.Add(ovcSkill);
            SkillDefs.UtilityOverclock = ovcSkill;

            SkillDef fortifySkill = SkillDef.CreateInstance<SkillDef>();
            fortifySkill.activationState = new SerializableEntityStateType(typeof(EntityStates.RMOR.Utility.BeginFortify));
            fortifySkill.skillNameToken = RMORMod.Content.RMORSurvivor.RMORSurvivor.RMOR_PREFIX + "UTILITY_RMOR_NAME";
            fortifySkill.skillName = "BeginOverclock";
            fortifySkill.skillDescriptionToken = RMORMod.Content.RMORSurvivor.RMORSurvivor.RMOR_PREFIX + "UTILITY_RMOR_DESC";
            fortifySkill.isCombatSkill = false;
            fortifySkill.cancelSprintingOnActivation = false;
            fortifySkill.canceledFromSprinting = false;
            fortifySkill.baseRechargeInterval = 7f;
            fortifySkill.interruptPriority = EntityStates.InterruptPriority.Any;
            fortifySkill.mustKeyPress = true;
            fortifySkill.beginSkillCooldownOnSkillEnd = false;
            fortifySkill.baseMaxStock = 1;
            fortifySkill.fullRestockOnAssign = true;
            fortifySkill.rechargeStock = 1;
            fortifySkill.requiredStock = 1;
            fortifySkill.stockToConsume = 1;
            fortifySkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityFortify.png");
            fortifySkill.activationStateMachineName = "Slide";
            fortifySkill.keywordTokens = new string[] { "KEYWORD_MORIYAHANDOVERCLOCKED_BOOST" };//HANDSurvivor.HAND_PREFIX + "KEYWORD_SPRINGY"
            Modules.Skills.FixScriptableObjectName(fortifySkill);
            Modules.ContentPacks.skillDefs.Add(fortifySkill);
            SkillDefs.UtilityFortify = fortifySkill;

            SkillDef ovcCancelDef = SkillDef.CreateInstance<SkillDef>();
            ovcCancelDef.activationState = new SerializableEntityStateType(typeof(EntityStates.RMOR.Utility.CancelOverclock));
            ovcCancelDef.activationStateMachineName = "Slide";
            ovcCancelDef.baseMaxStock = 1;
            ovcCancelDef.baseRechargeInterval = 7f;
            ovcCancelDef.beginSkillCooldownOnSkillEnd = true;
            ovcCancelDef.canceledFromSprinting = false;
            ovcCancelDef.dontAllowPastMaxStocks = true;
            ovcCancelDef.forceSprintDuringState = false;
            ovcCancelDef.fullRestockOnAssign = true;
            ovcCancelDef.icon = RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityOverclockCancel.png");
            ovcCancelDef.interruptPriority = InterruptPriority.Skill;
            ovcCancelDef.isCombatSkill = false;
            ovcCancelDef.keywordTokens = new string[] { };
            ovcCancelDef.mustKeyPress = true;
            ovcCancelDef.cancelSprintingOnActivation = false;
            ovcCancelDef.rechargeStock = 1;
            ovcCancelDef.requiredStock = 0;
            ovcCancelDef.skillName = "CancelOverclock";
            ovcCancelDef.skillNameToken = RMORMod.Content.RMORSurvivor.RMORSurvivor.RMOR_PREFIX + "UTILITY_CANCEL_NAME";
            ovcCancelDef.skillDescriptionToken = RMORMod.Content.RMORSurvivor.RMORSurvivor.RMOR_PREFIX + "UTILITY_CANCEL_DESC";
            ovcCancelDef.stockToConsume = 0;
            Modules.Skills.FixScriptableObjectName(ovcCancelDef);
            RMORMod.Modules.ContentPacks.skillDefs.Add(ovcCancelDef);
            SkillDefs.UtilityOverclockCancel = ovcCancelDef;

            EntityStates.RMOR.Utility.BeginOverclock.overlayMaterial = UnityEngine.Material.Instantiate(LegacyResourcesAPI.Load<Material>("Materials/matWolfhatOverlay"));

            EntityStates.RMOR.Utility.BeginOverclock.texGauge = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGauge.png");
            EntityStates.RMOR.Utility.BeginOverclock.texGaugeArrow = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGaugeArrow.png");
            RMORMod.Content.Shared.Components.Body.OverclockController.ovcDef = ovcSkill;

            SkillDef focusSkill = SkillDef.CreateInstance<SkillDef>();
            focusSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.RMOR.Utility.BeginFocus));
            focusSkill.skillNameToken = RMORMod.Content.RMORSurvivor.RMORSurvivor.RMOR_PREFIX + "UTILITY_NEMESIS_NAME";
            focusSkill.skillName = "BeginFocus";
            focusSkill.skillDescriptionToken = RMORMod.Content.RMORSurvivor.RMORSurvivor.RMOR_PREFIX + "UTILITY_NEMESIS_DESC";
            focusSkill.isCombatSkill = false;
            focusSkill.cancelSprintingOnActivation = false;
            focusSkill.canceledFromSprinting = false;
            focusSkill.baseRechargeInterval = 12f;
            focusSkill.interruptPriority = EntityStates.InterruptPriority.Any;
            focusSkill.mustKeyPress = true;
            focusSkill.beginSkillCooldownOnSkillEnd = false;
            focusSkill.baseMaxStock = 1;
            focusSkill.fullRestockOnAssign = true;
            focusSkill.rechargeStock = 1;
            focusSkill.requiredStock = 1;
            focusSkill.stockToConsume = 1;
            focusSkill.icon = RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityNemesis.png");
            focusSkill.activationStateMachineName = "Slide";
            focusSkill.keywordTokens = new string[] { "KEYWORD_MORIYAHANDOVERCLOCKED_BOOST" };//HANDSurvivor.HAND_PREFIX + "KEYWORD_SPRINGY"
            Modules.Skills.FixScriptableObjectName(focusSkill);
            RMORMod.Modules.ContentPacks.skillDefs.Add(focusSkill);
            SkillDefs.UtilityFocus = focusSkill;

            EntityStates.RMOR.Utility.BeginFocus.overlayMaterial = UnityEngine.Material.Instantiate(LegacyResourcesAPI.Load<Material>("Materials/matFullCrit"));
            EntityStates.RMOR.Utility.BeginFortify.overlayMaterial = UnityEngine.Material.Instantiate(LegacyResourcesAPI.Load<Material>("Materials/matFullCrit"));

            EntityStates.RMOR.Utility.BeginFocus.texGaugeNemesis = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGaugeNemesis.png");
            EntityStates.RMOR.Utility.BeginFocus.texGaugeArrowNemesis = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGaugeArrowNemesis.png");

            EntityStates.RMOR.Utility.BeginFortify.texGaugeFortify = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGaugeFortify.png");
            EntityStates.RMOR.Utility.BeginFortify.texGaugeArrowFortify = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGaugeArrowFortify.png");
        }
    }
}
