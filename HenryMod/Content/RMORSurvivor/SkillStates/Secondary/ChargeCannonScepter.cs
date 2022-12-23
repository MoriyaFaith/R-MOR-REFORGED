namespace EntityStates.RMOR.Secondary
{
    public class ChargeCannonScepter : ChargeCannon
    {
        public override void ModifyStats()
        {
            maxChargeLevel = 4;
            baseDuration = 1.0f;
        }
    }
}
