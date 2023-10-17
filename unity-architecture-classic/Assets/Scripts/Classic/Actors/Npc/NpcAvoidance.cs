using System;
using System.Collections.Generic;
using UnityEngine;

namespace Classic.Actors.Npc
{
    public class NpcAvoidance : ActorComponent
    {
        private readonly List<Collider> _neighbours = new(4);

        private void OnTriggerEnter(Collider other)
        {
            
            if(other.gameObject.layer != this.gameObject.layer) return;

            if (_neighbours.Count >= 5) return;
            _neighbours.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.gameObject.layer != this.gameObject.layer) return;
            if(_neighbours.Contains(other))
                _neighbours.Remove(other);
        }
    }
}