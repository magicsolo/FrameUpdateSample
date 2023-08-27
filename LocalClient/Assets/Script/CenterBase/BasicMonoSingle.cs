using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//对象单例
public class BasicMonoSingle<T> : MonoBehaviour where T :BasicMonoSingle<T>
{
    public static T instance;

    protected void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            string errMssg = $"multiple MonoSingle {gameObject.name} {typeof(T).Name}";
            Debug.LogError(errMssg);
            throw new Exception($"errMssg");
        }
        OnAwake();
    }

    public virtual void OnAwake()
    {
        
    }
}
