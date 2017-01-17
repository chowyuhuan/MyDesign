using UnityEngine;
using System.Collections;

namespace AssetUpdate
{
    public enum E_WorkFlow_State
    {
        Init,
        Start,
        Update,
        Finished
    }

    public abstract class AU_WorkFlow
    {
        public string _WorkName { get; protected set;}
        public E_WorkFlow_State _State {get; protected set;}

        public AU_WorkFlow()
        {
            _State = E_WorkFlow_State.Init;
        }

        public void Start()
        {
            _State = E_WorkFlow_State.Start;
            StartWorkFlow();
        }
        public bool Update()
        {
#if UNITY_EDITOR
            if(_State == E_WorkFlow_State.Init)
            {
                Start();
                Debug.LogError("Should not happen, please check class AU_WorkFlow and AU_WorkPipeLine !");
            }
#endif
            if(_State == E_WorkFlow_State.Start)
            {
                _State = E_WorkFlow_State.Update;
            }
            if(UpdateWorkFlow())
            {
                return true;
            }
            else
            {
                EndWorkFlow();
                _State = E_WorkFlow_State.Finished;
                return false;
            }
        }

        protected abstract void StartWorkFlow();
        protected abstract bool UpdateWorkFlow();
        protected abstract void EndWorkFlow();
    }
}
