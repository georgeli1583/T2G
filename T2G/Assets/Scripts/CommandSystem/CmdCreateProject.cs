using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CmdCreateProject : Command
{
    public static readonly string CommandKey = "CreateProject";

    private string _projectPathName;
    private Process _process;

    public override bool Execute(params string[] args)
    {
        if (!PlayerPrefs.HasKey(Defs.k_UnityEditorPath))
        {
            OnExecutionCompleted?.Invoke(false, ConsoleController.eSender.System, "Unity Editor path is not set!");
            return false;
        }
        string unityEditorPath = PlayerPrefs.GetString(Defs.k_UnityEditorPath);

        if (!PlayerPrefs.HasKey(Defs.k_ResourcePath))
        {
            OnExecutionCompleted?.Invoke(false, ConsoleController.eSender.System, "Resource path is not set!");
            return false;
        }
        string resourcePath = PlayerPrefs.GetString(Defs.k_ResourcePath);

        if(args.Length < 1)
        {
            OnExecutionCompleted?.Invoke(false, ConsoleController.eSender.System, "The project's path and name argument is missing!");
            return false;
        }
        _projectPathName = args[0];

        try
        {
            string projectDirectory = Path.GetDirectoryName(_projectPathName);
            if (Directory.Exists(_projectPathName))
            {
                Directory.Delete(_projectPathName, true);  //delete the old project
            }

            if (!Directory.Exists(projectDirectory))
            {
                Directory.CreateDirectory(projectDirectory);
            }

            ConsoleController.Instance.ProjectPathName = _projectPathName;

            var arguments = $"-batchMode -createproject {_projectPathName}";
            //string packagePath = Path.Combine(resourcePath, "com.t2g.unityadapter/package.json");   //Only allows 1 package
            //arguments += " -importPackage " + packagePath;
            //arguments += " -quit";

            _process = new Process();
            
            _process.EnableRaisingEvents = true;
            _process.Exited += new EventHandler(ProcessExitedHandler);
            _process.StartInfo.FileName = unityEditorPath;
            _process.StartInfo.Arguments = arguments;
            _process.EnableRaisingEvents = true;
            _process.Start();

            return true;
        }
        catch (Exception e)
        {
            _process.Close();
            OnExecutionCompleted?.Invoke(false, ConsoleController.eSender.System, "Error: " + e.Message);
            return false;
        }
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
        if (_process.ExitCode == 0)
        {
            OnExecutionCompleted?.Invoke(true, ConsoleController.eSender.System, 
                $"Project {_projectPathName} was created!");
        }
        else
        {
            OnExecutionCompleted?.Invoke(false, ConsoleController.eSender.System, 
                $"Failed to create project {_projectPathName}! Exit Code: {_process.ExitCode}");
        }
        _process.Close();
    }
}