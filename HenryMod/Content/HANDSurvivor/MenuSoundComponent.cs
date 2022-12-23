using UnityEngine;
using RoR2;

namespace RMORMod.Content.HANDSurvivor
{
    public class MenuSoundComponent : MonoBehaviour
    {
        private void OnEnable()
        {
            Util.PlaySound("Play_HOC_StartHammer", base.gameObject);
        }
    }
}
