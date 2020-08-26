using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GraphicsSettingsMenu : MonoBehaviour {

    public enum saveFormat { playerprefs, iniFile };
    public saveFormat saveAs;

    [Tooltip("Check for IOS and Windows Store Apps.")]
    public bool usePersistentDatapath;
    //public bool pauseTimeWhenMenuOpen;//if Checked in inspector - Sets TimeScale to 0 when menu is open.

    [Header("THESE NEED TO BE DRAGGED IN")]
    //if you use the prefab "_QualitySettingsMenu" they should all be assigned for you;
    public Slider qualityLevelSlider;
    public Slider antiAliasSlider, shadowResolutionSlider, textureQualitySlider, anisotropicModeSlider, anisotropicLevelSlider;
    public Text qualityText, antiAliasText, shadowText, textureText, anisotropicModeText, anisotropicLevelText, fpsCounterText;
    public GameObject resolutionsPanel, resButtonPrefab, menuTransform;
    public Text currentResolutionText;
    public Toggle FPSToggle, windowedModeToggle, vSyncToggle;

    private GameObject resolutionsPanelParent;
    private Resolution[] resolutions;

    private bool setMenu, openMenu, showFPS, fullScreenMode, toggleVSync;

    private const float fpsMeasurePeriod = 0.2f;
    private float fpsNextPeriod = 0, setTextQualDelay;
    private int fpsAccumulator = 0, currentFps, wantedResX, wantedResY;

    private string saveFileDataPath;

    private Coroutine setTextQualityCoR;

    private class MenuVariables
    {
        public int Qualitylevel;
        public bool ShowFPS;
        public int ResolutionX, ResolutionY;
        public bool WindowedMode;
        public bool VSync;
        public int AntiAliaslevel;
        public int ShadowResolution;
        public int TextureQuality;
        public int AnisotropicMode;
        public int AnisotropicLevel;

        public string Warning;
    }

    MenuVariables saveVars;

    //set these to the values you want to Reset to (Default values).
    MenuVariables DefaultSettings = new MenuVariables
    {
        Qualitylevel = 1,
        ShowFPS = false,
        ResolutionX = 0,//not used use Screen.width instead
        ResolutionY = 0,//not used use Screen.height instead
        WindowedMode = false,
        VSync = false,
        AntiAliaslevel = 0,
        ShadowResolution = 0,
        TextureQuality = 0,
        AnisotropicMode = 0,
        AnisotropicLevel = 0,

        Warning = "Edit this file at your own risk!"
    };

// Use this for initialization
void Start()
    {
        //DontDestroyOnLoad(transform.gameObject);
        fpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
        resolutionsPanelParent = resolutionsPanel.transform.parent.parent.gameObject;

        //Use Persistent for "IOS" and "Windows Store Apps" or if you prefer to saves the file in a seperate persistent Folder.
        if (!usePersistentDatapath)
            saveFileDataPath = Application.dataPath + "/QualitySettings.ini";//puts the file in the games/applications folder.
        else
            saveFileDataPath = Application.persistentDataPath + "/QualitySettings.ini";

        //this reads all the values of the sliders and toggles and sets the Graphic settings accordingly.
        //(if the settings were saved before, they wil all be set to the saved setting before reading them)
        //(if this is the first time the game starts the toggles and sliders wil be where they were when the game was build)
        //(if you want the game to start at certain settings the first time, make sure to set everyting before you build)
        SetValues();
    }

    // Update is called once per frame
    void Update()
    {
        //this FPScounter is a standard Unity asset (thought it was handy to put it in).
        if (showFPS)
        {
            fpsAccumulator++;
            if (Time.realtimeSinceStartup > fpsNextPeriod)
            {
                currentFps = (int)(fpsAccumulator / fpsMeasurePeriod);
                fpsAccumulator = 0;
                fpsNextPeriod += fpsMeasurePeriod;
                fpsCounterText.text = "FPS:" + currentFps;
            }
        }
        else fpsCounterText.text = "";
    }

    public void SetQuality() //changes the general Quality setting without changing the Vsync,Antialias or Anisotropic settings.
    {
        int graphicSetting = Mathf.RoundToInt(qualityLevelSlider.value);
        QualitySettings.SetQualityLevel(graphicSetting, true);
        qualityText.text = QualitySettings.names[graphicSetting];
        //keep settings the way the Sliders and Toggels are set.
        SetWindowedMode();
        SetVSync();
        SetAntiAlias();
        SetShadowResolution();
        SetTextureQuality();
        SetAnisotropicFiltering();
        SetAnisotropicFilteringLevel();
    }

    public void ShowFPS()
    {
        showFPS = !showFPS;
    }

    public void SetWindowedMode()
    {
        if (windowedModeToggle.isOn)
            fullScreenMode = false;
        else fullScreenMode = true;
        Screen.SetResolution(wantedResX, wantedResY, fullScreenMode);
    }

    public void SetVSync()
    {
        if (vSyncToggle.isOn)
            QualitySettings.vSyncCount = 1;
        else QualitySettings.vSyncCount = 0;
    }

    public void SetAntiAlias()
    {
        int sliderValue = Mathf.RoundToInt(antiAliasSlider.value);
        switch (sliderValue)
        {
            case 0:
                QualitySettings.antiAliasing = 0;
                antiAliasText.text = "Off";
                break;
            case 1:
                QualitySettings.antiAliasing = 2;
                antiAliasText.text = QualitySettings.antiAliasing.ToString() + "x Multi Sampling";
                break;
            case 2:
                QualitySettings.antiAliasing = 4;
                antiAliasText.text = QualitySettings.antiAliasing.ToString() + "x Multi Sampling";
                break;
            case 3:
                QualitySettings.antiAliasing = 8;
                antiAliasText.text = QualitySettings.antiAliasing.ToString() + "x Multi Sampling";
                break;
        }
    }

    public void SetShadowResolution()
    {
        switch (Mathf.RoundToInt(shadowResolutionSlider.value))
        {
            case 0:
                QualitySettings.shadowResolution = ShadowResolution.Low;
                shadowText.text = "Low";
                break;
            case 1:
                QualitySettings.shadowResolution = ShadowResolution.Medium;
                shadowText.text = "Medium";
                break;
            case 2:
                QualitySettings.shadowResolution = ShadowResolution.High;
                shadowText.text = "High";
                break;
            case 3:
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                shadowText.text = "VeryHigh";
                break;
        }
    }

    //TextureQuality needs a little delay before setting. (unstable otherwise)
    public void SetTextureQuality()
    {
        if (setTextQualDelay <= 0)
        {
            setTextQualDelay = 1f;
            setTextQualityCoR = StartCoroutine(SetTextureQualityCoroutine());
        }
        else
        {
            setTextQualDelay = 1f;
        }
    }

    IEnumerator SetTextureQualityCoroutine()
    {
        while (setTextQualDelay > 0)
        {
            setTextQualDelay -= 0.6f;
            yield return new WaitForSecondsRealtime(0.2f);
        }

        switch (Mathf.RoundToInt(textureQualitySlider.value))
        {
            case 0:
                QualitySettings.masterTextureLimit = 3;
                textureText.text = "Eighth Res";
                break;
            case 1:
                QualitySettings.masterTextureLimit = 2;
                textureText.text = "Quarter Res";
                break;
            case 2:
                QualitySettings.masterTextureLimit = 1;
                textureText.text = "Half Res";
                break;
            case 3:
                QualitySettings.masterTextureLimit = 0;
                textureText.text = "Full Res";
                break;
        }
    }

    public void SetAnisotropicFiltering()
    {
        switch (Mathf.RoundToInt(anisotropicModeSlider.value))
        {
            case 0:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                anisotropicModeText.text = "Disabled";
                break;
            case 1:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                anisotropicModeText.text = "Enabled";
                break;
            case 2:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                anisotropicModeText.text = "ForceEnabled";
                break;
        }
    }

    public void SetAnisotropicFilteringLevel()
    {
        int SliderValue = Mathf.RoundToInt(anisotropicLevelSlider.value);
        Texture.SetGlobalAnisotropicFilteringLimits(SliderValue, SliderValue);
        anisotropicLevelText.text = SliderValue.ToString();
    }

    private void SetValues()//set all settings according to the menu buttons.
    {
        //his reads how many Quality levels your "Game" has and sices the slider accordingly.
        qualityLevelSlider.maxValue = QualitySettings.names.Length - 1;

        resolutions = Screen.resolutions;//checking the available resolution options.
        //filling the Screen Resolution option menu with buttons, one for every available resolution option your monitor has.
        int prefResX = 0;
        int prefRezY = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width != prefResX && resolutions[i].height != prefRezY)//prevent creating duplicate resolution buttons.
            {
                GameObject button = Instantiate(resButtonPrefab);//the button prefab.
                button.GetComponentInChildren<Text>().text = resolutions[i].width + "x" + resolutions[i].height;
                int index = i;
                button.GetComponent<Button>().onClick.AddListener(() => { SetResolution(index); });//adding a "On click" SetResolution() function to the button.
                button.transform.SetParent(resolutionsPanel.transform, false);

                prefResX = resolutions[i].width;
                prefRezY = resolutions[i].height;
            }
        }

        LoadMenuVariables(); // if any settings were saved before, this is where they are loaded and Sliders and toggles are set to the saved position.

        //reading Sliders and toggles and setting everything accordingly.
        int graphicSetting = Mathf.RoundToInt(qualityLevelSlider.value);
        QualitySettings.SetQualityLevel(graphicSetting, true);
        qualityText.text = QualitySettings.names[graphicSetting];
        SetVSync();
        SetWindowedMode();
        SetAntiAlias();
        SetShadowResolution();
        SetTextureQuality();
        SetAnisotropicFiltering();
        SetAnisotropicFilteringLevel();
    }

    public void SetResolution(int index)//the "On click" function on the resolutions buttons.
    {
        wantedResX = resolutions[index].width;
        wantedResY = resolutions[index].height;
        Screen.SetResolution(wantedResX, wantedResY, fullScreenMode);
        currentResolutionText.text = wantedResX + "x" + wantedResY;
    }

    public void ShowResolutionOptions()//opens the dropdown menu with available resolution options.
    {
        resolutionsPanelParent.SetActive(!resolutionsPanelParent.activeSelf);
    }

    public void SaveMenuVariables()
    {
        if (saveAs == saveFormat.playerprefs)
        {
            PlayerPrefs.SetInt("graphicsPrefsSaved", 0);

            PlayerPrefs.SetInt("graphicsSlider", Mathf.RoundToInt(qualityLevelSlider.value));
            PlayerPrefs.SetInt("antiAliasSlider", Mathf.RoundToInt(antiAliasSlider.value));
            PlayerPrefs.SetInt("shadowResolutionSlider", Mathf.RoundToInt(shadowResolutionSlider.value));
            PlayerPrefs.SetInt("textureQualitySlider", Mathf.RoundToInt(textureQualitySlider.value));
            PlayerPrefs.SetInt("anisotropicModeSlider", Mathf.RoundToInt(anisotropicModeSlider.value));
            PlayerPrefs.SetInt("anisotropicLevelSlider", Mathf.RoundToInt(anisotropicLevelSlider.value));

            PlayerPrefs.SetInt("wantedResolutionX", wantedResX);
            PlayerPrefs.SetInt("wantedResolutionY", wantedResY);

            int toggle = 0;
            if (!showFPS)
                toggle = 0;
            else toggle = 1;
            PlayerPrefs.SetInt("FPSToggle", toggle);

            if (vSyncToggle.isOn)
                toggle = 1;
            else toggle = 0;
            PlayerPrefs.SetInt("vSyncToggle", toggle);

            if (windowedModeToggle.isOn)
                toggle = 1;
            else toggle = 0;
            PlayerPrefs.SetInt("windowedModeToggle", toggle);
        }
        else if (saveAs == saveFormat.iniFile)
        {
            saveVars = new MenuVariables
            {
                Qualitylevel = Mathf.RoundToInt(qualityLevelSlider.value),
                ShowFPS = showFPS,
                ResolutionX = wantedResX,
                ResolutionY = wantedResY,
                WindowedMode = windowedModeToggle.isOn,
                VSync = vSyncToggle.isOn,
                AntiAliaslevel = Mathf.RoundToInt(antiAliasSlider.value),
                ShadowResolution = Mathf.RoundToInt(shadowResolutionSlider.value),
                TextureQuality = Mathf.RoundToInt(textureQualitySlider.value),
                AnisotropicMode = Mathf.RoundToInt(anisotropicModeSlider.value),
                AnisotropicLevel = Mathf.RoundToInt(anisotropicLevelSlider.value),

                Warning = "Edit this file at your own risk!"
            };

            string saveVasrJS = JsonUtility.ToJson(saveVars, true);
            File.WriteAllText(saveFileDataPath, saveVasrJS);

            saveVasrJS = null;
            saveVars = null;
        }
    }

    private void LoadMenuVariables()
    {
        if (saveAs == saveFormat.playerprefs)
        {
            if (PlayerPrefs.HasKey("graphicsPrefsSaved"))//to check if there are any.
            {
                qualityLevelSlider.value = PlayerPrefs.GetInt("graphicsSlider");
                antiAliasSlider.value = PlayerPrefs.GetInt("antiAliasSlider");
                shadowResolutionSlider.value = PlayerPrefs.GetInt("shadowResolutionSlider");
                textureQualitySlider.value = PlayerPrefs.GetInt("textureQualitySlider");
                anisotropicModeSlider.value = PlayerPrefs.GetInt("anisotropicModeSlider");
                anisotropicLevelSlider.value = PlayerPrefs.GetInt("anisotropicLevelSlider");

                wantedResX = PlayerPrefs.GetInt("wantedResolutionX");
                wantedResY = PlayerPrefs.GetInt("wantedResolutionY");
                currentResolutionText.text = wantedResX + "x" + wantedResY;

                int toggle = PlayerPrefs.GetInt("FPSToggle");
                if (toggle == 1)
                {
                    FPSToggle.isOn = true;
                    showFPS = true;
                }
                else
                {
                    FPSToggle.isOn = false;
                    showFPS = false;
                }

                toggle = PlayerPrefs.GetInt("windowedModeToggle");
                if (toggle == 1)
                    windowedModeToggle.isOn = true;
                else windowedModeToggle.isOn = false;

                toggle = PlayerPrefs.GetInt("vSyncToggle");
                if (toggle == 1)
                    vSyncToggle.isOn = true;
                else vSyncToggle.isOn = false;
            }
            else //no player prefs are saved.
            {
                //if nothing was saved use the full screen resolutions
                wantedResX = Screen.width;
                wantedResY = Screen.height;
                currentResolutionText.text = Screen.width + "x" + Screen.height;//sets the text of the Screen Resolution button to the res we start with.
            }
        }
        else if (saveAs == saveFormat.iniFile)
        {
            if (File.Exists(saveFileDataPath))//to check if the file exists.
            {
                string loadedVasrJS = File.ReadAllText(saveFileDataPath);
                saveVars = JsonUtility.FromJson<MenuVariables>(loadedVasrJS);

                qualityLevelSlider.value = saveVars.Qualitylevel;
                antiAliasSlider.value = saveVars.AntiAliaslevel;
                anisotropicModeSlider.value = saveVars.AnisotropicMode;
                anisotropicLevelSlider.value = saveVars.AnisotropicLevel;
                shadowResolutionSlider.value = saveVars.ShadowResolution;
                textureQualitySlider.value = saveVars.TextureQuality;

                wantedResX = saveVars.ResolutionX;
                wantedResY = saveVars.ResolutionY;
                currentResolutionText.text = wantedResX + "x" + wantedResY;

                FPSToggle.isOn = saveVars.ShowFPS;
                showFPS = saveVars.ShowFPS;

                windowedModeToggle.isOn = saveVars.WindowedMode;

                vSyncToggle.isOn = saveVars.VSync;

                loadedVasrJS = null;
                saveVars = null;
            }
            else //no settings were saved.
            {
                //if nothing was saved use the full screen resolutions
                wantedResX = Screen.width;
                wantedResY = Screen.height;
                currentResolutionText.text = Screen.width + "x" + Screen.height;
            }
        }
    }

    public void ResetToDefault()
    {
        //Setting Sliders and toggles 
        qualityLevelSlider.value = DefaultSettings.Qualitylevel;
        antiAliasSlider.value = DefaultSettings.AntiAliaslevel;
        anisotropicModeSlider.value = DefaultSettings.AnisotropicMode;
        anisotropicLevelSlider.value = DefaultSettings.AnisotropicLevel;
        shadowResolutionSlider.value = DefaultSettings.ShadowResolution;
        textureQualitySlider.value = DefaultSettings.TextureQuality;

        wantedResX = Screen.width;
        wantedResY = Screen.height;
        currentResolutionText.text = wantedResX + "x" + wantedResY;

        FPSToggle.isOn = DefaultSettings.ShowFPS;
        showFPS = DefaultSettings.ShowFPS;

        windowedModeToggle.isOn = DefaultSettings.WindowedMode;

        vSyncToggle.isOn = DefaultSettings.VSync;

        //Reading Sliders and toggles and setting everything accordingly.
        int graphicSetting = Mathf.RoundToInt(qualityLevelSlider.value);
        QualitySettings.SetQualityLevel(graphicSetting, true);
        qualityText.text = QualitySettings.names[graphicSetting];
        SetVSync();
        SetWindowedMode();
        SetAntiAlias();
        SetShadowResolution();
        SetTextureQuality();
        SetAnisotropicFiltering();
        SetAnisotropicFilteringLevel();
    }
}
