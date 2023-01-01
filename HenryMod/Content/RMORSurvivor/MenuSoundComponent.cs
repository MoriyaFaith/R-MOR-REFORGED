using UnityEngine;
using RoR2;

namespace RMORMod.Content.RMORSurvivor
{
    public class MenuSoundComponent : MonoBehaviour
    {
        private void OnEnable()
        {
            Util.PlaySound("Play_RMOR_Boot", gameObject);
            ShakeEmitter se = ShakeEmitter.CreateSimpleShakeEmitter(gameObject.transform.position, new Wave()
            {
                amplitude = 1f,
                cycleOffset = 0f,
                frequency = 1f
            },
                0.25f, 5f, true);
            se.transform.parent = gameObject.transform;
        }
    }
}
