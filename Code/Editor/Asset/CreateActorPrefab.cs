using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;

public class CreateActorPrefab : MonoBehaviour
{
    static string _assetPath;
    static string _fileName;
    static GameObject _modelGameObj;
    [MenuItem("工具/资源/Prefab/同步/FBX To Prefab")]
    static void TryToSyncFBX()
    {
        if (Prepare())
        {
            BuildPrefab();
        }
    }

    static void BuildPrefab()
    {
        string prefabPath = "Assets/Resources/" + AssetManage.AM_PathHelper.GetActorFullPathByName(_fileName);
        prefabPath = System.IO.Path.ChangeExtension(prefabPath, "prefab");
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            prefab = PrefabUtility.CreatePrefab(prefabPath, _modelGameObj, ReplacePrefabOptions.Default);
        }
        else if (EditorUtility.DisplayDialog("警告", "检测到已经存在Prefab:\n" + prefabPath + "\n继续将同步修改到该Prefab", "继续"))
        {
            GameObject fbxObj = GameObject.Instantiate<GameObject>(_modelGameObj);
            GameObject preObj = GameObject.Instantiate<GameObject>(prefab);
            SyncActorPrefab(preObj, fbxObj);
            string path = System.IO.Path.GetDirectoryName(prefabPath);
            path += "/" + prefab.name + "_Syncing.prefab";
            GameObject newPrefab = PrefabUtility.ReplacePrefab(fbxObj, prefab);
            DestroyImmediate(preObj);
            DestroyImmediate(fbxObj);
            AssetDatabase.SaveAssets();
            AssetsUtility.SelectAndHighLightObject(newPrefab);
            return;
        }
        else
        {
            return;
        }

        FixPhysics(prefab, true);

        ActorPreDefine define = prefab.GetComponent<ActorPreDefine>();
        if (define == null)
        {
            define = prefab.AddComponent<ActorPreDefine>();
        }
        Transform rhand = FindChild(prefab.transform, "Bip001 Prop1");
        if (rhand != null)
        {
            define.AtkTransform = rhand;
        }
        else
        {
            define.AtkTransform = null;
        }

        Animator anim = prefab.GetComponent<Animator>();
        if (anim != null)
        {
            GameObject.DestroyImmediate(anim,true);
        }

        // ----------------------- 目前Unity没有提供代码组装controller的功能 ------------------------------
        //AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(System.IO.Path.ChangeExtension(_assetPath, ""));
        //AnimatorControllerLayer layer = controller.layers[0];
        //AnimatorStateMachine stateMachine = layer.stateMachine;
        //// ?? 

        //string[] files = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(_assetPath), "*.anim");
        //for (int i = 0; i < files.Length; ++i)
        //{
        //    AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(files[i]);
        //    if (clip != null)
        //    {
        //        anim.AddClip(clip, clip.name);
        //    }
        //}
        // ---------------------------------------------------------------------------------------------

        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
        AssetsUtility.SelectAndHighLightObject(prefab);
        EditorUtility.DisplayDialog("提示", "成功创建Preafb", "确定");
    }

    static bool Prepare()
    {
        if (Selection.objects == null || Selection.objects.Length < 1)
        {
            EditorUtility.DisplayDialog("错误", "请选择模型文件", "确定");
            return false;
        }

        if (Selection.objects.Length > 1)
        {
            EditorUtility.DisplayDialog("错误", "只能同时处理一个，请重新选择", "确定");
            return false;
        }

        _assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!_assetPath.EndsWith(".FBX"))
        {
            EditorUtility.DisplayDialog("错误", "您选择的文件不是模型文件，请重新选择", "确定");
            return false;
        }

        if (!_assetPath.StartsWith("Assets/Actors"))
        {
            EditorUtility.DisplayDialog("错误", "请在Assets/Actors下选择模型文件", "确定");
            return false;
        }

        _fileName = System.IO.Path.GetFileName(_assetPath);
        if (!_fileName.StartsWith("Actor_"))
        {
            EditorUtility.DisplayDialog("错误", "选择的模型资源不符合命名规范，请修正", "确定");
            return false;
        }
        _modelGameObj = Selection.activeGameObject;
        return true;
    }

    static void FixPhysics(GameObject obj, bool defaultColEnabled)
    {
        Rigidbody brig = obj.GetComponent<Rigidbody>();
        if (brig == null)
        {
            brig = obj.AddComponent<Rigidbody>();
        }
        brig.useGravity = false;
        brig.isKinematic = true;

        BoxCollider bcol = obj.GetComponent<BoxCollider>();
        if (bcol == null)
        {
            bcol = obj.AddComponent<BoxCollider>();
        }
        bcol.isTrigger = true;
        bcol.enabled = defaultColEnabled;
    }

    public static void SyncActorPrefab(GameObject oldP, GameObject newP)
    {
        if (oldP == null || newP == null)
        {
            return;
        }

        EditorUtility.CopySerializedIfDifferent(oldP.transform, newP.transform);
        SyncChilds(oldP, newP);

        SyncComponent<BoxCollider>(oldP, newP);
        SyncComponent<Rigidbody>(oldP, newP);
        ActorPreDefine com = SyncComponent<ActorPreDefine>(oldP, newP);
        if (com != null)
        {
            ActorPreDefine temp = oldP.GetComponent<ActorPreDefine>();
            if (temp.AtkTransform != null)
            {
                Transform atk = FindChild(newP.transform, temp.AtkTransform.name);
                if (atk != null)
                {
                    com.AtkTransform = atk;
                }
            }
        }
    }

    static void SyncChilds(GameObject oldObj, GameObject newObj)
    {
        if (oldObj == null || newObj == null)
        {
            return;
        }

        Transform oldTrans = oldObj.transform;
        for (int i = 0; i < oldTrans.childCount; ++i)
        {
            Transform oldChild = oldTrans.GetChild(i);
            Transform newChild = newObj.transform.FindChild(oldChild.name);
            if (newChild == null)
            {
                oldChild.SetParent(newObj.transform);
                --i; // SetParent之后，oldTrans减少一个child，childCount-1
                //GameObject temp = GameObject.Instantiate<GameObject>(oldChild.gameObject);
                //temp.transform.SetParent(newObj.transform);
                //temp.transform.localPosition = oldChild.localPosition;
                //temp.transform.localRotation = oldChild.localRotation;
                //temp.transform.localScale = oldChild.localScale;
            }
            else
            {
                SyncComponent<Rigidbody>(oldChild.gameObject, newChild.gameObject);
                SyncComponent<BoxCollider>(oldChild.gameObject, newChild.gameObject);
                SyncChilds(oldChild.gameObject, newChild.gameObject);
            }
        }
    }

    static T SyncComponent<T>(GameObject oldObj, GameObject newObj) where T : Component
    {
        T oldCom = oldObj.GetComponent<T>();
        if (oldCom != null)
        {
            T newCom = newObj.GetComponent<T>();
            if(newCom == null)
            {
                newCom = newObj.AddComponent<T>();
            }
            EditorUtility.CopySerializedIfDifferent(oldCom, newCom);
            return newCom;
        }
        return null;
    }

    static string GetActorPrefabPath(string name)
    {
        string prefabName = System.IO.Path.ChangeExtension(name, "prefab");
        string subName = name.Substring(6);
        if (subName.StartsWith("Mon"))
        {
            return "Assets/Resources/Actors/Monsters/" + prefabName;
        }
        else if (subName.StartsWith("Bos"))
        {
            return "Assets/Resources/Actors/Monsters/" + prefabName;
        }
        //else if (subName.StartsWith("NPC")) // 暂时不用
        //{
        //}
        else // 勇士
        {
            return "Assets/Resources/Actors/Heroes/" + prefabName;
        }
    }

    static Transform FindChild(Transform root, string name)
    {
        for (int i = 0; i < root.childCount; ++i)
        {
            Transform child = root.GetChild(i);
            if (child.name == name)
            {
                return child;
            }
            else
            {
                Transform childOfChild = FindChild(child, name);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
        }
        return null;
    }
}


class SyncPrefabs : EditorWindow
{
    GameObject _srcPrefab;
    GameObject _dstPrefab;

    [MenuItem("工具/资源/Prefab/同步/PrefabA To PrefabB")]
    static void TryToSyncPrefab()
    {
        SyncPrefabs win = EditorWindow.GetWindow<SyncPrefabs>();
        win.Show();
    }

    void OnGUI()
    {
        _srcPrefab = EditorGUILayout.ObjectField("PrefabA", _srcPrefab, typeof(GameObject)) as GameObject;
        EditorGUILayout.PrefixLabel("同步到");
        _dstPrefab = EditorGUILayout.ObjectField("PrefabB", _dstPrefab, typeof(GameObject)) as GameObject;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if(GUILayout.Button("开始同步", GUILayout.MaxHeight(40)))
        {
            string pathSrc = AssetDatabase.GetAssetPath(_srcPrefab);
            string pathDst = AssetDatabase.GetAssetPath(_dstPrefab);
            if (!pathSrc.EndsWith(".prefab") || !pathSrc.EndsWith(".prefab"))
            {
                ShowNotification(new GUIContent("选择的对象不全是Prefab，请重新选择"));
                return;
            }

            GameObject fromObj = GameObject.Instantiate<GameObject>(_srcPrefab);
            GameObject toObj = GameObject.Instantiate<GameObject>(_dstPrefab);
            CreateActorPrefab.SyncActorPrefab(fromObj, toObj);
            GameObject newPrefab = PrefabUtility.ReplacePrefab(toObj, _dstPrefab);
            DestroyImmediate(fromObj);
            DestroyImmediate(toObj);
            AssetDatabase.SaveAssets();
            AssetsUtility.SelectAndHighLightObject(newPrefab);
            ShowNotification(new GUIContent("同步完成"));
        }
    }
}