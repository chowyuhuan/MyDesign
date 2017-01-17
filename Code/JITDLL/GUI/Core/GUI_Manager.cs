using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUI_Manager : Singleton<GUI_Manager>
{
    #region window info
    public class GUI_WindowInfo
    {
        public string WindowName { get; protected set; }
        public string ClassName { get; protected set; }
        public string PrefabName { get; protected set; }

        public GUI_WindowInfo(string wndName, string className, string prefabName)
        {
            WindowName = wndName;
            ClassName = className;
            PrefabName = prefabName;
        }
    }

    protected Dictionary<string, GUI_WindowInfo> _WindowInfoDic = new Dictionary<string, GUI_WindowInfo>();

    protected void LoadWindowInfo()
    {
        _WindowInfoDic.Clear();
        for (int index = 0; index < CSV_c_gui_windows.DateCount; ++index)
        {
            CSV_c_gui_windows gw = CSV_c_gui_windows.GetData(index);
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(gw.WindowName)
                || string.IsNullOrEmpty(gw.ClassName)
                || string.IsNullOrEmpty(gw.PrefabName))
            {
                Debug.LogError("window config csv has empty value !!!!");
                return;
            }
            if (!_WindowInfoDic.ContainsKey(gw.WindowName))
            {

#endif
                GUI_WindowInfo wi = new GUI_WindowInfo(gw.WindowName, gw.ClassName, gw.PrefabName);
                _WindowInfoDic.Add(gw.WindowName, wi);
#if UNITY_EDITOR

            }
            else
            {
                Debug.LogError("Repeat window config :" + gw.WindowName);
            }
#endif
        }
        CSV_c_gui_windows.Recycle();
    }
    #endregion

    #region window to show after scene load finished
    protected Queue<string> _WaitingQueue = new Queue<string>();

    public void PushWndToWaitingQueue(string windowName)
    {
        if (!string.IsNullOrEmpty(windowName))
        {
            _WaitingQueue.Enqueue(windowName);
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogError("Try add empty windowname to waiting queue !");
        }
#endif
    }

    public void ShowWaitingQueue()
    {
        while (_WaitingQueue.Count > 0)
        {
            ShowWindowWithName(_WaitingQueue.Dequeue(), false);
        }
    }
    #endregion

    #region window display control
    protected class DisplayWindow
    {
        List<string> _DisplayWindowNameList = new List<string>();
        List<GUI_Window_DL> _DisplayWindowList = new List<GUI_Window_DL>();
        Dictionary<string, GUI_IOnTopHandler> _OnTopHandlerFastList = new Dictionary<string, GUI_IOnTopHandler>();

        public GUI_Window_DL _CurrentDisplayWindow
        {
            get
            {
                return _DisplayWindowList.Count > 0 ? _DisplayWindowList[_DisplayWindowList.Count - 1] : null;
            }
        }

        public List<string> GetAllDisplayWindow()
        {
            return _DisplayWindowNameList;
        }

        public void Refresh()
        {
            _DisplayWindowList.Clear();
            _DisplayWindowNameList.Clear();
        }

        public bool HasWindow(string windowName)
        {
            return _DisplayWindowNameList.Contains(windowName);
        }

        public bool HasWindow(GUI_Window_DL window)
        {
            return _DisplayWindowList.Contains(window);
        }

        public bool TryGetWindow(string windowName, out GUI_Window_DL window)
        {
            if (_DisplayWindowNameList.Contains(windowName))
            {
                window = _DisplayWindowList[_DisplayWindowNameList.IndexOf(windowName)];
                return true;
            }
            else
            {
                window = null;
                return false;
            }
        }

        public void AddWindow(string windowName, GUI_Window_DL window)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(windowName) || null == window)
            {
                Debug.LogError("Error: emtpy windowname or null window value!");
                return;
            }
#endif
            _DisplayWindowList.Add(window);
            _DisplayWindowNameList.Add(windowName);

            GUI_IOnTopHandler onTopHandler = window as GUI_IOnTopHandler;
            if (null != onTopHandler)
            {
                _OnTopHandlerFastList.Add(windowName, onTopHandler);
            }
        }

        public void RemoveWindow(string windowName)
        {
            if (_DisplayWindowNameList.Contains(windowName))
            {
                int index = _DisplayWindowNameList.IndexOf(windowName);
                _DisplayWindowNameList.Remove(windowName);
                _DisplayWindowList.RemoveAt(index);

                GUI_IOnTopHandler onTopHandler;
                if (_OnTopHandlerFastList.TryGetValue(windowName, out onTopHandler))
                {
                    _OnTopHandlerFastList.Remove(windowName);
                }

                if (_DisplayWindowNameList.Count > 0)
                {
                    string topWindowName = _DisplayWindowNameList[_DisplayWindowNameList.Count - 1];
                    if (_OnTopHandlerFastList.TryGetValue(topWindowName, out onTopHandler))
                    {
                        onTopHandler.OnTop();
                    }
                }
            }
        }

        public bool OnTop(string windowName)
        {
            if (_DisplayWindowNameList.Count > 0)
            {
                return _DisplayWindowNameList.IndexOf(windowName) == (_DisplayWindowNameList.Count - 1);
            }
            return false;
        }
    }
    protected DisplayWindow _DisplayWindowsDic = new DisplayWindow();
    public GUI_Window_DL _CurShowWindow
    {
        get { return _DisplayWindowsDic._CurrentDisplayWindow; }
    }

    public bool OnTop(string windowName)
    {
        return _DisplayWindowsDic.OnTop(windowName);
    }


    public GUI_Window_DL ShowWindowWithName(string windowName, bool hideCurrent)
    {
        GUI_Window_DL window = null;
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(windowName))
        {
            Debug.LogError("The window to be showed must have one none empty name !!");
            return window;
        }
#endif
        window = FindWindowWithName(windowName, true);
        if (null != window)
        {
            if (hideCurrent && null != _CurShowWindow)
            {
                HideWindowWithName(_CurShowWindow.WindowName);
            }
            window.ShowWindow();
        }
        return window;
    }

    public T ShowWindowWithName<T>(string windowName, bool hideCurrent) where T : GUI_Window_DL
    {
        return ShowWindowWithName(windowName, hideCurrent) as T;
    }

    public GUI_Window_DL FindWindowWithName(string windowName, bool createIfNotFound)
    {
        GUI_Window_DL window = null;
        if (!_DisplayWindowsDic.TryGetWindow(windowName, out window))
        {
            if (createIfNotFound)
            {
                window = LoadWindowByName(windowName);
            }
        }
        return window;
    }

    public T FindWindowWithName<T>(string windowName, bool createIfNotFound) where T : GUI_Window_DL
    {
        return FindWindowWithName(windowName, createIfNotFound) as T;
    }

    public void HideWindowWithName(string windowName)
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(windowName))
        {
            Debug.LogError("The window to be hide must have one none empty name !!");
            return;
        }
#endif
        GUI_Window_DL window = FindWindowWithName(windowName, false);
        if (null != window)
        {
            window.HideWindow();
        }
    }

    public void HideAllWindow()
    {
        List<string> allWindows = _DisplayWindowsDic.GetAllDisplayWindow();
        for (int index = 0; index < allWindows.Count; )
        {
            HideWindowWithName(allWindows[index]);
        }
    }

    public bool RegistWindow(string windowName, GUI_Window_DL window)
    {
        if (!string.IsNullOrEmpty(windowName))
        {
            if (null != window && !_DisplayWindowsDic.HasWindow(windowName))
            {
                _DisplayWindowsDic.AddWindow(windowName, window);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("Happens when regist window : " + windowName);
            }
#endif
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogError("Try regist empty windowname to manager !");
        }
#endif
        return false;
    }

    public bool UnRegistWindow(string windowName)
    {
        if (!string.IsNullOrEmpty(windowName))
        {
            if (_DisplayWindowsDic.HasWindow(windowName))
            {
                _DisplayWindowsDic.RemoveWindow(windowName);
                return true;
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("The window you are trying to unregist not exists !");
            }
#endif
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogError("Try unregist empty windowname from manager !");
        }
#endif
        return false;
    }
    #endregion

    #region window resource control
    public void ReleaseWindowRes(GUI_Window_DL window)
    {
        if (null != window)
        {
            GameObject.Destroy(window.WindowObject);
            AssetManage.AM_Manager.UnloadAsset(FixWindowPath(window.WindowName));
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogError("Try release null window from manager !");
        }
#endif
    }

    string FixWindowPath(string windowName)
    {
        return "GUI/UIPrefab/" + windowName;
    }

    GUI_Window_DL LoadWindowByName(string windowName)
    {
        return LoadWindowByName(windowName, GUI_Root_DL.Instance.GUIRootObject);
    }

    GUI_Window_DL LoadWindowByName(string windowName, GameObject parent)
    {
        GUI_Window_DL window = null;
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(windowName)
            || !_WindowInfoDic.ContainsKey(windowName))
        {
            Debug.LogError("Error: A window named " + windowName + " not found !!");
        }
        else
        {
#endif
            GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>(FixWindowPath(_WindowInfoDic[windowName].PrefabName), true, AssetManage.E_AssetType.UIPrefab);
            if (null != go)
            {
                go = GameObject.Instantiate(go) as GameObject;
                window = go.GetComponent(_WindowInfoDic[windowName].ClassName) as GUI_Window_DL;
                if (null == window)
                {
                    string winClassName = _WindowInfoDic[windowName].ClassName.Trim();
                    System.Type T = System.Type.GetType(winClassName);
                    if (T == null)
                    {
#if UNITY_EDITOR
                        Debug.LogError("The class of window named " + windowName + " not found!");
#endif
                    }
                    window = go.AddComponent(T) as GUI_Window_DL;
                }
                if (null != window)
                {
                    window.SetWindowName(windowName);
                }
                if (null != parent)
                {
                    GUI_Tools.CommonTool.AddUIChild(parent, go, false);
                }
            }
#if UNITY_EDITOR
        }
#endif
        return window;
    }
    #endregion

    public void Refresh()
    {
        _DisplayWindowsDic.Refresh();
        _WindowInfoDic.Clear();
        _WaitingQueue.Clear();
        LoadWindowInfo();
    }

    public void Init()
    {
        Refresh();
    }
}