using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

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
        GenerateButtons(page);
        if (page.bgName != null)
            SetBG(page.bgName);

        StartCoroutine(RebuildLayout());
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

       SetBgSize();
    }

    private void SetBgSize()
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
        LayoutRebuilder.MarkLayoutForRebuild(_contentHolder.GetComponent<RectTransform>());
        _contentHolder.GetComponent<CanvasGroup>().alpha = 1;
    }



    private void GenerateButtons (Page page)
    {
        if (page.buttons.Count == 0)
        {
            CreateMainMenuButton();
        }
        else
        {
            foreach (PageButton pageButton in page.buttons)
            {
                switch (pageButton.targetId)
                {
                    case GameManager.DeathPageId:
                        CreateButton("Начать заново", () =>
                        {
                            GameManager.instance.LoadPage(GameManager.instance.FindPage(GameManager.FirstPageId));
                        });
                        break;

                    case GameManager.FinalPageId:
                        CreateMainMenuButton();
                        break;

                    default:
                        CreateButton(pageButton.text, () =>
                        {
                            GameManager.instance.LoadPage(GameManager.instance.FindPage(pageButton.targetId));
                        });
                        break;
                }
            }
        } 
    }

    private void CreateMainMenuButton()
    {
        CreateButton("В главное меню", () =>
        {
            PlayerPrefs.DeleteKey(GameManager.ProgressSaveId);
            LoadScene(GameManager.MainMenuSceneId);
        });
    }

    private void CreateButton(string buttonText, UnityAction onClickAction)
    {
        GameObject button = Instantiate(_buttonPrefab);

        button.GetComponent<Button>().onClick.AddListener(onClickAction);

        button.transform.GetChild(0).GetComponent<TMP_Text>().text = buttonText;
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
