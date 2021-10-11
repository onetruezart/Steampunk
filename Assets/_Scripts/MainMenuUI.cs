using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _continueButton;

    private void Start()
    {
        if (PlayerPrefs.HasKey(GameManager.ProgressSaveId))
            _continueButton.interactable = true;
        else
            _continueButton.interactable = false;
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
