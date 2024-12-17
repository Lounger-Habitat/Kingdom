using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{

    public static EnvironmentManager Instance;
    
    private Dictionary<string, EnvironmentTag> envDic = new();
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        var envList = GetComponentsInChildren<EnvironmentTag>();
        foreach (var item in envList)
        {
            envDic.Add(item.enviromentName,item);
        }
    }


    public Transform TargetTransform(string envName)
    {
        if (envDic.ContainsKey(envName))
        {
            return envDic[envName].point[0].transform;
        }

        return null;
    }
}
