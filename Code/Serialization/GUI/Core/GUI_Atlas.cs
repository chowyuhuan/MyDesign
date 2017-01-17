using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GUI_Atlas : MonoBehaviour {
    public string _Name;
    public List<Sprite> _SpriteList = new List<Sprite>();
    protected Dictionary<string, int> _SpriteIndexDic = new Dictionary<string, int>();

    //在使用此类前执行初始化
    public void Init()
    {
        RefreshIndexDic();
    }

    private void RefreshIndexDic()
    {
        _SpriteIndexDic.Clear();
        for (int index = 0; index < _SpriteList.Count; ++index)
        {
            _SpriteIndexDic.Add(_SpriteList[index].name, index);
        }
    }

#if UNITY_EDITOR

    public void RemoveSpriteFromFullPath(string fullPath)
    {
        RefreshIndexDic();
        string name = System.IO.Path.GetFileNameWithoutExtension(fullPath);
        int index;
        if(_SpriteIndexDic.TryGetValue(name, out index))
        {
            _SpriteList.RemoveAt(index);
        }
    }

    public bool AddSprite(Sprite sprite)
    {
        RefreshIndexDic();
        if (null != sprite)
        {
            if (!_SpriteIndexDic.ContainsKey(sprite.name))
            {
                _SpriteIndexDic.Add(sprite.name, _SpriteList.Count);
                _SpriteList.Add(sprite);
                return true;
            }
            else
            {
                Debug.LogError("图集中已包含名为 " + sprite.name + " 的Sprite");
            }
        }
        return false;
    }
#endif

    public bool HasSprite(string spriteName)
    {
        return GetSprite(spriteName) != null;
    }

    public Sprite GetSprite(string spriteName)
    {
        int index;
        if (_SpriteIndexDic.TryGetValue(spriteName, out index))
        {
            return _SpriteList[index];
        }
        return null;
    }

    public Sprite GetSprite(int index)
    {
        if (index >= 0 && index < _SpriteList.Count)
        {
            return _SpriteList[index];
        }
        return null;
    }
}
