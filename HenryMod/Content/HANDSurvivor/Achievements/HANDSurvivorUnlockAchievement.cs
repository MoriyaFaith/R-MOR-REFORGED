using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace HAND_Overclocked.Content.HANDSurvivor.Achievements
{
	[RegisterAchievement("MoffeinHANDOverclockedSurvivorUnlock", "Characters.HANDOverclocked", null, typeof(HANDOverclockedSurvivorUnlockServerAchievement))]
	public class HANDOverclockedSurvivorUnlockAchievement : BaseAchievement
	{
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		private class HANDOverclockedSurvivorUnlockServerAchievement : BaseServerAchievement
        {
			public override void OnInstall()
			{
				base.OnInstall();
                EntityStates.HAND_Overclocked.BrokenJanitor.BrokenJanitorMain.onBrokenJanitorPurchaseGlobal += onPurchasedJanitor;
			}

            private void onPurchasedJanitor(EntityStates.HAND_Overclocked.BrokenJanitor.BrokenJanitorMain state)
			{
				CharacterBody currentBody = this.serverAchievementTracker.networkUser.GetCurrentBody();
				if (currentBody && currentBody.GetComponent<Interactor>() == state.activator)
				{
					base.Grant();
				}
			}

            public override void OnUninstall()
			{
				EntityStates.HAND_Overclocked.BrokenJanitor.BrokenJanitorMain.onBrokenJanitorPurchaseGlobal -= onPurchasedJanitor;
				base.OnInstall();
			}
		}
	}
}
