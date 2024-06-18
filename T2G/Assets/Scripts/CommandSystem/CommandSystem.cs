using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

public class CommandSystem : MonoBehaviour
{
    private static CommandSystem _instance = null;
    public static CommandSystem Instance => _instance;
       
    private Dictionary<string, Type> _commandsRegistry = new Dictionary<string, Type>();

    void RegisterCommands()
    {
        _commandsRegistry.Add(CmdCreateProject.CommandKey.ToLower(), typeof(CmdCreateProject));
        _commandsRegistry.Add(CmdInitProject.CommandKey.ToLower(), typeof(CmdInitProject));
        _commandsRegistry.Add(CmdOpenProject.CommandKey.ToLower(), typeof(CmdOpenProject));
        _commandsRegistry.Add(CmdConnect.CommandKey.ToLower(), typeof(CmdConnect));
        _commandsRegistry.Add(CmdDisconnect.CommandKey.ToLower(), typeof(CmdDisconnect));
        //_commandsRegistry.Add(BeginGameDescCommand.CommandKey.ToLower(), typeof(BeginGameDescCommand));
        //_commandsRegistry.Add(SaveGameDescCommand.CommandKey.ToLower(), typeof(SaveGameDescCommand));
        //_commandsRegistry.Add(EndGameDescCommand.CommandKey.ToLower(), typeof(EndGameDescCommand));
        //_commandsRegistry.Add(GenerateGameCommand.CommandKey.ToLower(), typeof(GenerateGameCommand));
    }

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        RegisterCommands();
    }

    private void OnDestroy()
    {
        _commandsRegistry.Clear();
    }

    public bool IsCommand(string inputString)
    {
        inputString = inputString.Trim();
        int idx = inputString.IndexOf(" ");
        string cmd = idx > 0 ? inputString.Substring(0, idx) : inputString;
        return _commandsRegistry.ContainsKey(cmd);
    }

    public bool ExecuteCommand(Action<bool, ConsoleController.eSender, string> OnExecutionCompleted, 
        string commandKey, params string[] args)
    {
        if (_commandsRegistry.ContainsKey(commandKey))
        {
            var command = (Command)Activator.CreateInstance(_commandsRegistry[commandKey]);
            command.OnExecutionCompleted = OnExecutionCompleted;
            return command.Execute(args);
        }
        return false;
    }
}
