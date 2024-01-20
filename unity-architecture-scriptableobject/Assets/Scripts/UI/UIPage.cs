using System;
using UnityEngine;

namespace GameObjectComponent.UI
{
    public class UIPage : MonoBehaviour
    {
        [field:SerializeField]public UIStateEnum activationState { get; private set; }
        private GameObject _pageObject;
    }
}