using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public delegate void AM_LoadCallBack(AM_LoadOperation loadOperation);
    public abstract class AM_LoadOperation : IEnumerator
    {
        protected AM_LoadCallBack _LoadCallBack;
        public bool _Done{ get; protected set; }
        public string _LoadError { get; protected set; }

        public object Current
        {
            get
            {
                return null;
            }
        }

        public bool MoveNext()
        {
            return !IsDone();
        }

        public void Reset()
        {
        }

        abstract public bool Update();

        abstract public bool IsDone();

        public virtual void LoadDone()
        {
            if (_LoadCallBack != null)
            {
                _LoadCallBack(this);
                PostProcess();
            }
        }

        public virtual void PostProcess()
        {

        }

        public AM_LoadOperation(AM_LoadCallBack loadCallBack)
        {
            _LoadCallBack = loadCallBack;
        }
    }
}
