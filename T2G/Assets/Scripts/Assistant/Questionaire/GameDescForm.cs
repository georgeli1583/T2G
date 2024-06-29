using UnityEngine;
using TMPro;

public class GameDescForm : MonoBehaviour
{
    [SerializeField] TMP_InputField _Name;
    [SerializeField] TMP_InputField _Version;
    [SerializeField] TMP_InputField _MinorVersion;
    [SerializeField] TMP_InputField _Author;
    [SerializeField] TMP_InputField _CreatedDateTime;
    [SerializeField] TMP_InputField _LastEditedDateTime;

    public GameDesc GameDesc;

    private void OnEnable()
    {
        _Name.text = GameDesc.Name;
        _Version.text = GameDesc.VersionNumber.ToString();
        _MinorVersion.text = GameDesc.MinorVersionNumber.ToString();
        _Author.text = GameDesc.Author;
        _CreatedDateTime.text = GameDesc.CreatedDateTime.ToShortTimeString();
        _LastEditedDateTime.text = GameDesc.LastUpdatedDateTime.ToShortTimeString();
    }

}
