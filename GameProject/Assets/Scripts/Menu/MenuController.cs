using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    public Button NewGameBtn;
    public Button OptionsBtn;
    public Button QuitGameBtn;

    public GameObject LoadingScreen;

    public CanvasGroup MainMenuGroup;
    public CanvasGroup OptionsGroup;

    public void LoadLevel(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
        NewGameBtn.interactable = false;
        OptionsBtn.interactable = false;
        QuitGameBtn.interactable = false;

        LoadingScreen.SetActive(true);
    }

    public void ShowOptions()
    {
        MainMenuGroup.alpha = 0;
        MainMenuGroup.interactable = false;
        MainMenuGroup.blocksRaycasts = false;

        OptionsGroup.alpha = 1;
        OptionsGroup.interactable = true;
        OptionsGroup.blocksRaycasts = true;
    }

    public void HideOptions()
    {
        OptionsGroup.alpha = 0;
        OptionsGroup.interactable = false;
        OptionsGroup.blocksRaycasts = false;

        MainMenuGroup.alpha = 1;
        MainMenuGroup.interactable = true;
        MainMenuGroup.blocksRaycasts = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
