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
        [SerializeField] float speed = 1;
        [Tooltip("If is Targeted, the projectile will continiously rotate towards the target.")]
        [SerializeField] bool targeted = false;
        float damage = 0;

        // Update is called once per frame
        void Update()
        {
            if (target == null) return;

            if (targeted) transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, float damage, GameObject caster)
        {
            this.caster = caster;
            this.damage = damage;
            this.target = target;
            transform.LookAt(GetAimLocation());
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
            if (targetHit != null) targetHit.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}