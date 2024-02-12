using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    public class CreateTypePopUpWindow : PopupWindowContent
    {
        private readonly GUISkin _skin;
        private readonly Rect _position;
        private string _typeText = "Type";
        private bool _baseClass = false;
        private bool _monoBehaviour = true;
        private bool _variable = true;
        private bool _event = true;
        private bool _eventListener = true;
        private bool _list = true;
        private bool _invalidTypeName;
        private string _path;
        private readonly Vector2 _dimensions = new Vector2(350, 350);
        private readonly GUIStyle _bgStyle;
        private Texture[] _icons;

        public override Vector2 GetWindowSize() => _dimensions;

        public CreateTypePopUpWindow(Rect origin)
        {
            _position = origin;
            _skin = Resources.Load<GUISkin>("GUISkins/SoapWizardGUISkin");
            _bgStyle = new GUIStyle(GUIStyle.none);
            _bgStyle.normal.background = SoapInspectorUtils.CreateTexture(SoapEditorUtils.SoapColor);
            LoadIcons();
        }

        private void LoadIcons()
        {
            _icons = new Texture[5];
            _icons[0] = EditorGUIUtility.IconContent("cs Script Icon").image;
            _icons[1] = Resources.Load<Texture>("Icons/icon_scriptableVariable");
            _icons[2] = Resources.Load<Texture>("Icons/icon_scriptableEvent");
            _icons[3] = Resources.Load<Texture>("Icons/icon_eventListener");
            _icons[4] = Resources.Load<Texture>("Icons/icon_scriptableList");
        }

        public override void OnGUI(Rect rect)
        {
            editorWindow.position = SoapInspectorUtils.CenterInWindow(editorWindow.position, _position);
            DrawTitle();
            DrawTextField();
            DrawTypeToggles();
            GUILayout.Space(10);
            DrawPath();
            DrawButtons();
        }

        private void DrawPath()
        {
            EditorGUILayout.LabelField("Selected path:", EditorStyles.boldLabel);
            var guiStyle = new GUIStyle(EditorStyles.label);
            guiStyle.fontStyle = FontStyle.Italic;
            _path = SoapFileUtils.GetSelectedFolderPathInProjectWindow();
            EditorGUILayout.LabelField($"{_path}", guiStyle);
        }

        private void DrawTextField()
        {
            EditorGUI.BeginChangeCheck();
            _typeText = EditorGUILayout.TextField(_typeText, _skin.textField);
            if (EditorGUI.EndChangeCheck())
            {
                _invalidTypeName = !SoapTypeUtils.IsTypeNameValid(_typeText);
            }

            //Draw Invalid Feedback Text
            var guiStyle = new GUIStyle(EditorStyles.label);
            guiStyle.normal.textColor = _invalidTypeName ? SoapEditorUtils.SoapColor : Color.white;
            guiStyle.fontStyle = FontStyle.Bold;
            var errorMessage = _invalidTypeName ? "Invalid type name." : "";
            EditorGUILayout.LabelField(errorMessage, guiStyle);
        }

        private void DrawTypeToggles()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            var capitalizedType = $"{_typeText.CapitalizeFirstLetter()}";
            if (!SoapTypeUtils.IsBuiltInType(_typeText))
            {
                DrawToggle(ref _baseClass, $"{capitalizedType}", "", 0, true,140);
                if (_baseClass)
                    _monoBehaviour = GUILayout.Toggle(_monoBehaviour, "MonoBehaviour?");
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            DrawToggle(ref _variable, $"{capitalizedType}", "Variable", 1, true,140);
            GUILayout.Space(5);
            DrawToggle(ref _event, "ScriptableEvent", $"{capitalizedType}", 2);
            GUILayout.Space(5);
            DrawToggle(ref _eventListener, "EventListener", $"{capitalizedType}", 3);
            GUILayout.Space(5);
            DrawToggle(ref _list, "ScriptableList", $"{capitalizedType}", 4);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTitle()
        {
            GUILayout.BeginVertical(_bgStyle);
            var titleStyle = _skin.customStyles.ToList().Find(x => x.name == "title");
            EditorGUILayout.LabelField("Create new Type", titleStyle);
            GUILayout.EndVertical();
        }

        private void DrawButtons()
        {
            GUI.enabled = !_invalidTypeName;
            if (GUILayout.Button("Create", GUILayout.ExpandHeight(true)))
            {
                if (!SoapTypeUtils.IsTypeNameValid(_typeText))
                    return;

                TextAsset newFile = null;
                var progress = 0f;
                EditorUtility.DisplayProgressBar("Progress", "Start", progress);

                if (_baseClass && !SoapTypeUtils.IsBuiltInType(_typeText))
                {
                    var templateName = _monoBehaviour ? "NewTypeMonoTemplate.cs" : "NewTypeTemplate.cs";
                    if (!SoapEditorUtils.CreateClassFromTemplate(templateName, _typeText, _path, out newFile))
                    {
                        Close();
                        return;
                    }
                }

                progress += 0.2f;
                EditorUtility.DisplayProgressBar("Progress", "Generating...", progress);

                if (_variable)
                {
                    if (!SoapEditorUtils.CreateClassFromTemplate("ScriptableVariableTemplate.cs", _typeText, _path,
                            out newFile, true))
                    {
                        Close();
                        return;
                    }
                }

                progress += 0.2f;
                EditorUtility.DisplayProgressBar("Progress", "Generating...", progress);

                if (_event)
                {
                    if (!SoapEditorUtils.CreateClassFromTemplate("ScriptableEventTemplate.cs", _typeText, _path,
                            out newFile, true))
                    {
                        Close();
                        return;
                    }
                }

                progress += 0.2f;
                EditorUtility.DisplayProgressBar("Progress", "Generating...", progress);

                if (_eventListener)
                {
                    if (!SoapEditorUtils.CreateClassFromTemplate("EventListenerTemplate.cs", _typeText, _path,
                            out newFile, true))
                    {
                        Close();
                        return;
                    }
                }

                progress += 0.2f;
                EditorUtility.DisplayProgressBar("Progress", "Generating...", progress);

                if (_list)
                {
                    if (!SoapEditorUtils.CreateClassFromTemplate("ScriptableListTemplate.cs", _typeText, _path,
                            out newFile, true))
                    {
                        Close();
                        return;
                    }
                }

                progress += 0.2f;
                EditorUtility.DisplayProgressBar("Progress", "Completed!", progress);

                EditorUtility.DisplayDialog("Success", $"{_typeText} was created!", "OK");
                Close(false);
                EditorGUIUtility.PingObject(newFile);
            }

            GUI.enabled = true;
            if (GUILayout.Button("Cancel", GUILayout.ExpandHeight(true)))
            {
                editorWindow.Close();
            }
        }

        private void Close(bool hasError = true)
        {
            EditorUtility.ClearProgressBar();
            editorWindow.Close();
            if (hasError)
                EditorUtility.DisplayDialog("Error", $"Failed to create {_typeText}", "OK");
        }


        private void DrawToggle(ref bool toggleValue, string typeName, string second, int iconIndex,
            bool isFirstRed = false, int maxWidth = 200)
        {
            EditorGUILayout.BeginHorizontal();
            var icon = _icons[iconIndex];
            var style = new GUIStyle(GUIStyle.none);
            GUILayout.Box(icon, style, GUILayout.Width(18), GUILayout.Height(18));
            toggleValue = GUILayout.Toggle(toggleValue, "", GUILayout.Width(maxWidth));
            GUIStyle firstStyle = new GUIStyle(GUI.skin.label);
            firstStyle.padding.left = 15-maxWidth;
            if (isFirstRed)
                firstStyle.normal.textColor = SoapEditorUtils.SoapColor;
            GUILayout.Label(typeName, firstStyle);
            GUIStyle secondStyle = new GUIStyle(GUI.skin.label);
            secondStyle.padding.left = -6;
            if (!isFirstRed)
                secondStyle.normal.textColor = SoapEditorUtils.SoapColor;
            GUILayout.Label(second, secondStyle);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}