using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.RMOR.Primary;
using EntityStates.RMOR.Secondary;
using RMORMod.Content.RMORSurvivor.Components.Projectiles;
using UnityEngine.AddressableAssets;
using EntityStates.RMOR.Special;

namespace RMORMod.Modules
{
    internal static class Projectiles
    {
        public static GameObject primaryGhost;
        public static GameObject level1Ghost;
        public static GameObject level2Ghost;
        public static GameObject level3Ghost;
        public static GameObject level4Ghost;
        public static GameObject droneProjectileGhost;

        internal static void RegisterProjectiles()
        {
            if (!RMORRocket.projectilePrefab) RMORRocket.projectilePrefab = CreateRocketProjectile();
            if (!FireCannon.level1Prefab) FireCannon.level1Prefab = CreateLevel1Projectile();
            if (!FireCannon.level2Prefab) FireCannon.level2Prefab = CreateLevel2Projectile();
            if (!FireCannon.level3Prefab) FireCannon.level3Prefab = CreateLevel3Projectile();
            if (!FireCannon.level4Prefab) FireCannon.level4Prefab = CreateLevel4Projectile();
            if (!FireSeekingDrone.projectilePrefab) FireSeekingDrone.projectilePrefab = CreateRMORMissile();
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }


        private static GameObject CreateRocketProjectile()
        {
            GameObject projectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/PaladinRocket.prefab").WaitForCompletion().InstantiateClone("RMORMod_RMOR_Rocket", true);
            primaryGhost = RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("PrimaryRocket");
            if (!primaryGhost.GetComponent<NetworkIdentity>()) primaryGhost.AddComponent<NetworkIdentity>();
            if (!primaryGhost.GetComponent<ProjectileGhostController>()) primaryGhost.AddComponent<ProjectileGhostController>();
            projectile.GetComponent<ProjectileController>().ghostPrefab = primaryGhost;
            if (!projectile.GetComponent<NetworkIdentity>()) projectile.AddComponent<NetworkIdentity>();
            ProjectileRestoreOverclockOnImpact ovc = projectile.AddComponent<ProjectileRestoreOverclockOnImpact>();
            ovc.duration = 0.4f;
            ProjectileImpactExplosion impactExplosion = projectile.GetComponent<ProjectileImpactExplosion>();
            impactExplosion.blastRadius *= 0.5f;
            impactExplosion.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            Modules.ContentPacks.projectilePrefabs.Add(projectile);
            return projectile;
        }
        private static GameObject CreateLevel1Projectile()
        {
            GameObject projectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/PaladinRocket").InstantiateClone("RMOR_ChargeShot", true);
            level1Ghost = RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Level1Blast").InstantiateClone("RMOR_ChargeShotGhost");
            if (!level1Ghost.GetComponent<NetworkIdentity>()) level1Ghost.AddComponent<NetworkIdentity>();
            if (!level1Ghost.GetComponent<ProjectileGhostController>()) level1Ghost.AddComponent<ProjectileGhostController>();
            projectile.GetComponent<ProjectileController>().ghostPrefab = level1Ghost;
            ProjectileRestoreOverclockOnImpact ovc = projectile.AddComponent<ProjectileRestoreOverclockOnImpact>();
            ovc.duration = 0.6f;
            ProjectileImpactExplosion impactExplosion = projectile.GetComponent<ProjectileImpactExplosion>();
            impactExplosion.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            Modules.ContentPacks.projectilePrefabs.Add(projectile);
            return projectile;
        }
        private static GameObject CreateLevel2Projectile()
        {
            GameObject projectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/PaladinRocket").InstantiateClone("RMOR_ChargeShot", true);
            level2Ghost = RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Level2Blast").InstantiateClone("RMOR_ChargeShotGhost");
            if (!level2Ghost.GetComponent<NetworkIdentity>()) level2Ghost.AddComponent<NetworkIdentity>();
            if (!level2Ghost.GetComponent<ProjectileGhostController>()) level2Ghost.AddComponent<ProjectileGhostController>();
            projectile.GetComponent<ProjectileController>().ghostPrefab = level2Ghost;
            ProjectileRestoreOverclockOnImpact ovc = projectile.AddComponent<ProjectileRestoreOverclockOnImpact>();
            ovc.duration = 1.2f;
            ProjectileImpactExplosion impactExplosion = projectile.GetComponent<ProjectileImpactExplosion>();
            impactExplosion.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            impactExplosion.blastRadius *= 1.5f;
            Modules.ContentPacks.projectilePrefabs.Add(projectile);
            return projectile;
        }
        private static GameObject CreateLevel3Projectile()
        {
            GameObject projectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/PaladinRocket").InstantiateClone("RMOR_ChargeShot", true);
            level3Ghost = RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Level3Blast").InstantiateClone("RMOR_ChargeShotGhost");
            if (!level3Ghost.GetComponent<NetworkIdentity>()) level3Ghost.AddComponent<NetworkIdentity>();
            if (!level3Ghost.GetComponent<ProjectileGhostController>()) level3Ghost.AddComponent<ProjectileGhostController>();
            projectile.GetComponent<ProjectileController>().ghostPrefab = level3Ghost;
            ProjectileRestoreOverclockOnImpact ovc = projectile.AddComponent<ProjectileRestoreOverclockOnImpact>();
            ovc.duration = 1.8f;
            ProjectileImpactExplosion impactExplosion = projectile.GetComponent<ProjectileImpactExplosion>();
            impactExplosion.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            impactExplosion.blastRadius *= 2;
            Modules.ContentPacks.projectilePrefabs.Add(projectile);
            return projectile;
        }
        private static GameObject CreateLevel4Projectile()
        {
            GameObject projectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/PaladinRocket").InstantiateClone("RMOR_ChargeShot", true);
            level4Ghost = RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Level4Blast").InstantiateClone("RMOR_ChargeShotGhost");
            if (!level4Ghost.GetComponent<NetworkIdentity>()) level4Ghost.AddComponent<NetworkIdentity>();
            if (!level4Ghost.GetComponent<ProjectileGhostController>()) level4Ghost.AddComponent<ProjectileGhostController>();
            projectile.GetComponent<ProjectileController>().ghostPrefab = level4Ghost;
            ProjectileRestoreOverclockOnImpact ovc = projectile.AddComponent<ProjectileRestoreOverclockOnImpact>();
            ovc.duration = 2.4f;
            ProjectileImpactExplosion impactExplosion = projectile.GetComponent<ProjectileImpactExplosion>();
            impactExplosion.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            impactExplosion.blastRadius *= 3;
            Modules.ContentPacks.projectilePrefabs.Add(projectile);
            return projectile;
        }
        private static GameObject CreateRMORMissile()
        {
            GameObject projectile = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MissileProjectile").InstantiateClone("RMOR_DroneMissile", true);
            droneProjectileGhost = RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("DroneMissilePrefab").InstantiateClone("RMOR_DroneMissileGhost");
            if (!droneProjectileGhost.GetComponent<NetworkIdentity>()) droneProjectileGhost.AddComponent<NetworkIdentity>();
            if (!droneProjectileGhost.GetComponent<ProjectileGhostController>()) droneProjectileGhost.AddComponent<ProjectileGhostController>();
            projectile.GetComponent<ProjectileController>().ghostPrefab = droneProjectileGhost;

            Object.Destroy(projectile.GetComponent<MissileController>());
            ProjectileSteerTowardTarget pst = projectile.AddComponent<ProjectileSteerTowardTarget>();
            pst.yAxisOnly = false;
            pst.rotationSpeed = 360f;

            ProjectileSimple ps = projectile.AddComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 40f;
            ps.lifetime = 30f;
            ps.updateAfterFiring = true;
            ps.enableVelocityOverLifetime = false;
            ps.oscillate = true;
            ps.oscillateMagnitude = 6f;
            ps.oscillateSpeed = 1.5f;

            ProjectileSphereTargetFinder pstf = projectile.AddComponent<ProjectileSphereTargetFinder>();
            pstf.lookRange = 90f;
            pstf.targetSearchInterval = 0.3f;
            pstf.onlySearchIfNoTarget = true;
            pstf.allowTargetLoss = false;
            pstf.testLoS = false;
            pstf.ignoreAir = false;
            pstf.flierAltitudeTolerance = Mathf.Infinity;

            ProjectileController pc = projectile.GetComponent<ProjectileController>();
            pc.allowPrediction = false;

            Modules.ContentPacks.projectilePrefabs.Add(projectile);
            return projectile;
        }
    }
}