using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class CmdOpenProject : Command
{
    public static readonly string CommandKey = "OpenProject";

    private string _projectPathName;
    private Process _process;
    private EventHandler _eventHandler;

    public override bool Execute(params string[] args)
    {
        if (!PlayerPrefs.HasKey(Defs.k_UnityEditorPath))
        {
            OnExecutionCompleted?.Invoke(false, ConsoleController.eSender.Error, "Unity Editor path is not set!");
            return false;
        }
        string unityEditorPath = PlayerPrefs.GetString(Defs.k_UnityEditorPath);

        if (args.Length < 1)
        {
            string defaultPath = ConsoleController.Instance.ProjectPathName;
            int startIdx = defaultPath.IndexOf("[") + 1;
            defaultPath = defaultPath.Substring(startIdx, defaultPath.IndexOf("]") - startIdx);

             if (string.IsNullOrWhiteSpace(defaultPath))
            {
                OnExecutionCompleted?.Invoke(false, ConsoleController.eSender.Error, "The project's path argument is missing!");
                return false;
            }
            else
            {
                _projectPathName = defaultPath;
            }
        }
        else
        {
            _projectPathName = ConsoleController.Instance.ProjectPathName = args[0];
        }

        if (!Directory.Exists(_projectPathName))
        {
            OnExecutionCompleted?.Invoke(false, ConsoleController.eSender.Error, $"Project was not found.");
            return false;
        }

        var arguments = $"-projectPath {_projectPathName}";
        _eventHandler = new EventHandler(ProcessExitedHandler);
        _process = new Process();
        _process.EnableRaisingEvents = true;
        _process.Exited += _eventHandler;
        _process.StartInfo.FileName = unityEditorPath;
        _process.StartInfo.Arguments = arguments;
        _process.Start();
        _process.WaitForInputIdle();
        
        OnExecutionCompleted?.Invoke(true, ConsoleController.eSender.System, $"Project was opened!");
        return true;
    }

    public override string GetKey()
    {
        return CommandKey.ToLower();
    }

    public override string[] GetArguments()
    {
        string[] args = { _projectPathName };
        return args;
    }

    void ProcessExitedHandler(object sender, EventArgs args)
    {
        if (_process.ExitCode != 0)
        {
            OnExecutionCompleted?.Invoke(false, ConsoleController.eSender.Error,
                $"Failed! Exit Code: {_process.ExitCode}");
        }
        _process.Close();
        _process.Exited -= _eventHandler;
    }
}
