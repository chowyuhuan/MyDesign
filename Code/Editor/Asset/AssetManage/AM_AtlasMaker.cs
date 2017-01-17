using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AM_AtlasMaker : EditorWindow
{
    static AM_AtlasMaker _Instance;
    GUI_Atlas _CurrentEditorAtlas;
    bool _ValidAtlasFile = false;
    bool _EditingAtlasChanged = false;
    [MenuItem("工具/资源/图集/图集编辑器")]
    static void ShowAtlasMaker()
    {
        if(_Instance != null)
        {
            _Instance.Close();
        }
        AM_AtlasMaker maker = EditorWindow.GetWindow<AM_AtlasMaker>();
        maker.position = new Rect(300, 300, 720, 300);
        _Instance = maker;
        maker.CheckDefaultSelect();
    }

    void OnDestroy()
    {
        SaveCurrent();
        EditorUtility.UnloadUnusedAssetsImmediate();
        System.GC.Collect();
    }

    void OnGUI()
    {
        if (_ValidAtlasFile)
        {
            DrawCurrent();
        }
        else
        {
            DrawSelect();
            DrawNew();
        }
        DragDropEvent();
    }

    void DragDropEvent()
    {
        if (_ValidAtlasFile && Event.current.type == EventType.DragExited)
        {
            DragAndDrop.AcceptDrag();

            Object[] obs = DragAndDrop.objectReferences;
            for(int index = 0; index < obs.Length; ++index)
            {
                if(obs[index].GetType() == typeof(Sprite))
                {
                    Sprite sp = obs[index] as Sprite;
                    if(_CurrentEditorAtlas.AddSprite(sp))
                    {
                        _EditingAtlasChanged = true;
                    }
                }
            }
        }
    }

    void DrawSelect()
    {
        if (GUILayout.Button("打开"))
        {
            string ap = EditorUtility.OpenFilePanel("选择要编辑的Atlas文件", "Assets/Resources/GUI/UIAtlas", "prefab");
            ap = ap.Replace("\\", "/");
            ap = FileUtil.GetProjectRelativePath(ap);
            GUI_Atlas at;
            if(ValidPath(ap, out at))
            {
                if(_ValidAtlasFile && _EditingAtlasChanged)
                {
                    int option = EditorUtility.DisplayDialogComplex("提示", "您将要关闭当前编辑的图集", "保存", "不保存", "取消");
                    switch(option)
                    {
                        case 0:
                            {
                                SaveCurrent();
                                CloseCurrent();
                                SetEditingAtlas(at);
                                break;
                            }
                        case 1:
                            {
                                CloseCurrent();
                                SetEditingAtlas(at);
                                break;
                            }
                        default:
                            {
                                EditorUtility.UnloadUnusedAssetsImmediate(at);
                                break;
                            }
                    }
                    Debug.LogError(option);
                }
                else
                {
                    SetEditingAtlas(at);
                }
            }
        }
    }

    void DrawNew()
    {
        if(GUILayout.Button("新建"))
        {
            if(CancelCloseCurrent())
            {
                return;
            }
            Object ob = Selection.activeObject;
            string selectTexName = null;
            if(null != ob)
            {
                string obPath = AssetDatabase.GetAssetPath(ob);
                TextureImporter ti = TextureImporter.GetAtPath(obPath) as TextureImporter;
                if(null != ti)
                {
                    if(ti.spriteImportMode == SpriteImportMode.Multiple)
                    {
                        selectTexName = System.IO.Path.GetFileNameWithoutExtension(obPath);
                    }
                    EditorUtility.UnloadUnusedAssetsImmediate(ti);
                }
            }
            string ap = EditorUtility.SaveFilePanel("新建图集", "Assets/Resources/GUI/UIAtlas", string.IsNullOrEmpty(selectTexName) ? "atlas" : selectTexName, "prefab");
            if(!string.IsNullOrEmpty(ap))
            {
                ap = ap.Replace("\\", "/");
                ap = FileUtil.GetProjectRelativePath(ap);
                GUI_Atlas newat = AM_AtlasExporter.CreateEmptyGUIAtlas(ap);
                SetEditingAtlas(newat);
            }
        }
    }

    void SetEditingAtlas(GUI_Atlas at)
    {
        _EditingAtlasChanged = false;
        _CurrentEditorAtlas = at;
        _ValidAtlasFile = (null != _CurrentEditorAtlas);
        Debug.LogError(_ValidAtlasFile);
    }

    Vector2 _ScrollPos = Vector2.zero;
    void DrawCurrent()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(_CurrentEditorAtlas._Name);
        if(GUILayout.Button("保存"))
        {
            SaveCurrent();
        }
        DrawSelect();
        DrawNew();
        EditorGUILayout.EndHorizontal();
        if(_CurrentEditorAtlas._SpriteList.Count > 0)
        {
            _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos); 
            EditorGUILayout.BeginVertical();
            for (int index = 0; index < _CurrentEditorAtlas._SpriteList.Count; )
            {
                if (!DrawSprite(index))
                {
                    ++index;
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        else
        {
            if(GUILayout.Button("+"))
            {
                _CurrentEditorAtlas._SpriteList.Insert(0, null);
            }
        }
    }

    bool DrawSprite(int index)
    {
        bool deleteSp = false;
        EditorGUILayout.BeginHorizontal(GUILayout.Width(700));
        EditorGUILayout.LabelField(AssetDatabase.GetAssetPath(_CurrentEditorAtlas._SpriteList[index]), GUILayout.Width(440));
        _CurrentEditorAtlas._SpriteList[index] = EditorGUILayout.ObjectField(_CurrentEditorAtlas._SpriteList[index], typeof(Sprite), false, GUILayout.Width(200f)) as Sprite;
        if(GUILayout.Button("-"))
        {
            _CurrentEditorAtlas._SpriteList.RemoveAt(index);
            deleteSp = true;
            _EditingAtlasChanged = true;
        }
        if(GUILayout.Button("+"))
        {
            _CurrentEditorAtlas._SpriteList.Insert(index, null);
            _EditingAtlasChanged = true;
        }
        EditorGUILayout.EndHorizontal();
        return deleteSp;
    }

    void CheckDefaultSelect()
    {
        Object ob = Selection.activeObject;
        if(null != ob)
        {
            string ap = AssetDatabase.GetAssetPath(ob);
            _ValidAtlasFile = ValidPath(ap, out _CurrentEditorAtlas);
        }
    }

    bool ValidPath(string ap, out GUI_Atlas at)
    {
        bool valid = false;
        at = AssetDatabase.LoadAssetAtPath<GUI_Atlas>(ap);
        valid = (null != at);
        return valid;
    }

    bool CancelCloseCurrent()
    {
        bool optionCancel = false;
        if(_ValidAtlasFile && _EditingAtlasChanged)
        {
            int option = EditorUtility.DisplayDialogComplex("提示", "您将要关闭当前编辑的图集", "保存", "不保存", "取消");
            switch (option)
            {
                case 0:
                    {
                        SaveCurrent();
                        CloseCurrent();
                        break;
                    }
                case 1:
                    {
                        CloseCurrent();
                        break;
                    }
                default:
                    {
                        optionCancel = true;
                        break;
                    }
            }
        }
        return optionCancel;
    }

    void SaveCurrent()
    {
        if(_ValidAtlasFile)
        {
            RemoveEmpty();
            EditorUtility.SetDirty(_CurrentEditorAtlas);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            _EditingAtlasChanged = false;
        }
    }

    void CloseCurrent()
    {
        if(_ValidAtlasFile)
        {
            EditorUtility.UnloadUnusedAssetsImmediate(_CurrentEditorAtlas);
            AssetDatabase.Refresh();
            _CurrentEditorAtlas = null;
            _ValidAtlasFile = false;
            _EditingAtlasChanged = false;
        }        
    }

    void RemoveEmpty()
    {
        if(null != _CurrentEditorAtlas)
        {
            for(int index = 0; index < _CurrentEditorAtlas._SpriteList.Count ;)
            {
                if(_CurrentEditorAtlas._SpriteList[index] == null)
                {
                    _CurrentEditorAtlas._SpriteList.RemoveAt(index);
                }
                else
                {
                    ++index;
                }
            }
        }
    }
}
