using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public class AM_RefCounter
    {
        protected int _RefCount = 0;

        public int IncreaseRef()
        {
            return ++_RefCount;
        }

        public int DecreaseRef()
        {
            return --_RefCount;
        }

        public bool Useless()
        {
            return 1 > _RefCount;
        }
    }
}
