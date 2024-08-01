using System;
using UnityEngine;
using TMPro;

public class GameDescRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _name;

    int _rowIndex;
    Action<int> _loadCallback = null;
    Action<int> _deleteCallback = null;

    public void OnInit(string gameDescName, int rowIndex, Action<int> loadCallback, Action<int> deleteCallback)
    {
        _name.text = gameDescName;
        _rowIndex = rowIndex;
        _loadCallback = loadCallback;
        _deleteCallback = deleteCallback;
    }

    public void OnLoad()
    {
        _loadCallback?.Invoke(_rowIndex);
    }

    public void OnDelete()
    {
        _deleteCallback?.Invoke(_rowIndex);
    }
}
