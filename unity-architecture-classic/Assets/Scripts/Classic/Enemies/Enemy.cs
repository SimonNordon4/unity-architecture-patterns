using System;
using UnityEngine;

namespace Classic.Enemies
{
    public class Enemy : MonoBehaviour
    {
        public static event Action<Enemy> OnEnemyDeath; 
        
    }
}