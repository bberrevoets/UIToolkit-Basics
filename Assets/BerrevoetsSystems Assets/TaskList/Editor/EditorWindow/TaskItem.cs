// =====================================================================
// Author: Bert Berrevoets (bberr)
// 
// Created  : 26-03-2022    21:18
// Updated  : 26-03-2022    21:26
// 
// Solution: UIToolkit Basics
// Project:  Assembly-CSharp-Editor
// Filename: TaskItem.cs
// =====================================================================

using UnityEditor;

using UnityEngine.UIElements;

namespace BerrevoetsSystems.Tasks
{
    public class TaskItem : VisualElement
    {
        private readonly Toggle _taskToggle;
        private readonly Label _taskLabel;

        public TaskItem(string taskText)
        {
            var original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TaskListEditor.Path + "TaskItem.uxml");
            Add(original.Instantiate());

            _taskToggle = this.Q<Toggle>();
            _taskLabel = this.Q<Label>();

            _taskLabel.text = taskText;
        }

        public Toggle GetTaskToggle()
        {
            return _taskToggle;
        }

        public Label GetTaskLabel()
        {
            return _taskLabel;
        }
    }
}
