using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Building_DL : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public BuildingType SelfType = BuildingType.None;

    public static System.Action<BuildingType, GameObject> OnBuildingClick;

    static List<Building_DL> buildingList = new List<Building_DL>();

    bool _dragged = false;

    Renderer _renderer = null;
    Material[] _materials = null;

    #region jit init
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        Building dataComponent = gameObject.GetComponent<Building>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：Building,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            SelfType = dataComponent.SelfType;
        }
    }
    #endregion

    void OnEnable()
    {
        buildingList.Add(this);
    }

    void OnDisable()
    {
        buildingList.Remove(this);
    }

    // Use this for initialization
    void Start()
    {
        _renderer = GetComponent<Renderer>();

        if (_renderer != null)
        {
            _materials = _renderer.materials;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag " + name);
        _dragged = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag " + name);
        _dragged = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag " + name);
        Vector3 pos = Camera.main.transform.position;
        pos.x = pos.x - eventData.delta.x * DefaultConfig.GetFloat("MainCityCameraFactor");

        MoveCamera(pos);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown " + name);

        if (SelfType == BuildingType.None)
        {
            return;
        }

        if (_materials == null)
        {
            return;
        }

        for (int n = 0; n < _materials.Length; ++n)
        {
            string[] keys = _materials[n].shaderKeywords;
            for (int i = 0; i < keys.Length; ++i)
            {
                if (keys[i] == "EFFECTS_LAYER_1_OFF")
                    keys[i] = "EFFECTS_LAYER_1_ON";
            }

            _materials[n].shaderKeywords = keys;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp " + name);

        if (SelfType == BuildingType.None)
        {
            return;
        }

        if (_materials == null)
        {
            return;
        }

        for (int n = 0; n < _materials.Length; ++n)
        {
            string[] keys = _materials[n].shaderKeywords;
            for (int i = 0; i < keys.Length; ++i)
            {
                if (keys[i] == "EFFECTS_LAYER_1_ON")
                    keys[i] = "EFFECTS_LAYER_1_OFF";
            }

            _materials[n].shaderKeywords = keys;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick " + name);
        if (!_dragged)
        {
            if (SelfType != BuildingType.None)
            {
                Focus();
                
                if (OnBuildingClick != null)
                {
                    OnBuildingClick(SelfType, gameObject);
                }
            }
        }
    }

    void MoveCamera(Vector3 pos)
    {
        pos.x = Mathf.Max(pos.x, DefaultConfig.GetFloat("MainCityCameraLeft"));
        pos.x = Mathf.Min(pos.x, DefaultConfig.GetFloat("MainCityCameraRight"));
        SpringObject.Begin(Camera.main.gameObject, pos, 10);
    }

    void Focus()
    {
        //Debug.Log("Focus");
        Vector3 pos = Camera.main.transform.position;
        pos.x = transform.position.x;

        MoveCamera(pos);
    }

    public static void FocusBuilding(BuildingType buildingType)
    {
        for (int i = 0; i < buildingList.Count; ++i)
        {
            if (buildingList[i].SelfType == buildingType)
            {
                buildingList[i].Focus();
                return;
            }
        }
    }
}
