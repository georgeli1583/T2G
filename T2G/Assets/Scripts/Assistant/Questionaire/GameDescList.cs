using System;
using System.Collections.Generic;
using UnityEngine;

public class GameDescList : MonoBehaviour
{
    public Action<string> LoadGameDescCallback = null;

    [SerializeField] Transform _Container;
    [SerializeField] GameDescRow _GameDescRow;

    List<string> _gameDescList;

    private void OnEnable()
    {
        _gameDescList = JsonParser.GetGameDescList();
        for(int i = 0; i < _gameDescList.Count; ++i)
        {
            var row = Instantiate<GameDescRow>(_GameDescRow, _Container);
            row.gameObject.SetActive(true);
            row.OnInit(_gameDescList[i], i, LoadGameDesc, DeleteGameDesc);
        }
    }

    private void OnDisable()
    {
        LoadGameDescCallback = null;
        _gameDescList = null;
    }

    void LoadGameDesc(int rowIndex)
    {
        LoadGameDescCallback?.Invoke(_gameDescList[rowIndex]);
        gameObject.SetActive(false);
    }

    void DeleteGameDesc(int rowIndex)
    {
        JsonParser.DeleteGameDesc(_gameDescList[rowIndex]);
        OnEnable();
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }
}
