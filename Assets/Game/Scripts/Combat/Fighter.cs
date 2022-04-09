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
            if (!CanAttack()) return;

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
            transform.LookAt(target.transform);
            if (!cooldown)
            {
                TriggerAttack();
                cooldown = true;
                Invoke("AttackCooldown", attackSpeed);
            }
        }

        //(Re)Set Animator attack triggers.
        private void TriggerAttack()
        {
            animator.ResetTrigger("cancelAttack");
            animator.SetTrigger("attack");
        }

        public void Attack(CombatTarget _target)
        {
            //this will trigger Hit() through animation events.
            GetComponent<ActionScheduler>().StartAction(this);
            target = _target.GetComponent<Health>();
        }

        private bool CanAttack()
        {
            if (target == null) return false;
            if (target.IsDead()) return false;
            return true;
        }

        public bool CanAttack(CombatTarget combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return (targetToTest != null) && (!targetToTest.IsDead());
        }

        private bool GetIsInWeaponRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
        }

        //Animation Event
        private void Hit()
        {
            if (target == null) return;
            target.TakeDamage(weaponDamage);
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
        }

        //(Re)Set Animator attack triggers.
        private void StopAttack()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("cancelAttack");
        }

        private void AttackCooldown()
        {
            cooldown = false;
        }


    }
}
