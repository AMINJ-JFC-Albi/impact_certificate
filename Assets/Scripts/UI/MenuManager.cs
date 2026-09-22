using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class MenuManager : MonoBehaviour
{
    [Header("Main Menu Panels")]
    public GameObject menuPanel;
    public GameObject mainPanel;
    public GameObject optionsPanel;

    [Header("Options Settings")]
    public Slider volumeSlider;
    public TMP_Dropdown languageDropdown;
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    private List<String> resolutionOptions;
    private int currentResolutionIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        mainPanel.SetActive(true);
        resolutionOptions = new();

        InitializeResolutions();

        LoadSettings();
    }

    #region Navigation
    public void ShowOptionsPanel()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void ShowMainPanel()
    {
        optionsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
    #endregion


    #region Buttons actions
    public void ResumeGame()
    {
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion

    #region Options
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetLanguage(int languageIndex)
    {
        Debug.Log("Langue choisie : " + languageDropdown.options[languageIndex].text);
        PlayerPrefs.SetInt("Language", languageIndex);
    }

    private void InitializeResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionOptions.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    }

    #endregion

    #region Startup
    private void LoadSettings()
    {
        // Volume
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        // Langue
        int savedLanguageIndex = PlayerPrefs.GetInt("Language", 0);
        languageDropdown.value = savedLanguageIndex;
        SetLanguage(savedLanguageIndex);

        // Résolution
        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        resolutionDropdown.value = savedResolutionIndex;
        SetResolution(savedResolutionIndex);
        resolutionDropdown.RefreshShownValue();
    }

    #endregion

    #region Update

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuPanel.activeSelf)
            {
                menuPanel.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                menuPanel.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    #endregion
}
