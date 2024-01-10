using UnityEngine;

namespace GameObjectComponent.Definitions
{
    [CreateAssetMenu(fileName = "ChestItem", menuName = "GameObjectComponent/ChestItem", order = 1)]
    public class ChestItemDefinition : ScriptableObject
    {
        public Sprite sprite;
        public string itemName = "New Item";
        public int tier = 1;
        public Modifier[] modifiers;

        #if UNITY_EDITOR
        [ContextMenu("Rename Image")]
        public void RenameImage()
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(sprite);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            // rename image to the name of this scriptable object
            UnityEditor.AssetDatabase.RenameAsset(path, this.name);
        }
        #endif
    }

}