using System;
using EggCard;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class GameRoot : MonoSingleton<GameRoot>
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
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