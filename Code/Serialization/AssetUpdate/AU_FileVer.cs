using UnityEngine;
using System.Collections;
using System;

namespace AssetUpdate
{
    public class AU_FileVer
    {
        public bool NeedUpdate;
        public string Branch;
        public string Name;
        public string Hash;
        public long Length;

        public AU_FileVer(string _branch, string _name, string _hash, string _length, bool _update)
        {
            Name = _name;
            Hash = _hash;
            Branch = _branch;
            long.TryParse(_length, out Length);
            NeedUpdate = _update;
        }

        public AU_FileVer(string _branch, string _name, string _hash, long _length, bool _update)
        {
            Name = _name;
            Hash = _hash;
            Branch = _branch;
            Length = _length;
            NeedUpdate = _update;
        }

        public override string ToString()
        {
            return Name + "|" + Hash + "$" + Length.ToString();
        }

        public void Download(AU_VersionControl.StringLongException onDown)
        {
            Action<WWW, string> load = (WWW, tag) =>
            {
                Exception _err = null;
                bool unmatched = false;
                try
                {
                    /* 保存到本地磁盘 */
                    AU_FileHelper.WriteFile(AU_AppConfig.GetPathOrUrl(AU_AppConfig.EFilePos.LocalCache), Branch + "/" + Name, WWW.bytes);
                    /* hash校准 */
                    string tm_hash = Convert.ToBase64String(AU_FileHelper.sha1.ComputeHash(WWW.bytes));
                    unmatched = Hash != tm_hash;
                    if (unmatched)
                    {
                        Hash = tm_hash;
                    }
                    NeedUpdate = false;
                }
                catch (Exception err)
                {
                    _err = err;
                }

                if (WWW.bytes.Length == 0)
                {
                    _err = new Exception("下载size==0" + WWW.url);
                }

                if (_err == null && unmatched)
                {
                    //保存信息
                    AU_VersionControl.SaveBranchVerToFile(Branch);
                }
                if (onDown != null)
                {
                    onDown(Branch, Name, Hash, Length, _err);
                }
            };
            AU_FileLoader.LoadFromWWW(AU_AppConfig.GetPathOrUrl(AU_AppConfig.EFilePos.Remote) + "/" + Branch + "/" + Name, "", load);
#if UNITY_EDITOR
            Debug.Log("[更新]开始下载文件： " + Branch + "/" + Name);
#endif
        }
    }
}
