using UnityEngine;
using System.Collections;

public class BattleBound : MonoBehaviour 
{
    public static Collider ColliderEx;
    int _enemyBulletLayer = -1;
    int _comradeBulletLayer = -1;
	// Use this for initialization
	void Start () 
    {
        ColliderEx = GetComponent<BoxCollider>();
        _enemyBulletLayer = LayerMask.NameToLayer("EnemyBullet");
        _comradeBulletLayer = LayerMask.NameToLayer("ComradeBullet");
	}

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer ==  _enemyBulletLayer||
            other.gameObject.layer == _comradeBulletLayer)
        {
            Destroy(other.gameObject);
        }
    }
}
