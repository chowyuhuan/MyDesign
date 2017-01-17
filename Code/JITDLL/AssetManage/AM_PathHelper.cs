using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AssetManage
{
    public class AM_PathHelper
    {
        public static string GetActorFullPathByName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length <= 6)
            {
                return name;
            }
            char key = name[6];
            switch (key)
            {
                case 'A': //Actor_Arc
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/Prefab/Archer/" + name;
                case 'B': //Actor_Bos
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Bosses/Prefab/" + name;
                case 'K': //Actor_Kni
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/Prefab/Knight/" + name;
                case 'M': //Actor_Mag or Actor_Mon
                    return name[7] == 'a' ? StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/Prefab/Magician/" + name :
                        StringOperationUtil.OptimizedStringOperation.i + "Actors/Monsters/Prefab/" + name;
                case 'N': //Actor_Npc
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/NPC/" + name;
                case 'P': //Actor_Pri
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/Prefab/Priset/" + name;
                case 'S': //Actor_Sho or Actor_Swo
                    return StringOperationUtil.OptimizedStringOperation.i + (name[7] == 'w' ? "Actors/Heroes/Prefab/Swordman/" : "Actors/Heroes/Prefab/Shooter/") + name;
            }
            return name;
        }

        public static string GetWeaponFullPathByName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length <= 7)
            {
                return name;
            }
            char key = name[7];
            switch (key)
            {
                case 'A': //Weapon_Arc
                    return StringOperationUtil.OptimizedStringOperation.i + "Weapon/Archer/" + name;
                case 'K': //Weapon_Kni
                    return StringOperationUtil.OptimizedStringOperation.i + "Weapon/Knight/" + name;
                case 'M': //Weapon_Mag or Weapon_Mon
                    return name[7] == 'a' ? StringOperationUtil.OptimizedStringOperation.i + "Weapon/Magician/" + name :
                        StringOperationUtil.OptimizedStringOperation.i + "Actors/Monsters/" + name;
                case 'N': //Weapon_Npc
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/NPC/" + name;
                case 'P': //Weapon_Pri
                    return StringOperationUtil.OptimizedStringOperation.i + "Weapon/Priset/" + name;
                case 'S': //Weapon_Sho or Actor_Swo
                    return StringOperationUtil.OptimizedStringOperation.i + (name[8] == 'w' ? "Weapon/Swardman/" : "Weapon/Shooter/") + name;
            }
            return name;
        }

        public static string GetActorAnimEffectFullPathByName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length <= 8)
            {
                return name;
            }
            char key = name[8]; // EffectA_
            switch (key)
            {
                case 'A': //XXX_Arc
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/AnimEffect/Archer/" + name;
                case 'B': //XXX_Bos
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Bosses/AnimEffect/Knight/" + name;
                case 'K': //XXX_Kni
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/AnimEffect/Knight/" + name;
                case 'M': //XXX_Mag
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/AnimEffect/Magician/" + name;
                case 'P': //XXX_Pri
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/AnimEffect/Priset/" + name;
                case 'S': //XXX_Sho or XXX_Swo
                    return StringOperationUtil.OptimizedStringOperation.i + (name[9] == 'w' ? "Actors/Heroes/AnimEffect/Swordman/" : "Actors/Heroes/AnimEffect/Shooter/") + name;
            }
            return name;
        }

        public static string GetActorEffectFullPathByName(string name)
        {
            if(string.IsNullOrEmpty(name) || name.Length <= 8)
            {
                return name;
            }
            char key = name[8]; // EffectB_ or EffectS_
            switch (key)
            {
                case 'A': //XXX_Arc
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/Effects/Archer/" + name;
                case 'B': //XXX_Bos
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Bosses/Effects/" + name;
                case 'K': //XXX_Kni
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/Effects/Knight/" + name;
                case 'M': //XXX_Mag
                    return name[9] == 'a' ? StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/Effects/Magician/" + name : 
                        StringOperationUtil.OptimizedStringOperation.i + "Actors/Monsters/Effects/" + name;
                case 'P': //XXX_Pri
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/Effects/Priset/" + name;
                case 'S': //XXX_Sho or XXX_Swo
                    return StringOperationUtil.OptimizedStringOperation.i + (name[9] == 'w' ? "Actors/Heroes/Effects/Swordman/" : "Actors/Heroes/Effects/Shooter/") + name;
            }
            return name;
        }

        public static string GetActorAnimCtrlFullPathByName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length <= 5)
            {
                return name;
            }
            char key = name[5]; // Ctrl_
            switch (key)
            {
                case 'A': //XXX_Arc
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/AnimCtrl/Archer/" + name;
                case 'B': //XXX_Bos
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Bosses/AnimCtrl/Knight/" + name;
                case 'K': //XXX_Kni
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/AnimCtrl/Knight/" + name;
                case 'M': //XXX_Mag
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/AnimCtrl/Magician/" + name;
                case 'P': //XXX_Pri
                    return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/AnimCtrl/Priset/" + name;
                case 'S': //XXX_Sho or XXX_Swo
                    return StringOperationUtil.OptimizedStringOperation.i + (name[6] == 'w' ? "Actors/Heroes/AnimCtrl/Swordman/" : "Actors/Heroes/AnimCtrl/Shooter/") + name;
            }
            return name;
        }
        

        public static string GetActorUIAnimCtrlFullPathByName(string name)
        {
            return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/AnimCtrlN/" + name;
        }

        public static string GetActorUIEvoAnimCtrlFullPathByName(string name)
        {
            return StringOperationUtil.OptimizedStringOperation.i + "Actors/Heroes/AnimCtrlN/" + name;
        }
    }
}
