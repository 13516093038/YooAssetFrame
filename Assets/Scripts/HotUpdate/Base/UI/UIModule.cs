using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EggCard;
using UnityEngine;

namespace HotUpdate
{
    public class UIModule : MonoSingleton<UIModule>
    {
        private Camera mUICamera;
        public UISetting uiSetting;

        //所有窗口的Dic
        private Dictionary<string, WindowBase> mAllWindowDic = new Dictionary<string, WindowBase>();
        //所有窗口的列表
        private List<WindowBase> mAllWindowList = new List<WindowBase>();
        //所有可见窗口的列表
        private List<WindowBase> mVisibleWindowList = new List<WindowBase>();

        private async void Start()
        {
            mUICamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            uiSetting = await Resource.Ins.LoadAsset<UISetting>(Utility.GetConfigPath("UISetting"));
        }

        /// <summary>
        /// 弹出窗口
        /// </summary>
        /// <typeparam name="T">弹窗类型</typeparam>
        /// <returns></returns>
        public async Task<T> PopUpWindow<T>() where T : WindowBase,new()
        {
            Type type = typeof(T);
            string wndName = type.Name;
            WindowBase wnd = GetWindow(wndName);
            if (wnd != null)
            {
                return ShowWindow(wndName) as T;
            }
            else
            {
                T t = new T();
                WindowBase window = await InitializeWindow(t, wndName);
                return window as T;
            }
        }

        private async Task<WindowBase> InitializeWindow(WindowBase windowBase, string wndName)
        {
            //1.生成对应的窗口预支体
            GameObject nWindow = await LoadWindow(wndName);
            //2.初始化窗口类
            if (nWindow != null)
            {
                windowBase.gameObject = nWindow;
                windowBase.Name = wndName;
                windowBase.transform = nWindow.transform;
                windowBase.Canvas = nWindow.GetComponent<Canvas>();
                windowBase.Canvas.worldCamera = mUICamera;
                windowBase.transform.SetAsLastSibling();
                windowBase.OnAwake();
                windowBase.SetVisible(true);
                windowBase.OnShow();
                RectTransform rectTrans = nWindow.GetComponent<RectTransform>();
                rectTrans.anchorMax = Vector2.one;
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.offsetMin = Vector2.zero;
                mAllWindowDic.Add(wndName,windowBase);
                mAllWindowList.Add(windowBase);
                mVisibleWindowList.Add(windowBase);
                SetWindowMaskVisible();
                return windowBase;
            }
            else
            {
                Debug.LogError("没有加载到窗口：" + wndName);
                return null;
            }
        }

        private WindowBase ShowWindow(string winName)
        {
            WindowBase window = null;
            if (mAllWindowDic.ContainsKey(winName))
            {
                window = mAllWindowDic[winName];
                if (window.gameObject && window.Visible == false)
                {
                    mVisibleWindowList.Add(window);
                    window.transform.SetAsLastSibling();
                    window.SetVisible(true);
                    window.OnShow();
                }
                SetWindowMaskVisible();
                return window;
            }
            else
            {
                Debug.LogError(winName + "窗口不存在，请调用PopUoWindow进行弹出");
                return null;
            }
        }

        private WindowBase GetWindow(string wndName)
        {
            if (mAllWindowDic.ContainsKey(wndName))
            {
                return mAllWindowDic[wndName];
            }

            return null;
        }

        /// <summary>
        /// 获取已经弹出的弹窗类
        /// </summary>
        /// <typeparam name="T">弹窗类型</typeparam>
        /// <returns></returns>
        public T GetWindow<T>() where T : WindowBase
        {
            Type type = typeof(T);
            foreach (var item in mVisibleWindowList)
            {
                if (item.Name == type.Name)
                {
                    return item as T;
                }
            }
            Debug.LogError("该窗口没有获取到：" + type.Name);
            return null;
        }
        
        /// <summary>
        /// 隐藏弹窗
        /// </summary>
        /// <typeparam name="T">弹窗类型</typeparam>
        public void HideWindow<T>() where T : WindowBase
        {
            WindowBase window = GetWindow<T>();
            HideWindow(window);
        }
        
        private void HideWindow(string wndName)
        {
            WindowBase window = GetWindow(wndName);
            HideWindow(window);
        }

        private void HideWindow(WindowBase window)
        {
            if (window != null && window.Visible)
            {
                mVisibleWindowList.Remove(window);
                window.SetVisible(false);
                SetWindowMaskVisible();
                window.OnHide();
            }
        }

        /// <summary>
        /// 销毁弹窗
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DestroyWindow<T>() where T : WindowBase
        {
            DestroyWindow(GetWindow<T>());
        }

        private void DestroyWindow(string winName)
        {
            DestroyWindow(GetWindow(winName));
        }

        private void DestroyWindow(WindowBase window)
        {
            if (window != null)
            {
                if (mAllWindowDic.ContainsKey(window.Name))
                {
                    mAllWindowDic.Remove(window.Name);
                    mAllWindowList.Remove(window);
                    mVisibleWindowList.Remove(window);
                }
                window.SetVisible(false);
                SetWindowMaskVisible();
                window.OnHide();
                window.OnDestroy();
                Destroy(window.gameObject);
            }
        }

        public void DestroyAllWindow(List<string> filterList = null)
        {
            for (int i = mAllWindowList.Count - 1; i >= 0; i--)
            {
                WindowBase window = mAllWindowList[i];
                if (filterList != null && filterList.Contains(window.Name))
                {
                    continue;
                }
                DestroyWindow(window);
            }
            Resources.UnloadUnusedAssets();
        }

        private void SetWindowMaskVisible()
        {
            if (!uiSetting.SINGMASK_SYSTEM)
            {
                return;
            }
            else
            {
                WindowBase maxOrderWindBase = null;
                //最大渲染层级
                int maxOrder = 0;
                //最大排序下标（在相同父节点下的位置下标）
                int maxIndex = 0;
                //1.关闭所有窗口的Mask,设置为不可见
                //2.从所有可见窗口中找到一个层级最大的窗口，把Mask设置为可见
                for (int i = 0; i < mVisibleWindowList.Count; i++)
                {
                    WindowBase window = mVisibleWindowList[i];
                    if (window == null && window.gameObject)
                    {
                        window.SetMaskVisible(false);
                        if (maxOrderWindBase == null)
                        {
                            maxOrderWindBase = window;
                            maxOrder = window.Canvas.renderOrder;
                            maxIndex = window.transform.GetSiblingIndex();
                        }
                        else
                        {
                            //找到最大渲染层级的窗口，找到它
                            if (maxOrder < window.Canvas.renderOrder)
                            {
                                maxOrderWindBase = window;
                                maxOrder = window.Canvas.renderOrder;
                            }
                            //如果两个窗口的渲染层级相同，就找到同节点下的最靠下的一个物体，优先渲染Mask
                            else if(maxOrder == window.Canvas.renderOrder && maxIndex < window.transform.GetSiblingIndex())
                            {
                                maxOrderWindBase = window;
                                maxIndex = window.transform.GetSiblingIndex();
                            }
                        }
                    }
                }

                if (maxOrderWindBase != null)
                {
                    maxOrderWindBase.SetMaskVisible(true);
                }
            }
        }
        
        private async Task<GameObject> LoadWindow(string windname)
        {
            GameObject window = await Resource.Ins.LoadAsset(Utility.GetWindowPath(windname));
            window.transform.localScale = Vector3.one;
            window.transform.localPosition = Vector3.zero;
            window.transform.rotation = Quaternion.identity;
            return Instantiate(window, transform);
        }
    }
}