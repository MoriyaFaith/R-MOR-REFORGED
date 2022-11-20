using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HANDMod.Content.HANDSurvivor.Components.Body
{
    public class DroneFollowerController : NetworkBehaviour
    {
        private float sleepTimer = 1.5f;    //Don't render drones until 1.5s after spawn.
        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            if (NetworkServer.active)
            {
                _droneCountServer = 0;
            }

            Transform bodyDroneFollower = null;
            Renderer[] bodyDroneRenderers = null;
            MeshFilter[] bodyDroneMeshes = null;
            if (characterBody && characterBody.modelLocator && characterBody.modelLocator.modelTransform)
            {
                GameObject modelObject = characterBody.modelLocator.modelTransform.gameObject;
                if (modelObject)
                {
                    ChildLocator cl = modelObject.GetComponent<ChildLocator>();
                    if (cl)
                    {
                        bodyDroneFollower = cl.FindChild("DroneFollower");
                        if (bodyDroneFollower)
                        {
                            bodyDroneRenderers = bodyDroneFollower.GetComponentsInChildren<Renderer>();
                            bodyDroneMeshes = bodyDroneFollower.GetComponentsInChildren<MeshFilter>();
                        }
                    }
                }
            }
            //follower.GetComponentInChildren<Renderer>().material = characterModel.baseRendererInfos[2].defaultMaterial;
            //follower.GetComponentInChildren<MeshFilter>().mesh = characterModel.baseRendererInfos[2].renderer.GetComponent<MeshFilter>().mesh;

            droneFollowers = new DroneFollower[maxFollowingDrones];
            for (int i = 0; i < droneFollowers.Length; i++)
            {
                droneFollowers[i].gameObject = Instantiate(dronePrefab);
                droneFollowers[i].gameObject.transform.localScale = Vector3.zero;
                droneFollowers[i].active = false;


                if (bodyDroneRenderers != null && bodyDroneMeshes != null)
                {
                    Renderer[] instanceDroneRenderers = bodyDroneFollower.GetComponentsInChildren<Renderer>();
                    MeshFilter[] instanceDroneMeshes = bodyDroneFollower.GetComponentsInChildren<MeshFilter>();

                    if (instanceDroneRenderers.Length > 0)
                    {
                        for (int j = 0; j < bodyDroneRenderers.Length && j < instanceDroneRenderers.Length; j++)
                        {
                            instanceDroneRenderers[j].material = bodyDroneRenderers[j].material;
                        }
                    }

                    if (instanceDroneMeshes.Length > 0)
                    {
                        for (int j = 0; j < bodyDroneMeshes.Length && j < instanceDroneMeshes.Length; j++)
                        {
                            instanceDroneMeshes[j].mesh = bodyDroneMeshes[j].mesh;
                        }
                    }
                }
            }
        }

        public void FixedUpdate()
        {
            if (hasAuthority)
            {
                if (characterBody.skillLocator.special.stock != _droneCountServer)
                {
                    CmdUpdateDroneCount(characterBody.skillLocator.special.stock);
                }
            }
            if (sleepTimer > 0f)
            {
                sleepTimer -= Time.fixedDeltaTime;
            }
        }

        public void OnDestroy()
        {
            for (int i = 0; i < droneFollowers.Length; i++)
            {
                if (droneFollowers[i].gameObject)
                {
                    Destroy(droneFollowers[i].gameObject);
                }
            }
        }

        private void Update()
        {

            if (sleepTimer <= 0f)
            {
                UpdateMotion();
            }
            //base.transform.position += this.velocity * Time.fixedDeltaTime;   //Why is this here?

            stopwatch += Time.deltaTime * (characterBody.HasBuff(HANDMod.Content.Shared.Buffs.Overclock) ? 2f : 1f);
            if (stopwatch > orbitDuration)
            {
                stopwatch -= orbitDuration;
            }
        }

        private void UpdateMotion()
        {
            droneCount = hasAuthority ? characterBody.skillLocator.special.stock : _droneCountServer;
            for (int i = 0; i < maxFollowingDrones; i++)
            {
                if (i < droneCount)
                {
                    if (!droneFollowers[i].active)
                    {
                        EffectManager.SimpleEffect(activateEffect, droneFollowers[i].gameObject.transform.position, droneFollowers[i].gameObject.transform.rotation, false);
                    }
                    droneFollowers[i].active = true;
                    droneFollowers[i].gameObject.transform.localScale = droneScale * Vector3.one;
                    if (characterBody.modelLocator && characterBody.modelLocator.modelTransform)
                    {
                        droneFollowers[i].gameObject.transform.rotation = characterBody.modelLocator.modelTransform.rotation;
                    }
                }
                else
                {
                    if (droneFollowers[i].active)
                    {
                        EffectManager.SimpleEffect(deactivateEffect, droneFollowers[i].gameObject.transform.position, droneFollowers[i].gameObject.transform.rotation, false);
                    }
                    droneFollowers[i].active = false;
                    droneFollowers[i].gameObject.transform.localScale = Vector3.zero;
                }

                Vector3 offset = Quaternion.AngleAxis(360f / maxFollowingDrones * i + stopwatch / orbitDuration * 360f, Vector3.up) * Vector3.right * 2.4f;
                offset.y = 1.2f;

                Vector3 desiredPosition = characterBody.corePosition + offset;
                droneFollowers[i].gameObject.transform.position = Vector3.SmoothDamp(droneFollowers[i].gameObject.transform.position, desiredPosition, ref this.velocity, 0.1f);
            }
        }


        [Command]
        private void CmdUpdateDroneCount(int newCount)
        {
            _droneCountServer = newCount;
        }

        public static GameObject activateEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXLoader");
        public static GameObject deactivateEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXLoader");
        public static GameObject dronePrefab;
        private CharacterBody characterBody;
        private int droneCount;
        public static float droneScale = 1f;
        public static int maxFollowingDrones = 10;

        private Vector3 velocity = Vector3.zero;

        private float stopwatch;
        public static float orbitDuration = 6f;

        DroneFollower[] droneFollowers;

        [SyncVar]
        private int _droneCountServer;

        public struct DroneFollower
        {
            public GameObject gameObject;
            public bool active;
        }
    }
}
