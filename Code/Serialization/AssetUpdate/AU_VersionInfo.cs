using UnityEngine;
using System.Collections;

namespace AssetUpdate
{
    public class AU_VersionInfo
    {
        public int Edition = 0;
        public int CodeVer = 0;
        public int ResVer = 0;

        public void Set(int _edition, int _code, int _res)
        {
            Edition = _edition;
            CodeVer = _code;
            ResVer = _res;
        }
        public void Set(AU_VersionInfo _info)
        {
            Edition = _info.Edition;
            CodeVer = _info.CodeVer;
            ResVer = _info.ResVer;
        }
        public string ToString()
        {
            return Edition.ToString() + '.' + CodeVer.ToString() + '.' + ResVer.ToString();
        }
        public bool DiffAPK(AU_VersionInfo _info)
        {
            return Edition != _info.Edition || CodeVer != _info.CodeVer;
        }
        public bool GreaterOrEqual(int _edition, int _code, int _res)
        {
            if (Edition > _edition)
            {
                return true;
            }
            else if (Edition < _edition)
            {
                return false;
            }
            else
            {
                if (CodeVer > _code)
                {
                    return true;
                }
                else if (CodeVer < _code)
                {
                    return false;
                }
                else
                {
                    return ResVer >= _res;
                }
            }
        }
        public static bool operator <(AU_VersionInfo l, AU_VersionInfo r)
        {
            if (l.Edition < r.Edition)
            {
                return true;
            }
            else if (l.Edition > r.Edition)
            {
                return false;
            }
            else
            {
                if (l.CodeVer < r.CodeVer)
                {
                    return true;
                }
                else if (l.CodeVer > r.CodeVer)
                {
                    return false;
                }
                else
                {
                    return l.ResVer < r.ResVer;
                }
            }
        }
        public static bool operator >(AU_VersionInfo l, AU_VersionInfo r)
        {
            if (l.Edition > r.Edition)
            {
                return true;
            }
            else if (l.Edition < r.Edition)
            {
                return false;
            }
            else
            {
                if (l.CodeVer > r.CodeVer)
                {
                    return true;
                }
                else if (l.CodeVer < r.CodeVer)
                {
                    return false;
                }
                else
                {
                    return l.ResVer > r.ResVer;
                }
            }
        }
        public static bool operator <=(AU_VersionInfo l, AU_VersionInfo r)
        {
            if (l.Edition < r.Edition)
            {
                return true;
            }
            else if (l.Edition > r.Edition)
            {
                return false;
            }
            else
            {
                if (l.CodeVer < r.CodeVer)
                {
                    return true;
                }
                else if (l.CodeVer > r.CodeVer)
                {
                    return false;
                }
                else
                {
                    return l.ResVer <= r.ResVer;
                }
            }
        }
        public static bool operator >=(AU_VersionInfo l, AU_VersionInfo r)
        {
            if (l.Edition > r.Edition)
            {
                return true;
            }
            else if (l.Edition < r.Edition)
            {
                return false;
            }
            else
            {
                if (l.CodeVer > r.CodeVer)
                {
                    return true;
                }
                else if (l.CodeVer < r.CodeVer)
                {
                    return false;
                }
                else
                {
                    return l.ResVer >= r.ResVer;
                }
            }
        }
    }
}
