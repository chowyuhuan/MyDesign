using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetUpdate
{
    public class AU_WWWFileLoader
    {
        //一个下载任务
        class DownTask
        {
            public DownTask(string path, string tag, Action<WWW, string> onload)
            {
                this.path = path;
                this.tag = tag;
                this.onload = onload;
            }
            public string path;
            public string tag;
            public Action<WWW, string> onload;
        }

        //负责下载
        class DownTaskRunner
        {
            public DownTaskRunner(DownTask task)
            {
                this.task = task;
                www = new WWW(this.task.path);
            }
            public WWW www;
            public DownTask task;
        }

        private int concurrentNum = 2; /**< 并行下载数，如果瞬间内存过大，被kill，缩小此值 */

        public AU_TaskState taskState
        {
            get;
            private set;
        }

        bool _downloadError = false;
        public bool DownLoadError
        {
            get { return _downloadError; }
        }

        Queue<DownTask> task = new Queue<DownTask>();
        List<DownTaskRunner> runnner = new List<DownTaskRunner>();
        List<DownTaskRunner> finished = new List<DownTaskRunner>();

        /**
         *	\brief 所有加载是否已经完成
         */
        public bool IsFinished()
        {
            return (_downloadError || (task.Count == 0 && runnner.Count == 0));
        }
        public void Update()
        {
            while (runnner.Count < concurrentNum && task.Count > 0)
            {
                runnner.Add(new DownTaskRunner(task.Dequeue()));
            }
            
            for (int i = 0; i < runnner.Count; ++i)
            {
                if (runnner[i].www.isDone)
                {
                    taskState.downloadcount++;
                    finished.Add(runnner[i]);
                    if(!string.IsNullOrEmpty(runnner[i].www.error))
                    {
                        _downloadError = true;
#if UNITY_EDITOR
                        Debug.LogError("[更新][下载文件错误]" + runnner[i].www.error+ "---->" + runnner[i].www.url);
#endif
                    }
                    runnner[i].task.onload(runnner[i].www, runnner[i].task.tag);
                }
            }
            for (int i = 0; i < finished.Count; ++i)
            {
                runnner.Remove(finished[i]);
            }

        }
        public void Load(string path, string tag, Action<WWW, string> onLoad)
        {
            task.Enqueue(new DownTask(path, tag, onLoad));
            taskState.taskcount++;
        }
        public AU_WWWFileLoader()
        {
            taskState = new AU_TaskState();
        }
    }
}
