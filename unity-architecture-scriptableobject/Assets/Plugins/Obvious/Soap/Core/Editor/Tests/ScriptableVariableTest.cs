using NUnit.Framework;
using UnityEngine;

namespace Obvious.Soap.Editor.Tests
{
    public class ScriptableVariableTest
    {
        private IntVariable _intVariable = null;

        [SetUp]
        public void SetUp()
        {
            _intVariable = ScriptableObject.CreateInstance<IntVariable>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_intVariable);
        }

        [Test]
        public void OnValueChangedEventTriggeredOnlyOnce()
        {
            var eventCount = 0;
            _intVariable.OnValueChanged += (value) => eventCount++;

            _intVariable.Value = 13;
            _intVariable.Value = 13;
            Assert.AreEqual(1, eventCount);
        }

        [Test]
        public void Add()
        {
            _intVariable.Value = 0;
            _intVariable.Add(20);
            Assert.AreEqual(20, _intVariable.Value);
        }

        [Test]
        public void PreviousValue()
        {
            var diff = 0;
            _intVariable.Value = 10;
            _intVariable.OnValueChanged += (newValue) => diff = newValue - _intVariable.PreviousValue;
            _intVariable.Value = 100;
            Assert.AreEqual(90, diff);
        }

        [Test]
        public void AssignToNull()
        {
            var gameObjectVariable = ScriptableObject.CreateInstance<GameObjectVariable>();

            var go = new GameObject("GameObject");
            gameObjectVariable.Value = go;
            Assert.AreEqual(go, gameObjectVariable.Value);

            Assert.DoesNotThrow(() => { gameObjectVariable.Value = null; });
            Assert.AreEqual(null, gameObjectVariable.Value);

            var go2 = new GameObject("GameObject2");
            Assert.DoesNotThrow(() => { gameObjectVariable.Value = go2; });
            Assert.AreEqual(go2, gameObjectVariable.Value);

            Object.DestroyImmediate(go);
            Object.DestroyImmediate(go2);
        }

        [Test]
        public void SaveAndLoad()
        {
            PlayerPrefs.DeleteAll();

            //Int
            _intVariable.Value = 10;
            _intVariable.Save();
            _intVariable.Value = 100;
            _intVariable.Load();
            Assert.AreEqual(10, _intVariable.Value);
            //Default value
            PlayerPrefs.DeleteAll();
            _intVariable.Load();
            Assert.AreEqual(_intVariable.DefaultValue, _intVariable.Value);

            //Float
            var floatVariable = ScriptableObject.CreateInstance<FloatVariable>();
            floatVariable.Value = 1.5f;
            floatVariable.Save();
            floatVariable.Value = 3.5f;
            floatVariable.Load();
            Assert.AreEqual(1.5f, floatVariable.Value);
            PlayerPrefs.DeleteAll();
            floatVariable.Load();
            Assert.AreEqual(_intVariable.DefaultValue, _intVariable.Value);

            //Bool
            var boolVariable = ScriptableObject.CreateInstance<BoolVariable>();
            boolVariable.Value = true;
            boolVariable.Save();
            boolVariable.Value = false;
            boolVariable.Load();
            Assert.AreEqual(true, boolVariable.Value);
            PlayerPrefs.DeleteAll();
            boolVariable.Load();
            Assert.AreEqual(_intVariable.DefaultValue, _intVariable.Value);

            //Color
            var colorVariable = ScriptableObject.CreateInstance<ColorVariable>();
            colorVariable.Value = Color.blue;
            colorVariable.Save();
            colorVariable.Value = Color.green;
            colorVariable.Load();
            Assert.AreEqual(Color.blue, colorVariable.Value);
            PlayerPrefs.DeleteAll();
            colorVariable.Load();
            Assert.AreEqual(_intVariable.DefaultValue, _intVariable.Value);

            //String
            var stringVariable = ScriptableObject.CreateInstance<StringVariable>();
            stringVariable.Value = "berp";
            stringVariable.Save();
            stringVariable.Value = "derp";
            stringVariable.Load();
            Assert.AreEqual("berp", stringVariable.Value);
            PlayerPrefs.DeleteAll();
            stringVariable.Load();
            Assert.AreEqual(_intVariable.DefaultValue, _intVariable.Value);

            //Vector2
            var vector2Variable = ScriptableObject.CreateInstance<Vector2Variable>();
            vector2Variable.Value = Vector2.one;
            vector2Variable.Save();
            vector2Variable.Value = Vector2.zero;
            vector2Variable.Load();
            Assert.AreEqual(Vector2.one, vector2Variable.Value);
            PlayerPrefs.DeleteAll();
            vector2Variable.Load();
            Assert.AreEqual(_intVariable.DefaultValue, _intVariable.Value);

            //Vector3
            var vector3Variable = ScriptableObject.CreateInstance<Vector3Variable>();
            vector3Variable.Value = Vector3.one;
            vector3Variable.Save();
            vector3Variable.Value = Vector3.zero;
            vector3Variable.Load();
            Assert.AreEqual(Vector3.one, vector3Variable.Value);
            PlayerPrefs.DeleteAll();
            vector3Variable.Load();
            Assert.AreEqual(_intVariable.DefaultValue, _intVariable.Value);
            
            Object.DestroyImmediate(floatVariable);
            Object.DestroyImmediate(boolVariable);
            Object.DestroyImmediate(colorVariable);
            Object.DestroyImmediate(stringVariable);
            Object.DestroyImmediate(vector2Variable);
            Object.DestroyImmediate(vector3Variable);
        }
    }
}