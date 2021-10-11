using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void EventKeeper();

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private List<Page> _pages;
    public const string FirstPageId = "Start";
    public const string DeathPageId = "death";
    public const string FinalPageId = "final";

    public const string ProgressSaveId = "LastPage";

    public const int PlaySceneId = 1;
    public const int MainMenuSceneId = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        Init();
    }

    //private void OnEnable()
    //{
    //    GameParser.OnLoadEnd += Init;
    //}

    //private void OnDisable()
    //{
    //    GameParser.OnLoadEnd -= Init;
    //}

    private void Init()
    {
        _pages = GameParser.instance.GetPages();

        if (PlayerPrefs.HasKey(ProgressSaveId))
            LoadPage(FindPage(PlayerPrefs.GetString(ProgressSaveId)));
        else
            LoadPage(FindPage(FirstPageId));
    }

    public void LoadPage(Page page)
    {
        GameUI.instance.LoadPage(page);
        AudioController.instance.SetMusic(page.musicName);
        PlayerPrefs.SetString(ProgressSaveId, page.id);
    }

    public Page FindPage(string pageId)
    {
        return _pages.Find(page => page.id == pageId);
    }
}
