using UnityEngine;

namespace Dna
{
    public class ScriptableDataTest : ScriptableData
    {
        private int _testIntInitial;
        [SerializeField] private int testInt;


        protected override void Init()
        {
            Debug.Log("Init");
            _testIntInitial = testInt;
        }

        protected override void ResetData()
        {
            Debug.Log("ResetData");
            testInt = _testIntInitial;
        }
    }
}