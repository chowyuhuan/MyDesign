using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public interface AM_IABLoader
    {
        AM_LoadOperation LoadABAsync(string abName, string abPath);
        AssetBundle LoadABSync(string abName, string abPath);
    }
}
