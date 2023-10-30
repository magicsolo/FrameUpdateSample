using System;
using Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public class ViewPlayer:MonoBehaviour
    {
        public LogicPlayer lgPlayer;
        
        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, lgPlayer.pos.ToVector(),0.5f);
            transform.rotation = Quaternion.Lerp(transform.rotation,lgPlayer.rot.ToQuaternion(),0.5f);
        }
    }
}