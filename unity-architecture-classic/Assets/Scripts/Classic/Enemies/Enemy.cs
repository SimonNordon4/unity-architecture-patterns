using System;
using Classic.Actors;
using Classic.Pools;
using UnityEngine;

namespace Classic.Enemies
{

    public class Enemy : MonoBehaviour
    {
        [field:SerializeField]
        public EnemyDefinition enemyDefinition { get; private set; }
    }
}