 
using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEditor;
using Object = UnityEngine.Object;

public class GameSequence : MonoBehaviour
{


    public static event Action EventChangeDifficulty;
    public static event Action OnLocalHighscore;
   
    public const int Child=0;   
    public const int Easy=1;
    public const int Normal=2;
    public const int Hard=3;
    public const int Insane=4;
    private int fpsCounter,quality;
    private bool SelectFPSMode;
    public static GameSequence instance;
    private int _CurrentDifficulty;
    private float timerFPS,idle;
    private bool started,startscene;
 
    
    //Resolution
    public Resolution[] allResolutionPC;
    public int numberCurrentResolution = 0;
    private int startResWidth, startResHeight;
    private int newResWidth, newResHeight;
    public float resolutionKoeff = 2.0f;
    //Language


    void Awake()
    {

        GameManager.CurrentState = GameManager.Mode.Start;

        init();
        CheckGraphicSettingsAndFPSOnStart();

    }


    public int CurrentDifficulty
    {

         get { return _CurrentDifficulty; }
        set
        {
             
           _CurrentDifficulty= Mathf.Clamp(value, 0, 4);
            PlayerPrefs.SetInt("Difficulty",_CurrentDifficulty);
            PlayerPrefs.Save();
        
         EventChangeDifficulty?.Invoke();
        }
    }

    public float CurrentHighTime
    {
        get
        {
            return PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "BestResultTime_" + CurrentDifficulty, 0);
        }
        set
        {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "BestResultTime_" + CurrentDifficulty, Mathf.RoundToInt(value));
            PlayerPrefs.Save();
        }
    }

    public bool CurrentLevelPassed => PlayerPrefs.HasKey(SceneManager.GetActiveScene().name + "BestResultTime_" + CurrentDifficulty);

   
    public static string GetStarsProgress(string levelName) => PlayerPrefs.GetString(levelName + "_starsProgress", "");

   


    public static GameSequence Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<GameSequence>();
                if (instance)
                    DontDestroyOnLoad(instance.gameObject);
            }
            if (!instance)
            {
                instance = new GameObject().AddComponent<GameSequence>();
                instance.gameObject.name = "GameSequence";

                instance.gameObject.transform.SetParent(null);
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    public void OpenLevel(string levelName)
    {
        PlayerPrefs.SetInt(levelName + "Opened_", CurrentDifficulty);
    }

    public void PassLevel(string levelName, int scores = 0)
    {
        scores = scores < 0 ? 0 : scores;
        PlayerPrefs.SetInt(levelName + "BestResultTime_" + CurrentDifficulty, scores);
    }

    public float GetLevelTime(string levelName)
    {
        var val = PlayerPrefs.GetInt(levelName + "BestResultTime_" + CurrentDifficulty, 0);
        if (val <= 0) val = 0;
        return val;
    }

    
    
    void Update()
    {
        if (started == false || GameManager.CurrentState == GameManager.Mode.Start && !SelectFPSMode)
        {
            idle += Time.unscaledDeltaTime;

            if (idle < 0)
            {
                timerFPS = 0;
                fpsCounter = 0;

            }
            else timerFPS += Time.unscaledDeltaTime;  

            if (SelectFPSMode && startscene)
            {

                if (Screen.width == 3840) ChangeResolutionMinus();

                GameManager.CurrentState = GameManager.Mode.Start;

                fpsCounter++;

                if (timerFPS > 1)
                {
                    if (fpsCounter < 29)
                    {

                        if (quality == 0) 
                        {
                            GameManager.ChangeState(GameManager.Mode.StartMenu);
                            started = true;
                        }
                        else if (quality <= 8) ChangeResolutionMinus();

                        GameObject.Find("CheckVideo").GetComponent<TextMeshProUGUI>().text += "."/* + quality*/;
                      
                        fpsCounter = 0;
                        idle = -2;
                        timerFPS = 0;

                    }
                    else
                    {

                        GameManager.ChangeState(GameManager.Mode.StartMenu);
                        started = true;

                    }
                }
            }
            else
            {
                if (startscene)
                {
                    if (timerFPS > 5)
                    {
                        started = true;
                        GameManager.ChangeState(GameManager.Mode.StartMenu);
                    }
                }
                else
                {
                    started = true;
                  
                }
            }
        }

    }

    void CheckGraphicSettingsAndFPSOnStart()
    {

        //Запуск перевода, чтобы в игре сразу был нужный язык, без надобности входить в настройки
       

        if (SceneManager.GetActiveScene().name == "001") startscene = true;
        else startscene = false;

        started = false;
        quality = 10;
        fpsCounter = 0;
        idle = -2;

        if (PlayerPrefs.HasKey("QualityLevel")) SelectFPSMode = false;
        else SelectFPSMode = true;

        //ResolutionControl
        startResWidth = Screen.width;
        startResHeight = Screen.height;

#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        allResolutionPC = Screen.resolutions;       
        numberCurrentResolution = allResolutionPC.Length - 1;
#endif
#if UNITY_STANDALONE_WIN
        if (PlayerPrefs.HasKey("ResWidth"))
        {

            int tempWidth = PlayerPrefs.GetInt("ResWidth");
            Screen.SetResolution(tempWidth, PlayerPrefs.GetInt("ResHeight"), Screen.fullScreen);
            for (int i = 0; i < allResolutionPC.Length; i++) if (Mathf.RoundToInt(allResolutionPC[i].width) < tempWidth) numberCurrentResolution = i; 

        }
        else ChangeResolutionPC();
#endif
#if UNITY_ANDROID || UNITY_IPHONE
        resolutionKoeff = PlayerPrefs.HasKey("ResolutionKoeff") ? PlayerPrefs.GetFloat("ResolutionKoeff") : 1.0f;
        SetResolutionPhone(Screen.fullScreen);
#endif
    }

    public void init()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 90;

        if (!PlayerPrefs.HasKey("001Opened_")) PlayerPrefs.SetInt("001Opened_", CurrentDifficulty);

        CheckContinue();
        idle = -2;
        timerFPS = 0;        

    }    

    public void Win()
    {
        
          var currentHighTime = CurrentHighTime;
        if (currentHighTime != -1)
        {
            if (CurrentLevelPassed)
            {
                if (currentHighTime == 0 || GameManager.MyTimer < currentHighTime)
                {
                    CurrentHighTime = GameManager.MyTimer;
                    if (OnLocalHighscore != null) OnLocalHighscore();
                }
            }
            else
            {
                CurrentHighTime = GameManager.MyTimer;
                OpenNextLevels();
            }
        }

        GameManager.ChangeState(GameManager.Mode.Win);
    }

    public void OpenNextLevels()
    {
        
    }

    public void CheckContinue()
    {

        if (PlayerPrefs.HasKey("Difficulty"))
        {
            CurrentDifficulty = PlayerPrefs.GetInt("Difficulty");
        }
        else CurrentDifficulty = Normal;

    }

    public void RemoveAllPassData()
    {
       
    }

    public void SaveLastLevelOpened()
    {
     
    }
  
    public bool CheckLevelIsWinned(string levelName = null)
    {
        if (levelName == null) levelName = SceneManager.GetActiveScene().name;
        return PlayerPrefs.HasKey(levelName + "BestResultTime_" + CurrentDifficulty);
    }
    public bool CheckLevelIsOpened(string levelName = null)
    {
        if (levelName == null) levelName = SceneManager.GetActiveScene().name;
        return PlayerPrefs.HasKey(levelName + "Opened_");
    }

   
        
    public void ChangeResolutionMinus()
    {
 
#if UNITY_ANDROID
        resolutionKoeff = (resolutionKoeff < 4.0f) ? (resolutionKoeff + 1.0f) : 4.0f;
        SetResolutionPhone(Screen.fullScreen);
#endif
    }

    public void ChangeResolutionPlus()
    {
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        numberCurrentResolution = numberCurrentResolution < allResolutionPC.Length - 1 ? numberCurrentResolution + 1 : numberCurrentResolution;
        ChangeResolutionPC();
#endif
#if UNITY_ANDROID
        resolutionKoeff = (resolutionKoeff > 2.0f) ? (resolutionKoeff - 1.0f) : 1.0f;
        SetResolutionPhone(Screen.fullScreen);
#endif
    }

    public void ChangeResolutionPC()
    {

        Screen.SetResolution(allResolutionPC[numberCurrentResolution].width, allResolutionPC[numberCurrentResolution].height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResWidth", allResolutionPC[numberCurrentResolution].width);
        PlayerPrefs.SetInt("ResHeight", allResolutionPC[numberCurrentResolution].height);
       
    }

    public void SetResolutionPhone(bool isFullScreen)
    {
        
        newResWidth = Mathf.RoundToInt(startResWidth / resolutionKoeff);
        newResHeight = Mathf.RoundToInt(startResHeight / resolutionKoeff);
      
        if (isFullScreen) Screen.SetResolution(newResWidth, newResHeight, FullScreenMode.FullScreenWindow);
        else Screen.SetResolution(newResWidth, newResHeight, FullScreenMode.Windowed);

        PlayerPrefs.SetFloat("ResolutionKoeff", resolutionKoeff);

      
    }

}

public static class ListHelper
{
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}