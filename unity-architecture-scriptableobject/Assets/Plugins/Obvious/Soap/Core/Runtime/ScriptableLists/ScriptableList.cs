using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Obvious.Soap
{
    public abstract class ScriptableList<T> : ScriptableListBase, IReset, IEnumerable<T>, IDrawObjectsInInspector
    {
        [Tooltip(
            "Clear the list when:" +
            " Scene Loaded : when a scene is loaded." +
            " Application Start : Once, when the application starts. Modifications persists between scenes")]
        [SerializeField]
        private ResetType _resetOn = ResetType.SceneLoaded;

        [SerializeReference] protected List<T> _list = new List<T>();
        private readonly HashSet<T> _hashSet = new HashSet<T>();
      
        public int Count => _list.Count;
        public bool IsEmpty => _list.Count == 0;
        public override Type GetGenericType => typeof(T);

        //feel free to uncomment this property if you need to access the list for more functionalities.
        //public List<T> List => _list; 

        /// <summary>
        /// Access an item in the list by index.
        /// </summary>
        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        /// <summary> Event raised when an item is added or removed from the list. </summary>
        public event Action OnItemCountChanged;

        /// <summary> Event raised  when an item is added to the list. </summary>
        public event Action<T> OnItemAdded;

        /// <summary> Event raised  when an item is removed from the list. </summary>
        public event Action<T> OnItemRemoved;

        /// <summary> Event raised  when multiple item are added to the list. </summary>
        public event Action<IEnumerable<T>> OnItemsAdded;

        /// <summary> Event raised  when multiple items are removed from the list. </summary>
        public event Action<IEnumerable<T>> OnItemsRemoved;

        /// <summary> Event raised  when the list is cleared. </summary>
        public event Action OnCleared;

        /// <summary>
        /// Adds an item to the list only if its not in the list.
        /// Raises OnItemCountChanged and OnItemAdded event.
        /// </summary>
        public void Add(T item)
        {
            if (_hashSet.Add(item))
            {
                _list.Add(item);
                OnItemCountChanged?.Invoke();
                OnItemAdded?.Invoke(item);
#if UNITY_EDITOR
                RepaintRequest?.Invoke();
#endif
            }
        }

        /// <summary>
        /// Adds a range of items to the list. An item is only added if its not in the list.
        /// Raises OnItemCountChanged and OnItemsAdded event once, after all items have been added.
        /// </summary>
        public void AddRange(IEnumerable<T> items)
        {
            var itemList = items.ToList();
            foreach (var item in itemList.Where(item => _hashSet.Add(item)))
                _list.Add(item);

            OnItemCountChanged?.Invoke();
            OnItemsAdded?.Invoke(itemList);
#if UNITY_EDITOR
            RepaintRequest?.Invoke();
#endif
        }

        /// <summary>
        /// Removes an item from the list only if its in the list.
        /// Raises OnItemCountChanged and OnItemRemoved event.
        /// </summary>
        public void Remove(T item)
        {
            if (_hashSet.Remove(item))
            {
                _list.Remove(item);
                OnItemCountChanged?.Invoke();
                OnItemRemoved?.Invoke(item);
#if UNITY_EDITOR
                RepaintRequest?.Invoke();
#endif
            }
        }

        /// <summary>
        /// Removes a range of items from the list.
        /// Raises OnItemCountChanged and OnItemsAdded event once, after all items have been added.
        /// </summary>
        /// <param name="index">Starting Index</param>
        /// <param name="count">Amount of Items</param>
        public void RemoveRange(int index, int count)
        {
            var itemsToRemove = _list.GetRange(index, count);

            foreach (var itemToRemove in itemsToRemove)
                _hashSet.Remove(itemToRemove);
            
            _list.RemoveRange(index, count);
            OnItemCountChanged?.Invoke();
            OnItemsRemoved?.Invoke(itemsToRemove);
#if UNITY_EDITOR
            RepaintRequest?.Invoke();
#endif
        }
        
        public bool Contains(T item)
        {
            return _hashSet.Contains(item);
        }
        
        public void Clear()
        {
            _hashSet.Clear();
            _list.Clear();
            OnCleared?.Invoke();
#if UNITY_EDITOR
            RepaintRequest?.Invoke();
#endif
        }

        private void Awake()
        {
            //Prevents from resetting if no reference in a scene
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        private void OnEnable()
        {
            Clear();

            if (_resetOn == ResetType.SceneLoaded)
                SceneManager.sceneLoaded += OnSceneLoaded;
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

        private void OnDisable()
        {
            if (_resetOn == ResetType.SceneLoaded)
                SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Single)
                Clear();
        }

        public override void Reset()
        {
            _resetOn = ResetType.SceneLoaded;
            Clear();
        }
        
#if UNITY_EDITOR
        public void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
                Clear();
        }
#endif

        public void ResetToInitialValue() => Clear();

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public void ForEach(Action<T> action)
        {
            for (var i = _list.Count - 1; i >= 0; i--)
                action(_list[i]);
        }
        
        public List<Object> GetAllObjects()
        {
            var list = new List<Object>(Count);
            list.AddRange(_list.OfType<Object>());
            return list;
        }
    }
}