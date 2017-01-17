using UnityEngine;
using System.Collections;

public class EvolutionHelper : MonoBehaviour {
    public static DataCenter.Hero EvolutionHero;
    public static CSV_b_hero_template CurrentHeroTemplate;
    public bool UseAnimationTime;
    public float HeroAnimationStartDelay = 0.2f;
    public float CurHeroEnterAnimationTime;
    public float EvolutionAnimationTime;
    public float EvolutionHeroAnimationTime;
    public string CurrentHeroEnterAnimation;
    public string CurrentHeroEvolutionAnimation;
    public string EvolutionHeroAnimation;
    public string EvolutionDoneAnimation;
    public GameObject EvolutionEffect;

    public GUI_Transform HeroTrans;
    public GameObject HeroSpawnRoot;
    GameObject _Hero;
    Animator _Anim;

    public float EvolutionSuccessUIShowDelay;

    void Start()
    {
        GUI_Root_DL.Instance.ShowLayer("Default");
        //EvolutionEffect.SetActive(false);
        GUI_Tools.ModelTool.SpawnModel(HeroSpawnRoot, CurrentHeroTemplate.Prefab, HeroTrans, out _Hero);
        ActorWeaponInfo actorWeaponInfo = EvolutionHero.GetActorWeaponInfo();
        ActorWeaponHelper.SetActorWeapon(_Hero, actorWeaponInfo);
        StartCoroutine("DelayAnim");
        Invoke("ShowEvolutionSuccessUI", EvolutionSuccessUIShowDelay);
    }
    float delay = 0f;
    IEnumerator DelayAnim()
    {
        while (delay < HeroAnimationStartDelay)
        {
            ++delay;
            yield return null;
        }
        Animator anim;
        if (GUI_Tools.ModelTool.AnimateEvolutionModel(_Hero, CurrentHeroTemplate.EvoAnimCtrl, out anim))
        {
            anim.Play(CurrentHeroEnterAnimation);
            if (UseAnimationTime)
            {
                Invoke("OnCurrentHeroEnterAnimationEnd", anim.GetCurrentAnimatorStateInfo(0).length);
            }
            else
            {
                Invoke("OnCurrentHeroEnterAnimationEnd", CurHeroEnterAnimationTime);
            }
        }
    }

    void OnCurrentHeroEnterAnimationEnd()
    {
        //EvolutionEffect.SetActive(true);
        Animator anim = _Hero.GetComponent<Animator>();
        if (null != anim)
        {
            anim.Play(CurrentHeroEvolutionAnimation);
            if (UseAnimationTime)
            {
                Invoke("OnEvolutionAnimationEnd", anim.GetCurrentAnimatorStateInfo(0).length);
            }
            else
            {
                Invoke("OnEvolutionAnimationEnd", EvolutionAnimationTime);
            }
        }
    }

    void OnEvolutionAnimationEnd()
    {
        //EvolutionEffect.SetActive(false);
        GameObject.Destroy(_Hero);
        CSV_b_hero_template evolutionTemplate = CSV_b_hero_template.FindData(EvolutionHero.CsvId);
        GUI_Tools.ModelTool.SpawnModel(HeroSpawnRoot, evolutionTemplate.Prefab, HeroTrans, out _Hero);
        ActorWeaponInfo actorWeaponInfo = EvolutionHero.GetActorWeaponInfo();
        ActorWeaponHelper.SetActorWeapon(_Hero, actorWeaponInfo);
        Animator anim;
        if (GUI_Tools.ModelTool.AnimateEvolutionModel(_Hero, evolutionTemplate.EvoAnimCtrl, out anim))
        {
            anim.Play(EvolutionHeroAnimation);
            if (UseAnimationTime)
            {
                Invoke("OnEvolutionHeroAnimationEnd", anim.GetCurrentAnimatorStateInfo(0).length);
            }
            else
            {
                Invoke("OnEvolutionHeroAnimationEnd", EvolutionHeroAnimationTime);
            }
        }
    }

    void OnEvolutionHeroAnimationEnd()
    {
        Animator anim = _Hero.GetComponent<Animator>();
        if (null != anim)
        {
            anim.Play(EvolutionDoneAnimation);
        }
    }

    void ShowEvolutionSuccessUI()
    {
        GUI_Manager.Instance.ShowWindowWithName("GUI_EvolutionSuccess", false);
    }
}
