using UnityEngine;
using System.Collections;

namespace AssetUpdate
{
    public class AU_TaskState
    {
        public int taskcount = 0;
        public int downloadcount = 0;
        public void Clear()
        {
            taskcount = 0;
            downloadcount = 0;
        }
        public override string ToString()
        {
            return downloadcount + "/" + taskcount;
        }
        public float Percent()
        {
            return (float)downloadcount / (float)taskcount;
        }
    }

}