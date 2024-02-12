using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Obvious.Soap.Editor.Tests
{
    public class TypeCreationTest
    {
        private readonly string _path = "Assets/SoapGenerated/";

        [UnityTest]
        public IEnumerator CreateCustomType()
        {
           var typeTexts = new[] { "double", "Inventory", "_iLovePurple" };
            var result = TryCreateClasses(typeTexts);

            Assert.AreEqual(true, result);
            yield return new WaitForDomainReload();

            LogAssert.NoUnexpectedReceived();

            //Clean up
            var folderPath = _path.Substring(0, _path.Length - 1);
            FileUtil.DeleteFileOrDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        private bool TryCreateClasses(string[] typeTexts)
        {
            TextAsset newFile = null;
            var targetAmount = typeTexts.Length * 4 + 2; //2 classes that need to be created.
            var filesCreated = 0;

            foreach (var typeText in typeTexts)
            {
                if (!SoapTypeUtils.IsBuiltInType(typeText))
                {
                    if (SoapEditorUtils.CreateClassFromTemplate("NewTypeTemplate.cs", typeText, _path, out newFile))
                        filesCreated++;
                }
                
                if (SoapEditorUtils.CreateClassFromTemplate("ScriptableVariableTemplate.cs", typeText, _path, out newFile,true))
                    filesCreated++;

                if (SoapEditorUtils.CreateClassFromTemplate("ScriptableEventTemplate.cs", typeText, _path, out newFile,true))
                    filesCreated++;

                if (SoapEditorUtils.CreateClassFromTemplate("EventListenerTemplate.cs", typeText, _path, out newFile,true))
                    filesCreated++;

                if (SoapEditorUtils.CreateClassFromTemplate("ScriptableListTemplate.cs", typeText, _path, out newFile,true))
                    filesCreated++;
            }

            return filesCreated == targetAmount;
        }
        
        [Test]
        public void IsTypeNameValid()
        {
            var typeTexts = new[] { ";BadClass", "#698Spaceship", "1stClass" };
            foreach (var typeText in typeTexts)
            {
                var result = SoapTypeUtils.IsTypeNameValid(typeText);
                Assert.AreEqual(result, false);
            }
        }

        [Test]
        public void IsTypeNameBuiltIn()
        {
            var typeTexts = new[] { "double", "long", "string", "Transform", "Inventory" };
            var count = 0;
            foreach (var typeText in typeTexts)
            {
                var result = SoapTypeUtils.IsBuiltInType(typeText);
                if (result)
                    count++;
            }

            Assert.AreEqual(count, 3);
        }
        
        [Test]
        public void CreateInvalidCustomType()
        {
            var typeTexts = new[] { ";BadClass", "#698Spaceship", "1stClass" };
            var result = TryCreateClasses(typeTexts);
            Assert.AreEqual(result, false);
        }
        
    }
}