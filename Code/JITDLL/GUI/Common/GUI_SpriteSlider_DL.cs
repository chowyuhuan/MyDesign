using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class GUI_SpriteSlider_DL : MonoBehaviour
{
    public SpriteRenderer FillSprite;

    [Range(0f, 1f)]
    public float _Value = 1f;
    public float Value
    {
        get
        {
            return _Value;
        }
        set
        {
            _Value = value;
            ValueChange();
        }
    }

    Rect _InitRect;
    Vector2[] _Vertices = new Vector2[4];
    ushort[] _Triangles = new ushort[6];
    void Awake()
    {
        CopyDataFromDataScript();

        ushort[] _Triangles = new ushort[6];
        _InitRect = new Rect(FillSprite.sprite.rect);
        _Vertices[0] = new Vector2(0, 0);
        _Vertices[1] = new Vector2(0, _InitRect.size.y);
        _Vertices[2] = new Vector2(_InitRect.width, _InitRect.size.y);
        _Vertices[3] = new Vector2(_InitRect.width, 0);

        Sprite sp = FillSprite.sprite;
        FillSprite.sprite = Sprite.Create(sp.texture, sp.rect, new Vector2(0.5f, 0.5f), sp.pixelsPerUnit);

    }

#if UNITY_EDITOR
    void Update()
    {
        ValueChange();
    }
#endif

    void ValueChange()
    {
        float curWidth = _InitRect.size.x * Value;
        _Vertices[0].Set(0, 0);
        _Vertices[1].Set(0, _InitRect.size.y);
        _Vertices[2].Set(curWidth, _InitRect.size.y);
        _Vertices[3].Set(curWidth, 0);



        _Triangles[0] = 1;
        _Triangles[1] = 3;
        _Triangles[2] = 0;

        _Triangles[3] = 2;
        _Triangles[4] = 3;
        _Triangles[5] = 1;

        // set sprite vertices
        FillSprite.sprite.OverrideGeometry(_Vertices, _Triangles);
    }
    protected void CopyDataFromDataScript()
    {
        GUI_SpriteSlider dataComponent = gameObject.GetComponent<GUI_SpriteSlider>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_SpriteSlider,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            FillSprite = dataComponent.FillSprite;
            _Value = dataComponent._Value;
        }
    }
}
