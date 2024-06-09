using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _UnityEditorPath;
    [SerializeField] private TMP_InputField _UserName;
    [SerializeField] private TMP_InputField _AssistantName;
    [SerializeField] private TMP_InputField _AssetsPath;

    private void OnEnable()
    {
        _UnityEditorPath.text = PlayerPrefs.GetString(Defs.k_UnityEditorPath, string.Empty);
        _UserName.text = PlayerPrefs.GetString(Defs.k_UserName, "You");
        _AssistantName.text = PlayerPrefs.GetString(Defs.k_AssistantName, "Assistant");
        _AssetsPath.text = PlayerPrefs.GetString(Defs.k_ResourcePath, string.Empty);
    }

    public void OnSave()
    {
        PlayerPrefs.SetString(Defs.k_UnityEditorPath, _UnityEditorPath.text);
        PlayerPrefs.SetString(Defs.k_UserName, _UserName.text);
        PlayerPrefs.SetString(Defs.k_AssistantName, _AssistantName.text);
        PlayerPrefs.SetString(Defs.k_ResourcePath, _AssetsPath.text);
    }
}
