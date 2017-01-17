using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AssetUpdate
{
    public class AU_BranchesVer
    {
        public AU_VersionInfo Ver = new AU_VersionInfo();
        public Dictionary<string, AU_BranchVer> Branches = new Dictionary<string, AU_BranchVer>();
        public bool LoadBranchesVerFromCacheFile(string _path)
        {
            return LoadBranchesVerFromLines(AU_FileHelper.ReadFileToLines(_path));
        }
        public bool LoadBranchesVerFromString(string _txt)
        {
            return LoadBranchesVerFromLines(_txt.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries));
        }
        private bool LoadBranchesVerFromLines(string[] _lines)
        {
            if (_lines == null || _lines.Length <= 0)
            {
                return true;
            }
            AU_VersionInfo tmpVer = new AU_VersionInfo();
            tmpVer.Set(Ver);
            Dictionary<string, AU_BranchVer> tmpBran = new Dictionary<string, AU_BranchVer>();
            foreach (var b in Branches)
            {
                tmpBran.Add(b.Key, b.Value);
            }
            try
            {
                if (_lines[0][0] == 0xFEFF)
                {
                    _lines[0] = _lines[0].Substring(1);
                }
                foreach (var l in _lines)
                {
                    if (l.IndexOf("Ver:") == 0)
                    {
                        string[] vs = l.Substring(4).Split('.');
                        int tempEditon = int.Parse(vs[0]);
                        int tempCode = int.Parse(vs[1]);
                        int tempRes = int.Parse(vs[2]);
                        if (Ver.GreaterOrEqual(tempEditon, tempCode, tempRes))
                        {
                            return true;
                        }
                        Ver.Set(tempEditon, tempCode, tempRes);
                    }
                    else
                    {
                        var sp = l.Split('|', '$');
                        Branches[sp[0]] = new AU_BranchVer(sp[0], sp[1]);
                    }
                }
                return false;
            }
            catch (Exception er)
            {
                Ver.Set(tmpVer);
                Branches = tmpBran;
#if UNITY_EDITOR
                Debug.Log("[更新]读取版本号文件异常：" + er.ToString());
#endif
                return true;
            }
        }
    }
}
