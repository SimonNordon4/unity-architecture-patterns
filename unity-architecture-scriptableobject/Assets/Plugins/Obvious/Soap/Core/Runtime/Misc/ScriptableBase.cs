using System;
using UnityEngine;

namespace Obvious.Soap
{
    /// <summary>
    /// Base classes of all ScriptableObjects in Soap
    /// </summary>
    [Serializable]
    public abstract class ScriptableBase : ScriptableObject
    {
        public virtual void Reset() { }
        public Action RepaintRequest;
        [HideInInspector]
        public int CategoryIndex = 0;
    }
}