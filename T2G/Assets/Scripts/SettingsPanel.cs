using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using T2G.UnityAdapter;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _UnityEditorPath;
    [SerializeField] private TMP_InputField _UserName;
    [SerializeField] private TMP_InputField _AssistantName;
    [SerializeField] private TMP_InputField _AssetsPath;

    private void OnEnable()
    {
        Settings.Load();
        _UnityEditorPath.text = Settings.UnityEditorPath;
        _UserName.text = Settings.RecoursePath;
        _AssistantName.text = Settings.User;
        _AssetsPath.text = Settings.Assistant;
    }

    public void OnSave()
    {
        Settings.UnityEditorPath = _UnityEditorPath.text;
        Settings.RecoursePath = _UserName.text;
        Settings.User = _AssistantName.text;
        Settings.Assistant = _AssetsPath.text;
        Settings.Save();
    }
}
