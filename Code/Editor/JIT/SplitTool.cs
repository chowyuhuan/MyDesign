using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

// 两种解决思路：
// 1.分析文本
//  优点：保留源代码现场，不会丢东西。减少由于工具自动化，带来的特殊化的错误
//  缺点：获取变量信息可能会有问题，目前可能只能通过词法语法分析收集，特殊情况照顾不周可能会造成错误；继承的序列化数据没有考虑（简单地通过文本获取父类的字段，目前来看不可行。总会需要反射相关，这样的话，还不如直接用反射做，即下面的方案）
// 2.分析c#的程序集
//  优点：写起来可能相对简单
//  缺点：不能保留源代码现场，一些字段的初始化可能会丢失


// 遇到的问题：
// 1.继承关系中，基类中有字段为非public但被标记[SerializeField]，该字段在子类的inspector中能够看到并且赋值，但在子类的脚本中，不能直接访问此字段。禁用[SerializeField]?
// 解决：子类中，不能访问父类的非public成员。所以，父类的非public成员被标记[SerializeField]，对于子类来说，没有意义；但子类可以通过调用父类的一个接口来间接访问这个字段。而且，测试发现，访问到的值恰恰是在子类的inspector面板中设置的值。
// 2.关于非public但被标记[SerializeField]的字段：逻辑脚本启动时，需要将数据脚本中的数据字段拷贝到逻辑脚本中，这就要保证逻辑脚本能够访问到数据脚本的字段。所以，数据脚本中的字段必须是public（SerializeField必然出现在没有public时，没有
// public但被internal修饰的字段，不能被其他程序集访问，而逻辑脚本所在程序集与数据脚本不是一个）。
// 解决：放弃SerializeField，如果需要序列化，只能使用public。这样，拷贝数据也轻松了许多，1的问题也就没有了。
// 3.拆分出来的数据脚本要包含逻辑脚本类名字和挂逻辑脚本的Awake函数，两种实现方式：a.每一个数据脚本中都有逻辑脚本名字声明和Awake函数定义；b.所有数据脚本最终都要继承自同一个类，这个类包含逻辑脚本名字声明和Awake函数定义
// 解决：继承自同一个类时，每一个数据脚本依然要实现自己的返回逻辑类名字的接口，而且还会破坏正常的Awake使用。而如果使用a方式，代码量还要比b方式少（不用给逻辑类名字变量赋值，直接硬编码就行），仅仅一个Awake函数就行。这样做，Awake的机制也
// 没有被破坏。
// 4.如果直接将逻辑脚本放到Assets外的单独一个工程中，程序在开发调试时，不是很方便。
// 解决：
//  a.尝试在IOS的方案基础上做改善：所有脚本放在Assets下，程序可以为了快速测试，临时忽略数据和逻辑的分开限定。待代码完善后，分出数据和逻辑。将逻辑部分放到JITDLL文件夹下。接下来要做的是，尝试添加自动化将JITDLL移到外面，建立VS工程，命
//    令行打出DLL，并将DLL放置到StreamingAssets下，并让Unity重新编译。
//  b.如果C#支持不建立工程就能打出DLL，则a没有必要将脚本移除Assets并建立工程，直接用使用JITDLL下代码生成DLL，之后删除JITDLL所有脚本即可。

// 结论：
// 1.分析序列化字段时，采用分析程序集，即反射做
// 2.向代码中插入拷贝函数时，分析文本。插入的文本没有格式化，需要手动格式化插入后的脚本文件
// 3.序列化字段只能用public修饰，不能用[SerializeField]（数据拷贝时需要能够访问这些数据，非public不可被访问）
// 4.编码要符合最基本的规范（见InsertCopyDataFunctionToLogicScript的注视）
// 5.现在的拆分思路是：在逻辑DLL中先写好，之后工具帮助拆出数据；考虑如果先在Unity原生环境编码，工具拆分到DLL中，是否更好？（这样，用户就可以利用Editor的可视化调试）
// 6.IOS因为AOT，不能解析DLL，所以，直接将逻辑脚本放到Assembly-CSharp.dll中即可，直接使用逻辑类的type就可以
// 7.JITDLL下的脚本不能被挂在Prefab上，只有Serialization下的脚本可以被直接挂在Prefab上
// 8.需要被分拆的脚本，不要使用命名空间（仅仅是为了保证工具能够验证被分拆的脚本是否继承MonoBehaviour）
public class SplitTool : MonoBehaviour
{
    // 分析反射
    #region BY_REFLECTION

    const string DataScriptAwakeFunc = "void Awake()\n{\n#if JIT && !UNITY_IOS\nScriptAssembly.Assemble(gameObject,\"XX\", this); // !!!不要删除，否则丢失逻辑组件\n#else\nScriptAssembly.Assemble<XX>(gameObject, this);\n#endif\n}\n";
    const string LogicScriptAwakeFunc =
        @"void Awake()
{
CopyDataFromDataScript();
}
";
    const string CopyDataFuncName = "CopyDataFromDataScript";
    const string CopyDataFuncPrefix =
        @"protected void CopyDataFromDataScript()
{
";
    const string DataScriptClassNameSuffix = "_SD"; // Serialized Data缩写
    const string LogicScriptClassNameSuffix = "_DL"; // Dynamic Logic缩写
    const string ClassAndStructDeclarationNote = "// !!!TODO:1.此类/结构体的声明可能与源文件不符，请核对源文件; 2.确保此类/结构体已经存在";
    const string InheritedNote = " // !!!TODO:存在继承关系时，确保基类已经存在；如果继承链中没有序列化数据，去掉继承，改成MonoBehaviour";
    const string ClassNote = "// TODO：确保此文件中使用到的类型已经存在";
    public const string JITDLLFolder = "Scripts/JITDLL";
    public const string SerializationFolder = "Scripts/Serialization";

    static bool FromSerialization = false;
    static string DataClassName = "";
    static string LogicClassName = "";
    static Type SampleType = null;
    static Dictionary<string, string> ReplaceTypeStr = new Dictionary<string, string>();

    static SplitTool()
    {
        ReplaceTypeStr["System.Boolean"] = "bool";
        ReplaceTypeStr["System.Double"] = "double";
        ReplaceTypeStr["System.Single"] = "float";
        ReplaceTypeStr["System.Int32"] = "int";
        ReplaceTypeStr["System.String"] = "string";
        ReplaceTypeStr["System.Char"] = "Char";
    }

    public static bool IsNeedKeywordNew(Type _type)
    {
        if (_type.IsValueType)
        {
            if (_type.IsEnum || _type.IsPrimitive)
            {
                return false;
            }
            else // 结构体
            {
            }
        }
        // 类和结构体
        return true;
    }

    public static string ReplaceTypeString(string primitiveStr)
    {
        string outValue;
        if (ReplaceTypeStr.TryGetValue(primitiveStr, out outValue))
        {
            return outValue;
        }
        else if (primitiveStr.StartsWith("System.Collections.Generic.List"))
        {
            string temp = primitiveStr.Replace("`1[", "<");
            return temp.Replace("]", ">");
        }
        return primitiveStr;
    }

    public static string GetFieldValue(FieldInfo _info, object _instance)
    {
        if (_instance == null)
        {
            return "TODO";
        }
        else if (_info.FieldType.IsValueType) // 值类型
        {
            if (_info.FieldType.IsEnum || _info.FieldType.IsPrimitive) // 基础类型
            {
                return _info.GetValue(_instance).ToString();
            }
            else // 结构体
            {
                return "new " + ReplaceTypeString(_info.FieldType.ToString()) + "()";
            }
        }
        else // 引用类型
        {
            if (_info.GetValue(_instance) == null)
            {
                return "null";
            }
            else
            {
                return "new " + ReplaceTypeString(_info.FieldType.ToString()) + "()";
            }
        }
    }

    public static string GetShortName(Type _type)
    {
        if (string.IsNullOrEmpty(_type.Namespace))
        {
            return _type.FullName;
        }
        else
        {
            return _type.FullName.Remove(0, _type.Namespace.Length + 1);
        }
    }

    // 前提：
    //  1._srcFile必须在JITDLLFolder或者SerializationFolder下
    //  2.Type必须继承MonoBehaviour
    public static void SplitScript(string _srcFile)
    {
        string fileName = System.IO.Path.GetFileNameWithoutExtension(_srcFile);
        Assembly assembly = Assembly.LoadFile(System.IO.Path.Combine(Application.dataPath, "../Library/ScriptAssemblies/Assembly-CSharp.dll"));

        // 中间变量赋值
        FromSerialization = _srcFile.Contains(SerializationFolder);
        SampleType = assembly.GetType(fileName);
        DataClassName = FromSerialization ? fileName : fileName + DataScriptClassNameSuffix;
        LogicClassName = FromSerialization ? fileName + LogicScriptClassNameSuffix : fileName;


        string relativePath = _srcFile.Substring((FromSerialization ? SerializationFolder.Length : JITDLLFolder.Length) + 8); // RefreshGUI已经限定，所选择的对象必须在Assets/Scripts/JITDLL下，所以，这里直接偏移20
        int idx = relativePath.LastIndexOf('/');
        if (idx != -1)
        {
            relativePath = relativePath.Remove(idx);
        }
        else
        {
            relativePath = "";
        }
        
        // 备份
        string backup = System.IO.Path.Combine(Application.dataPath, "WebplayerTemplates/SplitedBackup");
        backup = System.IO.Path.Combine(backup, relativePath);
        if (!System.IO.Directory.Exists(backup))
        {
            System.IO.Directory.CreateDirectory(backup);
        }
        backup = System.IO.Path.Combine(backup, fileName + ".cs");
        AssetDatabase.CopyAsset(_srcFile, backup);


        string disFile = System.IO.Path.Combine(Application.dataPath, FromSerialization ? JITDLLFolder : SerializationFolder);
        disFile = System.IO.Path.Combine(disFile, relativePath);
        if (!System.IO.Directory.Exists(disFile))
        {
            System.IO.Directory.CreateDirectory(disFile);
        }
        disFile = System.IO.Path.Combine(disFile, (FromSerialization ? LogicClassName : DataClassName) + ".cs");

        if (FromSerialization) // Serialization下脚本直接拷贝到JITDLL下，作为JIT的逻辑脚本
        {
            AssetDatabase.CopyAsset(_srcFile, FileUtil.GetProjectRelativePath(disFile));
            ModifyClassName(disFile, fileName);
        }

        System.IO.File.WriteAllText(FromSerialization ? _srcFile : disFile, GenerateSerialiableScript());
        InsertCopyFunctionToLogic(FromSerialization ? disFile : _srcFile);
        AssetDatabase.Refresh();
    }

    public static string GenerateSerialiableScript()
    {
        StringBuilder txt = new StringBuilder();

        //创建类的实例    
        

        object obj = null;
        if (!SampleType.IsAbstract)
        {
            obj = Activator.CreateInstance(SampleType, true);
        }

        // ---------------------------- 类头 ---------------------------------------------
        txt.AppendLine(ClassNote);
        txt.Append(SampleType.IsAbstract ? "public abstract class " : "public class ");
        txt.Append(DataClassName);
        if (SampleType.BaseType != null)
        {
            txt.Append(" : ");
            txt.Append(SampleType.BaseType);
            if (!FromSerialization && SampleType.BaseType != typeof(MonoBehaviour))
            {
                txt.Append(DataScriptClassNameSuffix);
            }
        }
        // string中添加Environment.NewLine也能实现回车的作用
        txt.AppendLine();
        txt.Append("{");
        txt.AppendLine();


        // ---------------------------- 字段 ---------------------------------------------
        FieldInfo[] infos = SampleType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        foreach (var i in infos)
        {
            if (ShouldSerialized(i))
            {
                txt.Append("public " + ReplaceTypeString(i.FieldType.ToString()) + " " + i.Name + " = " + GetFieldValue(i, obj));
                txt.AppendLine(";");
                continue;
            }
        }

        // ------------------------------------- Awake ------------------------------------
        txt.Append(DataScriptAwakeFunc.Replace("XX", LogicClassName));


        // ---------------------------------- 类尾 -------------------------------------------------
        txt.Append("}");
        return txt.ToString();
    }

    public static string GenerateCopyImplement()
    {
        StringBuilder txt = new StringBuilder();

        txt.Append(CopyDataFuncPrefix);

        if(!SampleType.IsSubclassOf(typeof(MonoBehaviour)))
        {
            // 调用父类拷贝函数
            txt.Append("base.");
            txt.Append(CopyDataFuncName);
            txt.AppendLine("();");
        }

        // -------------------------------- 取数据组件组件 --------------------------------------
        string dataComponent = DataClassName;
        txt.Append(dataComponent);
        txt.Append(" dataComponent = gameObject.GetComponent<");
        txt.Append(dataComponent);
        txt.AppendLine(">();");

        // -------------------------------- 判空 ---------------------------------------------
        txt.AppendLine("if (dataComponent == null)");
        txt.AppendLine("{");
        txt.Append("UnityEngine.Debug.LogError(\"[热更新]没有找到数据组件：");
        txt.Append(dataComponent);
        txt.Append(",GameObject：\"");
        txt.AppendLine(" + gameObject.name, gameObject);");
        txt.AppendLine("}");
        txt.AppendLine("else");
        txt.AppendLine("{");

        // ---------------------------- 字段 ---------------------------------------------
        FieldInfo[] infos = SampleType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        foreach (var i in infos)
        {
            if (ShouldSerialized(i))
            {
                txt.Append(i.Name + " = ");
                txt.Append("dataComponent.");
                txt.Append(i.Name);
                txt.AppendLine(";");
            }
        }

        txt.AppendLine("}");

        txt.Append("}");

        return txt.ToString();
    }

    // 仅支持比较通用的编码规范
    // 1.函数格式:void function(){ 后面不允许有内容，必须另起一行，即大括号后必须另起一行编码
    // 2.脚本内除了正常的Awake函数外，其他地方避免出现void Awake()字样
    // 3.脚本内仅有一个继承MonoBehaviour的类，保证代码在文件的最后，且此类的右大括号后不要有其他内容包含右大括号
    public static void InsertCopyFunctionToLogic(string _path)
    {
        List<string> lines = new List<string>();
        lines.AddRange(System.IO.File.ReadAllLines(_path));
        int idx_func = -1;
        for (int i = 0; i < lines.Count; ++i)
        {
            if (lines[i] == null)
            {
                continue;
            }

            // 判断方式比较简单，能够满足99%的编程。如果出现意外，手动改吧
            int idx_void = lines[i].IndexOf("void");
            int idx_awake = lines[i].IndexOf("Awake");
            int idx_parentheses = lines[i].IndexOf("()");
            if (idx_void != -1 && idx_awake != -1 && idx_parentheses != -1 && idx_parentheses > idx_awake && idx_awake > idx_void)
            {
                idx_func = i;
                break;
            }
        }
        if (idx_func != -1)
        {
            for (int i = idx_func; i < lines.Count; ++i)
            {
                int idx_brace = lines[i].IndexOf("{");
                if (idx_brace != -1)
                {
                    lines.Insert(i + 1, CopyDataFuncName + "();");
                    break;
                }
            }
        }


        // 找到文件最后的}
        int idx_last = -1;
        for (int i = lines.Count - 1; i > -1; --i)
        {
            if (lines[i].Contains("}"))
            {
                idx_last = i;
                break;
            }
        }

        if (idx_last == -1)
        {
            Debug.LogError("指定脚本编码不符合标准：没有找到类的右大括号");
        }

        // 没有Awake函数
        if (idx_func == -1)
        {
            lines.Insert(idx_last, LogicScriptAwakeFunc);
            // 添加CopyDataFromDataScript函数
            lines.Insert(idx_last + 1, GenerateCopyImplement());
        }
        else
        {
            // 添加CopyDataFromDataScript函数
            lines.Insert(idx_last, GenerateCopyImplement());
        }
        System.IO.File.WriteAllLines(_path, lines.ToArray());
    }

    public static void ModifyClassName(string _file, string _oldName)
    {
        List<string> lines = new List<string>();
        lines.AddRange(System.IO.File.ReadAllLines(_file));
        bool replaced = false;
        for (int i = 0; i < lines.Count; ++i)
        {
            if (lines[i] == null)
            {
                continue;
            }

            // 判断方式比较简单，能够满足99%的编程。如果出现意外，手动改吧
            int idx_public = lines[i].IndexOf("public");
            int idx_class = lines[i].IndexOf("class");
            int idx_name = lines[i].IndexOf(_oldName);
            if (idx_public != -1 && idx_class != -1 && idx_name != -1 && idx_name > idx_class && idx_class > idx_public)
            {
                lines[i] = lines[i].Replace(_oldName, _oldName + LogicScriptClassNameSuffix);
                replaced = true;
                break;
            }
        }
        if(replaced)
        {
            System.IO.File.WriteAllLines(_file,lines.ToArray());
        }
    }

    // 此函数还有完善空间，根据遇到的问题再完善
    public static bool ShouldSerialized(FieldInfo info)
    {
        return info.IsPublic && CanSerialized(info.FieldType) && !Attribute.IsDefined(info, typeof(HideInInspector)) && !info.FieldType.IsSubclassOf(typeof(Delegate));
    }

    // custom non abstract classes with [Serializable] attribute.
    // custom structs with [Serializable] attribute. (Added in Unity 4.5)
    // references to objects that derive from UnityEngine.Object
    // primitive data types (int, float, double, bool, string, etc.)
    // array of a fieldtype we can serialize
    // List<T> of a fieldtype we can serialize
    // 此函数还有完善空间，根据遇到的问题再完善
    public static bool CanSerialized(Type type)
    {
        if(type.IsPrimitive)
        {
            return true;
        }
        else if (type.IsClass)
        {
            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                return true;
            }
            else
            {
                return type.IsSerializable && !type.IsAbstract;
            }
        }
        else if(type.IsValueType)
        {
            return type.IsSerializable;
        }
        return false;
    }

    #endregion

    // 分析文本
    // 做到一半，遇到一些问题，使用反射会容易解决，所以转向使用反射了，见region BY_REFLECTION
    #region BY_TEXT 
    const string DataScrpitTemplatePrefix =
    @"public class ClassName : MonoBehaviour
{
    string LogicScriptName = 'LogicName';
        ";
    public static void EmbedTemplate(ref List<string> _dataLines)
    {
        _dataLines.Insert(0, DataScrpitTemplatePrefix);
        _dataLines.Add("}");
    }

    // TODO:半成品，分析文本。转向使用反射了，见GenerateSerialiableScript
    public static List<string> GenerateData(string _fullPath)
    {
        List<string> dataLines = new List<string>();
        string[] lines = System.IO.File.ReadAllLines(_fullPath);
        for (int i = 0; i < lines.Length; ++i)
        {
            if (lines[i].Contains("static") || lines[i].Contains("const") || lines[i].Contains("readonly"))
            {
                continue;
            }

            // ------------------------------ [SerializeField] ------------------------------------------------
            int index = lines[i].IndexOf("[SerializeField]");
            if (index != -1)
            {
                dataLines.Add(lines[i]); // 包含[SerializeField]那行
                if (!lines[i].Contains(';'))
                {
                    while (!lines[++i].Contains(';'))
                    {
                        dataLines.Add(lines[i]);
                    }
                    dataLines.Add(lines[i]); // 包含;那行
                }
                continue;
            }

            // -------------------------------- public --------------------------------------------------------
            index = lines[i].IndexOf("public");
            if (index != -1) // 看看public后面的内容，如果不是函数、属性，那可能就是字段
            {
                int newLine = i;
                string[] words = NextWords(lines, i, index + 7, out newLine);
                if (words != null && words.Length > 2 && words[2] == "=")
                {
                    dataLines.Add(lines[i]); // 包含public那行
                    if (!lines[i].Contains(';'))
                    {
                        while (!lines[++i].Contains(';'))
                        {
                            dataLines.Add(lines[i]);
                        }
                        dataLines.Add(lines[i]); // 包含;那行
                    }
                    continue;
                }
            }
        }
        EmbedTemplate(ref dataLines);
        return dataLines;
    }
    static string NextWord(string[] _lines, int _startLine, int _wordOffset)
    {
        if (_lines.Length <= _startLine || _lines[_startLine].Length <= _wordOffset)
        {
            return null;
        }
        do
        {
            string tmpLine = _lines[_startLine].Substring(_wordOffset);
            string[] words = tmpLine.Split(new char[] { '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (words != null && words.Length > 0)
            {
                return words[0];
            }
            _wordOffset = 0;
        } while (++_startLine < _lines.Length);
        return null;
    }

    static string[] NextWords(string[] _lines, int _startLine, int _wordOffset)
    {
        if (_lines.Length <= _startLine || _lines[_startLine].Length <= _wordOffset)
        {
            return null;
        }
        do
        {
            string tmpLine = _lines[_startLine].Substring(_wordOffset);
            string[] words = tmpLine.Split(new char[] { '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (words != null && words.Length > 0)
            {
                return words;
            }
            _wordOffset = 0;
        } while (++_startLine < _lines.Length);
        return null;
    }

    static string NextWord(string[] _lines, int _startLine, int _wordOffset, out int _newLine)
    {
        if (_lines.Length <= _startLine || _lines[_startLine].Length <= _wordOffset)
        {
            _newLine = 0;
            return null;
        }
        do
        {
            _newLine = _startLine;
            string tmpLine = _lines[_startLine].Substring(_wordOffset);
            string[] words = tmpLine.Split(new char[] { '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (words != null && words.Length > 0)
            {
                return words[0];
            }
            _wordOffset = 0;
        } while (++_startLine < _lines.Length);
        return null;
    }

    static string[] NextWords(string[] _lines, int _startLine, int _wordOffset, out int _newLine)
    {
        if (_lines.Length <= _startLine || _lines[_startLine].Length <= _wordOffset)
        {
            _newLine = 0;
            return null;
        }
        do
        {
            _newLine = _startLine;
            string tmpLine = _lines[_startLine].Substring(_wordOffset);
            string[] words = tmpLine.Split(new char[] { '\n', '\t', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words != null && words.Length > 0)
            {
                return words;
            }
            _wordOffset = 0;
        } while (++_startLine < _lines.Length);
        return null;
    }
    #endregion
}