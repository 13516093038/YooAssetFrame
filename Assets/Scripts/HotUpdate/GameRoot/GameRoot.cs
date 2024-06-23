using System;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class GameRoot : MonoBehaviour
    {
        public Image image;
        private void Awake()
        {
            Debug.Log("11111");
            Debug.Log(image.name);
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