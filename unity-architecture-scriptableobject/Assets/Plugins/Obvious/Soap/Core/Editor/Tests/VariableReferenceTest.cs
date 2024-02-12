using NUnit.Framework;
using UnityEngine;

namespace Obvious.Soap.Editor.Tests
{
    public class VariableReferenceTest
    {
        [Test]
        public void UseLocalValue()
        {
            var intReference = new IntReference
            {
                UseLocal = true,
                LocalValue = 100,
                Variable = ScriptableObject.CreateInstance<IntVariable>()
            };

            Assert.AreEqual(100, intReference.Value);
        }

        [Test]
        public void UseVariableValue()
        {
            var intReference = new IntReference
            {
                UseLocal = false,
                LocalValue = 100,
                Variable = ScriptableObject.CreateInstance<IntVariable>()
            };

            intReference.Variable.Value = 1000;
            Assert.AreEqual(1000, intReference.Value);
        }
        
        [Test]
        public void SetValue()
        {
            var intReference = new IntReference
            {
                UseLocal = false,
                LocalValue = 100,
                Variable = ScriptableObject.CreateInstance<IntVariable>()
            };

            intReference.Value = 1000;
            Assert.AreEqual(1000, intReference.Variable.Value);
            
            intReference.UseLocal = true;
            intReference.Value = 1000;
            Assert.AreEqual(1000, intReference.LocalValue);
        }
    }
}