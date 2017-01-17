using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_LevelBonusItem_DL : MonoBehaviour
{
    public GameObject ItemRoot;
    public Image ItemIcon;
    public Image SchoolIcon;
    public GameObject ItemStarRoot;
    public Text ItemStarNum;
    public Text ItemCount;
    public Image BoxIcon;

    DataCenter.DropChestInfo _DropChestInfo;
    CSV_c_treasure_chest _ChestTemplate;

    public void SetItem(DataCenter.DropChestInfo dropChestInfo)
    {
        if (null != dropChestInfo)
        {
            _DropChestInfo = dropChestInfo;
            SetChestIcon();
        }
        else
        {
            ItemRoot.SetActive(false);
        }
    }

    void SetChestIcon()
    {
        ItemRoot.SetActive(false);
        BoxIcon.gameObject.SetActive(true);
        _ChestTemplate = CSV_c_treasure_chest.FindData((int)_DropChestInfo.ChestType);
        if (null != _ChestTemplate)
        {
            GUI_Tools.IconTool.SetIcon(_ChestTemplate.IconAtlas, _ChestTemplate.IconSprite, BoxIcon);
        }
    }

    public void CheckChestOpenOperation()
    {
        if (null != _ChestTemplate)
        {
            if (_ChestTemplate.AutoOpen == 1)
            {
                GUI_MessageManager.Instance.ShowErrorTip("开启宝箱");
                BoxIcon.gameObject.SetActive(false);
                ItemRoot.SetActive(true);
                DisplayItem();
            }
        }
    }

    void DisplayItem()
    {
        switch (_DropChestInfo.AwardType)
        {
            case PbCommon.EAwardType.E_Award_Type_Gold_Coin:
                {
                    DisplayCountableItem((int)PbCommon.EAwardType.E_Award_Type_Gold_Coin);
                    break;
                }
            case PbCommon.EAwardType.E_Award_Map_Piece:
                {
                    DisplayCountableItem((int)PbCommon.EAwardType.E_Award_Map_Piece);
                    break;
                }
            case PbCommon.EAwardType.E_Award_Type_Bread:
                {
                    DisplayBreadItem((int)PbCommon.EAwardType.E_Award_Type_Bread);
                    break;
                }
        }
    }

    void DisplayCountableItem(int itemTypeId)
    {
        CSV_c_item_descripion itemDes = CSV_c_item_descripion.FindData(itemTypeId);
        if (null != itemDes)
        {
            DisplayItem(itemDes.IconAtlas, itemDes.IconSprite, 0, (int)_DropChestInfo.AwardValue);
        }
    }

    void DisplayBreadItem(int breadId, int breadCount = 1)
    {
        CSV_b_bread_template breadItem = CSV_b_bread_template.FindData(breadId);
        if (null != breadItem)
        {
            DisplayItem(breadItem.IconAtlas, breadItem.BreadIcon, breadItem.Star, breadCount);
        }
    }

    void DisplayItem(string iconAtlas, string iconSprite, int starNum, int itemCount)
    {
        GUI_Tools.IconTool.SetIcon(iconAtlas, iconSprite, ItemIcon);
        if (starNum > 0)
        {
            ItemStarRoot.SetActive(true);
            ItemStarNum.text = starNum.ToString();
        }
        else
        {
            ItemStarRoot.SetActive(false);
            ItemStarNum.text = "";
        }
        ItemCount.text = itemCount.ToString();
    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_LevelBonusItem dataComponent = gameObject.GetComponent<GUI_LevelBonusItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_LevelBonusItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            ItemRoot = dataComponent.ItemRoot;
            ItemIcon = dataComponent.ItemIcon;
            SchoolIcon = dataComponent.SchoolIcon;
            ItemStarRoot = dataComponent.ItemStarRoot;
            ItemStarNum = dataComponent.ItemStarNum;
            ItemCount = dataComponent.ItemCount;
            BoxIcon = dataComponent.BoxIcon;
        }
    }
}
