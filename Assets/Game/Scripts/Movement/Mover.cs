using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Animator animator;

        void Start()
        {
            agent = this.gameObject.GetComponent<NavMeshAgent>();
            animator = this.gameObject.GetComponent<Animator>();
        }

        void Update()
        {
            UpdateAnimator();
        }

        public void MoveTo(Vector3 destionation)
        {
            agent.destination = destionation;
        }

        private void UpdateAnimator()
        {
            animator.SetFloat("Velocity", transform.InverseTransformDirection(agent.velocity).z);
        }
    }
}
