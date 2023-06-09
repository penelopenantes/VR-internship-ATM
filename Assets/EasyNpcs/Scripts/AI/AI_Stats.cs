using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Package
{
    public class AI_Stats : Stats
    {
        public float walkSpeed = 2;
        public float runSpeed = 4;

        public float visionRange = 10;
        public float visionAngle = 25f;
        public LayerMask visionLayers;
        
        public enum Weapon { melee, ranged };
        public Weapon assignedWeapon;
        [HideInInspector]
        public Projectile projectile;
        [HideInInspector]
        public float launchHight;

        public Job job;
        public Gender gender;

        public List<string> enemies;
        public List<string> protects;
    }
}

