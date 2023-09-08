using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGen : MonoBehaviour
{
    public Button m_YourFirstButton;
    public Transform[] children;

    // Start is called before the first frame update
    void Start()
    {
        m_YourFirstButton.onClick.AddListener(TaskOnClick);

        children = GetComponentsInChildren<Transform>();
    }

    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */

    void TaskOnClick()
    {
        for (int i = 0; i < 15; i++)
        {
            Debug.Log(children);
        }
    }
}
