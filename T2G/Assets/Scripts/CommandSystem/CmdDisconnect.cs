using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using T2G.UnityAdapter;

public class CmdDisconnect : Command
{
    public static readonly string CommandKey = "Disconnect";

    public override bool Execute(params string[] args)
    {
        CommunicatorClient.Instance.Disconnect();
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
