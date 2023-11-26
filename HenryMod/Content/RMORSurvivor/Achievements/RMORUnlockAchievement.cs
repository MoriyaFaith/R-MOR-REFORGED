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

            On.RoR2.CharacterBody.RecalculateStats += this.CheckArmor;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            On.RoR2.CharacterBody.RecalculateStats -= this.CheckArmor;
        }

        private void CheckArmor(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self && base.localUser.cachedBody && base.localUser.cachedBody == self) // check if local player body is the body recalculating its stats here
            {
                if (self.armor >= 1000f)
                {
                    base.Grant();
                }
            }
        }
    }
}
