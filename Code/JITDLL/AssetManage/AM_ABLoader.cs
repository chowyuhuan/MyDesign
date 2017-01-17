using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace AssetManage
{
    public class AM_ABLoader : AM_IABLoader
    {
        public  AM_LoadOperation LoadABAsync(string abName, string abPath)
        {
            WWW loader = new WWW(abPath);
            AM_LoadOperation loadop = new AM_LoadABWithWWW(loader, abName);
            AM_LoadOperationManager.AddLoadABOperation(abName, loadop);
            return loadop;
        }

        public AssetBundle LoadABSync(string abName, string abPath)
        {
            abPath = Application.persistentDataPath + "/JITData/res/Android/" + abPath;
            byte[] abdata = AM_FileReader.ReadFileToBytes(abPath);
            AssetBundle ab = AssetBundle.LoadFromMemory(abdata);
            return ab;
        }
    }
}
