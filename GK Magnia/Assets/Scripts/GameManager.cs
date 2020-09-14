using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
 
 



using UnityEngine.Internal;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
 
#endif
using UnityEngine.Rendering;
using UnityEngine.Scripting;
using Debug = UnityEngine.Debug;
using UnityEngine.Events;
using Lean.Touch;
 
[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{ 
    public static LayerMask LayerGO,
        LayerNoPhysics,
        LayerDefault,
        LayerNoPhysicsSpam,
        LayerMouseTarget,
        LayerDefaultGOSpam,
        LayerSpam;
    public static bool TransformingNow;
    public static Mode CurrentState;
    public static int WinPercent, jointWinPercent;
    public static float MyTimer, GuiTimer;
 
    public static GameObject MouseTarget;
    public enum Mode
    {

        None,
        Play,
        Pause,
        Resume,
        Win,
        Restart,
        Lose,
        Settings,
        GoToLevel, WaitToStart,
        MainMenu,
        ExitGame,
        StartMenu,
        Start,
        Achievements,
        Share,
        NextLevel
     }
    GameObject nodesParentGameObject;
    public static List<link> linkNodes = new List<link>();
    public static List<link> AllNodes = new List<link>();

    public static float[] nodesglued;
    private GameObject[][] Aims;
    public static List<GameObject> Levels = new List<GameObject>();    
    private List<link> TempNodes;
   

    public static link LockedObject ;
    
    

    [HideInInspector] public static bool IsGlued;
    public static string temp, sceneName;
    public static event Action EventChangeState;
      

    public static int PercentText, LastProcent;

    private Vector3[] _startPos;
    private Quaternion[] _startRot;
    public static float _GlueTime, starthash, starthashrot, WaitToStartTimer,StartVfxTimer;

    public static int WinJoint;
    public static Level LevelGameDesign;
    public static Camera _MainCamera;
    public static Vector3 camStartPos;
    public static Quaternion camStartRot;

    private List<GameObject> m_ListGameObject;
    public static int currentLevel;
    public static string currentLevelName;
    private float m_duration = 1.0f;
    private Vector3 oldLevelPoint = new Vector3(-2.0f, 0.0f, 0.0f);
    private Vector3 nextLevelPoint = new Vector3(2.0f, 0.0f, 0.0f);
    private GameObject nextLevelGO;
    private Transform nextLevelTransform;
    private GameObject oldLevelGO;
    private Transform oldLevelTransform;    
    private List<link> allLinkNodes;
    private List<link> oldLinkNodes;
    public static int starsCount;
    public static event Action EventChangeCurrentLevel;
    public bool isManualChooseLevel = false;
    private static ParticleSystem vfxConfety;


    virtual public void Awake()
    {
         
        Time.timeScale = 1;       

        if (Application.isPlaying)
        {
     
            Initialization();
            ResetLevel();  
            
        }

    }
    
    public virtual void Update()
    {

        if (Application.isPlaying)
        {

            GuiTimer += Time.unscaledDeltaTime;

            switch (CurrentState)
            {
                case Mode.Play:
                    MyTimer += Time.deltaTime;
                    CheckLoose();
                    break;

                case Mode.WaitToStart:                    
                    WaitForStartGame();
                    break;
 
                case Mode.Win:
                    AfterWinAction();
                    break;

                case Mode.NextLevel:
                    if (!isManualChooseLevel) InitializedNextLevel();
                    ChangeState(Mode.WaitToStart);
                    break;
                    
                case Mode.Lose:
                    ProcessLose();
                    break;
               
                case Mode.ExitGame:
                    Debug.Log("Exit Game");
                    Application.Quit();
                    break;
                case Mode.StartMenu:
                     
                    break;
                default:
                    break;
            }

            KeyBoardInGame();

            if (Debug.isDebugBuild) KeyBoardHacks();
           
        }

    }      

    public static void ChangeState(Mode s)
    {

        if (Application.isPlaying)
        {
           
            GuiTimer = 0;
            switch (s)
            { 
                case Mode.Pause:
                    CurrentState = Mode.Pause;
                    Time.timeScale = 0;

                    break;

                case Mode.Win:
                    Time.timeScale = 1;
                    
                    CurrentState = Mode.Win;
                    WaitToStartTimer = 0;
                     SwitchIsKinematicNodes(true);                    
                    //getSocialCapture.StopCapture();
                    //   SpawnWinFX();
                    break;
                case Mode.Lose:
                    Time.timeScale = 1;
                    CurrentState = Mode.Lose;
                    WaitToStartTimer = 0;
                    SwitchIsKinematicNodes(true);
                    //getSocialCapture.StopCapture();
                    break;
                case Mode.Resume:
                    CurrentState = Mode.Play;

                    Time.timeScale = 1;
                    break;

               

                case Mode.WaitToStart:

                    
                   
                    SwitchIsKinematicNodes(true);
                    WaitToStartTimer = 0;
                    if (linkNodes.Count > 0) LockedObject = linkNodes[0];
                    CurrentState = Mode.WaitToStart;
                    break;

               
                case Mode.Restart:
                    Time.timeScale = 1;
                    WaitToStartTimer = 0;


                    ChangeState(Mode.WaitToStart);
                    break;

                case Mode.Play:
                         
                    if(linkNodes[0]!=null) LockedObject = linkNodes[0];
                    CurrentState = Mode.Play;
                    Time.timeScale = 1;
                    break;
               

               
                case Mode.MainMenu:
                    Time.timeScale = 1;
                    CurrentState = Mode.MainMenu;
                    break;

                case Mode.NextLevel:
                    CurrentState = Mode.NextLevel;                    
                    break;

                case Mode.ExitGame:
                    CurrentState = Mode.ExitGame;
                    break;
              
                default:
                    CurrentState = s;
                    break;

            }



       EventChangeState();
        }
    } 
    void SimplifyLevel(int c)
    {

        List<link> TempLinks = new List<link>();
        TempLinks.AddRange(linkNodes);

        for (int i = 0; i < c; i++)
        {
            link temp = TempLinks[UnityEngine.Random.Range(0, TempLinks.Count)];
            WinJoint++;

            temp.joints[0] = (FixedJoint)temp.gameObject.AddComponent(typeof(FixedJoint));
            temp.joints[0].connectedBody = temp.LinksToGlue[0].RB;


            int id = Array.FindIndex(temp.LinksToGlue[0].LinksToGlue, (x) => x == temp);
            temp.LinksToGlue[0].joints[id] = (FixedJoint)temp.LinksToGlue[0].gameObject.AddComponent(typeof(FixedJoint));
            temp.LinksToGlue[0].joints[id].connectedBody = temp.RB;
            temp.LinksToGlue[0].GluedInGameNodes.Add(temp);
            temp.GluedInGameNodes.Add(temp.LinksToGlue[0]);

            TempLinks.Remove(temp);
            TempLinks.Remove(temp.LinksToGlue[0]);

        }
    }
    public virtual void Explode()
    {
        
        ResetLevel();            
        {
                
            if (linkNodes.Count == 5) SimplifyLevel(1);            
            if (linkNodes.Count == 6) SimplifyLevel(2);            
            if (linkNodes.Count == 7) SimplifyLevel(3);            
            if (linkNodes.Count == 8) SimplifyLevel(3);            
            if (linkNodes.Count == 9) SimplifyLevel(4);            
            if (linkNodes.Count == 10) SimplifyLevel(5);            
            if (linkNodes.Count > 10) SimplifyLevel(5);
            
        }
      
        SwitchIsKinematicNodes(false);

        List<Vector3> Targets = new List<Vector3>();
        List<link> linktargets = linkNodes;
        GameObject GO = new GameObject();

        GO.transform.position = nodesParentGameObject.transform.position;
        GO.transform.SetParent(null);
        for (int i = 0; i < linkNodes.Count; i++)
        {

            GameObject GO1 = new GameObject();// GameObject.CreatePrimitive(PrimitiveType.Sphere);

            GO1.transform.position = GO.transform.position + Vector3.up * 2;
            GO1.transform.SetParent(GO.transform);                       

            GO.transform.Rotate(Vector3.forward, i * (360 / linkNodes.Count) + UnityEngine.Random.Range(-30, 30));

            //GO.transform.Rotate(nodesParentGameObject.transform.rotation.eulerAngles, Space.Self);
            Targets.Add(GO1.transform.position);
            Destroy(GO1, 1);

        }

        for (int i = 0; i < linkNodes.Count; i++)
        {

            float min = 1000000;
            int id = 0;
            for (int j = 0; j < Targets.Count; j++)
            {
                float dis = Vector3.SqrMagnitude(linkNodes[i].transform.position - Targets[j]);
                if (dis < min)
                {
                    min = dis;
                    id = j;
                }
            }

            linkNodes[i].RB.AddForce(0.13f * LevelGameDesign.ExplodeForce * (Targets[id] - linkNodes[i].transform.position), ForceMode.VelocityChange);
            Targets.RemoveAt(id);

            if (GameSequence.Instance.CurrentDifficulty > 2)
            {
                linkNodes[i].RB.AddTorque(linkNodes[i].transform.position * UnityEngine.Random.Range(120f, 360f) * UnityEngine.Random.Range(-1f, 1f));
            }

        }

        Destroy(GO, 5);

    } 

    public virtual void Initialization()
    {

        sceneName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("Level", sceneName);
        Debug.Log("Scene Load: " + sceneName);

        currentLevelName = PlayerPrefs.HasKey("CurrentLevelName") ? PlayerPrefs.GetString("CurrentLevelName") : "001sportcar_a";
        PlayerPrefs.SetString("CurrentLevelName", currentLevelName);

        InitializedAllLevels();
        InitializedLevelGameDesign();
        
        ///<summary>
        ///Закомментировано, чтобы не мешало отсутствие курсора при тестировании 
        /// </summary>
        //Cursor.visible = false;

        _MainCamera = Camera.main;

        starsCount = PlayerPrefs.HasKey("StarsCount") ? PlayerPrefs.GetInt("StarsCount") : 0;
        
        InitializedParametrs();

        LayerGO = LayerMask.NameToLayer("GO");
        LayerDefaultGOSpam = LayerMask.GetMask("GO", "Default", "Spam");
        LayerMouseTarget = LayerMask.GetMask("MouseTarget");
        LayerDefault = LayerMask.NameToLayer("Default");
        LayerNoPhysics = LayerMask.NameToLayer("IgnorePhysics");
        LayerNoPhysicsSpam = LayerMask.NameToLayer("IgnorePhysicsSpam");
        LayerSpam = LayerMask.NameToLayer("Spam");
         
        camStartPos = _MainCamera.transform.position;
        camStartRot = _MainCamera.transform.rotation;
     
        MouseTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MouseTarget.transform.position = Vector3.zero;
        MouseTarget.name = "MouseTarget";
        MouseTarget.transform.localScale = new Vector3(2 * 5, 3 * 4, 0.01f);
        MouseTarget.GetComponent<MeshRenderer>().enabled = false;
        MouseTarget.AddComponent<BoxCollider>();
        MouseTarget.layer = LayerMask.NameToLayer("MouseTarget");
        MouseTarget.GetComponent<BoxCollider>().isTrigger = true;
        MouseTarget.SetActive(false);
               
    }

    private void InitializedAllLevels()
    {

        m_ListGameObject = new List<GameObject>();
        allLinkNodes = new List<link>();
        allLinkNodes.AddRange(FindObjectsOfType<link>());
        GameObject tempGOLinkParent;
        for (int i = 0; i < allLinkNodes.Count; i++)
        {

            allLinkNodes[i].InitializeNode();

            tempGOLinkParent = allLinkNodes[i].transform.parent.gameObject;

            if (!m_ListGameObject.Contains(tempGOLinkParent)) m_ListGameObject.Add(tempGOLinkParent);

        }

        int tempCount = m_ListGameObject.Count;
        List<string> tempMassString = new List<string>();
        GameObject[] tempMassGOforSort = new GameObject[tempCount];
        
        for (int i = 0; i < tempCount; i++)
        {

            tempMassString.Add(m_ListGameObject[i].name);
            tempMassGOforSort[i] = m_ListGameObject[i];

        }

        tempMassString.Sort();

        for (int i = 0; i < tempMassString.Count; i++)
        {

            for (int j = 0; j < tempMassGOforSort.Length; j++)
            {

                if (tempMassString[i] == tempMassGOforSort[j].name)
                {

                    m_ListGameObject[i] = tempMassGOforSort[j];
                    break;

                }

            }

        }

        string tempString;
        for (int i = 0; i < m_ListGameObject.Count; i++)
        {

            tempString = m_ListGameObject[i].name.Length > currentLevelName.Length ? m_ListGameObject[i].name.Substring(0, currentLevelName.Length) : m_ListGameObject[i].name;

            if (tempString == currentLevelName)
            {

                nodesParentGameObject = m_ListGameObject[i];
                currentLevel = i;

            }
            else m_ListGameObject[i].SetActive(false);

        }

        if (nodesParentGameObject == null)
        {

            currentLevel = 0;
            if (m_ListGameObject.Count > 0)
            {

                nodesParentGameObject = m_ListGameObject[currentLevel];
                currentLevelName = nodesParentGameObject.name;

            }

        }

        InitializedLinkNodes();

        if (nodesParentGameObject != null) nodesParentGameObject.SetActive(true);

        vfxConfety = GameObject.Find("vfx_confetyFanFare").GetComponent<ParticleSystem>();
        vfxConfety.Pause();
    }

    private void InitializedLevelGameDesign()
    {

        LevelGameDesign = Resources.Load<Level>("GameDesign/" + currentLevelName);
        if (LevelGameDesign == null) LevelGameDesign = Resources.Load<Level>("GameDesign/DefaultLevel");

    }

    private void InitializedParametrs()
    {

        GuiTimer = 0.0f;
        WinPercent = 0;
        IsGlued = false;

    }

    private void InitializedLinkNodes()
    {

        oldLinkNodes = linkNodes;
        linkNodes = new List<link>();
        if (nodesParentGameObject != null) linkNodes.AddRange(nodesParentGameObject.GetComponentsInChildren<link>());

    }

    public void InitializedNextLevel()
    {

        if (CurrentState == Mode.Win || CurrentState == Mode.NextLevel)
        {

            InitializedParametrs();

            oldLevelGO = m_ListGameObject[currentLevel];
            oldLevelTransform = oldLevelGO.transform;

            currentLevel = m_ListGameObject.Count > currentLevel + 1 ? currentLevel + 1 : 0;
            EventChangeCurrentLevel();

            currentLevelName = m_ListGameObject[currentLevel].name;
            PlayerPrefs.SetString("CurrentLevelName", currentLevelName);

            m_ListGameObject[currentLevel].SetActive(true);
            nextLevelGO = m_ListGameObject[currentLevel];
            nextLevelTransform = nextLevelGO.transform;
            nextLevelTransform.position = nextLevelPoint;
            nodesParentGameObject = nextLevelGO;
            InitializedLinkNodes();

            ////nextLevelTransform.localScale = Vector3.one;
            ////oldLevelTransform.localScale = Vector3.one;

            LeanTween.move(nodesParentGameObject, Vector3.zero, m_duration)
                .setFrom(nextLevelPoint)
                .setEase(LeanTweenType.easeOutBack);

            LeanTween.move(oldLevelGO, oldLevelPoint, m_duration)
                .setEase(LeanTweenType.easeSpring)
                .setOnComplete(SetOffOldLevelGO);
            
        } 

    }

    public void ManualNextLevel()
    {

        isManualChooseLevel = true;

        InitializedNextLevel();

    }

    public void InitializedPreviousLevel()
    {

        if (CurrentState == Mode.Win || CurrentState == Mode.NextLevel)
        {

            isManualChooseLevel = true;
            
            InitializedParametrs();

            oldLevelGO = m_ListGameObject[currentLevel];
            oldLevelTransform = oldLevelGO.transform;

            currentLevel = currentLevel > 0 ? currentLevel - 1 : m_ListGameObject.Count - 1;
            EventChangeCurrentLevel();
            currentLevelName = m_ListGameObject[currentLevel].name;
            PlayerPrefs.SetString("CurrentLevelName", currentLevelName);

            m_ListGameObject[currentLevel].SetActive(true);
            nextLevelGO = m_ListGameObject[currentLevel];
            nextLevelTransform = nextLevelGO.transform;
            nextLevelTransform.position = oldLevelPoint;
            nodesParentGameObject = nextLevelGO;
            InitializedLinkNodes();

            nextLevelTransform.localScale = Vector3.one;
            oldLevelTransform.localScale = Vector3.one;

            LeanTween.move(nodesParentGameObject, Vector3.zero, m_duration)
                .setFrom(oldLevelPoint)
                .setEase(LeanTweenType.easeOutBack);

            LeanTween.move(oldLevelGO, nextLevelPoint, m_duration)
                .setEase(LeanTweenType.easeSpring)
                .setOnComplete(SetOffOldLevelGO);

        }            

    }

    private void SetOffOldLevelGO()
    {
        
        oldLevelGO.SetActive(false);

    }
    
    void KeyBoardInGame()
    {
        /*
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (CurrentState == Mode.Play) ChangeState(Mode.Pause);
                    else if (CurrentState == Mode.Pause) ChangeState(Mode.Play);
                    else if (CurrentState == Mode.Win) ChangeState(Mode.Restart);
                    else if (CurrentState == Mode.WaitToStart) ChangeState(Mode.MainMenu);
                    else if (CurrentState == Mode.MainMenu) ChangeState(Mode.WaitToStart);
                }
                */

        if (Input.GetKeyDown(KeyCode.Escape))
        {
        
        }



        if (Input.GetKeyDown(KeyCode.Space)&& Input.GetKeyDown(KeyCode.F5))
        {
            MyTimer = 99;
            GameSequence.instance.Win();
        }    
        if (Input.GetKeyDown(KeyCode.Space)&& Input.GetKeyDown(KeyCode.F6))
        {
         //   LevelsContainer.Instance.OpenAllLevels();
          //  LevelsContainer.Instance.PassAllLevels();

        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && Input.GetKeyDown(KeyCode.F5))
        {
            int tempscene = SceneManager.GetActiveScene().buildIndex + 1;

            if (tempscene >= SceneManager.sceneCountInBuildSettings) tempscene = 0;
            if (SceneManager.GetSceneByBuildIndex(tempscene).name == "Resources") tempscene = 0;
            SceneManager.LoadScene(tempscene);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && Input.GetKeyDown(KeyCode.F5))
        {
            int tempscene = SceneManager.GetActiveScene().buildIndex - 1;

            if (tempscene < 0) tempscene = SceneManager.sceneCountInBuildSettings - 2;
            if (SceneManager.GetSceneByBuildIndex(tempscene).name == "Resources") tempscene = 0;
            SceneManager.LoadScene(tempscene);
        }
    }

    void KeyBoardHacks()
    {



        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyTimer = 999;
            GameSequence.instance.Win();
        }



        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            int tempscene = SceneManager.GetActiveScene().buildIndex - 1;

            if (tempscene < 0) tempscene = SceneManager.sceneCountInBuildSettings - 2;
            if (SceneManager.GetSceneByBuildIndex(tempscene).name == "Resources") tempscene = 0;
            SceneManager.LoadScene(tempscene);
        }



        ;

        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeState(Mode.Restart);
        }

    }










    public static void SwitchIsKinematicNodes(bool state)
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < linkNodes.Count; i++)
            {

                if (state == true)
                {
                    linkNodes[i].gameObject.layer = LayerNoPhysics;
                    linkNodes[i].MC.isTrigger = true;
                }
                else
                {
                    linkNodes[i].gameObject.layer = LayerGO;
                    linkNodes[i].MC.isTrigger = false;
                }

                linkNodes[i].RB.isKinematic = state;

            }
            /*
                        for (int i = 0; i < spamNodes.Count; i++)
                        {

                            if (state == true)
                                spamNodes[i].gameObject.layer = LayerNoPhysicsSpam;
                            else
                                spamNodes[i].gameObject.layer = LayerDefault;

                            spamNodes[i].RB.isKinematic = state;

                        }
                    }
                    */
        }
    }   /*/
    protected virtual List<link> CleanSpam(List<link> nodes)
    {

        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i] is Spam || nodes[i] is Catapult || nodes[i] is LinkUntouchAble)
            {

                nodes.Remove(nodes[i]);
                i--;

            }

        }

        return nodes;

    }
    */
   

   
    public static void SetLevel(string name)
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            Levels[i].SetActive(Levels[i].name == name);
        }
    }

    public virtual void ResetLevel() 
    {

        MyTimer = 0;        
        WinJoint = 0;
        _GlueTime = 0;
        LastProcent = 0;       

        linkNodes.Clear();
        linkNodes.AddRange(FindObjectsOfType<link>());
        AllNodes = new List<link>();
        AllNodes.AddRange(linkNodes);

        //CleanSpam(linkNodes);
        
        for (int i = 0; i < linkNodes.Count; i++)
        {
                         
            for (int j = 0; j < linkNodes[i].joints.Length; j++)             
                linkNodes[i].ResetNodeLocalForPuzzle();

        }         

        nodesglued = new float[linkNodes.Count];
        starthash = 0;
        starthashrot = 0;

        for (int i = 0; i < linkNodes.Count; i++)
        {

            nodesglued[i] = 0;

            if (i != linkNodes.Count - 1)
            {

                starthash +=
                    Vector3.SqrMagnitude(linkNodes[i].transform.position - linkNodes[i + 1].transform.position);
                starthashrot += Vector3.Angle(linkNodes[i].transform.rotation.eulerAngles,
                    linkNodes[i + 1].transform.rotation.eulerAngles);

            }

        }

        InitializedLevelGameDesign();

    }
    
    void LerpToStartPositions()    
    {

        //nodesParentGameObject.transform.rotation = Quaternion.Slerp(nodesParentGameObject.transform.rotation, Quaternion.identity, WaitToStartTimer);

        WaitToStartRotationModel();

        for (int i = 0; i < linkNodes.Count; i++)            
        {

            linkNodes[i].transform.localPosition = Vector3.Lerp(linkNodes[i].transform.localPosition, linkNodes[i].LocalStartPos, WaitToStartTimer);

            linkNodes[i].transform.localRotation = Quaternion.Lerp(linkNodes[i].transform.localRotation, linkNodes[i].LocalStartRot, WaitToStartTimer);

        }



        // nodesParentGameObject.transform.rotation = Quaternion.Slerp(nodesParentGameObject.transform.rotation, startrot, WaitToStartTimer);
        // if (WaitToStartTimer < 1.4f)
        {
      //      nodesParentGameObject.transform.rotation = Quaternion.Lerp(nodesParentGameObject.transform.rotation,
      //          parentStartRotation, Time.deltaTime * 10);
        }
    }
    public virtual void WaitForStartGame()
    {

        if (CurrentState == Mode.WaitToStart)
        {

            WaitToStartTimer += Time.deltaTime;

            if (WaitToStartTimer > 0.55f)
            {

                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {

                    if (nodesParentGameObject != null)
                    {

                        nodesParentGameObject.transform.position = Vector3.zero;
                        isManualChooseLevel = false;
                        Explode();
                        ChangeState(Mode.Play);
                        
                    }
                }
                else WaitToStartRotationModel();

            }
            else LerpToStartPositions();            
        }

    }

    private void WaitToStartRotationModel()
    {

        if (nodesParentGameObject != null)
        {

            nodesParentGameObject.transform.position = Vector3.Lerp(nodesParentGameObject.transform.position, Vector3.zero, m_duration * Time.deltaTime);
            nodesParentGameObject.transform.RotateAround(Vector3.up, 1 * Time.deltaTime);

        }        

        for (int i = 0; i < oldLinkNodes.Count; i++)
        {

            oldLinkNodes[i].transform.localPosition = Vector3.Lerp(oldLinkNodes[i].transform.localPosition, oldLinkNodes[i].LocalStartPos, WaitToStartTimer);
            oldLinkNodes[i].transform.localRotation = Quaternion.Lerp(oldLinkNodes[i].transform.localRotation, oldLinkNodes[i].LocalStartRot, WaitToStartTimer);

        }


    }

    private void InsaneRotation()
    {
        nodesParentGameObject.transform.Rotate(new Vector3(360, 360, 360) *
                                               Time.deltaTime);
        Vector3 VerticalShift = new Vector3(0,
            0.15f * Mathf.Sin(Time.time + linkNodes[0].RandomRotateOnStart.magnitude * 10), 0);

        nodesParentGameObject.transform.position = Vector3.Lerp(nodesParentGameObject.transform.position,
            nodesParentGameObject.transform.position + VerticalShift, Time.deltaTime);
    }

    private void NormalRotationOnStart()
    {
        
            nodesParentGameObject.transform.Rotate(new Vector3(0, 360, 0) *
                                                   Time.deltaTime*0.33f);
            Vector3 VerticalShift = new Vector3(0,
                0.15f * Mathf.Sin(Time.time + linkNodes[0].RandomRotateOnStart.magnitude * 10), 0);

            nodesParentGameObject.transform.position = Vector3.Lerp(nodesParentGameObject.transform.position,
                nodesParentGameObject.transform.position + VerticalShift, Time.deltaTime);
 
    }

    private void EasyNormalRotationOnStart()
    {
    
        nodesParentGameObject.transform.Rotate(new Vector3(0, 360, 0) *Time.deltaTime*0.2f);
    }

    protected void TutorialState()
    {
        WaitToStartTimer += Time.unscaledDeltaTime;
    }

    public virtual void AfterWinAction()
    {

        WaitToStartTimer += Time.deltaTime;

        float temp = WaitToStartTimer * WaitToStartTimer  ;
      
            for (int i = 0; i < linkNodes.Count; i++)
            {

                linkNodes[i].transform.localPosition = Vector3.Lerp(linkNodes[i].transform.localPosition,
                    linkNodes[i].LocalStartPos, temp * 0.5f);
                linkNodes[i].transform.localRotation = Quaternion.Lerp(linkNodes[i].transform.localRotation,
                    linkNodes[i].LocalStartRot, temp * 0.5f);

                //linkNodes[i].transform.localPosition = Vector3.Lerp(linkNodes[i].transform.localPosition,
                //    linkNodes[i].LocalStartPos, temp * 0.03f);
                //linkNodes[i].transform.localRotation = Quaternion.Lerp(linkNodes[i].transform.localRotation,
                //    linkNodes[i].LocalStartRot, temp * 0.03f);

            }

            /*
            for (int i = 0; i < spamNodes.Count; i++)
            {
    
                spamNodes[i].transform.position = Vector3.Lerp(spamNodes[i].transform.position, spamNodes[i].WorldStartPos,
                    temp * 0.03f);
                spamNodes[i].transform.rotation = Quaternion.Lerp(spamNodes[i].transform.rotation,
                    spamNodes[i].WorldStartRot, temp * 0.03f);
    
            }
            */
            nodesParentGameObject.transform.position =
                Vector3.Lerp(nodesParentGameObject.transform.position, Vector3.zero, temp);
            nodesParentGameObject.transform.RotateAround(Vector3.up, 5 * Time.deltaTime);
            Vector3 VerticalShift = new Vector3(0, 0.15f * Mathf.Sin(Time.time), 0);
            nodesParentGameObject.transform.position = Vector3.Lerp(nodesParentGameObject.transform.position,
                VerticalShift, WaitToStartTimer);
            nodesParentGameObject.transform.position = Vector3.Lerp(nodesParentGameObject.transform.position,
                VerticalShift, WaitToStartTimer);


        
    }

    
    public  static void CheckWinGame()
    {

        if (CurrentState == Mode.Play)
        {

            float curdist = 0;
            float currot = 0;
            WinPercent = 0;
            int numberLink=0;
            for (int i = 0; i < linkNodes.Count; i++)
            {

                if (i != linkNodes.Count - 1)
                {

                    curdist += Vector3.SqrMagnitude(linkNodes[i].transform.position -
                                                    linkNodes[i + 1].transform.position);
                    currot += Vector3.Angle(linkNodes[i].transform.rotation.eulerAngles,
                        linkNodes[i + 1].transform.rotation.eulerAngles);
                    numberLink = i;

                }

            }

            /// <summary>
            /// Если уже есть соединения, то добавляем к процентам фиксированный процент. 
            /// Чтобы не было такого: типо мы посклеивали уже разные части, но при удалении всё-равно процент сборки ниже 10 %
            /// Игрок должен наглядно видеть результаты своих мучений
            /// </summary>
            /// Здесь вот именно рассчитываем сам процент уже собранного
            if (WinJoint > 0) jointWinPercent = Mathf.RoundToInt((100 / (linkNodes.Count + 1)) * (WinJoint / 2));
            else jointWinPercent = 0;
           
            /// <summary>
            /// При высчитывании процентов, аппроксимируем наши проценты на оставшийся отрезок процентов. 
            /// Т.е. проценты состоят из 2х частей - процент от собранных частей (ниже него WinPercent не должен опускаться) и вычислений от расстояний.
            /// Вот вычисления от расстояния мы и переделываем, учитывая, что процент не от 0 уже считается, а от процента собранных частей. 
            /// </summary>
            WinPercent =
                Mathf.Clamp(
                    Mathf.RoundToInt((((starthash / curdist) * 100) * (100 - jointWinPercent)) / 100) + jointWinPercent,
                    0, 99);

            if (WinJoint >= linkNodes.Count - 1)
            {

                //Увеличил расстояние с 0,00001f  до 0,001f, чтобы фиксация победы лучше была
                if ((curdist - starthash) * (curdist - starthash) <  0.0001f)
                {

                    if (currot < 5)
                    {
                        
                        StartVfxTimer += Time.deltaTime;
                        if (StartVfxTimer > 0.4f)
                        {
                            linkNodes[numberLink].UseGlueEffect(true);
                        }
                        if (StartVfxTimer > 1f)
                        {
                            
                            GameSequence.instance.Win();
                            if (!vfxConfety.isPlaying) vfxConfety.Play();
                            StartVfxTimer = 0f;
                        }
                    }

                }

            }

        }

    }

    private void CheckLoose()
    {

        if (MyTimer > LevelGameDesign.Stars[LevelGameDesign.Stars.Length - 1]) ChangeState(Mode.Lose);

    }

    private void ProcessLose()
    {



    }


#if UNITY_EDITOR
    protected void OnDrawGizmos()
    {

          if (!Application.isPlaying)         
          
     
        Gizmos.color =Color.green;
        if(Selection.activeGameObject !=null)
            if (Selection.activeGameObject.GetComponent<MeshFilter>()!=null)
        if (Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh != null)

            Gizmos.DrawWireMesh(Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh, Selection.activeGameObject.transform.position, Selection.activeGameObject.transform.rotation, Selection.activeGameObject.transform.localScale);


    }
 
    [ExecuteInEditMode]
    protected void OnGUI()
    {
       
     

    if (!Application.isPlaying)

    {
        Event e = new Event();
        while (Event.PopEvent(e))
        {

            if (e.rawType == EventType.MouseDown && e.button == 0)
            {

                Ray ray = Camera.main.ScreenPointToRay(new Vector2(e.mousePosition.x,
                    Screen.height - e.mousePosition.y));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {

                    Selection.activeGameObject = hit.collider.gameObject;
                }
            }

            
            if (e.rawType == EventType.MouseDown && e.button == 1)
            {

                Ray ray = Camera.main.ScreenPointToRay(new Vector2(e.mousePosition.x, Screen.height - e.mousePosition.y));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("HUITA");
                    Selection.activeGameObject = hit.collider.gameObject;
                }
            }
            
        }
    }
        
    }


#endif
}
 