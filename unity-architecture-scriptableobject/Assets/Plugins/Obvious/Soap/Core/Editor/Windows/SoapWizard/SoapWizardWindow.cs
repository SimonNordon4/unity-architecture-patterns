using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    public class SoapWizardWindow : SoapWindowBase
    {
        protected override string HeaderTitle => "Soap Wizard";

        private Vector2 _scrollPosition = Vector2.zero;
        private Vector2 _scrollPositionCategory = Vector2.zero;
        private Vector2 _itemScrollPosition = Vector2.zero;
        private List<ScriptableBase> _scriptableObjects;
        private ScriptableType _currentType = ScriptableType.All;
        private readonly float _tabWidth = 55f;
        private readonly float _buttonHeight = 40f;
        private Texture[] _icons;
        private readonly Color[] _colors = { Color.gray, Color.cyan, Color.green, Color.yellow, Color.gray };
        private string _searchText = "";
        private UnityEditor.Editor _editor;

        [SerializeField] private string _currentFolderPath = "Assets";
        [SerializeField] private int _selectedScriptableIndex;
        [SerializeField] private int _typeTabIndex = -1;
        [SerializeField] private int _categoryMask = 0;
        [SerializeField] private bool _isInitialized;
        [SerializeField] private ScriptableBase _scriptableBase;
        [SerializeField] private ScriptableBase _previousScriptableBase;
        [SerializeField] private FavoriteData _favoriteData;
        [SerializeField] private bool _categoryAsButtons = false;

        private List<ScriptableBase> Favorites => _favoriteData.Favorites;
        public const string PathKey = "SoapWizard_Path";
        public const string FavoriteKey = "SoapWizard_Favorites";
        public const string CategoriesKey = "SoapWizard_Categories";
        public const string CategoriesLayoutKey = "SoapWizard_CategoriesLayout";

        [Serializable]
        private class FavoriteData
        {
            public List<ScriptableBase> Favorites = new List<ScriptableBase>();
        }

        private enum ScriptableType
        {
            All,
            Variable,
            Event,
            List,
            Favorite
        }

        [MenuItem("Window/Obvious/Soap/Soap Wizard")]
        public new static void Show()
        {
            var window = GetWindow<SoapWizardWindow>(typeof(SceneView));
            window.titleContent = new GUIContent("Soap Wizard", Resources.Load<Texture>("Icons/icon_soapLogo"));
        }

        [MenuItem("Tools/Obvious/Soap/Soap Wizard")]
        private static void OpenSoapWizard() => Show();

        protected override void OnEnable()
        {
            Init();
            if (_isInitialized)
            {
                SelectTab(_typeTabIndex);
                return;
            }

            SelectTab((int)_currentType, true); //default is 0
            _isInitialized = true;
        }

        private void OnDisable()
        {
            var favoriteData = JsonUtility.ToJson(_favoriteData, false);
            EditorPrefs.SetString(FavoriteKey, favoriteData);
            EditorPrefs.SetInt(CategoriesKey, _categoryMask);
            EditorPrefs.SetInt(CategoriesLayoutKey, _categoryAsButtons ? 1 : 0);
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        protected override void Init()
        {
            base.Init();
            LoadAssets();
            LoadSavedData();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void LoadAssets()
        {
            _icons = new Texture[11];
            _icons[0] = EditorGUIUtility.IconContent("Favorite On Icon").image;
            _icons[1] = Resources.Load<Texture>("Icons/icon_scriptableVariable");
            _icons[2] = Resources.Load<Texture>("Icons/icon_scriptableEvent");
            _icons[3] = Resources.Load<Texture>("Icons/icon_scriptableList");
            _icons[4] = EditorGUIUtility.IconContent("Favorite Icon").image;
            _icons[5] = Resources.Load<Texture>("Icons/icon_ping");
            _icons[6] = Resources.Load<Texture>("Icons/icon_edit");
            _icons[7] = Resources.Load<Texture>("Icons/icon_duplicate");
            _icons[8] = Resources.Load<Texture>("Icons/icon_delete");
            _icons[9] = Resources.Load<Texture>("Icons/icon_categoryLayout");
            _icons[10] = EditorGUIUtility.IconContent("Folder Icon").image;
        }

        private void LoadSavedData()
        {
            _currentFolderPath = EditorPrefs.GetString(PathKey, "Assets");
            _favoriteData = new FavoriteData();
            var favoriteDataJson = JsonUtility.ToJson(_favoriteData, false);
            var favoriteData = EditorPrefs.GetString(FavoriteKey, favoriteDataJson);
            JsonUtility.FromJsonOverwrite(favoriteData, _favoriteData);
            
            _categoryMask = EditorPrefs.GetInt(CategoriesKey, 1);
            _categoryAsButtons = EditorPrefs.GetInt(CategoriesLayoutKey, 0) != 0;
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            DrawFolder();
            GUILayout.Space(2);
            SoapInspectorUtils.DrawLine(2);
            GUILayout.Space(2);
            if (_categoryAsButtons)
            {
                DrawCategoryAsButtons();
                GUILayout.Space(2);
                SoapInspectorUtils.DrawLine();
            }

            EditorGUILayout.BeginHorizontal();
            DrawLeftPanel();
            DrawRightPanel();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFolder()
        {
            EditorGUILayout.BeginHorizontal();
            var buttonContent = new GUIContent(_icons[10], "Change Selected Folder");
            var buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.margin = new RectOffset(2, 2, 0, 0);
            if (GUILayout.Button(buttonContent,buttonStyle,GUILayout.Height(20f), GUILayout.MaxWidth(40)))
            {
                var path = EditorUtility.OpenFolderPanel("Select folder to set path.", _currentFolderPath, "");

                //remove Application.dataPath from path & replace \ with / for cross platform compatibility
                path = path.Replace(Application.dataPath, "Assets").Replace("\\", "/");

                if (!AssetDatabase.IsValidFolder(path))
                    EditorUtility.DisplayDialog("Error: File Path Invalid",
                        "Make sure the path is a valid folder in the project.", "Ok");
                else
                {
                    _currentFolderPath = path;
                    EditorPrefs.SetString(PathKey, _currentFolderPath);
                    OnTabSelected(_currentType, true);
                }
            }

            EditorGUILayout.LabelField(_currentFolderPath, GUI.skin.textField);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCategoryHeader()
        {
            GUILayout.Space(2);
            var buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.margin = new RectOffset(2, 2, 0, 0);
            buttonStyle.padding = new RectOffset(4, 4, 4, 4);
            var buttonContent = new GUIContent(_icons[9], "Switch Categories Layout");
            if (GUILayout.Button(buttonContent,buttonStyle, GUILayout.MaxWidth(25), GUILayout.MaxHeight(20)))
                _categoryAsButtons = !_categoryAsButtons;
            
            buttonContent = new GUIContent(_icons[6], "Edit Categories");
            if (GUILayout.Button(buttonContent,buttonStyle, GUILayout.MaxWidth(25), GUILayout.MaxHeight(20)))
                PopupWindow.Show(new Rect(), new CategoryPopUpWindow(position));
            EditorGUILayout.LabelField("Categories", GUILayout.MaxWidth(70));
        }
        
        private void DrawCategoryAsLayerMask()
        {
            var height = 20f;
            var width = _tabWidth * 5 + 5f;
            EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(height), GUILayout.MaxWidth(width));  
            DrawCategoryHeader();
            var categories = SoapEditorUtils.GetOrCreateSoapSettings().Categories.ToArray();
            _categoryMask = EditorGUILayout.MaskField(_categoryMask, categories);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCategoryAsButtons()
        {
            var height = 20f;
            var categories = SoapEditorUtils.GetOrCreateSoapSettings().Categories;
            EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(height));
            DrawCategoryHeader();
            var buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.margin = new RectOffset(2, 2, 0, 0);
            if (GUILayout.Button("Nothing",buttonStyle, GUILayout.Width(74),GUILayout.Height(height)))
            {
                _categoryMask = 0;
            }
            if (GUILayout.Button("Everything",buttonStyle, GUILayout.Width(74f),GUILayout.Height(height)))
            {
                _categoryMask = (1 << categories.Count) - 1;
            }

            GUILayout.Space(5f);
            _scrollPositionCategory = EditorGUILayout.BeginScrollView(_scrollPositionCategory);
            EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(height));
            
            buttonStyle = new GUIStyle(EditorStyles.toolbarButton);
            buttonStyle.margin = new RectOffset(2, 2, 0, 0);
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.onHover.textColor = Color.white;
            for (int i = 0; i < categories.Count; i++)
            {
                var originalColor = GUI.backgroundColor;
                var isSelected = (_categoryMask & (1 << i)) != 0;
                GUI.backgroundColor = isSelected ? SoapEditorUtils.SoapColor.Lighten(.5f) : Color.gray;
                if (GUILayout.Button(categories[i],buttonStyle,GUILayout.Height(height)))
                {
                    _categoryMask ^= 1 << i; //toggle the bit
                }

                GUI.backgroundColor = originalColor;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            GUILayout.Space(2);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTabs()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(2);
            var width = _tabWidth * 5 + 2f; //offset to match

            var style = new GUIStyle(EditorStyles.toolbarButton);
            _typeTabIndex = GUILayout.Toolbar(_typeTabIndex, Enum.GetNames(typeof(ScriptableType)), style,
                GUILayout.MaxWidth(width));

            if (_typeTabIndex != (int)_currentType)
                OnTabSelected((ScriptableType)_typeTabIndex, true);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSearchBar()
        {
            var width = _tabWidth * 5 + 4f;
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(width));
            _searchText = GUILayout.TextField(_searchText, EditorStyles.toolbarSearchField);
            if (GUILayout.Button("", GUI.skin.FindStyle("SearchCancelButton")))
            {
                _searchText = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawLeftPanel()
        {
            EditorGUILayout.BeginVertical();
            if (!_categoryAsButtons)
            {
                DrawCategoryAsLayerMask();
                GUILayout.Space(2);
                SoapInspectorUtils.DrawLine();
            }

            DrawTabs();
            DrawSearchBar();
            var width = _tabWidth * 5f;

            var color = GUI.backgroundColor;
            GUI.backgroundColor = _colors[(int)_currentType];
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(width));
            GUI.backgroundColor = color;
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DrawScriptableBases(_scriptableObjects);

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Create New Type", GUILayout.MaxHeight(_buttonHeight)))
                PopupWindow.Show(new Rect(), new CreateTypePopUpWindow(position));

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private void DrawScriptableBases(List<ScriptableBase> scriptables)
        {
            if (scriptables is null)
                return;

            var count = scriptables.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                var scriptable = scriptables[i];
                if (scriptable == null)
                    continue;

                //filter category
                if ((_categoryMask & (1 << scriptable.CategoryIndex)) == 0)
                    continue;
                
                //filter search
                if (scriptable.name.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                EditorGUILayout.BeginHorizontal();

                var icon = _currentType == ScriptableType.All || _currentType == ScriptableType.Favorite
                    ? GetIconFor(scriptable)
                    : _icons[(int)_currentType];

                var style = new GUIStyle(GUIStyle.none);
                style.contentOffset = new Vector2(0, 5);
                GUILayout.Box(icon, style, GUILayout.Width(18), GUILayout.Height(18));

                var clicked = GUILayout.Toggle(_selectedScriptableIndex == i, scriptable.name, Skin.button,
                    GUILayout.ExpandWidth(true));
                DrawFavorite(scriptable);

                EditorGUILayout.EndHorizontal();
                if (clicked)
                {
                    _selectedScriptableIndex = i;
                    _scriptableBase = scriptable;
                }
            }
        }

        private void DrawFavorite(ScriptableBase scriptableBase)
        {
            var icon = Favorites.Contains(scriptableBase) ? _icons[4] : _icons[0];
            var style = new GUIStyle(GUIStyle.none);
            style.margin.top = 5;
            if (GUILayout.Button(icon, style, GUILayout.Width(18), GUILayout.Height(18)))
            {
                if (Favorites.Contains(scriptableBase))
                    Favorites.Remove(scriptableBase);
                else
                    Favorites.Add(scriptableBase);
            }
        }

        private void DrawRightPanel()
        {
            if (_scriptableBase == null)
                return;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            _itemScrollPosition = EditorGUILayout.BeginScrollView(_itemScrollPosition, GUILayout.ExpandHeight(true));
            
            DrawUtilityButtons();

            //Draw Selected Scriptable
            if (_editor == null || _scriptableBase != _previousScriptableBase)
            {
                if (_previousScriptableBase != null)
                    _previousScriptableBase.RepaintRequest -= OnRepaintRequested;
                UnityEditor.Editor.CreateCachedEditor(_scriptableBase, null, ref _editor);
                _previousScriptableBase = _scriptableBase;
                _scriptableBase.RepaintRequest += OnRepaintRequested;
            }

            _editor.DrawHeader();
            _editor.OnInspectorGUI();
            SoapInspectorUtils.DrawLine();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawUtilityButtons()
        {
            EditorGUILayout.BeginHorizontal();
            var buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding = new RectOffset(8, 8, 8, 8);
            var buttonContent = new GUIContent(_icons[5], "Select in Project");
            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.MaxHeight(_buttonHeight)))
            {
                Selection.activeObject = _scriptableBase;
                EditorGUIUtility.PingObject(_scriptableBase);
            }

            buttonContent = new GUIContent(_icons[6], "Rename");
            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.MaxHeight(_buttonHeight)))
                PopupWindow.Show(new Rect(), new RenamePopUpWindow(position, _scriptableBase));

            buttonContent = new GUIContent(_icons[7], "Create Copy");
            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.MaxHeight(_buttonHeight)))
            {
                SoapEditorUtils.CreateCopy(_scriptableBase);
                Refresh(_currentType);
            }


            buttonContent = new GUIContent(_icons[8], "Delete");
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red.Lighten(.75f);
            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.MaxHeight(_buttonHeight)))
            {
                var isDeleted = SoapEditorUtils.DeleteObjectWithConfirmation(_scriptableBase);
                if (isDeleted)
                {
                    _scriptableBase = null;
                    OnTabSelected(_currentType, true);
                }
            }

            GUI.backgroundColor = originalColor;
            EditorGUILayout.EndHorizontal();
        }

        private void OnTabSelected(ScriptableType type, bool deselectCurrent = false)
        {
            Refresh(type);
            _currentType = type;
            if (deselectCurrent)
            {
                _selectedScriptableIndex = -1;
                _scriptableBase = null;
            }
        }

        private void Refresh(ScriptableType type)
        {
            switch (type)
            {
                case ScriptableType.All:
                    _scriptableObjects = SoapEditorUtils.FindAll<ScriptableBase>(_currentFolderPath);
                    break;
                case ScriptableType.Variable:
                    var variables = SoapEditorUtils.FindAll<ScriptableVariableBase>(_currentFolderPath);
                    _scriptableObjects = variables.Cast<ScriptableBase>().ToList();
                    break;
                case ScriptableType.Event:
                    var events = SoapEditorUtils.FindAll<ScriptableEventBase>(_currentFolderPath);
                    _scriptableObjects = events.Cast<ScriptableBase>().ToList();
                    break;
                case ScriptableType.List:
                    var lists = SoapEditorUtils.FindAll<ScriptableListBase>(_currentFolderPath);
                    _scriptableObjects = lists.Cast<ScriptableBase>().ToList();
                    break;
                case ScriptableType.Favorite:
                    _scriptableObjects = Favorites;
                    break;
            }
        }

        private void SelectTab(int index, bool deselect = false)
        {
            _typeTabIndex = index;
            OnTabSelected((ScriptableType)_typeTabIndex, deselect);
        }

        private Texture GetIconFor(ScriptableBase scriptableBase)
        {
            var iconIndex = 0;
            switch (scriptableBase)
            {
                case ScriptableVariableBase _:
                    iconIndex = 1;
                    break;
                case ScriptableEventBase _:
                    iconIndex = 2;
                    break;
                case ScriptableListBase _:
                    iconIndex = 3;
                    break;
            }

            return _icons[iconIndex];
        }

        #region Repaint

        private void OnPlayModeStateChanged(PlayModeStateChange pm)
        {
            if (_scriptableBase == null)
                return;
            if (pm == PlayModeStateChange.EnteredPlayMode)
                _scriptableBase.RepaintRequest += OnRepaintRequested;
            else if (pm == PlayModeStateChange.EnteredEditMode)
                _scriptableBase.RepaintRequest -= OnRepaintRequested;
        }

        private void OnRepaintRequested()
        {
            //Debug.Log("Repaint Wizard " + _scriptableBase.name);
            Repaint();
        }

        #endregion
    }
}