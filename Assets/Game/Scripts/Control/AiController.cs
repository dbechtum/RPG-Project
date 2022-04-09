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
        void Start()
        {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();

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
            float distance = Vector3.Distance(transform.position, nearestPlayer.transform.position);
            if (InChaseRangeOfPlayer() && fighter.CanAttack(nearestPlayer))
            {
                fighter.Attack(nearestPlayer);
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
