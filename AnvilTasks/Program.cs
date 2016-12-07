using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Anvil;

namespace AnvilTasks
{
    public class Program
    {
        public static Form MainForm;
        public static int FormWidth = 800, FormHeight = 600;

        public static string Version = "1.0b";
        public static string FormTitle = string.Format("Anvil Tasks (v {0})", Version);

        public static PictureBox GUIPictureBox;

        public static Timer UpdateTimer = new Timer();

        [STAThread]
        public static void Main()
        {
            AnvilCore.Init();
            //
            MainForm = UI.CreateForm(new Rect(0, 0, FormWidth, FormHeight), FormTitle);
            MainForm.Size = new Size(FormWidth, FormHeight);
            MainForm.MinimumSize = new Size(700, 300);
            MainForm.Icon = Properties.Resources.TasksIcon;

            GUIPictureBox = UI.CreatePictureBox(new Rect(0, 0, MainForm.ClientSize.Width, MainForm.ClientSize.Height), UI.Anchor(true, true, true, true), "GUI");

            UI.Append(GUIPictureBox, MainForm);
            //
            App.Start();
            //
            UpdateTimer.Interval = 60;
            UpdateTimer.Tick += delegate {
                GUI.Update(GUIPictureBox);
                App.Update();
                GC.Collect();
            };

            UpdateTimer.Start();
            //
            Application.Run(MainForm);
        }
    }

    public class App
    {
        public static Form EditForm;
        //
        public static List<Task> Tasks = new List<Task>();

        public static Vector2 ScrollOffset = new Vector2(0, 0);

        public static int CurrentTask = -1;
        public static Action OnEditTask = null;

        public static int TasksCount = 0;

        public static void Start()
        {
            Styles.Init();
            //
            LoadTasks();
        }

        public static void Update()
        {
            if (EditForm != null) Input.Enabled = !EditForm.Visible;
            else Input.Enabled = true;

            GUI.Clear(GUIColors.White);

            //
            if (Tasks.Count != 0)
            {
                int TaskHeight = 65;
                int ScrollHeight = TaskHeight * TasksCount;

                ScrollOffset = GUI.BeginScrollView(new Rect(0, 0, GUI.Width - 230, GUI.Height), new Rect(0, 0, GUI.Width - 230, ScrollHeight), ScrollOffset);

                int offset = 0;
                int count = 0;

                TasksCount = 0;
                foreach (Task T in Tasks)
                {
                    if (T.Deleted == 0)
                    {
                        Rect TaskArea = new Rect(0, offset * TaskHeight, GUI.Width - 230, TaskHeight);
                        Rect ClickTaskArea = new Rect(0, offset * TaskHeight - ScrollOffset.Y, GUI.Width - 230 - 8, TaskHeight);
                        GUI.BeginArea(TaskArea);

                        if (CurrentTask == count)
                        {
                            GUI.DrawQuad(new Rect(0, 0, 5, TaskHeight), GUIColors.Orange);
                        }

                        GUI.Label(new Rect(7, 7, 550, 30), T.Title, Styles.TaskTitle);
                        GUI.Label(new Rect(9, 37, 550, 30), T.Description, Styles.TaskDescription);

                        if (T.State == 0)
                        {
                            GUI.Box(new Rect(GUI.Width - 230 - 115, 15, 100, 35), "Not solved", Styles.TaskNotSolved);
                        }
                        if (T.State == 1)
                        {
                            GUI.Box(new Rect(GUI.Width - 230 - 115, 15, 100, 35), "Solved", Styles.TaskSolved);
                        }
                        if (T.State == 2)
                        {
                            GUI.Box(new Rect(GUI.Width - 230 - 115, 15, 100, 35), "Processing", Styles.TaskProcessing);
                        }

                        GUI.Box(new Rect(0, TaskHeight - 1, GUI.Width - 230, 1), "", Styles.Divider);

                        GUI.EndArea();

                        if (Input.Hover(ClickTaskArea) && Input.GetMouseLeftDown())
                        {
                            if (CurrentTask != count) CurrentTask = count;
                            else CurrentTask = -1;
                        }

                        //

                        offset++;
                        TasksCount++;
                    }

                    count++;
                }

                GUI.EndScrollView();
            }
            else
            {
                GUI.Label(new Rect(10, 10, 500, 500), "No tasks yet");
            }

            GUI.Box(new Rect(GUI.Width - 230, 0, 1, GUI.Height), "", Styles.Divider);


            if (GUI.Button(new Rect(GUI.Width - 215, 15, 200, 50), "New task"))
            {
                SetupEditForm();

                OnEditTask = delegate {
                    AddTask(UI.Controls["TaskTitle"].Text, UI.Controls["TaskDescription"].Text);
                };
                //UI.Controls["FormPanel"].Visible = !UI.Controls["FormPanel"].Visible;
                EditForm.Visible = !EditForm.Visible;
            }

            if (CurrentTask >= 0) //if selected any task
            {
                Task T = Tasks[CurrentTask];

                GUI.Box(new Rect(GUI.Width - 230, 80, 230, 1), "", Styles.Divider);

                if (GUI.Button(new Rect(GUI.Width - 215, 150, 200, 40), "Edit"))
                {
                    SetupEditForm();

                    UI.Controls["TaskTitle"].Text = T.Title;
                    UI.Controls["TaskDescription"].Text = T.Description;

                    OnEditTask = delegate {
                        EditTask(CurrentTask, UI.Controls["TaskTitle"].Text, UI.Controls["TaskDescription"].Text, T.State, T.Deleted);
                    };
                    //UI.Controls["FormPanel"].Visible = !UI.Controls["FormPanel"].Visible;
                    EditForm.Visible = !EditForm.Visible;
                }

                if (T.State == 0)
                {
                    if (GUI.Button(new Rect(GUI.Width - 215, 95, 200, 40), "Processing", Styles.YellowButton))
                    {
                        EditTask(CurrentTask, T.Title, T.Description, 2, T.Deleted);
                    }
                }
                else 
                if (T.State == 2)
                {
                    if (GUI.Button(new Rect(GUI.Width - 215, 95, 200, 40), "Make as solved", Styles.GreenButton))
                    {
                        EditTask(CurrentTask, T.Title, T.Description, 1, T.Deleted);
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(GUI.Width - 215, 95, 200, 40), "Not solved", Styles.RedButton))
                    {
                        EditTask(CurrentTask, T.Title, T.Description, 0, T.Deleted);
                    }
                }

                if (GUI.Button(new Rect(GUI.Width - 215, 205, 200, 40), "Delete", Styles.RedButton))
                {
                    EditTask(CurrentTask, T.Title, T.Description, T.State, 1);
                }
            }
        }

        //

        public static void SetupEditForm()
        {
            EditForm = UI.CreateForm(new Rect(0, 0, 420, 260), "Task");
            EditForm.BackColor = Color.White;
            EditForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            EditForm.MaximizeBox = false;
            EditForm.Icon = Properties.Resources.TasksIcon;

            UI.Append(UI.CreateLabel(new Rect(10, 20, 360, 25), UI.DefaultAnchor, "TaskTitleL", "Title"), EditForm);
            UI.Append(UI.CreateTextBox(new Rect(20, 55, 360, 25), UI.DefaultAnchor, "TaskTitle", "", 200), EditForm);
            UI.Append(UI.CreateLabel(new Rect(10, 90, 360, 25), UI.DefaultAnchor, "TaskDescriptionL", "Description"), EditForm);
            UI.Append(UI.CreateTextBox(new Rect(20, 125, 360, 25), UI.DefaultAnchor, "TaskDescription", "", 200), EditForm);

            UI.Append(UI.CreateButton(new Rect(140, 165, 120, 40), UI.DefaultAnchor, "TaskButton", "Apply", delegate {
                OnEditTask();
                //
                UI.Controls["TaskTitle"].Text = "";
                UI.Controls["TaskDescription"].Text = "";
                EditForm.Visible = !EditForm.Visible;
            }), EditForm);
        }

        //

        public static void AddTask(string title, string description)
        {
            Tasks.Reverse();

            Tasks.Add(new Task
            {
                Title = title,
                Description = description,
                State = 0,
                Deleted = 0,
            });

            Tasks.Reverse();

            SaveTasks();
        }

        public static void EditTask(int at, string title, string description, int state, int deleted)
        {
            Tasks[at] = new Task
            {
                Title = title,
                Description = description,
                State = state,
                Deleted = deleted,
            };

            SaveTasks();
        }

        public static void SaveTasks()
        {
            for (int i = 0; i < Tasks.Count; i++)
            {
                Task T = Tasks[i];

                DataPrefs.SetString("TaskTitle" + i, T.Title);
                DataPrefs.SetString("TaskDescription" + i, T.Description);
                DataPrefs.SetInt("TaskState" + i, T.State);
                DataPrefs.SetInt("TaskDeleted" + i, T.Deleted);
            }

            DataPrefs.SetInt("TasksNumber", Tasks.Count);
        }

        public static void LoadTasks()
        {
            int number = DataPrefs.GetInt("TasksNumber", 0);

            for (int i = 0; i < number; i++)
            {
                string title = DataPrefs.GetString("TaskTitle" + i, "N/A");
                string description = DataPrefs.GetString("TaskDescription" + i, "N/A");
                int state = DataPrefs.GetInt("TaskState" + i, 0);
                int deleted = DataPrefs.GetInt("TaskDeleted" + i, 0);

                Tasks.Add(new Task
                {
                    Title = title,
                    Description = description,
                    State = state,
                    Deleted = deleted,
                });
            }
        }

        public struct Task
        {
            public string Title;
            public string Description;
            public int State;
            public int Deleted;
        }

        public class Styles
        {
            public static GUIStyle TaskTitle = new GUIStyle
            {
                FontSize = 15,
            };
            public static GUIStyle TaskDescription = new GUIStyle
            {
                FontSize = 10,
            };
            public static GUIStyle TaskSolved = new GUIStyle
            {
                FontSize = 12,
                NormalBackgroundColor = GUIColors.Green,
                NormalColor = GUIColors.White,
                BorderRadius = 5,
            };
            public static GUIStyle TaskProcessing = new GUIStyle
            {
                FontSize = 12,
                NormalBackgroundColor = Color.FromArgb(255, 252, 206, 84),
                NormalColor = GUIColors.White,
                BorderRadius = 5,
            };
            public static GUIStyle TaskNotSolved = new GUIStyle
            {
                FontSize = 12,
                NormalBackgroundColor = GUIColors.Red,
                NormalColor = GUIColors.White,
                BorderRadius = 5,
            };
            public static GUIStyle Divider = new GUIStyle
            {
                NormalBackgroundColor = GUIColors.Gray,
            };
            public static GUIStyle GreenButton = GUISkin.CreateButton();
            public static GUIStyle YellowButton = GUISkin.CreateButton();
            public static GUIStyle RedButton = GUISkin.CreateButton();

            public static void Init()
            {
                GreenButton.NormalBackgroundColor = GUIColors.Green;
                GreenButton.HoverBackgroundColor = GUIColors.GreenHover;
                GreenButton.ActiveBackgroundColor = GUIColors.Green;

                YellowButton.NormalBackgroundColor = Color.FromArgb(255, 252, 206, 84);
                YellowButton.HoverBackgroundColor = Color.FromArgb(255, 252, 226, 94);
                YellowButton.ActiveBackgroundColor = Color.FromArgb(255, 252, 206, 84);

                RedButton.NormalBackgroundColor = GUIColors.Red;
                RedButton.HoverBackgroundColor = GUIColors.RedHover;
                RedButton.ActiveBackgroundColor = GUIColors.Red;
            }
        }
    }
}
