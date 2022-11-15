using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace HANDMod.Content.HANDSurvivor.CharacterUnlock
{
    public class BrokenJanitorInteractable
    {
        public static GameObject interactablePrefab;
        private static SceneDef rallypointSceneDef = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/frozenwall/frozenwall.asset").WaitForCompletion();


        public static bool initialized = false;
        public static void Initialize()
        {
            if (initialized) return;
            initialized = true;
            interactablePrefab = BuildPrefab();
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
                    interactable.transform.position = new Vector3(2.3f, 10.7f, 12.0f);
                    interactable.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                    NetworkServer.Spawn(interactable);
                }
            }
        }

        private static GameObject BuildPrefab()
        {
            GameObject gameObject = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlHANDOverclocked").InstantiateClone("BrokenJanitorInteractable", false);
            Modules.Assets.ConvertAllRenderersToHopooShader(gameObject);

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
            //pi.onPurchase = 

            ModelLocator ml = gameObject.AddComponent<ModelLocator>();
            ml.modelTransform = gameObject.transform;

            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.BrokenJanitor.BrokenJanitorMain));
            Modules.ContentPacks.entityStates.Add(typeof(EntityStates.HAND_Overclocked.BrokenJanitor.BrokenJanitorActivate));
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
}

namespace EntityStates.HAND_Overclocked.BrokenJanitor
{
    public class BrokenJanitorMain : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("Body", "UnlockIdle", "Unlock.playbackRate", 0.1f);
        }
    }
    public class BrokenJanitorActivate : BaseState
    {
        public static float baseDuration = 1.5f;
        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("Body", "UnlockActivate", "Unlock.playbackRate", baseDuration);
        }
    }
}
