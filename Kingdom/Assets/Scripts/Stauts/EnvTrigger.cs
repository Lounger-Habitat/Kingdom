using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class EnvTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 如果碰撞体的物体是玩家
        if (other.CompareTag("Player"))
        {
            // 获取玩家的NPCFuction组件
            var agentController = other.GetComponent<AgentController>();
            // 如果玩家的NPCFuction组件不为空
            if (agentController != null)
            {
                // 调用玩家的OpenShop方法
                // agentController.Jump();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 如果碰撞体的物体是玩家
        if (other.CompareTag("Player"))
        {
            // 获取玩家的NPCFuction组件
            var agentController = other.GetComponent<AgentController>();
            // 如果玩家的NPCFuction组件不为空
            if (agentController != null)
            {
                // agentController.CloseShop();
            }
        }
    }
}