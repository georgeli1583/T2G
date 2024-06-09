using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using T2G.UnityAdapter;

public class Defs
{
    public static readonly string k_UnityEditorPath = "UnityEditorPath";
    public static readonly string k_UserName = "UserName";
    public static readonly string k_AssistantName = "AssistantName";
    public static readonly string k_ResourcePath = "ResoucePath";
    public static readonly string k_ProjectPathname = "ProjectPathName";

    public static readonly string k_InitRefreshAssetDatabase = "RefreshAssetData";
    public static readonly string k_InitOpenScene = "OpenScene";
    public static readonly string k_InitStartListener = "StartListener";
}

public class ConsoleController : MonoBehaviour
{
    private static ConsoleController _instance = null;
    public static ConsoleController Instance => _instance;

    public enum eSender
    {
        User,
        Assistant,
        System,
        Warning,
        Error
    }

    private string[] _senderTextColors = { "white", "white", "red", "orange", "red" };

    private const string k_ConsoleSizeIndex = "ConsoleSizeIndex";

    private RectTransform _rectTransform;

    [SerializeField] private Button _ExpandButton;
    [SerializeField] private Button _ShrinkButton;

    [SerializeField] private GameObject _SettingsPanel;
    [SerializeField] private GameObject _HelpPanel;

    [SerializeField] private GameObject _OverlayPanel;

    [SerializeField] private TMP_InputField _InputMessage;
    [SerializeField] private TextMeshProUGUI _ConsoleDisplay;

    [SerializeField] private TextMeshProUGUI _ProjectPathName;

    private readonly int[] _consoleSizes = { 60, 300, 600, 900 };
    private int _consoleSizeIndex;

    List<string> _inputHistory = new List<string>();
    int _inputHistoryIndex = -1;
    readonly int _maxInputHistorySize = 32;

    
    bool IsBusy
    {
        set
        {
            _OverlayPanel.SetActive(value);
        }
    }


    public string ProjectPathName
    {
        get { return _ProjectPathName.text; }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                _ProjectPathName.text = string.Empty;
            }
            else
            {
                _ProjectPathName.text = "Project[" + value + "]";
            }
            PlayerPrefs.SetString(Defs.k_ProjectPathname, value);
        }
    }

    private void Awake()
    {
        _instance = this;
        CommunicatorClient.Instance.OnFailedToConnectToServer += HandleOnFailedConnectToServer;
    }

    void Start()
    {
        _rectTransform = (RectTransform)transform;

        _consoleSizeIndex = PlayerPrefs.GetInt(k_ConsoleSizeIndex, 1);
        ResizeConsole();

        ProjectPathName = PlayerPrefs.GetString(Defs.k_ProjectPathname, string.Empty);

        _InputMessage.onFocusSelectAll = false;
        _InputMessage.onSubmit.AddListener(OnInputEnds);
        RestoreCommandOptions();
    }

    private void OnDestroy()
    {
        CommunicatorClient.Instance.OnFailedToConnectToServer -= HandleOnFailedConnectToServer;

        _CommandOptions.onValueChanged.RemoveAllListeners();
        _InputMessage.onSubmit.RemoveAllListeners();
    }

    private void Update()
    {
        CommunicatorClient.Instance.UpdateClient();

        if (_InputMessage.isFocused)
        {
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (_inputHistoryIndex > 0)
                {
                    --_inputHistoryIndex;
                    _InputMessage.text = _inputHistory[_inputHistoryIndex];
                }
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                if (_inputHistoryIndex < _inputHistory.Count - 1)
                {
                    ++_inputHistoryIndex;
                    _InputMessage.text = _inputHistory[_inputHistoryIndex];
                }
                else
                {
                    _InputMessage.text = string.Empty;
                }
            }
        }
    }

    void ResizeConsole()
    {
        PlayerPrefs.SetInt(k_ConsoleSizeIndex, _consoleSizeIndex);
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x,
            _consoleSizes[_consoleSizeIndex]);
        _ExpandButton.interactable = (_consoleSizeIndex < _consoleSizes.Length - 1);
        _ShrinkButton.interactable = (_consoleSizeIndex > 0);
    }

    public void ExpandConsole()
    {
        if (_consoleSizeIndex < _consoleSizes.Length - 1)
        {
            ++_consoleSizeIndex;
            ResizeConsole();
        }
    }

    public void ShrinkConsole()
    {
        if (_consoleSizeIndex > 0)
        {
            --_consoleSizeIndex;
            ResizeConsole();
        }
    }

    public void OpenCloseSettings()
    {
        bool isActive = !_SettingsPanel.activeSelf;
        _SettingsPanel.SetActive(isActive);
        if(!isActive)
        {
            SetFocusONConsoleInputField();
        }
    }

    public void OpenCloseHelp()
    {
        bool isActive = !_HelpPanel.activeSelf;
        _HelpPanel.SetActive(isActive);
        if (!isActive)
        {
            SetFocusONConsoleInputField();
        }
    }

    public void WriteConsoleMessage(eSender sender, string message)
    {
        string senderPrompt = string.Empty;

        switch (sender)
        {
            case eSender.User:
                senderPrompt = PlayerPrefs.GetString(Defs.k_UserName, "You");
                if (string.IsNullOrEmpty(senderPrompt))
                {
                    senderPrompt = "You";
                }
                break;
            case eSender.Assistant:
                senderPrompt = PlayerPrefs.GetString(Defs.k_AssistantName, "Assistant");
                if (string.IsNullOrEmpty(senderPrompt))
                {
                    senderPrompt = "Assistant";
                }
                break;
            case eSender.System:
                senderPrompt = "System";
                break;
            case eSender.Warning:
                senderPrompt = "Warning";
                break;
            case eSender.Error:
                senderPrompt = "Error";
                break;
        }
        string textColor = _senderTextColors[(int)sender];
        _ConsoleDisplay.text += $"\n<color={textColor}>{senderPrompt}> {message}</color>";
    }

    public void OnInputEnds(string inputString)
    {
        string inputStr = _InputMessage.text;
        if (string.IsNullOrWhiteSpace(inputString))
        {
            return;
        }
        else
        {
            inputStr = inputString;
        }

        //Input history
        WriteConsoleMessage(eSender.User, inputStr);
        _inputHistory.Add(inputStr);
        _inputHistoryIndex = _inputHistory.Count;
        if(_inputHistory.Count > _maxInputHistorySize)
        {
            _inputHistory.RemoveRange(0, _maxInputHistorySize / 10);
        }

        //Execute if it is a command 
        if (CommandSystem.Instance.IsCommand(inputStr))             //Process command
        {
            var cmdSys = CommandSystem.Instance;
            var cmdStr = Regex.Replace(inputStr, " {2,}", " ");  //ensure only one space delimeter
            string[] cmdAndArgs = cmdStr.Split(" ");
            string cmd = cmdAndArgs[0].ToLower();

            string[] args = cmdAndArgs.Where((item, index) => index != 0).ToArray();
            IsBusy = true;
            if(!cmdSys.ExecuteCommand(
                (succeeded, sender, message) => 
                {
                    WriteConsoleMessage(sender, message);
                    IsBusy = false;
                },
                cmd, args))
            {
                IsBusy = false;
            }
        }
        else  //process a prompt
        {

        }

        //Clear input
        _InputMessage.text = string.Empty;
    }

    [SerializeField]
    TMP_Dropdown _CommandOptions;
    public void SaveCommand()
    {
        if (!string.IsNullOrWhiteSpace(_InputMessage.text))
        {
            _CommandOptions.options.Add(new TMP_Dropdown.OptionData(_InputMessage.text));
            SaveCommandOptionsToPlayerPref();
        }
    }

    void SaveCommandOptionsToPlayerPref()
    {
        string cmdList = _CommandOptions.options[0].text;
        for (int i = 1; i < _CommandOptions.options.Count; ++i)
        {
            cmdList += "\n" + _CommandOptions.options[i].text;
        }

        PlayerPrefs.SetString("SavedCommands", cmdList);
    }

    public void RemoveCommandOption()
    {
        if (string.IsNullOrWhiteSpace(_InputMessage.text))
        {
            return;
        }

        for (int i = 0; i < _CommandOptions.options.Count; ++i)
        {
            if (string.Compare(_CommandOptions.options[i].text, _InputMessage.text) == 0)
            {
                _CommandOptions.options.RemoveAt(i);
                SaveCommandOptionsToPlayerPref();
                break;
            }
        }
    }

    public void SelectCommand(int index)
    {
        _InputMessage.text = _CommandOptions.captionText.text;
    }

    public void RestoreCommandOptions()
    {
        string commandOptionsString = PlayerPrefs.GetString("SavedCommands", string.Empty);
        string[] commandOptions = commandOptionsString.Split("\n");
        _CommandOptions.options.Clear();
        foreach (var commandOption in commandOptions)
        {
            _CommandOptions.options.Add(new TMP_Dropdown.OptionData(commandOption));
        }

        _CommandOptions.onValueChanged.AddListener(SelectCommand);
    }

    void SetFocusONConsoleInputField()
    {
        EventSystem.current.SetSelectedGameObject(_InputMessage.gameObject);
        _InputMessage.Select();
        _InputMessage.ActivateInputField();
    }

    void HandleOnFailedConnectToServer()
    {
        WriteConsoleMessage(eSender.System, "Failed to connect to the server!");
    }

}
