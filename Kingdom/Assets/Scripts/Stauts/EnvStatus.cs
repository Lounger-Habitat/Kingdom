using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Collections;

public class EnvManager : MonoBehaviour
{
    List<Transform> envTransforms;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 初始化当前场景下的环境
        InitEnv();
    }

    private void InitEnv()
    {
        // 获取当前物体下的所有环境子对象物体
        envTransforms = transform
            .GetComponentsInChildren<Transform>(true)
            .Where(t => t != transform && t.gameObject.name.Contains("Env-"))
            .ToList();

        // 遍历所有环境子对象物体
        foreach (var env in envTransforms)
        {
            // 获取环境子对象物体下的所有碰撞体
            var colliders = env.GetComponentsInChildren<Collider>(true);
            // 遍历所有碰撞体
            foreach (var collider in colliders)
            {
                // 设置碰撞体为触发器
                collider.isTrigger = true;
                // 添加碰撞体的触发器事件
                collider.gameObject.AddComponent<EnvTrigger>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveEnvStatus();
        }
    }

    // 保存环境信息
    void SaveEnvStatus(){
        // 保存环境信息到文件里，使用json格式、
        // 环境信息包括，环境中物体的名字、位置、旋转、缩放
        // 保存的文件名为EnvStatus.json

        // 创建一个空的环境信息列表
        List<GoStatus> goStatusList = new List<GoStatus>();
        // 遍历所有环境子对象物体
        foreach (Transform go in envTransforms)
        {
            // 创建一个新的环境信息
            GoStatus goStatus = new GoStatus();
            // 设置环境信息的名字
            goStatus.name = go.name;
            // 设置环境信息的位置
            goStatus.position = new Pos(go.position.x, go.position.y, go.position.z);
            goStatusList.Add(goStatus);
        }
        // 将环境信息列表转换为json字符串
        string json = JsonConvert.SerializeObject(goStatusList);
        // 将json字符串保存到文件
        if (!System.IO.Directory.Exists("MessageQueue"))
        {
            System.IO.Directory.CreateDirectory("MessageQueue");
        }
        System.IO.File.WriteAllText("MessageQueue/EnvStatus.json", json);

        Debug.Log("SaveEnvStatus");
    }
}

[Serializable]
public class GoStatus
{
    public string name;
    public Pos position;

}

public class Pos
{
    public float x;
    public float y;
    public float z;

    public Pos(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}