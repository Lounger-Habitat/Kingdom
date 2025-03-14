using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using DG.Tweening;

public class AgentControllerBak : MonoBehaviour
{
    public bool isGround = true;
    public string testJson = @"{
    ""Actions"": [
        {
            ""ActionName"": ""Move"",
            ""Params"": [
                ""Env-Cube 0""
            ]
        },
        {
            ""ActionName"": ""Move"",
            ""Params"": [
                ""Env-Cube 2""
            ]
        },
        {
            ""ActionName"": ""Move"",
            ""Params"": [
                ""Env-Cube 1""
            ]
        },
        {
            ""ActionName"": ""Move"",
            ""Params"": [
                ""Env-Cube 3""
            ]
        },
        {
            ""ActionName"": ""Jump"",
            ""Params"": [
                ""10""
            ]
        }
    ]";
    void Update()
    {

        CheckGround();

        // WASD控制移动
        if (Input.GetKey(KeyCode.W))
        {
            Move(Vector3.forward * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Move(Vector3.back * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Move(Vector3.left * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Move(Vector3.right * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            TestTodoList();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadAgentAction();
        }

    }

    private void CheckGround()
    {
        // 检测是否在地面上
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }

    public void Move(Vector3 direction)
    {
        // 移动
        transform.Translate(direction);
    }
    public IEnumerator MoveTarget(string target)
    {
        // action 开始
        // action 执行
        // action 结束
        // 根据名字获取物体transform
        GameObject go = GameObject.Find(target);
        if (go != null)
        {
            // 移动
            transform.DOMove(go.transform.position, 1);
        }
        yield return new WaitUntil(() => transform.position == go.transform.position);
    }
    public IEnumerator Jump(float force = 5)
    {
        // 给一个向上的力
        GetComponent<Rigidbody>().AddForce(Vector3.up * force, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => isGround);
    }

    public void LoadAgentAction()
    {
        // 从文件中读取agent的行为
        string json = System.IO.File.ReadAllText("MessageQueue/AgentAction.json");
        ActionList actionList = JsonConvert.DeserializeObject<ActionList>(json);
        StartCoroutine(ExecuteTasks(actionList));
    }

    public void TestTodoList()
    {
        // 解析json
        ActionList actionList = JsonConvert.DeserializeObject<ActionList>(testJson);
        StartCoroutine(ExecuteTasks(actionList));
    }

    IEnumerator ExecuteTasks(ActionList actionList)
    {
        foreach (var action in actionList.Actions)
        {
            Debug.Log($"开始执行任务{action.ActionName}");
            yield return StartCoroutine(ExecuteSingleTask(action));
            Debug.Log($"任务{action.ActionName}执行完毕");
        }
        Debug.Log("所有任务执行完毕");
    }

    IEnumerator ExecuteSingleTask(ActionWithParams action)
    {
        switch (action.ActionName)
        {
            case "Move":
                yield return MoveTarget(action.Params[0]);
                break;
            case "Jump":
                yield return Jump(action.Params.Count > 0 ? float.Parse(action.Params[0]) : 5);
                break;
            default:
                break;
        }
        Debug.Log($"任务{action.ActionName}的执行逻辑");
    }



}

[Serializable]
public class ActionList
{
    public List<ActionWithParams> Actions;
}

[Serializable]
public class ActionWithParams
{
    public string ActionName;
    public List<string> Params;


}