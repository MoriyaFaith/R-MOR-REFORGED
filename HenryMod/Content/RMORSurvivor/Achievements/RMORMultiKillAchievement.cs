using RoR2;
using RoR2.Achievements;
using UnityEngine;
using System;

namespace RMORMod.Content.RMOR.Achievements
{
    [RegisterAchievement("MoriyaRMORSlashUnlock", "Skills.RMOR.SlashAttack", null, typeof(RMORMultiKillAchievement.RMORMultikillServerAchievement))]
    public class RMORMultiKillAchievement : BaseAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("RMORBody");
        }
        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            base.SetServerTracked(true);
        }
        public override void OnBodyRequirementBroken()
        {
            base.SetServerTracked(false);
            base.OnBodyRequirementBroken();
        }
        private class RMORMultikillServerAchievement : BaseServerAchievement
        { 
            public override void OnInstall()
            {
                base.OnInstall();
                GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
            }
            public override void OnUninstall()
            {
                GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
                base.OnUninstall();
            }
            private void OnCharacterDeath(DamageReport damageReport)
            {
                GameObject attacker = damageReport.damageInfo.attacker;
                if (!attacker)
                {
                    return;
                }
                CharacterBody component = attacker.GetComponent<CharacterBody>();
                if (!component)
                {
                    return;
                }
                if (component.multiKillCount >= multiKillThreshold && component.masterObject == this.serverAchievementTracker.networkUser.masterObject)
                {
                    base.Grant();
                }
            }
            private const int multiKillThreshold = 20;
        }
    }
}
