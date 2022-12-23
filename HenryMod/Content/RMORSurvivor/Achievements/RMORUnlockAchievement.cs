using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace RMORMod.Content.RMOR.Achievements
{
	[RegisterAchievement("MoriyaRMORSurvivorUnlock", "Characters.RMOR", null, null)]
	public class RMORSurvivorUnlockAchievement : BaseAchievement
	{
        public override void OnInstall()
        {
            base.OnInstall();

            RoR2Application.onUpdate += this.CheckArmor;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            RoR2Application.onUpdate -= this.CheckArmor;
        }

        public void CheckArmor()
        {
            if (base.localUser != null && base.localUser.cachedBody != null && base.localUser.cachedBody.armor >= 1000f)
            {
                base.Grant();
            }
        }
    }
}
