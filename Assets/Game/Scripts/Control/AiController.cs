using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;

namespace RPG.Control
{
    public class AiController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        GameObject[] players;
        GameObject nearestPlayer;
        private Fighter fighter;
        private Mover mover;
        private Health health;

        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 0.5f;
        int currentWaypointIndex = 0;

        private Vector3 guardPosition;
        private Quaternion guardRotation;
        private Vector3 nextPosition;
        [SerializeField] float searchTimeAfterChase = 5f;
        [SerializeField] float waitTimeAfterWaypointArrival = 3f;
        private float timeSinceLastSawPlayer;
        private float timeSinceArrivedAtWaypoint = 0f;

        void Start()
        {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();

            guardPosition = transform.position;
            guardRotation = transform.rotation;
            nextPosition = guardPosition;
            timeSinceLastSawPlayer = searchTimeAfterChase;

            InvokeRepeating("FindNearestPlayer", 1f, 2.5f); //loops through all the players to find the closests one.
        }
        private void Update()
        {
            if (health.IsDead()) return;
            Chase();
        }

        private void Chase()
        {
            if (nearestPlayer == null) return;
            if (InChaseRangeOfPlayer() && fighter.CanAttack(nearestPlayer))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < searchTimeAfterChase)
            {
                SearchBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(nearestPlayer);
        }

        private void SearchBehaviour()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            fighter.Cancel();
        }

        private void PatrolBehaviour()
        {
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint += Time.deltaTime; //linger at waypoint timer
                    if (timeSinceArrivedAtWaypoint >= waitTimeAfterWaypointArrival)
                    {
                        timeSinceArrivedAtWaypoint = 0; //reset timer
                        GetNextWaypoint();
                    }
                }
                nextPosition = GetCurrentWaypoint();
                mover.StartMoveAction(nextPosition);
                return;
            }
            mover.StartMoveAction(guardPosition, guardRotation);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void GetNextWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            return (Vector3.Distance(transform.position, GetCurrentWaypoint()) < waypointTolerance);
        }

        private bool InChaseRangeOfPlayer()
        {
            float distance = Vector3.Distance(transform.position, nearestPlayer.transform.position);
            return distance < chaseDistance;
        }

        //Get a list of all the players and figure out which one is closest
        private void FindNearestPlayer()
        {
            if (health.IsDead())
            {
                CancelInvoke("FindNearestPlayer"); //stop repeating this function if this AI is dead.
            }
            else
            {
                //get all the players
                players = GameObject.FindGameObjectsWithTag("Player");
                float lowestDistance = 1000;

                foreach (GameObject player in players)
                {
                    if (player.GetComponent<Health>().IsDead()) continue; //ignore dead players
                    float distance = Vector3.Distance(transform.position, player.transform.position);
                    if ((distance < lowestDistance))
                    {
                        nearestPlayer = player;
                        lowestDistance = distance;
                    }
                }
            }
        }

        // Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
