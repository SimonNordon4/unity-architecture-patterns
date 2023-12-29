using System.Collections.Generic;
using UnityEngine;

namespace Classic.UI
{
    public class UIPageController : MonoBehaviour
    {
        [SerializeField]private UIState uiState;
        [SerializeField]private UIPage[] pages;
        
        public void SetPages(UIPage[] newPages)
        {
            pages = newPages;
        }
        
        private void OnEnable()
        {
            uiState.onStateChanged.AddListener(OnStateChanged);
        }

        private void OnStateChanged(UIStateEnum state)
        {
            foreach (var page in pages)
            {
                page.gameObject.SetActive(page.activationState == state);
            }
        }
    }
    
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(UIPageController))]
    public class UIControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var uiController = (UIPageController) target;
            if (GUILayout.Button("Fetch Pages"))
            {
                Debug.Log("Finding pages...");
                var pages = FindObjectsByType<UIPage>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                Debug.Log("Pages found: " + pages.Length);
                uiController.SetPages(pages);
                UnityEditor.EditorUtility.SetDirty(uiController);
            }
        }
    }
    #endif
}