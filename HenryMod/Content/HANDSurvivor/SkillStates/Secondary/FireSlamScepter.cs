using HANDMod.Content;
using R2API;

namespace EntityStates.HAND_Overclocked.Secondary
{
    public class FireSlamScepter : FireSlam
    {
        public override void ModifyStats()
        {
            this.bonusForce *= 1.5f;
            this.damageCoefficient *= 1.5f;
        }

        public override void ModifyDamageTypes()
        {
            this.attack.AddModdedDamageType(DamageTypes.HANDSecondaryScepter);
            this.attack.AddModdedDamageType(DamageTypes.SquashOnKill);
            this.attack.AddModdedDamageType(DamageTypes.ResetVictimForce);
        }
    }
}
