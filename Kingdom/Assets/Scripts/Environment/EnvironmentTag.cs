using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentTag : MonoBehaviour
{
    public string enviromentName;//环境名称
    public string des;//详细描述
    public string mark;//当前位置备注信息，可以是摊主留下得信息，顾客留下得信息

    //位置信息、可能有多个信息
    public Transform[] point;
}
