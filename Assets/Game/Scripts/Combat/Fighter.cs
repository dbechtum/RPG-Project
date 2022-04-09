using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 10f;
        [SerializeField] float attackSpeed = 1.5f;
        bool cooldown;
        Health target;
        Mover mover;
        Animator animator;

        private void Start()
        {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (target == null) return;
            if (target.IsDead()) return;

            if (!GetIsInWeaponRange())
            {
                mover.MoveTo(target.transform.position);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            if (!cooldown)
            {
                animator.SetTrigger("attack");
                cooldown = true;
                Invoke("AttackCooldown", attackSpeed);
            }
        }

        private bool GetIsInWeaponRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
        }

        public void Attack(CombatTarget _target)
        {
            //this will trigger Hit() through animation events.
            GetComponent<ActionScheduler>().StartAction(this);
            target = _target.GetComponent<Health>();
        }

        //Animation Event
        private void Hit()
        {
            if (target == null) return;
            target.TakeDamage(weaponDamage);
        }

        public void Cancel()
        {
            animator.SetTrigger("cancelAttack");
            target = null;
        }

        private void AttackCooldown()
        {
            cooldown = false;
        }


    }
}
