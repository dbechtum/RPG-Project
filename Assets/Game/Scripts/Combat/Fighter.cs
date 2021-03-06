using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {

        [SerializeField] Transform rightHand = null;
        [SerializeField] Transform leftHand = null;
        [SerializeField] Weapon defaultWeapon = null;
        [SerializeField] float attackSpeedModifier = 1f;


        bool cooldown;
        Health target;
        Mover mover;
        Animator animator;
        Weapon currentWeapon = null;

        private void Start()
        {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();

            EquipWeapon(defaultWeapon);
        }


        private void Update()
        {
            if (!CanAttack()) return;

            if (!GetIsInWeaponRange())
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            weapon.Spawn(rightHand, leftHand, animator);
        }


        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (!cooldown)
            {
                TriggerAttack();
                cooldown = true;
                Invoke("AttackCooldown", currentWeapon.GetSpeed() * attackSpeedModifier);
            }
        }

        //(Re)Set Animator attack triggers.
        private void TriggerAttack()
        {
            animator.ResetTrigger("cancelAttack");
            animator.SetTrigger("attack");
        }

        public void Attack(GameObject _target)
        {
            //this will trigger Hit() through animation events.
            GetComponent<ActionScheduler>().StartAction(this);
            target = _target.GetComponent<Health>();
        }

        private bool CanAttack()
        {
            if (target == null) return false;
            if (target.gameObject == this.gameObject) return false;
            if (target.IsDead()) return false;
            return true;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return (targetToTest != null) && (!targetToTest.IsDead());
        }

        private bool GetIsInWeaponRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetRange();
        }

        //Animation Event Melee
        private void Hit()
        {
            if (target == null) return;

            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHand, leftHand, target, this.gameObject);
            }
            else
            {
                target.TakeDamage(currentWeapon.GetDamage());
            }
        }

        //Animation Event Bows
        private void Shoot()
        {
            Hit();
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
