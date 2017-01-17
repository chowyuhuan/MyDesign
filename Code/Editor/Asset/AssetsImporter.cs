using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// 规范化资源
/// </summary>
public class AssetsImporter : AssetPostprocessor
{

    static bool _displayDialog = true;
    void OnPreprocessModel()
    {
        ModelImporter model = assetImporter as ModelImporter;
        string name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        if (name.StartsWith("Actor_"))
        {
            model.animationType = ModelImporterAnimationType.Generic;
            bool anim = assetPath.Contains("@");
            model.importMaterials = anim ? false : true;
            model.importAnimation = anim ? true : false;
            model.optimizeGameObjects = anim ? true : model.optimizeGameObjects;
        }
        else
        {
            model.optimizeGameObjects = true;
        }

        model.motionNodeName = null;
        model.isReadable = false;
        model.optimizeMesh = true;
        model.animationCompression = ModelImporterAnimationCompression.Optimal;
        model.meshCompression = ModelImporterMeshCompression.High;
        model.addCollider = false;
    }

    //void OnPreprocessAnimation()
    //{
    //}

    //void OnPostprocessModel(GameObject gameObject)
    //{
    //    // 在导入动画时，调用OnPostprocessModel时，AnimationClip还并没有被加载。且在5.4版本中，gameObject上也没有Animation组件，就不能通过Animation间接访问到AnimationClip。
    //    // 所以，本应放在这里的处理逻辑移到OnPostprocessAllAssets了

    //    // OnPostprocessAllAssets在OnPostprocessModel后面执行，所以，如果需要获取OnPostprocessAllAssets处理之后的资源，就不能使用这个接口

    //    //!!!这里传入的gameObject并不是完全体，就像上面描述的，AnimationClip并没有被加载，同时测试发现，mesh也没有加载，是空
    //}

    void OnPreprocessTexture()
    {
        if (assetPath.Contains(".fbm"))
        {
            if (_displayDialog && !EditorUtility.DisplayDialog("警告", "贴图：" + assetPath + "\n此贴图为模型嵌入的贴图，模型禁止嵌入贴图\n请重新设置模型导出选项并重新导入", "确定", "不再提示"))
            {
                _displayDialog = false;
            }
            Debug.LogError("贴图：" + assetPath + "\n此贴图为模型嵌入的贴图，模型禁止嵌入贴图\n请重新设置模型导出选项并重新导入");
        }
        TextureImporter texImporter = assetImporter as TextureImporter;
        texImporter.isReadable = false;
        texImporter.mipmapEnabled = false;
        CheckMobileTexture(assetPath, texImporter, "Android", false);
        CheckMobileTexture(assetPath, texImporter, "iPhone", true);
    }

    //// return null的话，就会交给Unity来自动完成
    //// 本项目：
    ////  1.材质、贴图名字为模型名字，仅仅是后缀不一样
    ////  2.材质、贴图的存放位置为模型位置，不单独建立Materials文件夹存储材质（单独建立Materials为Unity默认行为，这里屏蔽）
    //Material OnAssignMaterialModel(Material mat, Renderer render)
    //{
    //    string name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
    //    if (name.IndexOf("Actor_") == 0)
    //    {
    //        string directory = System.IO.Path.GetDirectoryName(assetPath);
    //        //string materialPath = directory + "/Materials/" + mat.name + ".mat"; // Unity默认放在新建的Materials文件夹下，现在直接放和其他资源放在同级文件夹下了
    //        string assetName = directory + "/" + name;
    //        Material temp = AssetDatabase.LoadAssetAtPath<Material>(assetName + ".mat");
    //        if (temp)
    //            return temp;
    //        mat.name = name;
    //        mat.shader = Shader.Find("Unlit/Actor_CastShadow");
    //        mat.color = Color.clear;
    //        mat.mainTexture = AssetDatabase.LoadAssetAtPath<Texture>(assetName + ".tga");
    //        //AssetDatabase.CreateFolder(directory, "Materials"); // Unity默认放在新建的Materials文件夹下，现在直接放和其他资源放在同级文件夹下了
    //        AssetDatabase.CreateAsset(mat, assetName + ".mat");
    //        return mat;
    //    }
    //    return null;
    //}

    //void OnPostprocessTexture(Texture2D texture)
    //{
    //}

    //void OnPostprocessGameObjectWithUserProperties (GameObject go, string[] propNames, System.Object[] values)
    //{
    //    Debug.LogError(2);
    //    for (int i = 0; i < propNames.Length; i++)
    //    {
    //        string propName = propNames[i];
    //        System.Object value = (System.Object)values[i];

    //        Debug.Log("Propname: " + propName + " value: " + values[i]);

    //        if (value.GetType().ToString() == "System.Int32")
    //        {
    //            int myInt = (int)value;
    //            // do something useful
    //        }

    //        // etc...
    //    }
    //}

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        bool handle = false;
        for (int i = 0; i < importedAssets.Length; ++i)
        {
            string assetPath = importedAssets[i];
            if (assetPath.EndsWith(".FBX"))
            {
                handle |= ReprocessFBX(assetPath);
            }
            else if (assetPath.EndsWith(".prefab"))
            {
                handle |= ReprocessPrefab(assetPath);
            }
            //else if (assetPath.EndsWith(".anim"))
            //{
            //    handle |= ReprocessAnimationClip(assetPath);
            //}
        }

        if (handle)
        {
            //AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }


    static bool ReprocessFBX(string assetPath)
    {
        //if (assetPath.Contains("@"))
        //{
        //    return ReprocessAnimFBX(assetPath);
        //}
        return false;
    }

    static bool ReprocessAnimFBX(string assetPath)
    {
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
        AnimationClip newClip = CurvesTransferer.Transfer(clip, false);
        return true;
    }

    static bool ReprocessPrefab(string assetPath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if (prefab == null)
        {
            return false;
        }
        bool handle = false;
        handle |= ReprocessRenderer(prefab);
        handle |= ReprocessAnimation(prefab);
        handle |= ReprocessActorPrefab(prefab);
        handle |= ReprocessBulletPrefab(prefab);
        handle |= ReprocessWeaponPrefab(prefab);
        return handle;
    }

    static bool ReprocessActorPrefab(GameObject prefab)
    {
        if (prefab.name.StartsWith("Actor_"))
        {
            bool dirty = false;
            BoxCollider bcol = prefab.GetComponent<BoxCollider>();
            if (bcol == null)
            {
                bcol = prefab.AddComponent<BoxCollider>();
                bcol.isTrigger = true;
                dirty = true;
            }
            else if (!bcol.isTrigger)
            {
                bcol.isTrigger = true;
                dirty = true;
            }
            Rigidbody brig = prefab.GetComponent<Rigidbody>();
            if (brig == null)
            {
                brig = prefab.AddComponent<Rigidbody>();
                brig.isKinematic = true;
                brig.useGravity = false;
                dirty = true;
            }
            else
            {
                if (!brig.isKinematic)
                {
                    brig.isKinematic = true;
                    dirty = true;
                }
                if (brig.useGravity)
                {
                    brig.useGravity = false;
                    dirty = true;
                }
            }
            ActorPreDefine define = prefab.GetComponent<ActorPreDefine>();
            if (define == null)
            {
                define = prefab.AddComponent<ActorPreDefine>();
                dirty = true;
            }
            if (define.AtkTransform == null)
            {
                Transform rhand = FindChild(prefab.transform, "Bip001 Prop1");
                if (rhand != null)
                {
                    define.AtkTransform = rhand;
                    dirty = true;
                }
            }

            if (dirty)
            {
                EditorUtility.SetDirty(prefab);
                return true;
            }
        }
        return false;
    }

    static bool ReprocessBulletPrefab(GameObject prefab)
    {
        if (prefab.name.StartsWith("EffectB_"))
        {
            return ReprocessCollidePrefab(prefab);
        }
        return false;
    }

    static bool ReprocessWeaponPrefab(GameObject prefab)
    {
        if (prefab.name.StartsWith("Weapon_"))
        {
            return ReprocessCollidePrefab(prefab);
        }
        return false;
    }

    static bool ReprocessCollidePrefab(GameObject prefab)
    {
        if (prefab != null)
        {
            bool dirty = false;
            BoxCollider bcol = prefab.GetComponent<BoxCollider>();
            if (bcol == null)
            {
                bcol = prefab.AddComponent<BoxCollider>();
                bcol.isTrigger = true;
                dirty = true;
            }
            else if (!bcol.isTrigger)
            {
                bcol.isTrigger = true;
                dirty = true;
            }
            Rigidbody brig = prefab.GetComponent<Rigidbody>();
            if (brig == null)
            {
                brig = prefab.AddComponent<Rigidbody>();
                brig.isKinematic = true;
                dirty = true;
            }
            else if (!brig.isKinematic)
            {
                brig.isKinematic = true;
                dirty = true;
            }

            if (dirty)
            {
                EditorUtility.SetDirty(prefab);
                return true;
            }
        }
        return false;
    }


    static bool ReprocessRenderer(GameObject obj)
    {
        bool bActor = obj.name.StartsWith("Actor_");
        bool dirty = false;
        Renderer[] renders = obj.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renders.Length; ++i)
        {
            Renderer temp = renders[i];
            if (!bActor && temp.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off)
            {
                temp.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                dirty = true;
            }
            if (temp.lightProbeUsage != UnityEngine.Rendering.LightProbeUsage.Off)
            {
                temp.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                dirty = true;
            }
            if (temp.motionVectors)
            {
                temp.motionVectors = false;
                dirty = true;
            }
            if (temp.name.Contains("dibiao"))
            {
                if (!temp.receiveShadows)
                {
                    temp.receiveShadows = true;
                    dirty = true;
                }
            }
            else if (temp.receiveShadows)
            {
                temp.receiveShadows = false;
                dirty = true;
            }
            if (temp.reflectionProbeUsage != UnityEngine.Rendering.ReflectionProbeUsage.Off)
            {
                temp.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                dirty = true;
            }

            SkinnedMeshRenderer skin = temp as SkinnedMeshRenderer;
            if (skin == null)
            {
                continue;
            }
            //skin.quality = SkinQuality.Auto; // TODO:视效率而定
            if (skin.skinnedMotionVectors)
            {
                skin.skinnedMotionVectors = false;
                dirty = true;
            }
            if (skin.updateWhenOffscreen)
            {
                skin.updateWhenOffscreen = false;
                dirty = true;
            }
        }
        if (dirty)
        {
            EditorUtility.SetDirty(obj);
            return true;
        }
        return false;
    }

    static bool ReprocessAnimation(GameObject obj)
    {
        bool dirty = false;
        Animation[] anims = obj.GetComponentsInChildren<Animation>();
        for (int i = 0; i < anims.Length; ++i)
        {
            Animation temp = anims[i];
            if (temp.cullingType != AnimationCullingType.BasedOnRenderers)
            {
                temp.cullingType = AnimationCullingType.BasedOnRenderers;
                dirty = true;
            }
            // 因为.anim在导入时，必然通过".anim"的判断和CheckAnimationClip检查过了，prefab上的仅仅是对他们的引用，这里就不检查了
        }

        Animator[] animors = obj.GetComponentsInChildren<Animator>();
        for (int i = 0; i < animors.Length; ++i)
        {
            Animator temp = animors[i];
            if (temp.cullingMode != AnimatorCullingMode.CullUpdateTransforms)
            {
                temp.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
                dirty = true;
            }
        }
        if (dirty)
        {
            EditorUtility.SetDirty(obj);
            return true;
        }
        return false;
    }

    static bool ReprocessAnimationClip(string assetPath)
    {
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
        if (clip == null)
        {
            return false;
        }
        if (clip.name.StartsWith("skill") || clip.name.StartsWith("attack"))
        {
            int count = 0;
            for (int i = 0; i < clip.events.Length; ++i)
            {
                if (string.IsNullOrEmpty(clip.events[i].functionName))
                {
                    ++count;
                }
            }
            if (count < 2)
            {
                if (_displayDialog && !EditorUtility.DisplayDialog("警告", "动画：" + AssetDatabase.GetAssetPath(clip.GetInstanceID()) + "\n技能类动作需要至少两个 空Event 标记，请添加Event标记", "确定", "不再提示"))
                {
                    _displayDialog = false;
                }
                AssetsUtility.SelectAndHighLightObject(clip);
                Debug.LogError("动画：" + AssetDatabase.GetAssetPath(clip.GetInstanceID()) + "\n技能类动作需要至少两个 空Event 标记，请添加Event标记", clip);
            }
        }
        return true;
    }

    // 平台参数："Web", "Standalone", "iPhone" and "Android"，来自官方文档
    void CheckMobileTexture(string assetPath, TextureImporter importer, string platform, bool notice)
    {
        int maxSize;
        TextureImporterFormat format;
        if (!importer.GetPlatformTextureSettings(platform, out maxSize, out format))
        {
            maxSize = importer.maxTextureSize;
            format = importer.textureFormat;
        }

        if (assetPath.Contains("Resources_Editor/GUI/UITexture/")) // UI
        {
            if (maxSize > 1024)
            {
                if (notice)
                {
                    if (_displayDialog && !EditorUtility.DisplayDialog("警告", "贴图：" + assetPath + "\n移动端UI贴图最大为1024，您设置的是" + maxSize + "，如果特殊需求使用此大小，请联系程序", "确定", "不再提示"))
                    {
                        _displayDialog = false;
                    }
                    AssetsUtility.SelectAndHighLightObject(assetPath);
                    Debug.LogError("贴图：" + assetPath + "\nUI贴图最大为1024，您设置的是" + maxSize + "，如果特殊需求使用此大小，请联系程序");
                }
                importer.SetPlatformTextureSettings(platform, 1024, format);
            }
        }
        else if (assetPath.Contains("Scenes/SceneResources")) // 场景
        {
            if (maxSize > 512)
            {
                if (notice)
                {
                    if (_displayDialog && !EditorUtility.DisplayDialog("警告", "贴图：" + assetPath + "\n场景贴图最大为512，您设置的是" + maxSize + "，如果特殊需求使用此大小，请联系程序", "确定", "不再提示"))
                    {
                        _displayDialog = false;
                    }
                    AssetsUtility.SelectAndHighLightObject(assetPath);
                    Debug.LogError("贴图：" + assetPath + "\n场景贴图最大为512，您设置的是" + maxSize + "，如果特殊需求使用此大小，请联系程序");
                }
                importer.SetPlatformTextureSettings(platform, 512, format);
            }
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
