using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HotUpdate
{
    public class WindowBase : WindowBehaviour
    {
        private List<Button> m_AllButtonList = new List<Button>();
        private List<Toggle> m_AllToggleList = new List<Toggle>();
        private List<InputField> m_AllInputFieldList = new List<InputField>();
        
        #region 生命周期

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnShow()
        {
            base.OnShow();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RemoveAllButtonListener();
            RemoveAllToggleListener();
            RemoveAllInputListener();
        }

        public override void SetVisible(bool isVisible)
        {
            base.SetVisible(isVisible);
        }

        #endregion

        #region 事件管理

        public void AddButtonClickListener(Button btn, UnityAction action)
        {
            if (btn)
            {
                if (!m_AllButtonList.Contains(btn))
                {
                    m_AllButtonList.Add(btn);
                }

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(action);
            }
        }

        public void AddToggleClickListener(Toggle toggle, UnityAction<bool, Toggle> action)
        {
            if (toggle)
            {
                if (!m_AllToggleList.Contains(toggle))
                {
                    m_AllToggleList.Add(toggle);
                }

                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    action?.Invoke(isOn,toggle);
                });
            }
        }

        public void AddInputFieldListener(InputField input, UnityAction<string> onChangeAction,
            UnityAction<string> endAction)
        {
            if (input)
            {
                if (!m_AllInputFieldList.Contains(input))
                {
                    m_AllInputFieldList.Add(input);
                }
                input.onValueChanged.RemoveAllListeners();
                input.onEndEdit.RemoveAllListeners();
                input.onValueChanged.AddListener(onChangeAction);
                input.onEndEdit.AddListener(endAction);
            }
        }

        public void RemoveAllButtonListener()
        {
            foreach (var item in m_AllButtonList)
            {
                item.onClick.RemoveAllListeners();
            }
            m_AllButtonList.Clear();
        }
        
        public void RemoveAllToggleListener()
        {
            foreach (var item in m_AllToggleList)
            {
                item.onValueChanged.RemoveAllListeners();
            }
            m_AllToggleList.Clear();
        }
        
        public void RemoveAllInputListener()
        {
            foreach (var item in m_AllInputFieldList)
            {
                item.onValueChanged.RemoveAllListeners();
                item.onEndEdit.RemoveAllListeners();
            }
            m_AllInputFieldList.Clear();
        }

        #endregion
    }
}