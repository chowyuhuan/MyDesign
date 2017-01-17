using System;
using System.Collections.Generic;
using System.Text;


public class AM_VerInfo
{
    public AM_VerInfo(string group)
    {
        this.group = group;
    }
    public string group
    {
        get;
        private set;
    }
    public int match = 0;
    public Dictionary<string, string> filehash = new Dictionary<string, string>();

    static System.Security.Cryptography.SHA1CryptoServiceProvider osha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
    public void GenHash(string parent_path)
    {
        string[] files = System.IO.Directory.GetFiles(System.IO.Path.Combine(parent_path,this.group), "*.*", System.IO.SearchOption.AllDirectories);
        foreach (var f in files)
        {
            if (f.IndexOf(".crc.txt") >= 0
                ||
                f.IndexOf(".meta") >= 0
                ||
                f.IndexOf(".db") >= 0
                ) continue;
            GenHashOne(f, parent_path);
        }
    }
    public void GenHashOne(string filename, string parent_path)
    {
        using (System.IO.Stream s = System.IO.File.OpenRead(filename))
        {
            var hash = osha1.ComputeHash(s);
            var shash = Convert.ToBase64String(hash) + "$" + s.Length;
            filename = filename.Substring(parent_path.Length + 1 + group.Length + 1);

            filename = filename.Replace('\\', '/');
            filehash[filename] = shash;
        }
    }
    public string SaveToPath(string path)
    {
        string outstr = "";
        foreach (var f in filehash)
        {
            outstr += f.Key + "|" + f.Value + "\n";
        }
        string g = this.group.Replace('/', '_');
        string outfile = System.IO.Path.Combine(path, g + ".cfg");
        System.IO.File.WriteAllText(outfile, outstr, Encoding.UTF8);
        using (System.IO.Stream s = System.IO.File.OpenRead(outfile))
        {
            var hash = osha1.ComputeHash(s);
            var shash = Convert.ToBase64String(hash);
            return shash;
        }
    }
    public bool Read(string hash, int filecount, string path)
    {
        string g = this.group.Replace('/', '_');
        string file = System.IO.Path.Combine(path, g + ".cfg");
        if (System.IO.File.Exists(file) == false) return false;
        using (System.IO.Stream s = System.IO.File.OpenRead(file))
        {
            var rhash = osha1.ComputeHash(s);
            var shash = Convert.ToBase64String(rhash);
            if (shash != hash) return false;//Hash 不匹配
        }
        string txt = System.IO.File.ReadAllText(file, Encoding.UTF8);
        string[] lines = txt.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var l in lines)
        {
            var sp = l.Split('|');
            filehash[sp[0]] = sp[1];
        }
        return true;
    }
}

public class AM_Verall
{
    static public string BRANCHESVERFILENAME = "Version.cfg";

    // Ver:1.2.5(Edition:1,CodeVer:2,ReSVer:5)
    public int Edition = 0;
    public int CodeVer = 0;
    public int ResVer = 0;

    public Dictionary<string, AM_VerInfo> groups = new Dictionary<string, AM_VerInfo>();

    public override string ToString()
    {
        int useful = 0;
        int filecount = 0;
        int filematch = 0;
        foreach (var i in groups)
        {
            if (i.Value.match > 0) useful++;
            filematch += i.Value.match;
            filecount += i.Value.filehash.Count;
        }
        return "ver=" + ResVer + " group=(" + useful + "/" + groups.Count + ") file=(" + filematch + "/" + filecount + ")";
    }

    public static AM_Verall Read(string path)
    {
        if (System.IO.File.Exists(System.IO.Path.Combine(path, BRANCHESVERFILENAME)) == false)
        {
            return null;
        }
        string txt = System.IO.File.ReadAllText(System.IO.Path.Combine(path, BRANCHESVERFILENAME), Encoding.UTF8);
        string[] lines = txt.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
        AM_Verall var = new AM_Verall();
        foreach (var l in lines)
        {
            if (l.IndexOf("Ver:") == 0)
            {
                string[] vs = l.Substring(4).Split('.');
                var.Edition = int.Parse(vs[0]);
                var.CodeVer = int.Parse(vs[1]);
                var.ResVer = int.Parse(vs[2]);
            }
            else
            {
                var sp = l.Split('|');
                var.groups[sp[0]] = new AM_VerInfo(sp[0]);
                var.groups[sp[0]].Read(sp[1], 0, path);
            }
        }
        return var;
    }
    public void SaveToPath(string path)
    {
        Dictionary<string, string> grouphash = new Dictionary<string, string>();
        foreach (var i in groups.Values)
        {
            grouphash[i.group] = i.SaveToPath(path);
        }

        string outstr = "Ver:" + this.Edition + '.' + this.CodeVer + '.' + this.ResVer + "\n";
        foreach (var g in grouphash)
        {
            outstr += g.Key + "|" + g.Value + "\n";
        }
        System.IO.File.WriteAllText(System.IO.Path.Combine(path, BRANCHESVERFILENAME), outstr, Encoding.UTF8);
    }

    public void SetVer(string targetVersion)
    {
        string[] vs = targetVersion.Split('.');
        Edition = int.Parse(vs[0]);
        if(vs.Length > 1)
        {
            CodeVer = int.Parse(vs[1]);
        }
        else
        {
            CodeVer = 0;
        }
        if(vs.Length > 2)
        {
            ResVer = int.Parse(vs[2]);
        }
        else
        {
            ResVer = 0;
        }
    }

    public string GetVerString()
    {
        return string.Format("{0}{1}{2}{3}{4}", this.Edition, ".", this.CodeVer, ".", this.ResVer);
    }
}

