using System;
using Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public class ViewPlayer:MonoBehaviour
    {
        public int slot;
        public void Init(int slot)
        {
            this.slot = slot;
        }

        private void Update()
        {
            if (slot >= ViewModel.instance.PlayerDatas.Length)
            {
                gameObject.SetActive(false);
                return;
            }

            PlayerInfo vPlInfo = ViewModel.instance.PlayerDatas[slot];
            transform.position = Vector3.Lerp(transform.position, vPlInfo.pos.ToVector(),0.5f);
            transform.rotation = Quaternion.Lerp(transform.rotation,vPlInfo.rot.ToQuaternion(),0.5f);
        }
    }
}