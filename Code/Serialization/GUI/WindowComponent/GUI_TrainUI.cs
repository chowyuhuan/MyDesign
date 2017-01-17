using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_TrainUI : GUI_Window
{
    public Image HeroIcon;
    public Slider BigSuccess;
    public Text BigSuccessNum;

    public Text TrainStageNum;
    public Color TrainStageHighestColor;
    public Text CurrentTrainingNum;
    public Text CurrentAppendAttribute;
    public Text StageFinishAppendAttribute;
    public Color AppendAttributeMaxColor;
    public List<Image> _SelectedBreadIconList = new List<Image>();

    public GameObject ScrollView;
    public Text CurrentItemCount;

    public GameObject MulitpleStageSlider;


    public Button TrainButton;
    public Text TrainCost;
    public Button SellButton;
    public GameObject SellArea;

    public Button ConfirmSellButton;
    public Text SellItemValue;
    public Button SellAllButton;
    public Button CancelButton;
    public GameObject TrainArea;

    public GameObject AppendAttributeArea;
    public GameObject TrainValueArea;
    public Text TrainValue;
    public Text CurrentTrainStage;

    public GameObject MaxTrainStageMask;
    public Text FinalFinishedTrainValue;
    public GUI_TweenPosition FinalFinishedValuePosTweener;
    public GUI_TweenAlpha FinalFinishedValueAlphaTweener;
    public GameObject BigSuccessEffect;
    public GUI_TweenScale BigSucessEffectScaleTweener;
    public Text SuccessText;
    public List<GUI_Tweener> SuccessTextTweeners = new List<GUI_Tweener>();
    public GameObject SuccessProgressEffectRoot;
    public GameObject SuccessProgressEffectProt;
    public GUI_Transform SuccessProgressTrans = new GUI_Transform();

    public float TrainValueSimulateRate = 0.02f;

    public Button ExtendBreadBagButton;
    public Button SortItemTypeButton;
    public Button SortItemDirctionButton;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_TrainUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_TrainUI_DL>(gameObject, this);
#endif
    }
}