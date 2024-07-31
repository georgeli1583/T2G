using UnityEngine;
using TMPro;
using SFB;

public class GameDescForm : MonoBehaviour
{
    [SerializeField] TMP_InputField _Name;
    [SerializeField] TMP_InputField _Version;
    [SerializeField] TMP_InputField _MinorVersion;
    [SerializeField] TMP_InputField _Author;

    [SerializeField] TMP_InputField _Title;
    [SerializeField] TMP_Dropdown _Genre;
    [SerializeField] TMP_Dropdown _ArtStyle;
    [SerializeField] Transform _ArtStyleOptionsContainer;
    [SerializeField] GameObject _ArtStyleTemplate;

    [SerializeField] TMP_Dropdown _GameEngine;
    [SerializeField] TMP_InputField _Path;
    [SerializeField] TMP_InputField _ProjectName;

    [SerializeField] GameDescList _GameDescList;

    public GameDesc GameDesc = new GameDesc();

    private void OnEnable()
    {
        _Name.text = GameDesc.Name;
        _Version.text = GameDesc.VersionNumber.ToString();
        _MinorVersion.text = GameDesc.MinorVersionNumber.ToString();
        _Author.text = GameDesc.Author;
    }

    public void OnSelectPath()
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Choose project path", string.Empty, false);
        if(paths.Length > 0)
        {
            _Path.text = paths[0];
        }
    }

    public void OnLoadGameDesc()
    {
        _GameDescList.LoadGameDescCallback = (gameDescName) =>
        {
            GameDesc = JsonParser.LoadGameDesc(gameDescName);
            OnEnable();
        };
        _GameDescList.gameObject.SetActive(true);
    }
}
