// =====================================================================
// Author: Bert Berrevoets (bberr)
// 
// Created  : 26-03-2022    12:41
// Updated  : 26-03-2022    21:54
// 
// Solution: UIToolkit Basics
// Project:  Assembly-CSharp-Editor
// Filename: TaskListEditor.cs
// =====================================================================

using System.Collections.Generic;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace BerrevoetsSystems.Tasks
{
    public class TaskListEditor : EditorWindow
    {
        public const string Path = "Assets/BerrevoetsSystems Assets/TaskList/Editor/EditorWindow/";
        private Button _addTaskButton;
        private VisualElement _container;
        private Button _loadTasksButton;
        private Label _notificationLabel;
        private ObjectField _savedTasksObjectField;
        private Button _saveProgressButton;
        private ToolbarSearchField _searchBox;
        private ScrollView _taskListScrollView;
        private TaskListSO _taskListSO;
        private ProgressBar _taskProgressBar;
        private TextField _taskText;

        private void CreateGUI()
        {
            _container = rootVisualElement;
            var original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Path + "TaskListEditor.uxml");
            _container.Add(original.Instantiate());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Path + "TaskListEditor.uss");
            _container.styleSheets.Add(styleSheet);

            _savedTasksObjectField = _container.Q<ObjectField>("savedTasksObjectField");
            _savedTasksObjectField.objectType = typeof(TaskListSO);

            _loadTasksButton = _container.Q<Button>("loadTasksButton");
            _loadTasksButton.clicked += LoadTasks;

            _taskText = _container.Q<TextField>("taskText");
            _taskText.RegisterCallback<KeyDownEvent>(AddTask);

            _addTaskButton = _container.Q<Button>("addTaskButton");
            _addTaskButton.clicked += AddTask;

            _taskListScrollView = _container.Q<ScrollView>("taskList");

            _saveProgressButton = _container.Q<Button>("saveProgressButton");
            _saveProgressButton.clicked += SaveProgress;

            _taskProgressBar = _container.Q<ProgressBar>("taskProgressBar");

            _searchBox = _container.Q<ToolbarSearchField>("searchBox");
            _searchBox.RegisterValueChangedCallback(OnSearchTextChanged);

            _notificationLabel = _container.Q<Label>("notificationLabel");

            UpdateNotifications("Please load a task list to continue.");
        }

        private void OnSearchTextChanged(ChangeEvent<string> changeEvent)
        {
            var searchText = changeEvent.newValue.ToUpper();
            foreach (var visualElement in _taskListScrollView.Children())
            {
                var task = (TaskItem) visualElement;
                var taskText = task.GetTaskLabel().text.ToUpper();

                if (!string.IsNullOrEmpty(searchText) && taskText.Contains(searchText))
                {
                    task.GetTaskLabel().AddToClassList("highlight");
                }
                else
                {
                    task.GetTaskLabel().RemoveFromClassList("highlight");
                }
            }
        }

        private void SaveProgress()
        {
            if (_taskListSO == null) return;

            List<string> tasks = new();

            foreach (var visualElement in _taskListScrollView.Children())
            {
                var task = (TaskItem) visualElement;
                if (!task.GetTaskToggle().value)
                {
                    tasks.Add(task.GetTaskLabel().text);
                }
            }

            _taskListSO.AddTasks(tasks);
            EditorUtility.SetDirty(_taskListSO);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            LoadTasks();
            UpdateNotifications("Tasks saved successfully.");
        }

        private void AddTask()
        {
            if (string.IsNullOrEmpty(_taskText.value)) return;

            _taskListScrollView.Add(CreateTask(_taskText.value));
            SaveTask(_taskText.value);
            _taskText.value = "";
            _taskText.Focus();
            UpdateProgress();
            UpdateNotifications("Task added successfully.");
        }

        private void SaveTask(string task)
        {
            _taskListSO.AddTask(task);
            EditorUtility.SetDirty(_taskListSO);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UpdateNotifications("Task added successfully.");
        }

        private TaskItem CreateTask(string task)
        {
            var taskItem = new TaskItem(task);
            taskItem.GetTaskToggle().RegisterValueChangedCallback(UpdateProgress);
            return taskItem;
        }

        private void AddTask(KeyDownEvent e)
        {
            if (Event.current.Equals(Event.KeyboardEvent("Return")))
            {
                AddTask();
            }
        }

        private void LoadTasks()
        {
            _taskListSO = _savedTasksObjectField.value as TaskListSO;

            if (_taskListSO == null)
            {
                _taskListScrollView.Clear();
                UpdateNotifications("Failed to load task list.");
                return;
            }

            _taskListScrollView.Clear();
            List<string> tasks = _taskListSO.GetTasks();
            foreach (var task in tasks)
            {
                _taskListScrollView.Add(CreateTask(task));
            }

            _taskText.value = "";
            _taskText.Focus();
            UpdateProgress();
            UpdateNotifications($"{_taskListSO.name} successfully loaded.");
        }

        private void UpdateProgress(ChangeEvent<bool> e)
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            var count = 0;
            var completed = 0;

            foreach (var visualElement in _taskListScrollView.Children())
            {
                var task = (TaskItem) visualElement;
                if (task.GetTaskToggle().value)
                {
                    completed++;
                }

                count++;
            }

            if (count > 0)
                _taskProgressBar.value = completed / (float) count;
            else
                _taskProgressBar.value = 1;

            _taskProgressBar.title = $"{_taskProgressBar.value * 100:F1} %";
            UpdateNotifications("Progress updated. Don't forget to save!");
        }

        private void UpdateNotifications(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            _notificationLabel.text = text;
        }

        [MenuItem("BerrevoetsSystems/Task List")]
        public static void ShowWindow()
        {
            var window = GetWindow<TaskListEditor>();
            window.titleContent = new GUIContent("Task List");
        }
    }
}
