using RoR2;
using RoR2.Achievements;
using UnityEngine;
using System;

namespace RMORMod.Content.RMOR.Achievements
{
    [RegisterAchievement("MoriyaRMORSlashUnlock", "Skills.RMOR.SlashAttack", null, null)]
    public class RMORMultiKillAchievement : BaseAchievement
    {
        public override void OnInstall()
        {
            base.OnInstall();
            RoR2Application.onFixedUpdate += CheckMultikillCount;
        }

        public override void OnUninstall()
        {
            RoR2Application.onFixedUpdate -= CheckMultikillCount;
            base.OnUninstall();
        }

        private void CheckMultikillCount()
        {
            if (base.localUser != null && base.localUser.cachedBody != null && base.localUser.cachedBody.multiKillCount > 20 && base.meetsBodyRequirement)
            {
                base.Grant();
            }
        }
    }
}
