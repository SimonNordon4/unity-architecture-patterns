using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
    [CustomEditor(typeof(ChestItems))]
    public class ChestItemsEditor : Editor
    {
        private SerializedProperty chestTableName;
        private SerializedProperty chestItems;
        private ReorderableList chestItemsList;

        private void OnEnable()
        {
            chestTableName = serializedObject.FindProperty("chestTableName");
            chestItems = serializedObject.FindProperty("chestItems");

            chestItemsList = new ReorderableList(serializedObject, chestItems, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Chest Items");
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = chestItems.GetArrayElementAtIndex(index);
                    var itemName = element.FindPropertyRelative("itemName");
                    var sprite = element.FindPropertyRelative("sprite");
                    var modifiers = element.FindPropertyRelative("modifiers");

                    // Display item name and editable sprite thumbnail
                    float iconSize = 80f;
                    var spriteRect = new Rect(rect.x, rect.y, iconSize, iconSize);
                    var nameRect = new Rect(rect.x + iconSize + 4, rect.y, rect.width - iconSize - 4, EditorGUIUtility.singleLineHeight);

                    // Display sprite thumbnail with ObjectField for easy replacement
                    sprite.objectReferenceValue = EditorGUI.ObjectField(spriteRect, sprite.objectReferenceValue, typeof(Sprite), false);
                    EditorGUI.PropertyField(nameRect, itemName, GUIContent.none);

                    // Display Modifiers in a nested list with add/remove functionality
                    if (modifiers.isArray)
                    {
                        rect.y += EditorGUIUtility.singleLineHeight + 5;

                        for (int i = 0; i < modifiers.arraySize; i++)
                        {
                            var modifier = modifiers.GetArrayElementAtIndex(i);
                            var statType = modifier.FindPropertyRelative("statType");
                            var modifierType = modifier.FindPropertyRelative("modifierType");
                            var modifierValue = modifier.FindPropertyRelative("modifierValue");

                            // Layout for each Modifier
                            var statRect = new Rect(rect.x + iconSize + 4, rect.y, (rect.width - iconSize - 4) * 0.3f, EditorGUIUtility.singleLineHeight);
                            var typeRect = new Rect(statRect.xMax + 4, rect.y, (rect.width - iconSize - 4) * 0.3f, EditorGUIUtility.singleLineHeight);
                            var valueRect = new Rect(typeRect.xMax + 4, rect.y, (rect.width - iconSize - 4) * 0.3f, EditorGUIUtility.singleLineHeight);

                            EditorGUI.PropertyField(statRect, statType, GUIContent.none);
                            EditorGUI.PropertyField(typeRect, modifierType, GUIContent.none);

                            // Check ModifierType to display as percentage if needed
                            if ((ModifierType)modifierType.enumValueIndex == ModifierType.Percentage)
                            {
                                modifierValue.floatValue = EditorGUI.Slider(valueRect, modifierValue.floatValue * 100f, 0f, 100f) / 100f;
                            }
                            else
                            {
                                EditorGUI.PropertyField(valueRect, modifierValue, GUIContent.none);
                            }

                            // Add button to remove individual modifiers
                            var removeButtonRect = new Rect(valueRect.xMax + 4, rect.y, 20, EditorGUIUtility.singleLineHeight);
                            if (GUI.Button(removeButtonRect, "X"))
                            {
                                modifiers.DeleteArrayElementAtIndex(i);
                            }

                            rect.y += EditorGUIUtility.singleLineHeight + 2;
                        }

                        // Add button to append a new modifier
                        var addButtonRect = new Rect(rect.x + iconSize + 4, rect.y, rect.width - iconSize - 4, EditorGUIUtility.singleLineHeight);
                        if (GUI.Button(addButtonRect, "Add Modifier"))
                        {
                            modifiers.arraySize++;
                        }
                    }
                },
                elementHeightCallback = index =>
                {
                    var element = chestItems.GetArrayElementAtIndex(index);
                    var modifiers = element.FindPropertyRelative("modifiers");
                    return (modifiers.arraySize + 2) * (EditorGUIUtility.singleLineHeight + 2) + 20; // Adjust for item name and sprite thumbnail
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(chestTableName);
            chestItemsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
