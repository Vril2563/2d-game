using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class GetRoamPoint : MonoBehaviour
    {
        [SerializeField] private float range = 5f;
        private Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
        }

        private bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }

            result = Vector3.zero;
            return false;
        }


        public Vector3 GetRandomPoint(Vector3 point, float radius = 0)
        {
            if (RandomPoint(point,
                    radius == 0 ? this.range : radius, out var _point))
            {
                Debug.DrawRay(_point, Vector3.up, Color.white, 1f);
                //  print("Returning new position");
                return _point;
            }

            if (point == null)
            {
                return startPos;
            }
                
            return point;
        }
    }
}