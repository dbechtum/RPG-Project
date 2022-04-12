using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        private NavMeshAgent agent;
        private Animator animator;
        private Health health;
        private ActionScheduler actionScheduler;

        [SerializeField] float maxSpeed = 5.662316f;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            health = GetComponent<Health>();
            actionScheduler = GetComponent<ActionScheduler>();
        }

        void Update()
        {
            agent.enabled = !health.IsDead();

            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            StartMoveAction(destination, Quaternion.identity, speedFraction);
        }
        public void StartMoveAction(Vector3 destination, Quaternion rotation, float speedFraction)
        {
            actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
            if (rotation != Quaternion.identity && Vector3.Distance(transform.position, destination) < 0.25f)
            {
                if (Quaternion.Angle(transform.rotation, rotation) > 0.01f)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * 250);
                }
            }
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.destination = destination;
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            agent.isStopped = false;
        }

        public void Cancel()
        {
            agent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            animator.SetFloat("Velocity", transform.InverseTransformDirection(agent.velocity).z);
        }
    }
}
