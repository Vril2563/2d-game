using System.Collections;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(GetRoamPoint))]
    [RequireComponent(typeof(Mover))]
    public class RoamingBehavior : MonoBehaviour
    {
        [SerializeField] private float radius = 5f;
        [SerializeField] private float roamSpeed = 3f;
        [SerializeField] private float roamWaitTime = 5f;
        private Mover mover;
        private bool lookingForRoamPoint = false;
        private GetRoamPoint getRoamPoint;
        private Vector3 startPos;

        private void Awake()
        {
            getRoamPoint = GetComponent<GetRoamPoint>();
            mover = GetComponent<Mover>();
        }

        private void Start()
        {
            startPos = transform.position;
        }

        void Update()
        {
            HandleRoam();
        }

        private void HandleRoam()
        {
            if (mover.HasPath() || lookingForRoamPoint) return;

            StartCoroutine(Roam());
        }

        private IEnumerator Roam()
        {
            lookingForRoamPoint = true;
        
            yield return new WaitForSeconds(roamWaitTime);
        
            if (mover.HasPath())
            {
                lookingForRoamPoint = false;
                yield break; // if we have a path, we don't need to find a new one
            }
        
            Vector3 destination = transform.position;
        
            while (Vector3.Distance(destination, startPos) < .5f || destination == getRoamPoint.transform.position)
            {
                destination = getRoamPoint.GetRandomPoint(startPos, radius);
            }
        
        
            mover.MoveTo(destination, roamSpeed);

            lookingForRoamPoint = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            if (startPos == Vector3.zero)
                Gizmos.DrawWireSphere(transform.position,radius);
            else 
                Gizmos.DrawWireSphere(startPos,radius);
        }
    }
}