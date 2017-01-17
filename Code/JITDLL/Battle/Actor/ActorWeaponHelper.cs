using UnityEngine;
using System.Collections;

public class ActorWeaponInfo
{
    public string prefabPath;
    public Vector3 LocalPosition;
    public Vector3 LocalRotation;
}

public class ActorWeaponHelper
{
    public static void SetActorWeapon(GameObject actorGo, ActorWeaponInfo weaponInfo)
    {
        ActorPreDefine actorPreDefine = actorGo.GetComponent<ActorPreDefine>();
        GameObject weapon = EntityPool.Spwan(weaponInfo.prefabPath) as GameObject;
        if (weapon != null)
        {
            SetWeapon(actorPreDefine.AtkTransform, weapon.transform, weaponInfo.LocalPosition, weaponInfo.LocalRotation);
        }
    }

    public static GameObject SpawnActorWeaponWithUIScale(Transform scaleParent, GUI_Transform modelTrans, ActorWeaponInfo weaponInfo)
    {
        GameObject weapon = EntityPool.Spwan(weaponInfo.prefabPath) as GameObject;
        Vector3 originalScal = weapon.transform.localScale;
        weapon.transform.parent = scaleParent;
        weapon.transform.localScale = new Vector3(originalScal.x * modelTrans.Scale.x, originalScal.y * modelTrans.Scale.y, originalScal.z * modelTrans.Scale.z);

        return weapon;
    }

    public static void SetActorWeapon(GameObject actorGo, GameObject weapon, ActorWeaponInfo weaponInfo)
    {
        ActorPreDefine actorPreDefine = actorGo.GetComponent<ActorPreDefine>();
        if (weapon != null)
        {
            SetWeapon(actorPreDefine.AtkTransform, weapon.transform, weaponInfo.LocalPosition, weaponInfo.LocalRotation);
        }
    }

    static void SetWeapon(Transform root, Transform weapon, Vector3 localPosition, Vector3 localRotation)
    {
        weapon.parent = root;
        weapon.localPosition = localPosition;
        weapon.localRotation = Quaternion.Euler(localRotation);
    }
}
