using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HANDMod.Content.HANDSurvivor.Components.Body
{
    public class HANDNetworkComponent : NetworkBehaviour
    {
        private CharacterBody characterBody;
        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
        }

        [Server]
        public void ResetSpecialStock()
        {
            if (NetworkServer.active)
            {
                RpcResetSpecialStock();
            }
        }

        [ClientRpc]
        private void RpcResetSpecialStock()
        {
            characterBody.skillLocator.special.stock = 0;
        }

        [Server]
        public void SquashEnemy(uint networkID)
        {
            if (NetworkServer.active)
            {
                RpcAddSquash(networkID);
            }
        }

        [ClientRpc]
        private void RpcAddSquash(uint networkID)
        {
            GameObject go = ClientScene.FindLocalObject(new NetworkInstanceId(networkID));
            if (go)
            {
                CharacterMaster cm = go.GetComponent<CharacterMaster>();
                if (cm)
                {
                    GameObject bodyObject = cm.GetBodyObject();
                    if (bodyObject)
                    {
                        SquashedComponent sq = bodyObject.GetComponent<SquashedComponent>();
                        if (sq)
                        {
                            sq.ResetGraceTimer();
                        }
                        else
                        {
                            bodyObject.AddComponent<SquashedComponent>();
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            Util.PlaySound("Play_MULT_shift_end", base.gameObject);
        }
    }
}
