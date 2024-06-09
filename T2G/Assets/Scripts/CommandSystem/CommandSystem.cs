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
        //_commandsRegistry.Add(OpenProjectCommand.CommandKey.ToLower(), typeof(OpenProjectCommand));
        //_commandsRegistry.Add(InitProjectCommand.CommandKey.ToLower(), typeof(InitProjectCommand));
        //_commandsRegistry.Add(ConnectCommand.CommandKey.ToLower(), typeof(ConnectCommand));
        //_commandsRegistry.Add(DisconnectCommand.CommandKey.ToLower(), typeof(DisconnectCommand));
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
        int idx = inputString.IndexOf(" ");
        return (idx > 0 && _commandsRegistry.ContainsKey(inputString.Substring(0, idx)));
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
