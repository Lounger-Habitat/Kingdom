using System.Collections.Generic;
using UnityEngine;

public class AgentGroup : Singleton<AgentGroup>
{
   //AgentGroup，管理所有场景里的角色，目前不管理测试用角色
    public Dictionary<string,GameObject> agentControllerDic = new ();

    //在开始时，查找物体下所有的AgentController,agentControllerDic,存储为字典，key为名字，value为物体。根据需要在物体上查找组件
    //后续可以在物体上挂载统一的组件，只能通过此组件操作
    private void Start()
    {
        foreach (Transform item in transform)
        {
            agentControllerDic.Add(item.name,item.gameObject);
        }
    }

    //根据名字获取AgentController
    public GameObject GetAgentController(string name)
    {
        if (agentControllerDic.ContainsKey(name))
        {
            return agentControllerDic[name];
        }
        return null;
    }
}
