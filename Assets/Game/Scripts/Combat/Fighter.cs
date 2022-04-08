using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField]
        float weaponRange = 2f;
        Transform target;
        Mover mover;

        private void Start()
        {
            mover = GetComponent<Mover>();
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
    }
}
