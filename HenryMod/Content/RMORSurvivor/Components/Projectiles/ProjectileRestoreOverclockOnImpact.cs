using RMORMod.Content.Shared.Components.Body;
using RoR2.Projectile;
using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using RMORMod.Content.HANDSurvivor.Components.Body;

namespace RMORMod.Content.RMORSurvivor.Components.Projectiles
{
    public class ProjectileRestoreOverclockOnImpact : MonoBehaviour, IProjectileImpactBehavior
    {
        private TeamFilter tf;  //If this doesn't work, lemme know.
        private ProjectileController pc;    //has info about the projectile owner
        private ProjectileImpactExplosion pie;  //steal the radius from here
        private bool triggeredImpact = false;   //not sure if this is necessary, but just to be safe

        public float duration = 1f; //When you add this component to a projectile, you can set this and it'll be remembered for that projectile type.

        public void Awake()
        {
            pc = base.GetComponent<ProjectileController>();
            pie = base.GetComponent<ProjectileImpactExplosion>();
            tf = base.GetComponent<TeamFilter>();
        }

        //Pretty sure projectile code only runs on the server, but I'm adding a network check here just to be safe.
        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (!NetworkServer.active || triggeredImpact) return;
            triggeredImpact = true;

            if (pc && pie)
            {
                float radius = pie.blastRadius;
                GameObject owner = pc.owner;
                TeamIndex projectileTeam = TeamIndex.None;
                if (tf) projectileTeam = tf.teamIndex;

                if (owner && IsEnemyInSphere(radius, base.transform.position, projectileTeam))
                {
                    //THIS WILL NEED TO BE CHANGED TO WORK ONLINE. LET ME KNOW WHEN IT WORKS IN-GAME, AND I WILL SHOW YOU HOW.
                    HANDNetworkComponent oc = owner.GetComponent<HANDNetworkComponent>();
                    if (oc)
                    {
                        oc.ExtendOverclockServer(duration);
                    }
                }
            }
        }

        private bool IsEnemyInSphere(float radius, Vector3 position, TeamIndex team)
        {
            List<HealthComponent> hcList = new List<HealthComponent>();
            Collider[] array = Physics.OverlapSphere(position, radius, LayerIndex.entityPrecise.mask);
            for (int i = 0; i < array.Length; i++)
            {
                HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                if (hurtBox)
                {
                    HealthComponent healthComponent = hurtBox.healthComponent;
                    if (healthComponent && !hcList.Contains(healthComponent))
                    {
                        hcList.Add(healthComponent);
                        if (healthComponent.body && healthComponent.body.teamComponent && healthComponent.body.teamComponent.teamIndex != team)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}