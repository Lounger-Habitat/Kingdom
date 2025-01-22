using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Collections;

public class AgentManager : MonoBehaviour
{
    List<Transform> agentTransforms;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 初始化当前场景下的环境
        InitAgent();
    }

    private void InitAgent()
    {
        // 获取当前物体下的所有环境子对象物体
        agentTransforms = transform
            .GetComponentsInChildren<Transform>(true)
            .Where(t => t != transform && t.gameObject.name.Contains("Agent-"))
            .ToList();

        // 遍历所有环境子对象物体
        foreach (var agent in agentTransforms)
        {
            agent.gameObject.AddComponent<AgentController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveAgentStatus();
        }
    }

    // 保存环境信息
    void SaveAgentStatus(){
        // 保存环境信息到文件里，使用json格式、
        // 环境信息包括，环境中物体的名字、位置、旋转、缩放
        // 保存的文件名为EnvStatus.json

        // 创建一个空的环境信息列表
        List<GoStatus> goStatusList = new List<GoStatus>();
        // 遍历所有环境子对象物体
        foreach (Transform t in agentTransforms)
        {
            // 创建一个新的环境信息
            GoStatus goStatus = new GoStatus();
            // 设置环境信息的名字
            goStatus.name = t.name;
            // 设置环境信息的位置
            goStatus.position = new Pos(t.position.x, t.position.y, t.position.z);
            goStatusList.Add(goStatus);
        }
        // 将环境信息列表转换为json字符串
        string json = JsonConvert.SerializeObject(goStatusList);
        // 将json字符串保存到文件
        if (!System.IO.Directory.Exists("MessageQueue"))
        {
            System.IO.Directory.CreateDirectory("MessageQueue");
        }
        System.IO.File.WriteAllText("MessageQueue/AgentStatus.json", json);

        Debug.Log("SaveAgentStatus");
    }
}