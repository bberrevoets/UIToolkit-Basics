// =====================================================================
// Author: Bert Berrevoets (bberr)
// 
// Created  : 26-03-2022    12:27
// Updated  : 26-03-2022    17:04
// 
// Solution: UIToolkit Basics
// Project:  Assembly-CSharp-Editor
// Filename: TaskListSO.cs
// =====================================================================

using System.Collections.Generic;

using UnityEngine;

namespace BerrevoetsSystems.Tasks
{
    [CreateAssetMenu(menuName = "Task List", fileName = "New Task List")]
    public class TaskListSO : ScriptableObject
    {
        [SerializeField] private List<string> Tasks = new();

        public List<string> GetTasks()
        {
            return Tasks;
        }

        public void AddTask(string savedTask)
        {
            Tasks.Add(savedTask);
        }

        public void AddTasks(List<string> savedTasks)
        {
            Tasks.Clear();
            Tasks = savedTasks;
        }
    }
}
