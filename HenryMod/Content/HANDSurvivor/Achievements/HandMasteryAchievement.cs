using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace HAND_Overclocked.Content.HANDSurvivor.Achievements
{
    [RegisterAchievement("MoffeinHANDOverclockedClearGameMonsoon", "Skins.HANDOverclocked.Mastery", "MoffeinHANDOverclockedSurvivorUnlock", null)]
    public class HandMasteryAchievement : BasePerSurvivorClearGameMonsoonAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("HANDOverclockedBody");
        }
    }
}
