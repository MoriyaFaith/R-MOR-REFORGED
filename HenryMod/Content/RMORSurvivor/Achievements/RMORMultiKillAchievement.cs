using RoR2;
using RoR2.Achievements;
using UnityEngine;
using System;

namespace RMORMod.Content.RMOR.Achievements
{
    [RegisterAchievement("MoriyaRMORSlashUnlock", "Skills.RMOR.SlashAttack", null, null)]
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
        private static readonly int requirement = 20;
        private class MageMultiKillServerAchievement : BaseServerAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                RoR2Application.onFixedUpdate += this.OnFixedUpdate;
            }
            public override void OnUninstall()
            {
                RoR2Application.onFixedUpdate -= this.OnFixedUpdate;
                base.OnUninstall();
            }
            private void OnFixedUpdate()
            {
                CharacterBody currentBody = base.GetCurrentBody();
                if (currentBody && RMORMultiKillAchievement.requirement <= currentBody.multiKillCount)
                {
                    base.Grant();
                }
            }
        }
    }
}
