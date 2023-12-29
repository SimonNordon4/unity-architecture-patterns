using System;
using UnityEngine;

namespace Classic.UI
{
    public class UIPage : MonoBehaviour
    {
        [field:SerializeField]public UIStateEnum activationState { get; private set; }
        private GameObject _pageObject;
    }
}