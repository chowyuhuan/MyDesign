using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;
using System.Collections;

public class AM_UITexPackInfo {
    public string AssetPath{get; protected set;}

    public string PackTag { get; protected set; }
    public SpriteImportMode ImportMode { get; protected set; }
    public bool MultipleSpriteTex { get; protected set; }

    public AM_UITexPackInfo(string assetPath, string packTag, SpriteImportMode importMode)
    {
        AssetPath = assetPath;
        PackTag = AM_EditorTool.ParseAtlasName(packTag);
        ImportMode = importMode;
        MultipleSpriteTex = (ImportMode == SpriteImportMode.Multiple);
    }

    public bool PackedInTexture()
    {
        return !string.IsNullOrEmpty(PackTag);
    }

    public string GetPackedTextureName()
    {
        return PackTag;
    }

    public bool PackedInAtlas()
    {
        bool packed = false;
        switch(ImportMode)
        {
            case SpriteImportMode.Single:
            case SpriteImportMode.Polygon:
                {
                    packed = !string.IsNullOrEmpty(PackTag);
                    break;
                }
            case SpriteImportMode.Multiple:
                {
                    packed = true;
                    break;
                }
        }
        return packed;
    }

    public string GetAtlasFullPathWithoutExtension()
    {
        string altasPath = null;
        if(PackedInAtlas())
        {
            altasPath = "Assets/Resources/GUI/UIAtlas/" + GetAtlasName();
        }
        return altasPath;
    }

    public string GetAtlasFullPathWithExtension()
    {
        string altasPath = null;
        if (PackedInAtlas())
        {
            altasPath = "Assets/Resources/GUI/UIAtlas/" + GetAtlasName() + ".prefab";
        }
        return altasPath;
    }

    public string GetAtlasName()
    {
        string atlasName = null;
        switch(ImportMode)
        {
            case SpriteImportMode.Single:
            case SpriteImportMode.Polygon:
                {
                    atlasName = PackTag;//普通单图片资源，使用packtag作为图集名
                    break;
                }
            case SpriteImportMode.Multiple:
                {
                    atlasName = System.IO.Path.GetFileNameWithoutExtension(AssetPath);//多sprite图片，图集名与图片名相同
                    break;
                }
        }
        return atlasName;
    }

    public string GetAtlasConfigPath()
    {
        string configPath = null;
        switch (ImportMode)
        {
            case SpriteImportMode.Single:
            case SpriteImportMode.Polygon:
                {
                    configPath = "GUI/UIAtlas/" + PackTag;
                    break;
                }
            case SpriteImportMode.Multiple:
                {
                    configPath = "GUI/UIAtlas/" + System.IO.Path.GetFileNameWithoutExtension(AssetPath);
                    break;
                }
        }
        return configPath;
    }

    public string GetABName()
    {
        string abName = null;
        if(PackedInTexture())
        {
            abName = "Assets/Resources/GUI/UIAtlas/" + PackTag;
        }
        else if (MultipleSpriteTex)
        {
            abName = "Assets/Resources/GUI/UIAtlas/" + System.IO.Path.GetFileNameWithoutExtension(AssetPath);
        }
        else
        {
            abName = "Assets/Resources/GUI/UITexture/" + System.IO.Path.GetFileNameWithoutExtension(AssetPath);
        }
        return abName.ToLower();
    }
}
