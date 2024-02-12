using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Obvious.Soap.Editor.Tests
{
    public class ScriptableListTest
    {
        private ScriptableListGameObject _gameObjectScriptableList = null;

        [SetUp]
        public void SetUp()
        {
            _gameObjectScriptableList = ScriptableObject.CreateInstance<ScriptableListGameObject>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObjectScriptableList);
        }

        [Test]
        public void OnItemAddedEvent()
        {
            _gameObjectScriptableList.ResetToInitialValue();
            var eventCalledCount = 0;
            _gameObjectScriptableList.OnItemAdded += (go) => eventCalledCount++;

            for (int i = 0; i < 5; i++)
            {
                var go = new GameObject($"GameObject{i}");
                _gameObjectScriptableList.Add(go);
            }

            Assert.AreEqual(5, eventCalledCount);
        }

        [Test]
        public void OnItemsAddedEvent()
        {
            _gameObjectScriptableList.ResetToInitialValue();
            var eventCalledCount = 0;
            _gameObjectScriptableList.OnItemsAdded += (go) => eventCalledCount++;

            var goList = new List<GameObject>();
            for (int i = 0; i < 5; i++)
            {
                var go = new GameObject($"GameObject{i}");
                goList.Add(go);
            }

            _gameObjectScriptableList.AddRange(goList);

            Assert.AreEqual(1, eventCalledCount);
        }

        [Test]
        public void OnItemRemoveEvent()
        {
            _gameObjectScriptableList.ResetToInitialValue();
            for (int i = 0; i < 5; i++)
            {
                var go = new GameObject($"GameObject{i}");
                _gameObjectScriptableList.Add(go);
            }

            var eventCalledCount = 5;
            _gameObjectScriptableList.OnItemRemoved += (go) => eventCalledCount--;

            for (int i = _gameObjectScriptableList.Count - 1; i >= 0; i--)
            {
                var go = _gameObjectScriptableList[i];
                _gameObjectScriptableList.Remove(go);
            }

            Assert.AreEqual(0, eventCalledCount);
        }

        [Test]
        public void OnItemsRemovedEvent()
        {
            _gameObjectScriptableList.ResetToInitialValue();
            var eventCalledCount = 0;

            for (int i = 0; i < 5; i++)
            {
                var go = new GameObject($"GameObject{i}");
                _gameObjectScriptableList.Add(go);
            }

            _gameObjectScriptableList.OnItemsRemoved += (go) => eventCalledCount++;
            _gameObjectScriptableList.RemoveRange(0, 5);
            Assert.AreEqual(1, eventCalledCount);
        }

        [Test]
        public void OnItemCountChangeEvent()
        {
            _gameObjectScriptableList.ResetToInitialValue();

            var eventCalledCount = 0;
            _gameObjectScriptableList.OnItemCountChanged += () => eventCalledCount++;

            for (int i = 0; i < 5; i++)
            {
                var go = new GameObject($"GameObject{i}");
                _gameObjectScriptableList.Add(go);
            }

            for (int i = _gameObjectScriptableList.Count - 1; i >= 0; i--)
            {
                var go = _gameObjectScriptableList[i];
                _gameObjectScriptableList.Remove(go);
            }

            Assert.AreEqual(10, eventCalledCount);
            Assert.AreEqual(0, _gameObjectScriptableList.Count);
        }

        [Test]
        public void OnClearEvent()
        {
            _gameObjectScriptableList.ResetToInitialValue();
            for (int i = 0; i < 5; i++)
            {
                var go = new GameObject($"GameObject{i}");
                _gameObjectScriptableList.Add(go);
            }

            var itemCount = _gameObjectScriptableList.Count;

            _gameObjectScriptableList.OnCleared += () => itemCount = _gameObjectScriptableList.Count;
            _gameObjectScriptableList.ResetToInitialValue();

            Assert.AreEqual(0, itemCount);
        }

        [Test]
        public void AddSingle()
        {
            _gameObjectScriptableList.ResetToInitialValue();
            var gameObject = new GameObject("GameObject");
            _gameObjectScriptableList.Add(gameObject);
            Assert.IsTrue(_gameObjectScriptableList.Contains(gameObject));
            _gameObjectScriptableList.Add(gameObject);
            Assert.AreEqual(1, _gameObjectScriptableList.Count);
        }

        [Test]
        public void RemoveSingle()
        {
            _gameObjectScriptableList.ResetToInitialValue();
            var gameObject = new GameObject("GameObject");
            _gameObjectScriptableList.Add(gameObject);
            Assert.IsTrue(_gameObjectScriptableList.Contains(gameObject));
            _gameObjectScriptableList.Remove(gameObject);
            Assert.IsFalse(_gameObjectScriptableList.Contains(gameObject));
            
            try
            {
                _gameObjectScriptableList.Remove(gameObject);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
            
            Assert.IsFalse(_gameObjectScriptableList.Contains(gameObject));
        }

        [Test]
        public void AddRange()
        {
            _gameObjectScriptableList.ResetToInitialValue();
            var targetCount = 5;
            var gameObjects = new List<GameObject>(targetCount);
            for (int i = 0; i < targetCount; i++)
                gameObjects.Add(new GameObject($"GameObject{i}"));

            _gameObjectScriptableList.AddRange(gameObjects);
            Assert.AreEqual(targetCount, _gameObjectScriptableList.Count);
        }

        [Test]
        public void RemoveRange()
        {
            _gameObjectScriptableList.ResetToInitialValue();
            var targetCount = 5;
            var gameObjects = new List<GameObject>(targetCount);
            for (int i = 0; i < targetCount; i++)
                gameObjects.Add(new GameObject($"GameObject{i}"));

            _gameObjectScriptableList.AddRange(gameObjects);
            Assert.AreEqual(targetCount, _gameObjectScriptableList.Count);
            _gameObjectScriptableList.RemoveRange(0, targetCount);
            Assert.AreEqual(0, _gameObjectScriptableList.Count);
        }
    }
}