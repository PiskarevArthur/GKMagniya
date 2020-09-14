using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    private int _currentNodeId;
    private bool isControl;
   private int СurrentNodeId
    {
        get { return _currentNodeId; }
         
        set
        {
            if (value == GameManager.linkNodes.Count)
            {
                _currentNodeId = 0;
                isControl = false;
            }
            else _currentNodeId = value;
        }
    }
    [SerializeField] RectTransform RectStick;

    private Vector3 MousePos
    {
        get
        {
            return new Vector3(Input.mousePosition.x, Input.mousePosition.y,0);
        }
    }
   private Vector3 startpos
    {
        get
        {
           return (new Vector3(Screen.width / 2, Screen.height * 0.2f,0));
        }
    }
    private void Awake()
    {
        GameManager.EventChangeState += OnChange;    
        RectStick.position = startpos;
        Debug.Log(startpos);
    }
    private void OnDestroy()
    {
        GameManager.EventChangeState -= OnChange;

    }
    void OnChange()
    {
            if(GameManager.CurrentState==GameManager.Mode.Play && GameManager.MyTimer < 0.1f)
        {
            GameManager.LockedObject = GameManager.linkNodes[UnityEngine.Random.Range(0, GameManager.linkNodes.Count)];
            _currentNodeId = 0;
        }
    }
    private void FixedUpdate()
    {
        if (GameManager.CurrentState == GameManager.Mode.Play)
        if (Input.GetMouseButton(0)|| Input.GetMouseButton(1))
        {
            Vector3 newpos = (MousePos- startpos);
            float Dist = Mathf.Clamp(newpos.magnitude, -Screen.width*0.1f, Screen.width*0.1f);

            RectStick.position = Vector2.Lerp(RectStick.position, startpos+ ( newpos.normalized*Dist), 10*Time.fixedDeltaTime);

             GameManager.LockedObject.MoveNode(2*newpos.normalized * Dist / Screen.width);
        }
        else  
        {
            RectStick.position = Vector2.Lerp(RectStick.position,startpos,10*Time.fixedDeltaTime);
        }


        if (GameManager.CurrentState == GameManager.Mode.Play)
        {
            ControlNodeInsideCamera();
            if (!isControl)
            {
                СurrentNodeId = 0;
                isControl = true;
            }
        }
    }

    private void ControlNodeInsideCamera()
    {
        if (GameManager.linkNodes != null)
        {
           
            Vector2 curpos = Camera.main.WorldToScreenPoint(GameManager.linkNodes[СurrentNodeId].transform.position);
            if (curpos.x < Screen.width * 0.05f) 
                GameManager.linkNodes[СurrentNodeId].RB.AddForce(-1.33f*GameManager.linkNodes[СurrentNodeId].transform.position.x,0,0, ForceMode.Impulse);
            if (curpos.x > Screen.width * 0.95f) 
                GameManager.linkNodes[СurrentNodeId].RB.AddForce(-1.33f * GameManager.linkNodes[СurrentNodeId].transform.position.x,0,0, ForceMode.Impulse);
            if (curpos.y > Screen.height * 0.95f)
                GameManager.linkNodes[СurrentNodeId].RB.AddForce(0,-1.33f * GameManager.linkNodes[СurrentNodeId].transform.position.y,0,ForceMode.Impulse);
            if (curpos.y < Screen.height * 0.25f) 
                GameManager.linkNodes[СurrentNodeId].RB.AddForce(0,-1.33f * GameManager.linkNodes[СurrentNodeId].transform.position.y,0, ForceMode.Impulse);

GameManager.linkNodes[СurrentNodeId].RB.AddForce(new Vector3(0,0,-1.33f * GameManager.linkNodes[СurrentNodeId].transform.position.z), ForceMode.VelocityChange);
         
            СurrentNodeId++;
        }
    }
}
