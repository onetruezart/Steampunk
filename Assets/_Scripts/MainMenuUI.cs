using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private GameObject _soundOnBtn, _soundOffBtn;

    private void Start()
    {
        if (PlayerPrefs.HasKey(GameManager.ProgressSaveId))
            _continueButton.interactable = true;
        else
            _continueButton.interactable = false;

        if (PlayerPrefs.HasKey(GameManager.MusicValueSaveId))
        {
            if (PlayerPrefs.GetInt(GameManager.MusicValueSaveId) == 1)
                SoundOn();
            else
                SoundOff();
        }
        else
        {
            SoundOn();
        }
    }

    public void SoundOn()
    {
        _soundOffBtn.SetActive(false);
        _soundOnBtn.SetActive(true);

        PlayerPrefs.SetInt(GameManager.MusicValueSaveId, 1);

    }

    public void  SoundOff()
    {
        _soundOffBtn.SetActive(true);
        _soundOnBtn.SetActive(false);

        PlayerPrefs.SetInt(GameManager.MusicValueSaveId, 0);
    }


    public void StartNewGame()
    {
        PlayerPrefs.DeleteKey(GameManager.ProgressSaveId);
        LoadScene(GameManager.PlaySceneId);
    }

    public void ContinueGame()
    {
        LoadScene(GameManager.PlaySceneId);
    }

    private void LoadScene(int id)
    {
        SceneManager.LoadScene(id);
    }

}
