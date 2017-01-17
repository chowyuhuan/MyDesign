using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public abstract class AM_AssetCounter<T> : AM_RefCounter where T : UnityEngine.Object
    {
        public string _Name { get; protected set; }
        public T _Asset { get; protected set; }

        public AM_AssetCounter(string assetName, T asset)
        {
            _Name = assetName;
            _Asset = asset;
        }

        public abstract void Unload();
    }
}
