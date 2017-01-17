using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public class AM_FileReader
    {
        public static byte[] ReadFileToBytes(string filename)
        {
            try
            {
                using (System.IO.Stream s = System.IO.File.OpenRead(filename))
                {
                    byte[] b = new byte[s.Length];
                    s.Read(b, 0, (int)s.Length);
                    return b;
                }
            }
            catch (System.Exception ex)
            {
#if UNITY_EDITOR
                Debug.Log("[文件]异常： FileHelper ReadFileToBytes: " + ex.ToString());
#endif
                return null;
            }
        }

        public static AssetBundle ReadABFromFile(string abPath)
        {
            byte[] abdata = ReadFileToBytes(abPath);
            if(abdata == null || abdata.Length == 0)
            {
                return null;
            }
            
            return AssetBundle.LoadFromMemory(abdata);
        }
    }
}
