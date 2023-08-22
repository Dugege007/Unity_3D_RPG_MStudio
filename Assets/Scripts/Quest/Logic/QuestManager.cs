using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * 创建人：杜
 * 功能说明：任务管理器
 * 创建时间：
 */

public class QuestManager : Singleton<QuestManager>
{
    [System.Serializable]
    public class QuestTask
    {
        public QuestData_SO questData;
        public bool IsStarted { get { return questData.isStarted; } set { questData.isStarted = value; } }
        public bool IsComplete { get { return questData.isStarted; } set { questData.isStarted = value; } }
        public bool IsFinished { get { return questData.isStarted; } set { questData.isStarted = value; } }
    }

    public List<QuestTask> tasks = new List<QuestTask>();

    private void Start()
    {
        LoadQuestManager();
    }

    public void LoadQuestManager()
    {
        var questCount = PlayerPrefs.GetInt("QuestCount");
        for (int i = 0; i < questCount; i++)
        {
            var newQuest = ScriptableObject.CreateInstance<QuestData_SO>();
            SaveManager.Instance.Load(newQuest, "task" + i);
            tasks.Add(new QuestTask { questData = newQuest });
        }
    }

    public void SaveQuestManager()
    {
        PlayerPrefs.SetInt("QuestCount", tasks.Count);
        for (int i = 0; i < tasks.Count; i++)
        {
            SaveManager.Instance.Save(tasks[i].questData, "task" + i);
        }
    }

    // 敌人死亡时 获取物品时 调用此方法
    public void UpdateQuestProgress(string requireName,int amount)
    {
        foreach(var task in tasks)
        {
            if (task.IsFinished)
            {
                continue;
            }

            var matchTask = task.questData.questRequires.Find(r => r.name == requireName);
            if (matchTask != null)
                matchTask.currentAmount += amount;

            task.questData.CheckQuestProgress();
        }
    }

    public bool HaveQuest(QuestData_SO data)
    {
        if (data!=null)
        {
            // linq 中 .Any() 用来查找列表中是否包含想要的条件
            return tasks.Any(q => q.questData.questName == data.questName);
        }
        return false;
    }

    public QuestTask GetTask(QuestData_SO data)
    {
        // linq 中 .Find() 用于查找
        return tasks.Find(q => q.questData.questName == data.questName);
    }
}
