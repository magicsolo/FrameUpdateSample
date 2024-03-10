using System;
using Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public class ViewPlayer:MonoBehaviour
    {
        private Animator animator;
        private SpriteRenderer sprite;
        public string curAnimState;
        public int slot;
        public int guid;
        [NonSerialized]
        public int playAnimTime = -1;

        private Vector4 skillCheckArea;
        [SerializeField]
        private ViewSkillInfo skillInfoAnimObj;
        
        private void OnDrawGizmos()
        {
            if (skillInfoAnimObj==null)
            {
                return;
            }
            skillCheckArea = skillInfoAnimObj.skillInfo;

            var offset = new Vector3(skillCheckArea.x, skillCheckArea.y,0);

            var worldPos = transform.localToWorldMatrix*(offset);
            var color = Color.green;
            
            Gizmos.color = color;
            Gizmos.DrawSphere(worldPos,skillCheckArea.z);
        }

        public void BindObj()
        {
            skillInfoAnimObj = transform.GetComponentInChildren<ViewSkillInfo>();
        }
        
        private void Awake()
        {
            animator = GetComponentInChildren<Animator>(true);
            sprite = animator.GetComponent<SpriteRenderer>();
        }

        public void Init(int slot)
        {
            this.slot = slot;
            //guid = ViewModel.instance.PlayerDatas[slot].guid;
        }

        private void Update()
        {
            if (slot >= ViewModel.instance.PlayerDatas.Length)
            {
                gameObject.SetActive(false);
                return;
            }

            PlayerInfo vPlInfo = ViewModel.instance.PlayerDatas[slot];
            var oldPosition = transform.position;
            transform.position = Vector3.Lerp(transform.position, vPlInfo.pos.ToVector(),0.5f);
            var dtMove = transform.position - oldPosition;
            var eulerAngle = vPlInfo.rot.ToQuaternion().eulerAngles;
            //transform.rotation = Quaternion.Lerp(transform.rotation,vPlInfo.rot.ToQuaternion(),0.5f); 
            
            var aniInfo = vPlInfo.aniInfo;

            if (!string.IsNullOrEmpty(aniInfo.stateName)&& playAnimTime!=aniInfo.startTime)
            {
                curAnimState = aniInfo.stateName;
                animator.Play(curAnimState, 0, aniInfo.curTime / (float)aniInfo.totalTime);
            }

            if (Math.Abs(dtMove.x)>0)
            {
                sprite.flipX = !vPlInfo.faceRight; //dtMove.x < 0;
            }
        }
    }
}