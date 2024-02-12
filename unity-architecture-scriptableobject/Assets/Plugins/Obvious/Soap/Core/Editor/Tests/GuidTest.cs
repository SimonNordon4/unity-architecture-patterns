using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor.Tests
{
    public class GuidTest
    {
        [Test]
        public void GuidOnCreate()
        {
            var boolVariable = ScriptableObject.CreateInstance<BoolVariable>();
            const string path = "Assets/boolVariable.asset";
            AssetDatabase.CreateAsset(boolVariable, path);
            Assert.IsNotEmpty(boolVariable.Guid);

            AssetDatabase.DeleteAsset(path);
            Object.DestroyImmediate(boolVariable);
        }

        [Test]
        public void GuidOnRename()
        {
            var boolVariable = ScriptableObject.CreateInstance<BoolVariable>();
            const string path = "Assets/boolVariable.asset";
            AssetDatabase.CreateAsset(boolVariable, path);
            
            AssetDatabase.RenameAsset(path, "renamedBool");
            var newPath = "Assets/renamedBool.asset";
            var renamedAsset = AssetDatabase.LoadAssetAtPath<BoolVariable>(newPath);
            
            Assert.AreEqual(boolVariable.Guid, renamedAsset.Guid);
            AssetDatabase.DeleteAsset(newPath);
            Object.DestroyImmediate(boolVariable);
        }

        [Test]
        public void GuidOnMove()
        {
            var boolVariable = ScriptableObject.CreateInstance<BoolVariable>();
            const string path = "Assets/boolVariable.asset";
            AssetDatabase.CreateAsset(boolVariable, path);

            const string newPath = "Assets/Obvious/boolVariable.asset";
            AssetDatabase.MoveAsset(path, newPath);
            var movedAsset = AssetDatabase.LoadAssetAtPath<BoolVariable>(newPath);
            
            Assert.AreEqual(boolVariable.Guid, movedAsset.Guid);
            
            AssetDatabase.DeleteAsset(newPath);
            Object.DestroyImmediate(boolVariable);
        }

        [Test]
        public void GuidOnDuplicate()
        {
            var boolVariable = ScriptableObject.CreateInstance<BoolVariable>();
            const string path = "Assets/boolVariable.asset";
            AssetDatabase.CreateAsset(boolVariable, path);

            const string copyPath = "Assets/boolVariable1.asset";
            AssetDatabase.CopyAsset(path, copyPath);
            var copy = AssetDatabase.LoadAssetAtPath<BoolVariable>(copyPath);

            Assert.AreNotEqual(boolVariable.Guid, copy.Guid);

            AssetDatabase.DeleteAsset(path);
            AssetDatabase.DeleteAsset(copyPath);
            Object.DestroyImmediate(boolVariable);
            Object.DestroyImmediate(copy);
        }
    }
}