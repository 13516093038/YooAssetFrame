using System.Collections.Generic;
using DG.Tweening;
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

        private CanvasGroup mUIMask;
        private CanvasGroup mCanvasGroup;
        private Transform mUIContent;
        protected bool mDisableAnim = false;

        /// <summary>
        /// 初始化基类组件
        /// </summary>
        private void InitializeBaseComponent()
        {
            mUIMask = transform.Find("UIMask").GetComponent<CanvasGroup>();
            mCanvasGroup = transform.GetComponent<CanvasGroup>();
            mUIContent = transform.Find("UIContent").transform;
        }
        
        #region 生命周期

        public override void OnAwake()
        {
            base.OnAwake();
            InitializeBaseComponent();
        }

        public override void OnShow()
        {
            base.OnShow();
            ShowAnimation();
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

        #endregion

        #region 动画管理

        protected virtual void ShowAnimation()
        {
            if (Canvas.sortingOrder > 99 && mDisableAnim == false)
            {
                //缩放动画
                mUIContent.localScale = Vector3.one * 0.8f;
                mUIContent.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            }
        }

        protected virtual void HideAnimation()
        {
            if (mDisableAnim == false)
            {
                mUIContent.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    UIModule.Ins.HideWindow(Name);
                });
            }
            else
            {
                UIModule.Ins.HideWindow(Name);
            }
        }

        #endregion

        public virtual void HideWindow()
        {
            //UIModule.Ins.HideWindow(Name);
            HideAnimation();
        }
        
        public override void SetVisible(bool isVisible)
        {
            base.SetVisible(isVisible);
            mCanvasGroup.alpha = isVisible ? 1 : 0;
            mCanvasGroup.blocksRaycasts = isVisible;
            Visible = isVisible;
        }

        public void SetMaskVisible(bool isVisible)
        {
            if (!UIModule.Ins.uiSetting.SINGMASK_SYSTEM)
            {
                return;
            }
            else
            {
                mUIMask.alpha = isVisible ? 1 : 0;
            }
        }

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