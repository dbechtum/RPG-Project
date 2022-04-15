using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {

        Health target = null;
        GameObject caster = null;
        [SerializeField] float speed = 1f;
        [SerializeField] float lifeTime = 2f;
        [Tooltip("If is Homing, the projectile will continiously rotate towards the target.")]
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        float damage = 0f;

        [Tooltip("All gameobjects to be destroyed immediately, upon impact. The rest will be destroyed after particles have run out.")]
        [SerializeField] GameObject[] destroyListOnImpact = null;

        // Update is called once per frame
        void Update()
        {
            if (target != null)
            {
                if (isHoming && !target.IsDead()) transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, float damage, GameObject caster)
        {
            this.caster = caster;
            this.damage = damage;
            this.target = target;
            transform.LookAt(GetAimLocation());
            Invoke("StopProjectile", lifeTime); //destroy projectile after x seconds
        }

        private Vector3 GetAimLocation()
        {//get center of character
            CombatTarget combatTarget = target.GetComponent<CombatTarget>();
            if (combatTarget == null) return target.transform.position;

            return combatTarget.GetCenterOfMass();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == caster) return;

            Health targetHit = other.gameObject.GetComponent<Health>();
            if (targetHit != null)
            {
                if (!targetHit.IsDead())
                {
                    targetHit.TakeDamage(damage);

                }
            }
            if (hitEffect != null) Instantiate(hitEffect, transform.position, transform.rotation);
            StopProjectile();
        }

        //Stops particles before destroying the projectile
        private void StopProjectile()
        {
            gameObject.GetComponent<Collider>().enabled = false; //disable collision.
            foreach (GameObject child in destroyListOnImpact) //destroy list of objects that should be hidden right away.
            {
                Destroy(child);
            }
            float destroyDelay = 0f;
            ParticleSystem[] particles = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particle in particles) //disable particles, and track which has the longest duration.
            {
                destroyDelay = (particle.main.duration > destroyDelay)
                    ? particle.main.duration
                    : destroyDelay;
                particle.Stop();
            }
            Invoke("DestroyProjectile", destroyDelay); // destroy the projectile after particles have died.
        }

        //Completely removes the projectile immediately (any remaining particles will despawn immediately)
        private void DestroyProjectile()
        {
            Destroy(gameObject);
        }
    }
}