using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ScriptableData : ScriptableObject
{
    protected abstract void Init();
    protected abstract void ResetData();
    
    private void OnEnable()
    {
        Debug.Log("ScriptableData OnEnable");
        #if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        #else
        Init();
        #endif
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        Debug.Log("ScriptableData OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetData();
    }
    
    #if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
    {
        if (playModeStateChange == PlayModeStateChange.ExitingEditMode)
        {
            Init();
        }

        else if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
        {
            ResetData();
        }
    }
    #endif
    
}
