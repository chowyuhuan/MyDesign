using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetUpdate
{
    public class AU_FileHelper
    {
        public static System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed();
        public static string[] ReadFileToLines(string filename)
        {
            try
            {
                return System.IO.File.ReadAllLines(filename, Encoding.UTF8);
            }
            catch (System.Exception ex)
            {
#if UNITY_EDITOR
                Debug.Log("异常： FileHelper ReadFileToLines: " + ex.ToString());
#endif
                return null;
            }
        }
        public static byte[] ReadFileToBytes(string path, string filename)
        {
            return ReadFileToBytes(System.IO.Path.Combine(path, filename));
        }
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
        public static void CreateDirectory(string path)
        {
            try
            {
                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }
            }
            catch (System.Exception ex)
            {
#if UNITY_EDITOR
                Debug.Log("[文件]异常： FileHelper CreateDirectory: " + ex.ToString());
#endif
            }
        }
        public static void WriteFile(string path, string filename, string data)
        {
            byte[] b = System.Text.Encoding.UTF8.GetBytes(data);
            WriteFile(path, filename, b);
        }
        public static void WriteFile(string filename, string data)
        {
            byte[] b = System.Text.Encoding.UTF8.GetBytes(data);
            WriteFile(filename, b);
        }
        public static void WriteFile(string path, string filename, byte[] data)
        {
            WriteFile(System.IO.Path.Combine(path, filename), data);
        }
        public static void WriteFile(string filename, byte[] data)
        {
            try
            {
                if (System.IO.Directory.Exists(filename))
                {
                    System.IO.Directory.Delete(filename, true);
                }
                string outpath = System.IO.Path.GetDirectoryName(filename);
                if (System.IO.Directory.Exists(outpath) == false)
                {
                    System.IO.Directory.CreateDirectory(outpath);
                }
                using (var s = System.IO.File.Create(filename))
                {
                    s.Write(data, 0, data.Length);
                }
                //System.IO.File.WriteAllBytes(filename, data);
            }
            catch (System.Exception ex)
            {
#if UNITY_EDITOR
                Debug.Log("[文件]异常： FileHelper WriteFile: " + ex.ToString());
#endif
            }
        }
        public static void CopyFile(string srcpath, string srcfilename, string dstpath, string dstfilename)
        {
            string srcfullname = System.IO.Path.Combine(srcpath, srcfilename);
            string dstfullname = System.IO.Path.Combine(dstpath, dstfilename);
            CopyFile(srcfullname, dstfullname);
        }
        public static void CopyFile(string srcpath, string dstpath)
        {
            try
            {
                System.IO.File.Copy(srcpath, dstpath, true);
            }
            catch (System.Exception ex)
            {
#if UNITY_EDITOR
                Debug.Log("[文件]异常： FileHelper CopyFile: " + ex.ToString());
#endif
            }
        }
    }
}
