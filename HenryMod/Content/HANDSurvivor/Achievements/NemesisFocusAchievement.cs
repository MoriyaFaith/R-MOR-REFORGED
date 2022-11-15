using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace HANDMod.Content.HANDSurvivor.Achievements
{
    [RegisterAchievement("MoffeinHANDOverclockedNemesisFocusUnlock", "Skills.HANDOverclocked.NemesisFocus", null, null)]
    public class HANDOverclockedNemesisFocusUnlockAchievement : BaseAchievement
	{
		public override void OnInstall()
		{
			base.OnInstall();
			EntityStates.HAND_Overclocked.Utility.BeginOverclock.onAuthorityFixedUpdateGlobal += CheckOverclockTime;
		}

		public override void OnUninstall()
		{
			EntityStates.HAND_Overclocked.Utility.BeginOverclock.onAuthorityFixedUpdateGlobal -= CheckOverclockTime;
			base.OnUninstall();
		}

		private void CheckOverclockTime(EntityStates.HAND_Overclocked.Utility.BeginOverclock state)
        {
			if (state.outer.commonComponents.characterBody == base.localUser.cachedBody)
			{
				if (state.fixedAge > 60f)
				{
					base.Grant();
				}
			}
		}
    }
}
