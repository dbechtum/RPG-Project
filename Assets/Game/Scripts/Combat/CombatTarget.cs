using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour
    {
        [SerializeField] Vector3 centerOfMass = new Vector3(0, 1, 0);

        public Vector3 GetCenterOfMass()
        {
            return centerOfMass + transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(centerOfMass + transform.position, 0.2f);
        }
    }
}
