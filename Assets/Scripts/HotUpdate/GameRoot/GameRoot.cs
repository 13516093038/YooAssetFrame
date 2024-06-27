using System;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class GameRoot : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
            Debug.Log("11111");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}