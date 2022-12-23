using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace RMORMod.Content.HANDSurvivor.Achievements
{
    [RegisterAchievement("MoffeinHANDOverclockedNemesisFocusUnlock", "Skills.HANDOverclocked.NemesisFocus", null, null)]
    public class HANDOverclockedNemesisFocusUnlockAchievement : BaseAchievement
	{
		public override void OnInstall()
		{
			base.OnInstall();
			EntityStates.RMOR.Utility.BeginOverclock.onAuthorityFixedUpdateGlobal += CheckOverclockTime;
		}

		public override void OnUninstall()
		{
			EntityStates.RMOR.Utility.BeginOverclock.onAuthorityFixedUpdateGlobal -= CheckOverclockTime;
			base.OnUninstall();
		}

		private void CheckOverclockTime(EntityStates.RMOR.Utility.BeginOverclock state)
        {
			if (state.outer.commonComponents.characterBody && state.outer.commonComponents.characterBody == base.localUser.cachedBody)
			{
				if (state.fixedAge >= 45f)
				{
					base.Grant();
				}
			}
		}
    }
}
