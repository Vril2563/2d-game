using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "BulletObject", menuName = "ScriptableObjects/BulletObject")]
    public class ScriptableBullet : ScriptableObject
    {
        public BulletTypes bulletTypes;
    
        [Header("Bullet Stats")]
        public BulletTypes.BulletType bulletType;
        public Color bulletColor = Color.white;
        public int damage = 10;
        public float speed = 1f;
        public float knockbackStrength = 1f;
        public float knockbackDuration = .2f;
        public float lifetime = 5f;


        [Header("Bullet Properties")]
        public bool isHoming = false;
        public bool isPiercing = false;
        public bool isBouncing = false;

    }
}
