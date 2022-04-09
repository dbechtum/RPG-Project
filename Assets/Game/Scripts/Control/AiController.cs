using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;


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

        private Vector3 guardLocation;
        [SerializeField] bool returnToGuardLocation = true;
        [SerializeField] float searchTimeAfterChase = 5f;
        private float timeSinceLastSawPlayer = 0f;

        void Start()
        {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();

            guardLocation = transform.position;


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
                GuardBehaviour();
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

        private void GuardBehaviour()
        {
            if (returnToGuardLocation)
            {
                mover.StartMoveAction(guardLocation);
            }
            else
            {
                fighter.Cancel();
            }
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
                float lowestDistance = 100;

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
