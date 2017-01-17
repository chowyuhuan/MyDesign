using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AssetUpdate
{
    public class AU_BranchVer {
        public string Branch;
        public string HashValue;
        public Dictionary<string, AU_FileVer> filelist = new Dictionary<string, AU_FileVer>();
        public AU_BranchVer(string _branch, string _hash)
        {
            Branch = _branch;
            HashValue = _hash;
        }
        /// <summary>
        /// 将分支内文件版本信息写入文件，直接创建/覆盖
        /// </summary>
        /// <param name="filename">文件路径</param>
        public void SaveToFile(string filename)
        {
            string outstr = "";
            foreach (var l in filelist)
            {
                outstr += l.Value.ToString() + "\n";
            }
            AU_FileHelper.WriteFile(filename, outstr);
        }
        public bool LoadFilesVerFromCacheFile(string path)
        {
            return LoadFilesVerFromLines(AU_FileHelper.ReadFileToLines(path));
        }
        public bool LoadFilesVerFromString(string str)
        {
            return LoadFilesVerFromLines(str.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries));
        }
        private bool LoadFilesVerFromLines(string[] lines)
        {
            try
            {
                if (lines == null || lines.Length == 0) return false;
                foreach (var l in lines)
                {
                    var sp = l.Split('|', '$');
                    filelist[sp[0]] = new AU_FileVer(Branch, sp[0], sp[1], sp[2], false);
                }
                if (filelist.Count == 0) return false;
            }
            catch(Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError("[更新]读取版本文件失败（LoadFilesVerFromLines）" + e.Message);
#endif
                return false;
            }
            return true;
        }
}
}
