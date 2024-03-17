using System;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{

    [Serializable]
    public struct LogicAnimInfo
    {
        public string key;
        public string controllerName;
        public int length;
        public string stateName;
        public int fireFrame;
        public TSVector2 skillCheckPos;
        public TSVector skillCheckArea;
    }
}