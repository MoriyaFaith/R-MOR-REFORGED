using RoR2;
using UnityEngine;
using RoR2.CharacterAI;
using R2API;

namespace HANDMod.Content.HANDSurvivor
{
    public class MasterAI
    {
        private static bool initialized = false;
        public static GameObject HANDMaster;
        public static void Init(GameObject bodyPrefab)
        {
            if (initialized) return;
            initialized = true;

            GameObject masterObject = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/commandomonstermaster"), "HANDOverclockedMonsterMaster", true);
            Modules.ContentPacks.masterPrefabs.Add(masterObject);

            CharacterMaster cm = masterObject.GetComponent<CharacterMaster>();
            cm.bodyPrefab = bodyPrefab;

            Component[] toDelete = masterObject.GetComponents<AISkillDriver>();
            foreach (AISkillDriver asd in toDelete)
            {
                UnityEngine.Object.Destroy(asd);
            }

            AISkillDriver specialSelfHeal = masterObject.AddComponent<AISkillDriver>();
            specialSelfHeal.skillSlot = SkillSlot.Special;
            specialSelfHeal.requireSkillReady = true;
            specialSelfHeal.requireEquipmentReady = false;
            specialSelfHeal.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            specialSelfHeal.minDistance = 0f;
            specialSelfHeal.maxDistance = float.PositiveInfinity;
            specialSelfHeal.selectionRequiresTargetLoS = false;
            specialSelfHeal.activationRequiresTargetLoS = false;
            specialSelfHeal.activationRequiresAimConfirmation = false;
            specialSelfHeal.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            specialSelfHeal.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            specialSelfHeal.ignoreNodeGraph = false;
            specialSelfHeal.driverUpdateTimerOverride = -1f;
            specialSelfHeal.noRepeat = false;
            specialSelfHeal.shouldSprint = true;
            specialSelfHeal.shouldFireEquipment = false;
            specialSelfHeal.buttonPressType = AISkillDriver.ButtonPressType.TapContinuous;
            specialSelfHeal.maxUserHealthFraction = 0.7f;

            AISkillDriver secondary = masterObject.AddComponent<AISkillDriver>();
            secondary.skillSlot = SkillSlot.Secondary;
            secondary.requireSkillReady = true;
            secondary.requireEquipmentReady = false;
            secondary.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            secondary.minDistance = 0f;
            secondary.maxDistance = 10f;
            secondary.selectionRequiresTargetLoS = true;
            secondary.activationRequiresTargetLoS = false;
            secondary.activationRequiresAimConfirmation = false;
            secondary.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            secondary.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            secondary.ignoreNodeGraph = false;
            secondary.driverUpdateTimerOverride = 1.5f;
            secondary.noRepeat = true;
            secondary.shouldSprint = true;
            secondary.shouldFireEquipment = false;
            secondary.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver primary = masterObject.AddComponent<AISkillDriver>();
            primary.skillSlot = SkillSlot.Primary;
            primary.requireSkillReady = false;
            primary.requireEquipmentReady = false;
            primary.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            primary.minDistance = 0f;
            primary.maxDistance = 7f;
            primary.selectionRequiresTargetLoS = true;
            primary.activationRequiresTargetLoS = false;
            primary.activationRequiresAimConfirmation = false;
            primary.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            primary.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            primary.ignoreNodeGraph = false;
            primary.driverUpdateTimerOverride = -1f;
            primary.noRepeat = false;
            primary.shouldSprint = true;
            primary.shouldFireEquipment = false;
            primary.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver utility = masterObject.AddComponent<AISkillDriver>();
            utility.skillSlot = SkillSlot.Utility;
            utility.requiredSkill = Content.Shared.SkillDefs.UtilityOverclock;
            utility.requireSkillReady = true;
            utility.requireEquipmentReady = false;
            utility.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            utility.minDistance = 0f;
            utility.maxDistance = 20f;
            utility.selectionRequiresTargetLoS = false;
            utility.activationRequiresTargetLoS = false;
            utility.activationRequiresAimConfirmation = false;
            utility.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            utility.aimType = AISkillDriver.AimType.AtMoveTarget;
            utility.ignoreNodeGraph = false;
            utility.driverUpdateTimerOverride = -1f;
            utility.noRepeat = true;
            utility.shouldSprint = true;
            utility.shouldFireEquipment = false;
            utility.buttonPressType = AISkillDriver.ButtonPressType.Abstain;

            AISkillDriver utilityFocus = masterObject.AddComponent<AISkillDriver>();
            utilityFocus.skillSlot = SkillSlot.Utility;
            utilityFocus.requiredSkill = Content.Shared.SkillDefs.UtilityFocus;
            utilityFocus.requireSkillReady = true;
            utilityFocus.requireEquipmentReady = false;
            utilityFocus.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            utilityFocus.minDistance = 0f;
            utilityFocus.maxDistance = 20f;
            utilityFocus.selectionRequiresTargetLoS = false;
            utilityFocus.activationRequiresTargetLoS = false;
            utilityFocus.activationRequiresAimConfirmation = false;
            utilityFocus.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            utilityFocus.aimType = AISkillDriver.AimType.AtMoveTarget;
            utilityFocus.ignoreNodeGraph = false;
            utilityFocus.driverUpdateTimerOverride = -1f;
            utilityFocus.noRepeat = true;
            utilityFocus.shouldSprint = true;
            utilityFocus.shouldFireEquipment = false;
            utilityFocus.buttonPressType = AISkillDriver.ButtonPressType.Abstain;

            AISkillDriver chase = masterObject.AddComponent<AISkillDriver>();
            chase.skillSlot = SkillSlot.None;
            chase.requireSkillReady = false;
            chase.requireEquipmentReady = false;
            chase.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chase.minDistance = 0f;
            chase.maxDistance = float.PositiveInfinity;
            chase.selectionRequiresTargetLoS = false;
            chase.activationRequiresTargetLoS = false;
            chase.activationRequiresAimConfirmation = false;
            chase.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chase.aimType = AISkillDriver.AimType.AtMoveTarget;
            chase.ignoreNodeGraph = false;
            chase.driverUpdateTimerOverride = -1f;
            chase.noRepeat = false;
            chase.shouldSprint = true;
            chase.shouldFireEquipment = false;

            AISkillDriver followFriendly = masterObject.AddComponent<AISkillDriver>();
            followFriendly.skillSlot = SkillSlot.None;
            followFriendly.requireSkillReady = false;
            followFriendly.requireEquipmentReady = false;
            followFriendly.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            followFriendly.minDistance = 0f;
            followFriendly.maxDistance = float.PositiveInfinity;
            followFriendly.selectionRequiresTargetLoS = false;
            followFriendly.activationRequiresTargetLoS = false;
            followFriendly.activationRequiresAimConfirmation = false;
            followFriendly.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            followFriendly.aimType = AISkillDriver.AimType.AtCurrentLeader;
            followFriendly.ignoreNodeGraph = false;
            followFriendly.driverUpdateTimerOverride = -1f;
            followFriendly.noRepeat = false;
            followFriendly.shouldSprint = true;
            followFriendly.shouldFireEquipment = false;
            followFriendly.maxUserHealthFraction = 0.6f;

            AISkillDriver afk = masterObject.AddComponent<AISkillDriver>();
            afk.skillSlot = SkillSlot.None;
            afk.requireSkillReady = false;
            afk.requireEquipmentReady = false;
            afk.moveTargetType = AISkillDriver.TargetType.NearestFriendlyInSkillRange;
            afk.minDistance = 0f;
            afk.maxDistance = float.PositiveInfinity;
            afk.selectionRequiresTargetLoS = false;
            afk.activationRequiresTargetLoS = false;
            afk.activationRequiresAimConfirmation = false;
            afk.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            afk.aimType = AISkillDriver.AimType.MoveDirection;
            afk.ignoreNodeGraph = false;
            afk.driverUpdateTimerOverride = -1f;
            afk.noRepeat = false;
            afk.shouldSprint = true;
            afk.shouldFireEquipment = false;

            HANDMaster = masterObject;
        }
    }
}
