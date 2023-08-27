using System;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;


public class CharacterModel : MonoBehaviour
{
    public FP speed = 1f;
    public TSTransform tsTrans;

    private void Awake()
    {
        tsTrans = GetComponent<TSTransform>();
    }

    public void UpdateInput(EInputEnum input)
    {
        if (tsTrans == null)
            return;
        
        TSVector dir = TSVector.zero;

        switch (input)
        {
            case EInputEnum.downDir:
                dir = TSVector.back;
                break;
            case EInputEnum.leftDir:
                dir = TSVector.left;
                break;
            case EInputEnum.rightDir:
                dir = TSVector.right;
                break;
            case EInputEnum.upDir:
                dir = TSVector.forward;
                break;
        }

        dir *= speed * FrameManager.instance.frameTime;
        tsTrans.position += dir;
    }
}
