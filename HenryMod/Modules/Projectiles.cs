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
        #region Ghosts
        public static GameObject primaryGhost;
        public static GameObject level1Ghost;
        public static GameObject level2Ghost;
        public static GameObject level3Ghost;
        public static GameObject level4Ghost;
        public static GameObject droneProjectileGhost;
        #endregion

        #region Explosions and Flares
        public static GameObject primaryExplosion = Assets.LoadEffect("PrimaryExplosion");
        public static GameObject level1Explosion = Assets.LoadEffect("Level1Explosion");
        public static GameObject level2Explosion = Assets.LoadEffect("Level2Explosion");
        public static GameObject level3Explosion = Assets.LoadEffect("Level3Explosion");
        public static GameObject level4Explosion = Assets.LoadEffect("Level4Explosion");

        public static GameObject primaryFlare = Assets.LoadEffect("RedFlare");
        public static GameObject secondaryFlare = Assets.LoadEffect("YellowFlare");
        public static GameObject midCharge = Assets.LoadEffect("RedCharge");
        public static GameObject fullCharge = Assets.LoadEffect("BlueCharge");
        #endregion

        internal static void RegisterProjectiles()
        {
            if (!PrimaryRocket.projectilePrefab) PrimaryRocket.projectilePrefab = CreateRocketProjectile();
            if (!PrimaryRocket.overclockPrefab) PrimaryRocket.overclockPrefab = CreateOverclockProjectile();
            if (!PrimaryRocket.effectPrefab) PrimaryRocket.effectPrefab = primaryFlare;
            if (!FireCannon.muzzleflashEffectPrefab) FireCannon.muzzleflashEffectPrefab = secondaryFlare;
            if (!ChargeCannon.partialChargeEffect) ChargeCannon.partialChargeEffect = midCharge;
            if (!ChargeCannon.fullChargeEffect) ChargeCannon.fullChargeEffect = fullCharge;
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
            GameObject projectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion().InstantiateClone("RMORMod_RMOR_Rocket", true);
            primaryGhost = RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("PrimaryRocket");

            ProjectileSimple ps = projectile.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 100f;// 20.96f should be equivalent to tf2 rockets (1100HU/S) but this doesn't seem to be the case in-game.
            ps.lifetime = 20f;

            projectile.GetComponent<ProjectileController>().ghostPrefab = primaryGhost;
            if (!primaryGhost.GetComponent<NetworkIdentity>()) primaryGhost.AddComponent<NetworkIdentity>();
            if (!primaryGhost.GetComponent<ProjectileGhostController>()) primaryGhost.AddComponent<ProjectileGhostController>();
            if (!projectile.GetComponent<NetworkIdentity>()) projectile.AddComponent<NetworkIdentity>();
            ProjectileRestoreOverclockOnImpact ovc = projectile.AddComponent<ProjectileRestoreOverclockOnImpact>();
            ovc.duration = 0.6f;

            ProjectileImpactExplosion impactExplosion = projectile.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(impactExplosion);
            impactExplosion.blastRadius = 6f;
            impactExplosion.destroyOnEnemy = true;
            impactExplosion.destroyOnWorld = true;
            impactExplosion.lifetime = 200f;
            impactExplosion.explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion();
            //impactExplosion.explosionEffect = primaryExplosion;

            Modules.ContentPacks.projectilePrefabs.Add(projectile);
            return projectile;
        }
        private static GameObject CreateOverclockProjectile()
        {
            GameObject projectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion().InstantiateClone("RMORMod_RMOR_OVC_Rocket", true);
            primaryGhost = RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("OVCRocket");

            ProjectileSimple ps = projectile.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 175f;// 20.96f should be equivalent to tf2 rockets (1100HU/S) but this doesn't seem to be the case in-game.
            ps.lifetime = 20f;

            ProjectileDamage pd = projectile.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Stun1s;

            projectile.GetComponent<ProjectileController>().ghostPrefab = primaryGhost;
            if (!primaryGhost.GetComponent<NetworkIdentity>()) primaryGhost.AddComponent<NetworkIdentity>();
            if (!primaryGhost.GetComponent<ProjectileGhostController>()) primaryGhost.AddComponent<ProjectileGhostController>();
            if (!projectile.GetComponent<NetworkIdentity>()) projectile.AddComponent<NetworkIdentity>();
            ProjectileRestoreOverclockOnImpact ovc = projectile.AddComponent<ProjectileRestoreOverclockOnImpact>();
            ovc.duration = 1.2f;

            ProjectileImpactExplosion impactExplosion = projectile.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(impactExplosion);
            impactExplosion.blastRadius = 6f;
            impactExplosion.destroyOnEnemy = true;
            impactExplosion.destroyOnWorld = true;
            impactExplosion.lifetime = 200f;
            impactExplosion.explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion();
            //impactExplosion.explosionEffect = primaryExplosion;

            Modules.ContentPacks.projectilePrefabs.Add(projectile);
            return projectile;
        }
        private static GameObject CreateLevel1Projectile()
        {
            GameObject projectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion().InstantiateClone("RMORMod_RMOR_LV1", true);
            level1Ghost = RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Level1Blast").InstantiateClone("RMOR_ChargeShotGhost");

            ProjectileSimple ps = projectile.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 75f;// 20.96f should be equivalent to tf2 rockets (1100HU/S) but this doesn't seem to be the case in-game.
            ps.lifetime = 20f;


            if (!level1Ghost.GetComponent<NetworkIdentity>()) level1Ghost.AddComponent<NetworkIdentity>();
            if (!level1Ghost.GetComponent<ProjectileGhostController>()) level1Ghost.AddComponent<ProjectileGhostController>();
            projectile.GetComponent<ProjectileController>().ghostPrefab = level1Ghost;
            ProjectileRestoreOverclockOnImpact ovc = projectile.AddComponent<ProjectileRestoreOverclockOnImpact>();
            ovc.duration = 0.8f;

            ProjectileImpactExplosion impactExplosion = projectile.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(impactExplosion);
            impactExplosion.blastRadius = 8f;
            impactExplosion.destroyOnEnemy = true;
            impactExplosion.destroyOnWorld = true;
            impactExplosion.lifetime = 200f;
            impactExplosion.explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion();
            //impactExplosion.explosionEffect = level1Explosion;

            Modules.ContentPacks.projectilePrefabs.Add(projectile);
            return projectile;
        }
        private static GameObject CreateLevel2Projectile()
        {
            GameObject projectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion().InstantiateClone("RMORMod_RMOR_LV2", true);
            level2Ghost = RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Level2Blast").InstantiateClone("RMOR_ChargeShotGhost");

            ProjectileSimple ps = projectile.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 100f;// 20.96f should be equivalent to tf2 rockets (1100HU/S) but this doesn't seem to be the case in-game.
            ps.lifetime = 20f;


            if (!level2Ghost.GetComponent<NetworkIdentity>()) level2Ghost.AddComponent<NetworkIdentity>();
            if (!level2Ghost.GetComponent<ProjectileGhostController>()) level2Ghost.AddComponent<ProjectileGhostController>();
            projectile.GetComponent<ProjectileController>().ghostPrefab = level2Ghost;
            ProjectileRestoreOverclockOnImpact ovc = projectile.AddComponent<ProjectileRestoreOverclockOnImpact>();
            ovc.duration = 1.6f;

            ProjectileImpactExplosion impactExplosion = projectile.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(impactExplosion);
            impactExplosion.blastRadius = 12f;
            impactExplosion.destroyOnEnemy = true;
            impactExplosion.destroyOnWorld = true;
            impactExplosion.lifetime = 200f;
            impactExplosion.explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion();
            //impactExplosion.explosionEffect = level2Explosion;

            Modules.ContentPacks.projectilePrefabs.Add(projectile);
            return projectile;
        }
        private static GameObject CreateLevel3Projectile()
        {
            GameObject projectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion().InstantiateClone("RMORMod_RMOR_LV3", true);
            level3Ghost = RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Level3Blast").InstantiateClone("RMOR_ChargeShotGhost");

            ProjectileSimple ps = projectile.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 125f;// 20.96f should be equivalent to tf2 rockets (1100HU/S) but this doesn't seem to be the case in-game.
            ps.lifetime = 20f;


            if (!level3Ghost.GetComponent<NetworkIdentity>()) level3Ghost.AddComponent<NetworkIdentity>();
            if (!level3Ghost.GetComponent<ProjectileGhostController>()) level3Ghost.AddComponent<ProjectileGhostController>();
            projectile.GetComponent<ProjectileController>().ghostPrefab = level3Ghost;
            ProjectileRestoreOverclockOnImpact ovc = projectile.AddComponent<ProjectileRestoreOverclockOnImpact>();
            ovc.duration = 2.4f;

            ProjectileImpactExplosion impactExplosion = projectile.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(impactExplosion);
            impactExplosion.blastRadius = 16f;
            impactExplosion.destroyOnEnemy = true;
            impactExplosion.destroyOnWorld = true;
            impactExplosion.lifetime = 200f;
            impactExplosion.explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion();
            //impactExplosion.explosionEffect = level3Explosion;

            Modules.ContentPacks.projectilePrefabs.Add(projectile);
            return projectile;
        }
        private static GameObject CreateLevel4Projectile()
        {
            GameObject projectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion().InstantiateClone("RMORMod_RMOR_LV4", true);
            level4Ghost = RMORMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Level4Blast").InstantiateClone("RMOR_ChargeShotGhost");

            ProjectileSimple ps = projectile.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 150f;// 20.96f should be equivalent to tf2 rockets (1100HU/S) but this doesn't seem to be the case in-game.
            ps.lifetime = 20f;


            if (!level4Ghost.GetComponent<NetworkIdentity>()) level4Ghost.AddComponent<NetworkIdentity>();
            if (!level4Ghost.GetComponent<ProjectileGhostController>()) level4Ghost.AddComponent<ProjectileGhostController>();
            projectile.GetComponent<ProjectileController>().ghostPrefab = level4Ghost;
            ProjectileRestoreOverclockOnImpact ovc = projectile.AddComponent<ProjectileRestoreOverclockOnImpact>();
            ovc.duration = 3.2f;

            ProjectileImpactExplosion impactExplosion = projectile.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(impactExplosion);
            impactExplosion.blastRadius = 24f;
            impactExplosion.destroyOnEnemy = true;
            impactExplosion.destroyOnWorld = true;
            impactExplosion.lifetime = 200f;
            impactExplosion.explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion();
            //impactExplosion.explosionEffect = level4Explosion;

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

            ProjectileDamage pd = projectile.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.WeakOnHit;

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


        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.SweetSpot;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

    }
}