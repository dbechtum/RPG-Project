using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        private Animator animator;
        [SerializeField]
        private float health = 100f;
        private bool isDead = false;
        public bool IsDead()
        {
            return isDead;
        }

        public void Start()
        {
            animator = GetComponent<Animator>();
        }
        public void TakeDamage(float damage)
        {
            if (isDead) return;

            health -= damage;
            if (health <= 0)
            {
                isDead = true;
                animator.SetTrigger("death");
                GetComponent<ActionScheduler>().CancelCurrentAction();
                health = 0;
            }
        }
    }
}
