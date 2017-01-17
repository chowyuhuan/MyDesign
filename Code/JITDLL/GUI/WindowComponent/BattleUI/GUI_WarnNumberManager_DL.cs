using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUI_WarnNumberManager_DL : MonoBehaviour
{
    Dictionary<EWarnNumberType, GUI_LogicObjectPool> _ComradeWarnNumberPool = new Dictionary<EWarnNumberType, GUI_LogicObjectPool>();
    Dictionary<EWarnNumberType, GUI_LogicObjectPool> _EnemyWarnNumberPool = new Dictionary<EWarnNumberType, GUI_LogicObjectPool>();
    public static GUI_WarnNumberManager_DL Instance;
    bool _InitDone = false;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        GameObject critProto_Comrade = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/WarnNumber_Crit", true, AssetManage.E_AssetType.UIPrefab);
        GameObject damageProto_Comrade = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/WarnNumber_Damage", true, AssetManage.E_AssetType.UIPrefab);
        GameObject cureProto_Comrade = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/WarnNumber_Cure", true, AssetManage.E_AssetType.UIPrefab);
        GameObject critProto_Enemy = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Enemy_WarnNumber_Crit", true, AssetManage.E_AssetType.UIPrefab);
        GameObject damageProto_Enemy = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Enemy_WarnNumber_Damage", true, AssetManage.E_AssetType.UIPrefab);
        GameObject cureProto_Enemey = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Enemy_WarnNumber_Cure", true, AssetManage.E_AssetType.UIPrefab);
        _ComradeWarnNumberPool[EWarnNumberType.Crit] = new GUI_LogicObjectPool(critProto_Comrade);
        _ComradeWarnNumberPool[EWarnNumberType.Damage] = new GUI_LogicObjectPool(damageProto_Comrade);
        _ComradeWarnNumberPool[EWarnNumberType.Cure] = new GUI_LogicObjectPool(cureProto_Comrade);
        _EnemyWarnNumberPool[EWarnNumberType.Crit] = new GUI_LogicObjectPool(critProto_Enemy);
        _EnemyWarnNumberPool[EWarnNumberType.Damage] = new GUI_LogicObjectPool(damageProto_Enemy);
        _EnemyWarnNumberPool[EWarnNumberType.Cure] = new GUI_LogicObjectPool(cureProto_Enemey);
    }

    public GUI_WarnNumber_DL WarnNumber(Actor target, EWarnNumberType type, int number, SKILL.Camp camp, int sortOrder)
    {
#if UNITY_EDITOR
        Debug.Assert(null != target);
        Debug.Assert(number >= 0);
#endif
        GUI_WarnNumber_DL wn;
        if (camp == SKILL.Camp.Comrade)
        {
            wn = _ComradeWarnNumberPool[type].GetOneLogicComponent() as GUI_WarnNumber_DL;
        }
        else
        {
            wn = _EnemyWarnNumberPool[type].GetOneLogicComponent() as GUI_WarnNumber_DL;
        }
        wn.WarnNumber(target, number, camp, type, sortOrder);
        return wn;
    }
    public void ClearWarning()
    {
        _ComradeWarnNumberPool[EWarnNumberType.Crit].RecycleAll();
        _ComradeWarnNumberPool[EWarnNumberType.Damage].RecycleAll();
        _ComradeWarnNumberPool[EWarnNumberType.Cure].RecycleAll();
        _EnemyWarnNumberPool[EWarnNumberType.Crit].RecycleAll();
        _EnemyWarnNumberPool[EWarnNumberType.Damage].RecycleAll();
        _EnemyWarnNumberPool[EWarnNumberType.Cure].RecycleAll();
    }
}