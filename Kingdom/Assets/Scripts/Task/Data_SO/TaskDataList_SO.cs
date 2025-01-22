using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TaskDataList_SO", menuName = "Task/TaskDataList")]
public class TaskDataList_SO : ScriptableObject
{
    public List<TaskDetail> taskList;

    public TaskDetail GetTaskDetail(int ID)
    {
        return taskList.Find(item => item.ID == ID);
    }

    public void ModifyTaskData(TaskDetail taskDetail)
    {
        for (int i = 0; i < taskList.Count; i++)
        {
            if (taskDetail.ID ==taskList[i].ID )
            {
                taskList[i] = taskDetail;
                return;
            }
        }
    }
}
