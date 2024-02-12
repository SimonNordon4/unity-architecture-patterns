using UnityEngine;

namespace Obvious.Soap
{
    [System.Serializable]
    public abstract class ScriptableVariableBase : ScriptableBase
    {
        [SerializeField] private string _guid;
        
        [Tooltip("ID used as the Player Prefs Key.\n" +
                 "Auto: Guid is generated automatically base on the asset path.\n" +
                 "Manual: Guid can be overwritten manually.")]
        [SerializeField]
        private SaveGuidType _saveGuid;
        public SaveGuidType SaveGuid => _saveGuid;
      
        /// <summary>
        /// Guid is needed to save/load the value to PlayerPrefs.
        /// </summary>
        public string Guid
        {
            get => _guid;
            set => _guid = value;
        }
        public virtual System.Type GetGenericType { get; }
    }
    
    public enum SaveGuidType
    {
        Auto,
        Manual,
    }
}