using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssetUpdate
{
    public class AU_WorkPipeLine
    {
        protected static List<AU_WorkFlow> _AUWorkFlow = new List<AU_WorkFlow>();

        public static void AddWorkFlow(AU_WorkFlow auwf)
        {
            if(null != auwf)
            {
                if(!_AUWorkFlow.Contains(auwf))
                {
                    _AUWorkFlow.Add(auwf);
                    auwf.Start();
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogError("Try to add repeat work flow : " + auwf._WorkName);
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("Try to add empty work flow !");
            }
#endif
        }

        public static void Update()
        {
            for(int index = 0; index < _AUWorkFlow.Count;)
            {
                if(_AUWorkFlow[index].Update())
                {
                    ++index;
                }
                else
                {
                    _AUWorkFlow.RemoveAt(index);
                }
            }
        }
    }
}
