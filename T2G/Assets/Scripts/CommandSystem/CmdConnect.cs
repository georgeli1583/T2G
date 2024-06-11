using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using T2G.UnityAdapter;

public class CmdConnect : Command
{
    public static readonly string CommandKey = "Connect";

    public override bool Execute(params string[] args)
    {
        CommunicatorClient.Instance.StartClient();        
        return true;
    }

    public override string GetKey()
    {
        return CommandKey.ToLower();
    }

    public override string[] GetArguments()
    {
        string[] args = { };
        return args;
    }
}
