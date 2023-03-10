using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum UILanguage
    {
        English,
        Dutch
    }

    public enum UIState
    {
        Disabled,
        MainMenu,
        PauseMenu,
        Settings
    }

    public SaveData saveData;

    public UIState uiState;
    [HideInInspector] public UIState previousUIState;
    public UILanguage language;
    [HideInInspector]public UILanguage previousUILanguage;

    [Header("Main Menu")]
    public GameObject mainMenu = null;

    [Header("Pause Menu")]
    public GameObject pauseUI = null;

    [Header("Settings Menu")]
    public GameObject settingsMenu = null;
    [Space]
    public Transform cam;
    [Space]
    public AudioMixer mainMixer;
    [Space]
    public Slider masterVol;
    public Slider playerHeight;

    [Header("Misc")]
    public CinemachineVirtualCamera cmCam;
    public CinemachineVirtualCamera playerCam;

    public Button[] buttons;

    public GameObject flicker;

    public void Awake()
    {
        Initialize();
    }

    public void Start()
    {
        Invoke("LoadSettings", 1.0f);
    }

    private void Update()
    {
        CheckUIState();
    }

    public void CheckUIState()
    {
        switch (uiState)
        {
            case UIState.Disabled:
                mainMenu.SetActive(false);
                pauseUI.SetActive(false);
                settingsMenu.SetActive(false);
                break;

            case UIState.MainMenu:
                mainMenu.SetActive(true);

                pauseUI.SetActive(false);
                settingsMenu.SetActive(false);
                break;

            case UIState.PauseMenu:
                pauseUI.SetActive(true);

                mainMenu.SetActive(false);
                settingsMenu.SetActive(false);
                break;

            case UIState.Settings:
                settingsMenu.SetActive(true);

                mainMenu.SetActive(false);
                pauseUI.SetActive(false);
                break;
        }
    }

    public void StartSimulation()
    {
        SaveManager.Save(saveData);

        CameraSwitcher.Register(cmCam);
        CameraSwitcher.Register(playerCam);

        CameraSwitcher.SwitchCamera(playerCam);
    }

    public void OpenSettings()
    {
        previousUIState = uiState;

        flicker.SetActive(true);

        uiState = UIState.Settings;

        LoadSettings();
    }

    public void BackToMain(string sceneToLoad)
    {
        SaveManager.Save(saveData);

        if(sceneToLoad != "")
        {
            SceneManager.LoadScene(sceneToLoad);
        }else
        {
            Debug.LogError("Error, No Scene specified!, Please specify a scene to load and try again");
        }
    }

    public void QuitSimulation()
    {
        SaveManager.Save(saveData);

        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        mainMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);

        Debug.Log(volume);
    }

    public void SetHeight(float height)
    {
        cam.localPosition = new Vector3(cam.localPosition.x, height, cam.localPosition.z);

        Debug.Log(height);
    }

    public void Back()
    {
        SaveSettings();

        flicker.SetActive(true);

        uiState = previousUIState;
    }

    public void Continue()
    {
        flicker.SetActive(true);

        uiState = UIState.Disabled;
    }

    public void SaveSettings()
    {
        saveData.masterVolume = masterVol.value;
        saveData.playerHeight = playerHeight.value;

        SaveManager.Save(saveData);
    }

    public void LoadSettings()
    {
        SaveManager.Load();

        masterVol.value = saveData.masterVolume;
        playerHeight.value = saveData.playerHeight;
    }

    public void Unhover()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<RectTransform>().localScale = Vector3.one;
            buttons[i].GetComponent<Animator>().SetTrigger("T");
        }
    }

    public void Initialize()
    {
        switch(uiState)
        {
            case UIState.Disabled:
                settingsMenu.SetActive(false);
                mainMenu.SetActive(false);
                pauseUI.SetActive(false);
                break;

            case UIState.MainMenu:
                mainMenu.SetActive(true);

                settingsMenu.SetActive(false);
                pauseUI.SetActive(false);
                break;

            case UIState.PauseMenu:
                pauseUI.SetActive(true);
                settingsMenu.SetActive(false);
                mainMenu.SetActive(false);
                break;
        }
    }
}
