using EntityStates;
using HANDMod.Content.Shared;
using HANDMod.Modules;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HANDMod.Content.Shared
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
            ovcSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.HAND_Overclocked.Utility.BeginOverclock));
            ovcSkill.skillNameToken = HANDMod.Content.HANDSurvivor.HANDSurvivor.HAND_PREFIX + "UTILITY_NAME";
            ovcSkill.skillName = "BeginOverclock";
            ovcSkill.skillDescriptionToken = HANDMod.Content.HANDSurvivor.HANDSurvivor.HAND_PREFIX + "UTILITY_DESC";
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
            Modules.Skills.FixScriptableObjectName(ovcSkill);
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
            ovcCancelDef.icon = HANDMod.Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityOverclockCancel.png");
            ovcCancelDef.interruptPriority = InterruptPriority.Skill;
            ovcCancelDef.isCombatSkill = false;
            ovcCancelDef.keywordTokens = new string[] { };
            ovcCancelDef.mustKeyPress = true;
            ovcCancelDef.cancelSprintingOnActivation = false;
            ovcCancelDef.rechargeStock = 1;
            ovcCancelDef.requiredStock = 0;
            ovcCancelDef.skillName = "CancelOverclock";
            ovcCancelDef.skillNameToken = HANDMod.Content.HANDSurvivor.HANDSurvivor.HAND_PREFIX + "UTILITY_CANCEL_NAME";
            ovcCancelDef.skillDescriptionToken = HANDMod.Content.HANDSurvivor.HANDSurvivor.HAND_PREFIX + "UTILITY_CANCEL_DESC";
            ovcCancelDef.stockToConsume = 0;
            Modules.Skills.FixScriptableObjectName(ovcCancelDef);
            HANDMod.Modules.ContentPacks.skillDefs.Add(ovcCancelDef);
            SkillDefs.UtilityOverclockCancel = ovcCancelDef;

            EntityStates.HAND_Overclocked.Utility.BeginOverclock.overlayMaterial = UnityEngine.Material.Instantiate(LegacyResourcesAPI.Load<Material>("Materials/matWolfhatOverlay"));

            EntityStates.HAND_Overclocked.Utility.BeginOverclock.texGauge = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGauge.png");
            EntityStates.HAND_Overclocked.Utility.BeginOverclock.texGaugeArrow = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGaugeArrow.png");
            HANDMod.Content.Shared.Components.Body.OverclockController.ovcDef = ovcSkill;

            SkillDef focusSkill = SkillDef.CreateInstance<SkillDef>();
            focusSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.HAND_Overclocked.Utility.BeginFocus));
            focusSkill.skillNameToken = HANDMod.Content.HANDSurvivor.HANDSurvivor.HAND_PREFIX + "UTILITY_NEMESIS_NAME";
            focusSkill.skillName = "BeginFocus";
            focusSkill.skillDescriptionToken = HANDMod.Content.HANDSurvivor.HANDSurvivor.HAND_PREFIX + "UTILITY_NEMESIS_DESC";
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
            focusSkill.icon = HANDMod.Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityNemesis.png");
            focusSkill.activationStateMachineName = "Slide";
            focusSkill.keywordTokens = new string[] { };//HANDSurvivor.HAND_PREFIX + "KEYWORD_SPRINGY"
            Modules.Skills.FixScriptableObjectName(focusSkill);
            HANDMod.Modules.ContentPacks.skillDefs.Add(focusSkill);
            SkillDefs.UtilityFocus = focusSkill;

            EntityStates.HAND_Overclocked.Utility.BeginFocus.overlayMaterial = UnityEngine.Material.Instantiate(LegacyResourcesAPI.Load<Material>("Materials/matFullCrit"));

            EntityStates.HAND_Overclocked.Utility.BeginFocus.texGaugeNemesis = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGaugeNemesis.png");
            EntityStates.HAND_Overclocked.Utility.BeginFocus.texGaugeArrowNemesis = Assets.mainAssetBundle.LoadAsset<Texture2D>("texGaugeArrowNemesis.png");
        }
    }
}
