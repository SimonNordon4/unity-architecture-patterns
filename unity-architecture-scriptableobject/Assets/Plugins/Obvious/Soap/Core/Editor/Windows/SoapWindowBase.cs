using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    public class SoapWindowBase : EditorWindow
    {
        private Texture[] _baseIcons;
        private GUIStyle _bgStyle;
        protected GUISkin Skin;

        protected virtual string HeaderTitle { get; }

        protected virtual void OnEnable()
        {
            Init();
        }

        protected virtual void Init()
        {
            Skin = Resources.Load<GUISkin>("GUISkins/SoapWizardGUISkin");
            _bgStyle = new GUIStyle(GUIStyle.none);
            _baseIcons = new Texture[2];
            _baseIcons[0] = Resources.Load<Texture>("Icons/icon_soapLogo");
            _baseIcons[1] = Resources.Load<Texture>("Icons/icon_obviousLogo");
        }

        protected virtual void OnGUI()
        {
            DrawHeader();
            GUILayout.Space(2);
        }

        private void DrawHeader()
        {
            _bgStyle.normal.background = SoapInspectorUtils.CreateTexture(SoapEditorUtils.SoapColor);
            GUILayout.BeginVertical(_bgStyle);
            GUILayout.BeginHorizontal();
            GUILayout.BeginHorizontal();
            var iconStyle = new GUIStyle(GUIStyle.none);
            iconStyle.margin = new RectOffset(5, 0, 0, 0);
            GUILayout.Box(_baseIcons[0], iconStyle, GUILayout.Width(60), GUILayout.Height(60));

            var titleStyle = Skin.customStyles.ToList().Find(x => x.name == "title");
            EditorGUILayout.LabelField(HeaderTitle, titleStyle);

            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            var copyrightStyle = Skin.customStyles.ToList().Find(x => x.name == "copyright");
            GUILayout.Label("© 2021 Obvious Game", copyrightStyle);
            iconStyle.padding = new RectOffset(0, 1, 35, 1);
            GUILayout.Box(_baseIcons[1], iconStyle, GUILayout.Width(25), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}