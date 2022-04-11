using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (i == 0)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.gray;
                }
                Gizmos.DrawSphere(GetWaypoint(i), 0.2f);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(GetNextIndex(i))); //draw line between this and previous waypoint
            }
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }

        public int GetNextIndex(int i)
        {
            if (i + 1 == transform.childCount)
            {
                return 0;
            }
            else
            {
                return i + 1;
            }
        }
    }
}