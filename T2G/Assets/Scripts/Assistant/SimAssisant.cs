using System;
using System.Collections.Generic;
using UnityEngine;



public class SimAssisant : MonoBehaviour
{
    [SerializeField] GameObject _AssistantDialogs;
    [SerializeField] GameDescForm _GameDescForm;

    GameDesc _gameDesc = new GameDesc();

    string[] _prompts = { 
        "hello",
        "hi",
        "create make a new game",
        "change game project information regenerate"
    };

    string[] _responses = {
        "Hi, how can I assist you to make a game?",
        "Hello, I am glad to help. What can I do for you?",
        "Okay! I need some initial information about the game, please fill up the Game Project Information form.",
        "Okay! Which game description (GameDesc) do you want to change?"
    };

  
    Dictionary<string, List<int>> _promptResponeMap = new Dictionary<string, List<int>>();
    Dictionary<string, Action<string>> _responseActionMap = new Dictionary<string, Action<string>>();

    List<string> _matchedPrompts = new List<string>();

    static SimAssisant _instance = null;
    public static SimAssisant Instance => _instance;

    private void Awake()
    {
        _instance = this;

        _promptResponeMap.Add(_prompts[0], new List<int>(new int[] { 0, 1 }));
        _promptResponeMap.Add(_prompts[1], new List<int>(new int[] { 0, 1 }));
        _promptResponeMap.Add(_prompts[2], new List<int>(new int[] { 2 }));
        _promptResponeMap.Add(_prompts[3], new List<int>(new int[] { 3 }));

        _responseActionMap.Add(_responses[2], CollectGameProjectInformation);
        _responseActionMap.Add(_responses[3], OpenGameDescForEditing);
    }

    public void ProcessPrompt(string prompt, Action<string> callBack)
    {
        if(_AssistantDialogs.activeSelf)
        {
            callBack?.Invoke("Please complete current task before doing anything else!");
            return;
        }

        string promptKey = string.Empty;
        string responseMessage = "Sorry, I don't understand what you mean! Could you provide more specific infromation?";

        _matchedPrompts.Clear();
        if(Utilities.FindTopMatches(prompt, _prompts, 3, 0.6f, ref _matchedPrompts))
        {
            int count = _matchedPrompts.Count;
            if (count > 1)
            {
                promptKey = _matchedPrompts[UnityEngine.Random.Range(0, count)];
            }
            else if(count > 0)
            {
                promptKey = _matchedPrompts[0];
            }

            if (_promptResponeMap.TryGetValue(promptKey, out var responseOptions))
            {
                count = responseOptions.Count;
                if (count > 1)
                {
                    responseMessage = _responses[responseOptions[UnityEngine.Random.Range(0, count)]];
                }
                else if (count > 0)
                {
                    responseMessage = _responses[responseOptions[0]];
                }

                if(_responseActionMap.TryGetValue(responseMessage, out var action))
                {
                    action?.Invoke(responseMessage);
                }
            }
        }
        callBack?.Invoke(responseMessage);
    }

    void CollectGameProjectInformation(string responseMessage)
    {
        _AssistantDialogs.SetActive(true);
        _GameDescForm.GameDesc = new GameDesc();
        _GameDescForm.gameObject.SetActive(true);
    }

    void OpenGameDescForEditing(string responseMessage)
    {
        //Choose 

        //Open
    }

    public void OnGameDescFormCancel()
    {
        _GameDescForm.gameObject.SetActive(false);
        _AssistantDialogs.SetActive(false);
    }

    public void OnGameDescFormOk()
    {
        //...
        _GameDescForm.gameObject.SetActive(false);
        _AssistantDialogs.SetActive(false);
    }
}
