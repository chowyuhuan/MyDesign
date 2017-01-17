using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetUpdate
{
    public class AU_StreamFileLoader {
    public AU_TaskState taskState
    {
        get;
        private set;
    }
    public enum FrameState
    {
        Nothing,
        Slow,
        Finish,
    }
    abstract class DelayTask
    {
        public string path;
        public string tag;
        public abstract FrameState Update();
    }
    class DelayTaskAssetBundle : DelayTask
    {
        public DelayTaskAssetBundle(string path, string tag, Action<AssetBundle, string> onLoad)
        {
            this.onload = onLoad;
            this.tag = tag;
            this.path = path;
        }
        AssetBundleCreateRequest request;
        public Action<AssetBundle, string> onload;
        public override FrameState Update()
        {
            if (request == null)
            {
                byte[] bs = AU_FileHelper.ReadFileToBytes(path);
                request = AssetBundle.LoadFromMemoryAsync(bs);
                return FrameState.Slow;
            }
            if (request.isDone)
            {
                onload(request.assetBundle, tag);
                return FrameState.Finish;
            }
            else
            {
                return FrameState.Nothing;
            }
        }
    }

    List<DelayTask> delaytask = new List<DelayTask>();
    public bool IsFinished()
    {
        return delaytask.Count == 0;
    }
    public void Update()
    {
        //处理帧检测任务
        for (int i = 0; i < delaytask.Count; ++i)
        {
            var state = delaytask[i].Update(); 
            if (state == FrameState.Slow)
                break;
            if (state == FrameState.Finish)
            {
                delaytask.Remove(delaytask[i]);
                taskState.downloadcount++;
                break;
            }
        }
    }
    public void LoadAssetBundle(string path, string tag, Action<AssetBundle, string> onLoad)
    {
        delaytask.Add(new DelayTaskAssetBundle(path, tag, onLoad));
        taskState.taskcount++;
    }
    public AssetBundle LoadAssetBundleImmediately(string path)
    {
        byte[] bs = AU_FileHelper.ReadFileToBytes(path);
        AssetBundle bundle = AssetBundle.LoadFromMemory(bs);
        bs = null;
        return bundle;
    }
    public byte[] LoadBytesImmediately(string path)
    {
        return AU_FileHelper.ReadFileToBytes(path);
    }
    public Texture2D LoadTexture2DImmediately(string path)
    {
        byte[] bs = AU_FileHelper.ReadFileToBytes(path);
        Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        tex.LoadImage(bs);
        return tex;
    }
    public string LoadStringImmediately(string path)
    {
        byte[] bs = AU_FileHelper.ReadFileToBytes(path);
        string t = System.Text.Encoding.UTF8.GetString(bs, 0, bs.Length);
        if ((UInt16)t[0] == 0xFEFF)
        {
            t = t.Substring(1);
        }
        return t;
    }

    public AU_StreamFileLoader()
    {
        taskState = new AU_TaskState();
    }
}
}
