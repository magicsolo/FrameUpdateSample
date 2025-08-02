///整型图形定义
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

namespace FPPhysic
{
    //平面
    public struct FPPlane
    {
        public TSVector normal;
        public TSVector center;
    }
    
    //
    public struct FPLine
    {
        public TSVector point;
        public TSVector direction;
    }

    //立方体
    struct FPCube
    {
        private TSVector xDir;
        private TSVector yDir;
        private TSVector zDir;
        private TSQuaternion rotation;
    }
    
    
}