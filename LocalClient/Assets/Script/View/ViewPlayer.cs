using System;
using Game;
using Script;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FrameDrive
{
    public struct ViewPlayerInfo
    {
        public PlayerInfo info;
        
        public Vector3 pos;
        public Quaternion rot;
        public FrameInputData FrameInputData;
        public PlayAnimInfo aniInfo;

        public ViewPlayerInfo(PlayerInfo info,PlayerLogicData data)
        {
            this.info = info;
            
            pos = data.pos.ToVector();
            rot = data.rot.ToQuaternion();
            FrameInputData = data.inputData;
            aniInfo = data.aniInfo;
        }
    }
    public class ViewPlayer:MonoBehaviour
    {
        private Animator animator;
        public string curAnimState;
        public int slot;

        ViewPlayerInfo viewInfo =>
            slot < ViewModel.instance.playerInfos.Length ? ViewModel.instance.playerInfos[slot] : default; 

        public int guid;
        [NonSerialized]
        public int playAnimTime = -1;

        private Vector4 skillCheckPos;
        private Vector4 skillCheckArea;
        [SerializeField]
        private ViewSkillInfo skillInfoAnimObj;

        public Transform hud;
        public Text nameTxt;
        
        private void OnDrawGizmos()
        {
            if (skillInfoAnimObj==null)
            {
                return;
            }
            skillCheckArea = skillInfoAnimObj.area;
            skillCheckPos = skillInfoAnimObj.pos;

            var offset = new Vector3(skillCheckPos.x, skillCheckPos.y,skillCheckPos.z);

            var worldPos = offset;
            var color = Color.green;
            color.a = 0.7f;
            Gizmos.color = color;
            Gizmos.matrix = transform.localToWorldMatrix;// Matrix4x4.Rotate(transform.rotation);// transform.rotation.ToRotationMatrix(); ///new Matrix4x4(transform.rotation);// transform.rotation.;
            Gizmos.DrawCube(worldPos,skillCheckArea);//DrawSphere(worldPos,skillCheckArea.z);
        }

        public void BindObj()
        {
            skillInfoAnimObj = transform.GetComponentInChildren<ViewSkillInfo>();
            hud = transform.Find("View/HUDCanvas");
            nameTxt = hud.Find("_name_txt").GetComponent<Text>();
        }
        
        private void Awake()
        {
            animator = GetComponentInChildren<Animator>(true);
        }
        
        public void Init(int slot)
        {
            this.slot = slot;
            bool isView = slot < ViewModel.instance.playerInfos.Length;
            
            gameObject.SetActive(isView);
            if (!isView)
                return;
            var plInfo = viewInfo;
            nameTxt.text = plInfo.info.playerName;
            nameTxt.color = plInfo.info.guid == ClientManager.instance.guid? Color.green : Color.red;
            //guid = ViewModel.instance.PlayerDatas[slot].guid;
        }
        private void Update()
        {
            if (slot >= ViewModel.instance.playerInfos.Length)
            {
                gameObject.SetActive(false);
                return;
            }
            //
            ViewPlayerInfo vPlInfo = ViewModel.instance.GetPlayerInfo(slot);
            var oldPosition = transform.position;
            transform.position = Vector3.Lerp(transform.position, vPlInfo.pos,0.5f);
            var dtMove = transform.position - oldPosition;
            var eulerAngle = vPlInfo.rot.eulerAngles;
            transform.rotation = Quaternion.Lerp(transform.rotation,vPlInfo.rot,0.5f); 
            
            var aniInfo = vPlInfo.aniInfo;
            
            if (curAnimState != aniInfo.stateName || playAnimTime != aniInfo.startFrame)
            {
                playAnimTime = aniInfo.startFrame;
                curAnimState = aniInfo.stateName;
                animator.Play(curAnimState, 0, aniInfo.curFrame / (float)aniInfo.totalFrame);
            }
            //
            if (CameraManager.instance.mainCamera != null)
            {
                hud.rotation = CameraManager.instance.mainCamera.transform.rotation;
            }

        }
    }
}