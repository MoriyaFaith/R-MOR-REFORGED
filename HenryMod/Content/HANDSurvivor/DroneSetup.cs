using RoR2;
using UnityEngine;
using RoR2.Projectile;
using R2API;
using EntityStates.HAND_Overclocked.Special;
using HANDMod.Content.HANDSurvivor.Components.Body;
using UnityEngine.AddressableAssets;

namespace HANDMod.Content.HANDSurvivor
{
    //Copypasted the code from the original HAN-D Overclocked.
    public class DroneSetup
    {
        public static void Init()
        {
            if (!FireSeekingDrone.projectilePrefab) FireSeekingDrone.projectilePrefab = CreateDroneProjectile();
            if (!DroneFollowerController.dronePrefab) DroneFollowerController.dronePrefab = CreateDroneFollower();
            if (!TargetingController.allyIndicatorPrefab) TargetingController.allyIndicatorPrefab = CreateAllyIndicator();
            if (!TargetingController.enemyIndicatorPrefab) TargetingController.enemyIndicatorPrefab = CreateEnemyIndicator();
        }

        private static GameObject CreateAllyIndicator()
        {
            GameObject indicator = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/PassiveHealing/WoodSpriteIndicator.prefab").WaitForCompletion();
            UnityEngine.Object.Destroy(indicator.GetComponentInChildren<RoR2.InputBindingDisplayController>());
            UnityEngine.Object.Destroy(indicator.GetComponentInChildren<TMPro.TextMeshPro>());
            return indicator;
        }
        private static GameObject CreateEnemyIndicator()
        {
            GameObject indicator = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiMissileTrackingIndicator.prefab").WaitForCompletion();
            return indicator;
        }

        private static GameObject CreateDroneProjectile()
        {
            GameObject droneProjectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/EngiHarpoon").InstantiateClone("HANDOverclockedDroneProjectile", true);

            GameObject droneProjectileGhost = PrefabAPI.InstantiateClone(HANDMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("DronePrefab.prefab"), "HANDOverclockedDroneProjectileGhost", false);

            Shader hotpoo = LegacyResourcesAPI.Load<Shader>("Shaders/Deferred/hgstandard");

            MeshRenderer[] mr = droneProjectileGhost.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer m in mr)
            {
                if (m.name.ToLower() == "saw")
                {
                    m.material.shader = hotpoo;
                }
            }

            SkinnedMeshRenderer[] smr = droneProjectileGhost.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer m in smr)
            {
                m.material.shader = hotpoo;
            }

            droneProjectileGhost.AddComponent<ProjectileGhostController>();
            droneProjectileGhost.transform.localScale = 2f * Vector3.one;

            droneProjectileGhost.layer = LayerIndex.noCollision.intVal;

            droneProjectile.GetComponent<ProjectileController>().ghostPrefab = droneProjectileGhost;

            Material droneMat = Modules.Materials.CreateHopooMaterial("DroneBody");
            Modules.Materials.SetEmission(droneMat, 3f, Color.white);
            droneProjectileGhost.GetComponentInChildren<SkinnedMeshRenderer>().material = droneMat;

            Collider[] collidersG = droneProjectileGhost.GetComponentsInChildren<Collider>();
            foreach (Collider cG in collidersG)
            {
                UnityEngine.Object.Destroy(cG);
            }

            HANDMod.Modules.ContentPacks.projectilePrefabs.Add(droneProjectile);

            MissileController mc = droneProjectile.GetComponent<MissileController>();
            mc.maxVelocity = 25f;
            mc.acceleration = 3f;
            mc.maxSeekDistance = 160f;
            mc.giveupTimer = 20f;
            mc.deathTimer = 20f;

            UnityEngine.Object.Destroy(droneProjectile.GetComponent<AkGameObj>());
            UnityEngine.Object.Destroy(droneProjectile.GetComponent<AkEvent>());
            UnityEngine.Object.Destroy(droneProjectile.GetComponent<ProjectileSingleTargetImpact>());

            ProjectileStickOnImpact stick = droneProjectile.AddComponent<ProjectileStickOnImpact>();
            stick.ignoreWorld = true;
            stick.ignoreCharacters = false;
            stick.alignNormals = false;

            droneProjectile.AddComponent< Components.DroneProjectile.DroneDamageController>();
            droneProjectile.AddComponent<Components.DroneProjectile.PreventGroundCollision>();

            Collider[] colliders = droneProjectile.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                UnityEngine.Object.Destroy(c);
            }
            SphereCollider sc = droneProjectile.AddComponent<SphereCollider>();
            sc.radius = 0.6f;
            sc.contactOffset = 0.01f;

            return droneProjectile;
        }

        private static GameObject CreateDroneFollower()
        {
            Shader hotpoo = LegacyResourcesAPI.Load<Shader>("Shaders/Deferred/hgstandard");
            GameObject droneFollower = PrefabAPI.InstantiateClone(HANDMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("DronePrefab.prefab"), "HANDOverclockedDroneFollower", false);
            droneFollower.GetComponentInChildren<MeshRenderer>().material.shader = hotpoo;
            droneFollower.transform.localScale = 2f * Vector3.one;

            droneFollower.layer = LayerIndex.noCollision.intVal;
            UnityEngine.Object.Destroy(droneFollower.GetComponentInChildren<ParticleSystem>());
            Collider[] colliders = droneFollower.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                UnityEngine.Object.Destroy(c);
            }

            Material droneMat = Modules.Materials.CreateHopooMaterial("DroneBody");
            Modules.Materials.SetEmission(droneMat, 3f, Color.white);
            droneFollower.GetComponentInChildren<SkinnedMeshRenderer>().material = droneMat;

            MeshRenderer[] mr = droneFollower.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer m in mr)
            {
                if (m.name.ToLower() == "saw")
                {
                    m.material.shader = hotpoo;
                }
            }

            SkinnedMeshRenderer[] smr = droneFollower.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer m in smr)
            {
                m.material.shader = hotpoo;
            }

            return droneFollower;
        }
    }
}
