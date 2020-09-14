using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class StateButton : MonoBehaviour
{
    [SerializeField] Button Btn;
    [SerializeField] GameManager.Mode setting;
   
     
    private void Reset()
    {
        
        Btn = GetComponent<Button>();
        if (Btn == null) Btn=gameObject.AddComponent<Button>();

    }
    void Awake()
    {
        
        if(Btn==null) Btn = gameObject.GetComponent<Button>();
        Btn.onClick.AddListener(TaskOnClick);

    }

    void TaskOnClick()
    { 
                 
        GameManager.ChangeState(setting);
         
    }

}
