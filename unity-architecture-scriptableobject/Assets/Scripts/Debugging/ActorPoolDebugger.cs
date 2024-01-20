using System.Collections.Generic;
using GameObjectComponent.Definitions;
using GameplayComponents.Actor;
using Pools;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameObjectComponent.Debugging
{
    public class ActorPoolDebugger : DebugComponent
    {
        [field:SerializeField]public ActorDefinition definition { get; private set; }
        [field:SerializeField]public ActorPool actorPool { get; private set; }
        public readonly Queue<PoolableActor> Actors = new();
        
        private void OnEnable()
        {
            actorPool.OnActorGet += actor => Print("Got " + actor.name);
            actorPool.OnActorReturn += actor => Print("Returned " + actor.name);
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(ActorPoolDebugger))]
    public class ActorPoolDebuggerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var actorPoolDebugger = (ActorPoolDebugger) target;
            var actorPool = actorPoolDebugger.actorPool;
            var definition = actorPoolDebugger.definition;
            
            if (GUILayout.Button("Get"))
            {
                var actor = actorPool.Get(definition, Vector3.zero);
                actorPoolDebugger.Actors.Enqueue(actor);
            }

            if (GUILayout.Button("Return"))
            {
                var actor = actorPoolDebugger.Actors.Dequeue();
                actor.Return();
            }
                
        }
    }
    #endif
}