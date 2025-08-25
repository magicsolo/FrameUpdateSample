using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCamera : MonoBehaviour
{
    public Transform target;
    [SerializeField] 
    private Vector3 offset;

    [SerializeField] 
    private float roate;
    private void LateUpdate()
    {
        if (target == null)
            return;
        transform.position = target.position + Quaternion.Euler(new Vector3(0,roate,0)) * offset;
        transform.transform.LookAt(target.transform.position);
    }
}
