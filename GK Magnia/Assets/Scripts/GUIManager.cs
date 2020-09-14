using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIManager : MonoBehaviour
{

    [Header("StartScreen")]
    [SerializeField] private GameObject startButton;

    [Header("Win Screens")]    
    [SerializeField] private TextMeshProUGUI timeCounter;
    [SerializeField] private TextMeshProUGUI starsCounter;
    [SerializeField] private RectTransform[] starsTransform;
    private GameObject[] starsGO;
    [SerializeField] private Transform starsCounterLogoTransform;
    [SerializeField] private Transform[] startPoint;
    [SerializeField] private RectTransform[] starsInPlayMode;
    private Transform starsCounterTransform;
    private GameObject starsCounterGO;
    private GameObject timeCounterGO;
    private Vector3[] starsStartPos, starsStartScale;
    private Quaternion[] starsStartRot;
    private GameObject[] starsInPlay;
    
    private float scaleEffectTime = 1.0f;
    private float moveEffectTime = 1.0f;

    private int levelsStarsCount = 0;

    private string tempString;



    private void Awake()
    {

        GameManager.EventChangeState += OnChangeState;
        GameManager.EventChangeCurrentLevel += ChangeCurrentLevel;

        starsCounterGO = starsCounter.gameObject;
        starsCounterTransform = starsCounterGO.transform;

        timeCounterGO = timeCounter.gameObject;
        
        starsGO = new GameObject[starsTransform.Length];
        starsStartPos = new Vector3[starsTransform.Length];
        starsStartRot = new Quaternion[starsTransform.Length];
        starsStartScale = new Vector3[starsTransform.Length];

        starsInPlay = new GameObject[starsInPlayMode.Length];

        for (int i = 0; i < starsTransform.Length; i++)
        {

            starsGO[i] = starsTransform[i].gameObject;
            starsStartPos[i] = starsTransform[i].position;
            starsStartRot[i] = starsTransform[i].rotation;
            starsStartScale[i] = starsTransform[i].localScale;

        }

        for (int i = 0; i < starsInPlayMode.Length; i++)
        {
            starsInPlay[i] = starsInPlayMode[i].gameObject;
        }
        startButton.transform.localScale = Vector3.zero;
                
    }

    private void Start()
    {

        StartCoroutine(StartActionGUI());
        

    }

    private void Update()
    {
        if (GameManager.CurrentState == GameManager.Mode.Play)
        {
            DisableStars();
        }

        if (GameManager.CurrentState == GameManager.Mode.WaitToStart)
        {
            EnableStars();
        }

    }

    private void OnChangeState()
    {

        if (GameManager.CurrentState == GameManager.Mode.Win) StartCoroutine(OnWinActionGUI());

    }

    IEnumerator StartActionGUI()
    {

        yield return new WaitForSeconds(0.01f);

        LeanTween.scale(startButton, Vector3.one * 2, scaleEffectTime)
            .setFrom(Vector3.one * 0.1f)
            .setEase(LeanTweenType.easeOutElastic);

    }

    IEnumerator OnWinActionGUI()
    {             

        yield return new WaitForSeconds(0.5f);

        ResetGUI();

        levelsStarsCount = 0;

        //Появление звёзд
        for (int i = 0; i < GameManager.LevelGameDesign.Stars.Length; i++)
        {

            if (GameManager.GuiTimer < GameManager.LevelGameDesign.Stars[i])
            {

                ScaleEffectOnEnable(starsGO[i]);
                levelsStarsCount++;

            }

        }

        
        //for (int i = 0; i < GameManager.LevelGameDesign.Stars.Length; i++) if (GameManager.GuiTimer < GameManager.LevelGameDesign.Stars[i]) MoveEffectOnEnable(starsGO[i], starsTransform[i], startPoint[i]);  

        //Обновление таймера
        timeCounter.text = Mathf.RoundToInt(GameManager.GuiTimer).ToString();
        ScaleEffectOnEnableForText(timeCounterGO);

        //Показ количества уже накопленных звёзд
        AddStarsCount(0);

        yield return new WaitForSeconds(1.0f);

        //Полёт звёздочек с анимациями до звезды снизу и прибавка к количеству звёзд после нажатия или по таймеру
        for (int i = 0; i < GameManager.LevelGameDesign.Stars.Length; i++)
        {

            if (GameManager.GuiTimer < GameManager.LevelGameDesign.Stars[i])
            {

                yield return new WaitForSeconds(0.1f);
                MoveEndEffectOnEnable(starsGO[i], starsCounterLogoTransform.position, starsStartPos[i]);

            }

        }

        tempString = "LSC_" + GameManager.currentLevelName.ToString();
        if (PlayerPrefs.HasKey(tempString))
        {

            if (PlayerPrefs.GetInt(tempString) < levelsStarsCount) PlayerPrefs.SetInt(tempString, levelsStarsCount);

        }
        else PlayerPrefs.SetInt(tempString, levelsStarsCount);

        

    }

    private void DisableStars()
    {
        for (int i = 0; i < starsInPlayMode.Length; i++)
        {
            if (GameManager.GuiTimer > GameManager.LevelGameDesign.Stars[i])
            {

                ScaleEffectOnDisable(starsInPlay[i]);

            }


        }
    }

    private void EnableStars()
    {
        for (int i = 0; i < starsInPlayMode.Length; i++)
        {
           starsInPlay[i].transform.localScale=Vector3.one;

        }
    }
    private void ScaleEffectOnDisable(GameObject go)
    {
        LeanTween.scale(go, Vector3.zero, scaleEffectTime*0.5f);

    }
    private void ScaleEffectOnEnable(GameObject go)
    {

        LeanTween.scale(go, Vector3.one, scaleEffectTime)
            .setFrom(Vector3.one * 0.1f)
            .setEase(LeanTweenType.easeOutElastic);

    }
    private void ScaleEffectOnEnableForText(GameObject go)
    {

        LeanTween.scale(go, Vector3.one, scaleEffectTime / 2)
            .setFrom(Vector3.one * 0.2f)
            .setEase(LeanTweenType.easeOutBack);

    }

    private void MoveEffectOnEnable(GameObject go, Vector3 endPoint, Vector3 startPoint)
    {

        LeanTween.move(go, endPoint, moveEffectTime)
            .setFrom(startPoint)
            .setEase(LeanTweenType.easeOutBounce);
        
    }

    private void MoveEndEffectOnEnable(GameObject go, Vector3 endPoint, Vector3 startPoint)
    {

        LeanTween.move(go, endPoint, moveEffectTime)
            .setFrom(startPoint)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(AddStarsCount);

        LeanTween.rotateAround(go, Vector3.forward, 360, moveEffectTime);
            
        LeanTween.scale(go, Vector3.zero, moveEffectTime)
            .setFrom(Vector3.one)
            .setEase(LeanTweenType.easeInQuad);

    }

    private void AddStarsCount(int m_value)
    {

        GameManager.starsCount += m_value;

        PlayerPrefs.SetInt("StarsCount", GameManager.starsCount);

        starsCounter.text = GameManager.starsCount.ToString();
        ScaleEffectOnEnableForText(starsCounterGO);

    }
    private void AddStarsCount()
    {

        AddStarsCount(1);

    }

    private void ResetGUI()
    {

        for (int i = 0; i < starsTransform.Length; i++)
        {

            starsTransform[i].position = starsStartPos[i];
            starsTransform[i].rotation = starsStartRot[i];
            starsTransform[i].localScale = starsStartScale[i];

        }

    }

    public void ChangeCurrentLevel()
    {

        ResetGUI();
        for (int i = 0; i < starsTransform.Length; i++) starsTransform[i].localScale = Vector3.zero;

        tempString = "LSC_" + GameManager.currentLevelName.ToString();

        levelsStarsCount = PlayerPrefs.HasKey(tempString) ? PlayerPrefs.GetInt(tempString) : 0;        

        for (int i = 0; i < levelsStarsCount; i++)
        {

            ScaleEffectOnEnable(starsGO[i]);

        }

        
        

    }

    private void OnEnable()
    {

        GameManager.EventChangeState += OnChangeState;
        GameManager.EventChangeCurrentLevel += ChangeCurrentLevel;

    }

    private void OnDisable()
    {

        GameManager.EventChangeState -= OnChangeState;
        GameManager.EventChangeCurrentLevel -= ChangeCurrentLevel;

    }

    private void OnDestroy()
    {

        GameManager.EventChangeState -= OnChangeState;
        GameManager.EventChangeCurrentLevel -= ChangeCurrentLevel;

    }

}
