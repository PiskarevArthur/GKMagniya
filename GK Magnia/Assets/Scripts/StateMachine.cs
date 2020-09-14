using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif

public class StateMachine : MonoBehaviour
{

    [SerializeField] private GameManager.Mode ThisState;
    [SerializeField] private GameManager.Mode ThisStateSecond = GameManager.Mode.None;
    [SerializeField] private GameManager.Mode ThisStateThird = GameManager.Mode.None;


    [SerializeField] private float InSpeed=1;
    [SerializeField] private float OutSpeed=4;
    bool Fade;

    [SerializeField] CanvasGroup CG;
    void Reset()
    {
        CG = GetComponent<CanvasGroup>();
    }

    void Awake()
    {
        Fade = true;
         
        GameManager.EventChangeState += Onchange;
        Onchange();
        if (GameManager.CurrentState == ThisState || GameManager.CurrentState == ThisStateSecond || GameManager.CurrentState == ThisStateThird) CG.alpha = 1;
        else CG.alpha = 0;

      
    }

    private void Update()
    {

         
      //  if(GameManager.GuiTimer<1.1f)
        if (Fade == false)
        {
            if (CG.alpha < 0.01f)
            {
                CG.alpha = 0;
                CG.interactable = Fade;
                CG.blocksRaycasts = Fade;
            }
            else
            CG.alpha = Mathf.Lerp(1, 0, GameManager.GuiTimer * GameManager.GuiTimer*OutSpeed);

        }
        else
        {
            

            if (CG.alpha > 0.99f)
            {
                CG.interactable = Fade;
                CG.blocksRaycasts = Fade;
                CG.alpha = 1;
            }

            else
                CG.alpha = Mathf.Lerp(0, 1, GameManager.GuiTimer* GameManager.GuiTimer * InSpeed );

        }

    }

    private void OnDestroy()
    {

        GameManager.EventChangeState -= Onchange;

    }

    void Onchange()
    {            
            
        if (GameManager.CurrentState == ThisState || GameManager.CurrentState == ThisStateSecond || GameManager.CurrentState == ThisStateThird) Fade = true;            
        else Fade = false;           

        CG.interactable = Fade;
        CG.blocksRaycasts = Fade;
        
    }    

#if UNITY_EDITOR

    static StateMachine()
    {

        Selection.selectionChanged += SelectionChange;

    }

    static void SelectionChange()
    {

        if (Selection.activeGameObject && !Selection.activeGameObject.GetComponent<StateMachine>()) return;

        StateMachine[] stateMachines = FindObjectsOfType<StateMachine>();

        for (int i = 0; i < stateMachines.Length; i++)
        {

            if (stateMachines[i].OnSelectionChange()) break;

        }

        for (int i = 0; i < stateMachines.Length; i++)
        {

            stateMachines[i].OnSelectionChange();

        }

    }

    public bool OnSelectionChange()
    {

        bool result = false;

        if (!Application.isPlaying)
        {

            if (Selection.activeGameObject == gameObject)
            {

                GameManager.CurrentState = ThisState;
                result = true;

            }

            if (GameManager.CurrentState == ThisState)
            {

                CG.alpha = 1;

            }
            else
            {

                CG.alpha = 0;

            }

        }

        return result;

    }

#endif

}
