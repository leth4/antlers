using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Foundation
{
    public class Console : MonoBehaviour
    {
        private static List<Command> _commands = new();

        private string _command = "";
        private List<Log> _logs = new();

        private bool _isActive = false;
        private Vector2 _scrollPosition;

        private CursorLockMode _startCursorLockMode;

        public static void AddCommand(string prefix, string description, Action callback)
        {
            AddCommand(new Command()
            {
                Prefix = prefix,
                Description = description,
                Types = new Type[] { },
                Callback = _ => callback()
            });
        }

        public static void AddCommand<T1>(string prefix, string description, Action<T1> callback)
        {
            AddCommand(new Command()
            {
                Prefix = prefix,
                Description = description,
                Callback = o => callback((T1)o[0]),
                Types = new Type[] { typeof(T1) }
            });
        }

        public static void AddCommand<T1, T2>(string prefix, string description, Action<T1, T2> callback)
        {
            AddCommand(new Command()
            {
                Prefix = prefix,
                Description = description,
                Callback = o => callback((T1)o[0], (T2)o[1]),
                Types = new Type[] { typeof(T1), typeof(T2) }
            });
        }

        private static void AddCommand(Command command)
        {
            for (int i = 0; i < _commands.Count; i++)
            {
                if (_commands[i].Prefix == command.Prefix)
                {
                    _commands[i] = command;
                    return;
                }
            }
            _commands.Add(command);
        }

        private void OnEnable()
        {
            CreateDefaultCommands();
            Application.logMessageReceived += ReceiveUnityLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= ReceiveUnityLog;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                _isActive = !_isActive;

                if (_isActive)
                {
                    _startCursorLockMode = Cursor.lockState;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.lockState = _startCursorLockMode;
                }
            }
        }

        private void HandleCommand()
        {
            if (_command == "") return;

            string[] words = _command.Split(" ");
            string prefix = words[0];

            PushLog($"→ {_command}", LogType.Log);

            _command = "";

            foreach (var command in _commands)
            {
                if (command.Prefix != prefix) continue;

                var invoked = Invoke(command, words);
                if (invoked) return;

                PushLog(command.Types.Length switch
                {
                    1 => $"Command {command.Prefix} takes a [{command.Types[0].Name}] argument",
                    2 => $"Command {command.Prefix} takes [{command.Types[0].Name}] and [{command.Types[1].Name}] arguments",
                    _ => $"Command {command.Prefix} takes no arguments",
                }, LogType.Error);

                return;
            }

            PushLog($"Command '{prefix}' does not exist!", LogType.Error);
        }

        private bool Invoke(Command command, string[] args)
        {
            if (args.Length - 1 != command.Types.Length) return false;
            var objectArgs = new object[args.Length - 1];
            for (int i = 1; i < args.Length; i++)
            {
                try
                {
                    if (command.Types[i - 1] == typeof(int)) objectArgs[i - 1] = Int32.Parse(args[i]);
                    else if (command.Types[i - 1] == typeof(float)) objectArgs[i - 1] = float.Parse(args[i]);
                    else if (command.Types[i - 1] == typeof(bool)) objectArgs[i - 1] = float.Parse(args[i]);
                    else if (command.Types[i - 1] == typeof(string)) objectArgs[i - 1] = args[i];
                    else return false;
                }
                catch { return false; }
            }
            command.Callback.Invoke(objectArgs);
            return true;
        }

        private void OnGUI()
        {
            if (!_isActive) return;

            if (Event.current.keyCode == KeyCode.Escape)
            {
                Cursor.lockState = _startCursorLockMode;
                _isActive = false;
            }
            if (Event.current.keyCode == KeyCode.DownArrow)
            {
                AutoCompleteCommand();
            }
            if (Event.current.keyCode == KeyCode.Return)
            {
                HandleCommand();
                _scrollPosition.y = 10000;
            }

            var margin = 20f;
            var width = (Screen.width) - (margin * 2);
            var height = (Screen.height) - (margin * 2);
            var windowRect = new Rect(margin, margin, width, height);

            var texture = new Texture2D(1, 1);
            texture.SetPixels(new Color[] { new Color(0, 0, 0, 0.85f) });
            texture.Apply();

            var skin = GUI.skin.window;
            skin.normal.background = texture;

            GUILayout.BeginArea(windowRect, skin);
            DrawWindow();
            GUILayout.EndArea();
        }

        private void AutoCompleteCommand()
        {
            if (_command == "") return;

            foreach (var command in _commands)
            {
                if (command.Prefix == _command) continue;
                if (command.Prefix.StartsWith(_command))
                {
                    _command = command.Prefix;
                    return;
                }
            }
        }

        private void DrawWindow()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            GUILayout.FlexibleSpace();

            var style = new GUIStyle("Label");
            style.fontSize = 25;

            foreach (var log in _logs)
            {
                style.normal.textColor = style.active.textColor = style.hover.textColor = log.Color;
                GUILayout.Label(log.Text, style);
            }
            GUILayout.EndScrollView();

            style.normal.textColor = style.active.textColor = style.hover.textColor = Color.white;
            GUI.SetNextControlName("CommandInput");
            _command = GUILayout.TextField(_command, style);
            GUI.FocusControl("CommandInput");
        }

        private void ReceiveUnityLog(string logString, string stackTrace, UnityEngine.LogType type)
        {
            var logType = type == UnityEngine.LogType.Error || type == UnityEngine.LogType.Exception ? LogType.Error : LogType.Log;
            PushLog(type is UnityEngine.LogType.Log ? logString : logString + "\n" + stackTrace, logType);
        }

        private void PushLog(string text, LogType type = LogType.Message)
        {
            _logs.Add(new Log(text, type));
            _scrollPosition = new(Mathf.Infinity, Mathf.Infinity);
        }

        private void CreateDefaultCommands()
        {
            AddCommand<string>(
                prefix: "log",
                description: "Displays text in the console.",
                callback: t => Debug.Log(t)
            );

            AddCommand(
                prefix: "help",
                description: "Shows a list of commands",
                callback: () =>
                {
                    string text = "";
                    foreach (var command in _commands)
                    {
                        var types = command.Types.Length switch
                        {
                            1 => $" [{command.Types[0].Name}]",
                            2 => $" [{command.Types[0].Name}] [{command.Types[1].Name}]",
                            _ => ""
                        };
                        text += $"{command.Prefix}{types} <color=\"#AFAFAF\">— {command.Description}</color>\n";
                    }
                    PushLog(text.Substring(0, text.Length - 1));
                }
            );

            AddCommand(
                prefix: "clear",
                description: "Clears logs",
                callback: () => _logs = new()
            );

            AddCommand<float>(
                prefix: "timescale",
                description: "Changes the game's speed. Default is 1",
                callback: t =>
                {
                    Time.timeScale = t;
                    PushLog($"Timescale set to {t}");
                }
            );

            AddCommand(
                prefix: "export",
                description: "Saves the console log to the game folder.",
                callback: () =>
                {
                    ExportLog();
                    PushLog($"Console log exported.");
                }
            );
        }

        private void ExportLog()
        {
            var logs = "";
            foreach (var log in _logs)
            {
                logs += log.Text + "\n\n";
            }

            var filePath = Application.dataPath + "/log.txt";
            if (File.Exists(filePath)) File.Delete(filePath);
            using (var streamWriter = File.CreateText(filePath))
            {
                streamWriter.Write(logs);
            }
        }

        private struct Command
        {
            public string Prefix;
            public string Description;
            public Action<object[]> Callback;
            public Type[] Types;
        }

        private struct Log
        {
            public string Text { get; private set; }
            public LogType Type { get; private set; }
            public Color Color { get; private set; }

            public Log(string text, LogType type)
            {
                Text = text;
                Type = type;

                Color = type switch
                {
                    LogType.Message => Color.white,
                    LogType.Log => new Color(0.7f, 0.7f, 0.7f),
                    LogType.Error => new Color(1, 0.4f, 0.4f),
                    _ => Color = Color.white
                };
            }
        }

        private enum LogType
        {
            Message,
            Log,
            Error
        }
    }
}