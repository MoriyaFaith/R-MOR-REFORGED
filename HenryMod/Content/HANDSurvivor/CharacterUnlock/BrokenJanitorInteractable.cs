using HANDMod.Content.HANDSurvivor.CharacterUnlock;
using R2API;
using RoR2;
using RoR2.Hologram;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace HANDMod.Content.HANDSurvivor.CharacterUnlock
{
    public class BrokenJanitorInteractable
    {
        public static GameObject interactablePrefab;
        public static GameObject repairPrefab;
        private static SceneDef rallypointSceneDef = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/frozenwall/frozenwall.asset").WaitForCompletion();


        public static bool initialized = false;
        public static void Initialize()
        {
            if (initialized) return;
            initialized = true;

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.BrokenJanitor.BrokenJanitorMain));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.BrokenJanitor.BrokenJanitorActivate));

            interactablePrefab = BuildPrefab();
            repairPrefab = BuildRepairPrefab();
            On.RoR2.Stage.Start += SpawnInteractable;
        }

        private static void SpawnInteractable(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);

            if (NetworkServer.active)
            {
                SceneDef currentScene = SceneCatalog.GetSceneDefForCurrentScene();
                if (currentScene == rallypointSceneDef)
                {
                    GameObject interactable = UnityEngine.Object.Instantiate(HANDMod.Content.HANDSurvivor.CharacterUnlock.BrokenJanitorInteractable.interactablePrefab);
                    interactable.transform.position = new Vector3(41.92087f, 5f, 87.45225f);
                    interactable.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    NetworkServer.Spawn(interactable);
                }
            }
        }

        private static GameObject BuildPrefab()
        {
            GameObject gameObject = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlHANDOverclocked").InstantiateClone("BrokenJanitorInteractable", false);
            Modules.Assets.ConvertAllRenderersToHopooShader(gameObject);

            Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                c.isTrigger = true;
            }

            gameObject.layer = LayerIndex.CommonMasks.interactable.value;
            SphereCollider interactionCollider = gameObject.AddComponent<SphereCollider>();
            interactionCollider.isTrigger = true;
            interactionCollider.radius = 3f;

            NetworkIdentity net = gameObject.AddComponent<NetworkIdentity>();
            gameObject.RegisterNetworkPrefab();
            Modules.ContentPacks.networkedObjectPrefabs.Add(gameObject);

            Highlight highlight = gameObject.AddComponent<Highlight>();

            SkinnedMeshRenderer[] smr = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            highlight.targetRenderer = smr[1];  //HANDMesh
            highlight.strength = 1f;
            highlight.highlightColor = Highlight.HighlightColor.interactive;
            highlight.isOn = false;

            PurchaseInteraction pi = gameObject.AddComponent<PurchaseInteraction>();
            pi.displayNameToken = "LOCKEDTREEBOT_NAME";
            pi.contextToken = "LOCKEDTREEBOT_CONTEXT";
            pi.costType = CostTypeIndex.VolatileBattery;
            pi.available = true;
            pi.cost = 1;
            pi.automaticallyScaleCostWithDifficulty = false;
            pi.ignoreSpherecastForInteractability = false;
            pi.setUnavailableOnTeleporterActivated = false;
            pi.isShrine = false;
            pi.isGoldShrine = false;

            ModelLocator ml = gameObject.AddComponent<ModelLocator>();
            ml.modelTransform = gameObject.transform;

            EntityStateMachine esm = gameObject.AddComponent<EntityStateMachine>();
            esm.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.HAND_Overclocked.BrokenJanitor.BrokenJanitorMain));
            esm.initialStateType = esm.mainStateType;

            NetworkStateMachine nsm = gameObject.AddComponent<NetworkStateMachine>();
            nsm.stateMachines = new EntityStateMachine[] { esm };

            EntityLocator el = gameObject.AddComponent<EntityLocator>();
            el.entity = gameObject;

            return gameObject;
        }

        private static GameObject BuildRepairPrefab()
        {
            GameObject gameObject = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlHANDOverclocked").InstantiateClone("BrokenJanitorRepair", false);
            Modules.Assets.ConvertAllRenderersToHopooShader(gameObject);

            Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                c.isTrigger = true;
            }

            gameObject.layer = LayerIndex.CommonMasks.interactable.value;
            SphereCollider interactionCollider = gameObject.AddComponent<SphereCollider>();
            interactionCollider.isTrigger = true;
            interactionCollider.radius = 3f;

            NetworkIdentity net = gameObject.AddComponent<NetworkIdentity>();
            gameObject.RegisterNetworkPrefab();
            Modules.ContentPacks.networkedObjectPrefabs.Add(gameObject);

            Highlight highlight = gameObject.AddComponent<Highlight>();

            SkinnedMeshRenderer[] smr = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            highlight.targetRenderer = smr[1];  //HANDMesh
            highlight.strength = 1f;
            highlight.highlightColor = Highlight.HighlightColor.interactive;
            highlight.isOn = false;

            PurchaseInteraction pi = gameObject.AddComponent<PurchaseInteraction>();
            pi.displayNameToken = "LOCKEDTREEBOT_NAME";
            pi.contextToken = "LOCKEDTREEBOT_CONTEXT";
            pi.costType = CostTypeIndex.Money;
            pi.available = true;
            pi.cost = 200;
            pi.automaticallyScaleCostWithDifficulty = true;
            pi.ignoreSpherecastForInteractability = false;
            pi.setUnavailableOnTeleporterActivated = false;
            pi.isShrine = false;
            pi.isGoldShrine = false;

            ModelLocator ml = gameObject.AddComponent<ModelLocator>();
            ml.modelTransform = gameObject.transform;

            ChildLocator cl = gameObject.GetComponent<ChildLocator>();
            HologramProjector hl = gameObject.AddComponent<HologramProjector>();
            hl.displayDistance = 15f;
            hl.disableHologramRotation = false;
            hl.hologramPivot = cl.FindChild("HologramPivot");

            EntityStateMachine esm = gameObject.AddComponent<EntityStateMachine>();
            esm.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.HAND_Overclocked.BrokenJanitor.BrokenJanitorMain));
            esm.initialStateType = esm.mainStateType;

            NetworkStateMachine nsm = gameObject.AddComponent<NetworkStateMachine>();
            nsm.stateMachines = new EntityStateMachine[] { esm };

            EntityLocator el = gameObject.AddComponent<EntityLocator>();
            el.entity = gameObject;

            return gameObject;
        }
    }

    public class CreateRepairOnDeath : MonoBehaviour
    {
        private HealthComponent healthComponent;
        private bool spawnedRepair = false;
        public void Awake()
        {
            healthComponent = base.GetComponent<HealthComponent>();
        }

        public void FixedUpdate()
        {
            if (NetworkServer.active && !spawnedRepair && !(healthComponent && healthComponent.alive))
            {
                spawnedRepair = true;
                GameObject interactable = UnityEngine.Object.Instantiate(HANDMod.Content.HANDSurvivor.CharacterUnlock.BrokenJanitorInteractable.repairPrefab);

                Vector3 pos = FindSafeTeleportPosition(base.gameObject, base.transform.position);
                interactable.transform.position = pos;
                interactable.transform.rotation = base.transform.rotation;
                NetworkServer.Spawn(interactable);
            }
        }
        public static Vector3 FindSafeTeleportPosition(GameObject gameObject, Vector3 targetPosition)
        {
            return FindSafeTeleportPosition(gameObject, targetPosition, float.NegativeInfinity, float.NegativeInfinity);
        }

        public static Vector3 FindSafeTeleportPosition(GameObject gameObject, Vector3 targetPosition, float idealMinDistance, float idealMaxDistance)
        {
            Vector3 vector = targetPosition;
            SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
            spawnCard.hullSize = HullClassification.Human;
            spawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            spawnCard.prefab = LegacyResourcesAPI.Load<GameObject>("SpawnCards/HelperPrefab");
            Vector3 result = vector;
            GameObject teleportGameObject = null;
            if (idealMaxDistance > 0f && idealMinDistance < idealMaxDistance)
            {
                teleportGameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                    minDistance = idealMinDistance,
                    maxDistance = idealMaxDistance,
                    position = vector
                }, RoR2Application.rng));
            }
            if (!teleportGameObject)
            {
                teleportGameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                    position = vector
                }, RoR2Application.rng));
                if (teleportGameObject)
                {
                    result = teleportGameObject.transform.position;
                }
            }
            if (teleportGameObject)
            {
                UnityEngine.Object.Destroy(teleportGameObject);
            }
            UnityEngine.Object.Destroy(spawnCard);
            return result;
        }
    }
}

namespace EntityStates.HAND_Overclocked.BrokenJanitor
{
    public class BrokenJanitorMain : BaseState
    {
        public static event Action<BrokenJanitorMain> onBrokenJanitorPurchaseGlobal;
        public Interactor activator;
        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("Body", "UnlockIdle", "Unlock.playbackRate", 0.1f);

            PurchaseInteraction purchaseInteraction = base.GetComponent<PurchaseInteraction>();
            if (purchaseInteraction)
            {
                purchaseInteraction.onPurchase.AddListener(new UnityAction<Interactor>(this.DoOnPurchase));
            }

        }
        private void DoOnPurchase(Interactor activator)
        {
            this.activator = activator;
            onBrokenJanitorPurchaseGlobal?.Invoke(this);

            this.outer.SetNextState(new BrokenJanitorActivate() { activator = activator.gameObject });
            return;
        }
    }
    public class BrokenJanitorActivate : BaseState
    {
        public GameObject activator;
        public static float spawnDelay = 0.25f;
        public static float baseDuration = 2f;
        private bool spawned = false;
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_HOC_StartHammer", base.gameObject);
            base.PlayAnimation("Body", "UnlockActivate", "Unlock.playbackRate", baseDuration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (NetworkServer.active)
            {
                if (!spawned && base.fixedAge >= spawnDelay)
                {
                    SpawnAllyServer();
                }

                if (base.fixedAge >= baseDuration)
                {
                    Destroy(base.gameObject);
                    return;
                }
            }
        }

        private void SpawnAllyServer()
        {
            spawned = true;
            if (activator)
            {
                MasterSummon ms = new MasterSummon
                {
                    useAmbientLevel = true,
                    ignoreTeamMemberLimit = true,
                    masterPrefab = HANDMod.Content.HANDSurvivor.MasterAI.HANDMaster,
                    position = base.transform.position,
                    rotation = base.transform.rotation,
                    summonerBodyObject = activator.gameObject
                };

                CharacterMaster minionMaster = ms.Perform();
                if (minionMaster)
                {
                    if (minionMaster.loadout != null)
                    {
                        minionMaster.loadout.bodyLoadoutManager.SetSkillVariant(BodyCatalog.FindBodyIndex("HANDOverclockedBody"), (int)SkillSlot.Primary, 1);
                        CharacterBody minionBody = minionMaster.GetBody();
                        if (minionBody)
                        {
                            minionBody.gameObject.AddComponent<CreateRepairOnDeath>();
                            minionBody.SetLoadoutServer(minionMaster.loadout);
                        }
                    }

                    SetDontDestroyOnLoad.DontDestroyOnLoad(minionMaster);
                    Inventory inventory = minionMaster.inventory;
                    if (inventory)
                    {
                        inventory.GiveItem(RoR2Content.Items.BoostHp, 50);
                        inventory.GiveItem(RoR2Content.Items.BoostDamage, 20);

                        ItemIndex riskyModAllyMarker = ItemCatalog.FindItemIndex("RiskyModAllyMarkerItem");
                        if (riskyModAllyMarker != ItemIndex.None)
                        {
                            inventory.GiveItem(riskyModAllyMarker);
                        }

                        ItemIndex riskyModAllyScaling = ItemCatalog.FindItemIndex("RiskyModAllyScalingItem");
                        if (riskyModAllyScaling != ItemIndex.None)
                        {
                            inventory.GiveItem(riskyModAllyScaling);
                        }

                        ItemIndex riskyModAllyRegen = ItemCatalog.FindItemIndex("RiskyModAllyRegenItem");
                        if (riskyModAllyRegen != ItemIndex.None)
                        {
                            inventory.GiveItem(riskyModAllyRegen, 7);   // 40 / 6 because it only counts base health.
                        }
                    }
                }
            }
        }
    }
}
