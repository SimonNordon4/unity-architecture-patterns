using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityArchitecture.ScriptableObjectPattern
{
    [CreateAssetMenu(menuName="SpaghettiPattern/ChestItems")]
    public class ChestItems : ScriptableObject
    {
        public List<ChestItem> chestItems = new();
    }
}