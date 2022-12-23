using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace RMORMod.Content.RMOR.Achievements
{
    [RegisterAchievement("MoriyaRMOROverclockUnlock", "Skills.RMOR.Overclock", null, null)]
    public class OverclockAchievement : BaseAchievement
	{
        public override void OnInstall()
        {
            base.OnInstall();

            RoR2Application.onUpdate += this.CheckMovementSpeed;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            RoR2Application.onUpdate -= this.CheckMovementSpeed;
        }

        public void CheckMovementSpeed()
        {
            if (base.localUser != null && base.localUser.cachedBody != null && base.localUser.cachedBody.baseMoveSpeed >= 2f && base.meetsBodyRequirement)
            {
                base.Grant();
            }
        }
    }
}
