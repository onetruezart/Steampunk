using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public static GameUI instance;

    [SerializeField] private TMP_Text _pageText;
    [SerializeField] private GameObject _buttonPrefab, _contentHolder;
    [SerializeField] private Image _bgImage;

    private List<GameObject> buttons = new List<GameObject>();

    private const float _sreenHeight = 600f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void LoadPage(Page page)
    {

        ClearPage();

        SetText(page);

        if (page.buttons.Count == 0)
        {
            CreateMainMenuButton();
        }
        else
        {
            foreach (PageButton pageButton in page.buttons)
            {
                CreateButton(pageButton);
            }
        }

        if (page.bgName != null)
            SetBG(page.bgName);

        StartCoroutine(RebuildLayout());

        // _contentHolder.SetActive(false);
    }

    private void SetText(Page page)
    {
        _pageText.text = page.text;
        _pageText.UpdateMeshPadding();
    }

    private void SetBG(string bgName)
    {
        Debug.Log(bgName);
        Sprite bg = Resources.Load<Sprite>("Backgrounds/" + bgName);
        if (bg != null)
            _bgImage.sprite = bg;

       NormalizeBgSize();
    }

    private void NormalizeBgSize()
    {
        _bgImage.SetNativeSize();

        RectTransform bgRectTransform = _bgImage.GetComponent<RectTransform>();

        bgRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        bgRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        bgRectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);

        Vector2 oldSize = bgRectTransform.sizeDelta;

        Vector2 newSize = new Vector2(oldSize.x / (oldSize.y / _sreenHeight), _sreenHeight);

        bgRectTransform.sizeDelta = newSize;

    }

    private IEnumerator RebuildLayout()
    {
        _contentHolder.GetComponent<CanvasGroup>().alpha = 0;
        yield return null;
        //_contentHolder.SetActive(true);
        LayoutRebuilder.MarkLayoutForRebuild(_contentHolder.GetComponent<RectTransform>());
        _contentHolder.GetComponent<CanvasGroup>().alpha = 1;
    }



    private void CreateButton (PageButton pageButton)
    {
        switch (pageButton.targetId)
        {
            case GameManager.DeathPageId:
                CreatePlayAgainButton();
                break;

            case GameManager.FinalPageId:
                CreateMainMenuButton();
                break;

            default:
                CreateNormalButton(pageButton);
                break;
        }
    }

    private void CreateNormalButton(PageButton pageButton)
    {
        GameObject button = Instantiate(_buttonPrefab);

        Page pageToLoad = GameManager.instance.FindPage(pageButton.targetId);

        button.GetComponent<Button>().onClick.AddListener(
            delegate
            {
                GameManager.instance.LoadPage(pageToLoad);
            });

        button.transform.GetChild(0).GetComponent<TMP_Text>().text = pageButton.text;

        buttons.Add(button);

        button.transform.SetParent(_contentHolder.transform);
    }

    private void CreatePlayAgainButton()
    {
        GameObject button = Instantiate(_buttonPrefab);

        Page pageToLoad = GameManager.instance.FindPage(GameManager.FirstPageId);

        button.GetComponent<Button>().onClick.AddListener(
            delegate
            {
                GameManager.instance.LoadPage(pageToLoad);
            });

        button.transform.GetChild(0).GetComponent<TMP_Text>().text = "Начать заново";

        buttons.Add(button);

        button.transform.SetParent(_contentHolder.transform);
    }

    private void CreateMainMenuButton()
    {
        GameObject button = Instantiate(_buttonPrefab);

        button.GetComponent<Button>().onClick.AddListener(
            delegate
            {
                PlayerPrefs.DeleteKey(GameManager.ProgressSaveId);
                LoadScene(GameManager.MainMenuSceneId);
            });

        button.transform.GetChild(0).GetComponent<TMP_Text>().text = "В главное меню";

        buttons.Add(button);

        button.transform.SetParent(_contentHolder.transform);
    }

    private void ClearPage()
    {
        foreach (GameObject button in buttons)
        {
            Destroy(button);
        }

        buttons = new List<GameObject>();
    }

    private void LoadScene(int id)
    {
        SceneManager.LoadScene(id);
    }
}
