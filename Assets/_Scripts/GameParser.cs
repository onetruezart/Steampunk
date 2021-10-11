using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public class GameParser : MonoBehaviour
{
    public static GameParser instance;

    public static event EventKeeper OnLoadEnd;

    [SerializeField] private TextAsset _gameText, _bgInfoText, _musicInfoText;

    private List<string[]> _textPages = new List<string[]>();

    private List<Page> _Pages = new List<Page>();

    private void Awake()
    {
        if (instance == null)
            instance = this;

        if (GameObject.FindGameObjectsWithTag("GameParser").Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Parse(_gameText.text);
    }

    private void Parse(string text)
    {
        //text = text.Replace("\n", "<br>");

        string[] lines = text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

        FindPages(lines);
        ParsePages();

        FindBackgroundInfo(_bgInfoText.text);
        FindMusicInfo(_musicInfoText.text);

        OnLoadEnd?.Invoke();
    }

    private void FindPages(string[] lines)
    {
        for (int i = 0; i < lines.Length;)
        {
            if (lines[i][0] == 'A' || lines[i][0] == 'S' || lines[i][0] != 'f' || lines[i][0] != 'd')
            {
                List<string> textPage = new List<string>();

                textPage.Add(lines[i]);
                i++;

                string pageContetnt = "";

                do
                {
                    pageContetnt += lines[i];
                    i++;

                }
                while (i < lines.Length && (lines[i][0] != 'f' && lines[i][0] != 'd' && lines[i][0] != 'A' && lines[i][0] != 'S'));

                textPage.Add(pageContetnt);

                _textPages.Add(textPage.ToArray());
            }
            else
            {
                i++;
            }
        }
    }

    private void ParsePages()
    {
        foreach (string[] textPage in _textPages)
        {
            Page page = new Page();

            page.id = textPage[0].Split(':')[0];

            //Debug.Log(page.id);

            ParsePageContent(textPage[1], ref page);

            _Pages.Add(page);
        }
    }

    private void ParsePageContent(string pageContentText, ref Page page)
    {
        string[] pageContentTextSplited = pageContentText.Split(new string[] { "answers" }, StringSplitOptions.None);

        string textsContentText = pageContentTextSplited[0];
        string answerContentText = pageContentTextSplited[1];

        ParsePageText(textsContentText, ref page);
        ParseButtonText(answerContentText, ref page);
    }

    private void ParsePageText(string textsContentText, ref Page page)
    {
        textsContentText = textsContentText.Replace(@"\'", "$");

        string[] linesOfText = textsContentText.Split('\'').Where((str, index) => index % 2 == 1).ToArray();

        string text = "";

        foreach (string line in linesOfText)
        {
            text += line;
        }

        text = text.ToNormalFormat();

        page.text = text;

    }

    private void ParseButtonText(string buttonContent, ref Page page)
    {
        buttonContent = buttonContent.Replace(@"\'", "$");
        string[] content = buttonContent.Split('\'');

        string[] buttonTexts = content.Where((str, index) => index % 4 == 1).ToArray();
        string[] buttonTargets = content.Where((str, index) => index % 4 == 3).ToArray();

        if (buttonTexts.Length != buttonTargets.Length)
            throw new Exception($"Button texts != button targets ({buttonTexts.Length}/{buttonTargets.Length})");

        page.buttons = new List<PageButton>();

        for (int i = 0; i < buttonTexts.Length; i++)
        {
            //Debug.Log($"{buttonTexts[i]}/{ buttonTargets[i]}");

            PageButton pageButton = new PageButton
            {
                text = buttonTexts[i].ToNormalFormat(),
                targetId = buttonTargets[i]
            };

            page.buttons.Add(pageButton);
        }

    }

    private void FindBackgroundInfo(string text)
    {
        string[] lines = text.WithoutExtraChars().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Where(str => str != "").ToArray();

        foreach (string line in lines)
        {
            ParseBackgroundInfo(line);
        }
    }

    private void ParseBackgroundInfo(string str)
    {
        string[] components = str.Split(' ');

        string bgName = components[0].ToLower();

        string[] pageIds = components.Where((str, index) => index > 0).ToArray();

        foreach (string pageId in pageIds)
        {
            int i = _Pages.FindIndex(page => page.id == pageId.Replace(" ", ""));
            Debug.Log(i + " " + pageId);
            Page page = _Pages[i];
            page.bgName = bgName;
            _Pages[i] = page;
        }
    }

    private void FindMusicInfo(string text)
    {
        string[] lines = text.WithoutExtraChars().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Where(str => str != "").ToArray();

        foreach (string line in lines)
        {
            ParseMusicInfo(line);
        }
    }

    private void ParseMusicInfo(string str)
    {
        string[] components = str.Split(' ');

        string musicName = components[0].ToLower();

        string[] pageIds = components.Where((str, index) => index > 0).ToArray();

        foreach (string pageId in pageIds)
        {
            int i = _Pages.FindIndex(page => page.id == pageId.Replace(" ", ""));
            Debug.Log(i + " " + pageId);
            Page page = _Pages[i];
            page.musicName = musicName;
            _Pages[i] = page;
        }
    }

    public List<Page> GetPages()
    {
        return _Pages;
    }
}

public struct Page
{
    public string id;
    public string text;
    public string bgName;
    public string musicName;

    public List<PageButton> buttons;

}

public struct PageButton
{
    public string text;
    public string targetId;
}
