using System;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "BulletTypes", menuName = "ScriptableObjects/BulletTypes")]
    public class BulletTypes : ScriptableObject
    {
        public enum BulletType
        {
            //Basic
            Ignita, // Fire (Tick damage)
            Hydra, // Water (Slight Knock-back)
            Terra, // Earth (Movement speed slow)
        
            //Advanced
            Glacies, // Ice (Freeze)
            Pyra, // Lightning (Bounce between enemies)
            Flora, // Nature (AOE Snare)
        
            //Ultimate
            Gravita, // Dark (Pulls enemies towards it as well as enemy bullets)
            Chrona, // Time (Stops all bullets except the player's)
            Umbra // Void (Homing + Go through walls && enemies)
        }
        
        [Serializable]
        public struct BulletCombo
        {
            public GameObject bulletPrefab;
            public BulletType bullet1;
            public BulletType bullet2;
        }
        
        public BulletCombo[] bulletCombos;
        
        public GameObject GetBulletCombo(BulletType bullet1, BulletType bullet2)
        {
            foreach (var combo in bulletCombos)
            {
                if (combo.bullet1 == bullet1 && combo.bullet2 == bullet2)
                    return combo.bulletPrefab;
                if (combo.bullet1 == bullet2 && combo.bullet2 == bullet1)
                    return combo.bulletPrefab;
            }

            Debug.LogError("Bullet combo not found");
            return null;
        }
    }
}
