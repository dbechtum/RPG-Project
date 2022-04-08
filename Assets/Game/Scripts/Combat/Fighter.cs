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
        [SerializeField] float attackSpeed = 1.5f;
        bool cooldown;
        Transform target;
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
            if (!GetIsInWeaponRange())
            {
                mover.MoveTo(target.position);
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
            return Vector3.Distance(transform.position, target.position) < weaponRange;
        }

        public void Attack(CombatTarget _target)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = _target.transform;
        }

        public void Cancel()
        {
            target = null;
        }

        private void AttackCooldown()
        {
            cooldown = false;
        }

        //Animation Event
        private void Hit()
        {

        }
    }
}
