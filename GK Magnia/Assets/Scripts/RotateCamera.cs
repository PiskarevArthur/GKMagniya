using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    private bool isRotate;

    private float timer;

    private float temp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isRotate = true;
        }
        if (isRotate)
        {
            timer += Time.deltaTime;

            if (timer < 1.25f)
            {
                temp = Mathf.Lerp(temp, 300, Time.deltaTime*3.2f);
                
                transform.RotateAround(GameManager.linkNodes[0].transform.position, Vector3.down,
                    temp *Time.deltaTime);
                
            }
            if (timer > 1.25f&&timer<2.5f)
            {
                temp = Mathf.Lerp(temp, 0, Time.deltaTime * 3.2f);
                Debug.Log(temp);
                transform.RotateAround(GameManager.linkNodes[0].transform.position, Vector3.down,
                    temp * Time.deltaTime);

            }

            if (timer > 2.5f)
            {
                isRotate = false;
                timer = 0f;
            }


        }
    }
}
