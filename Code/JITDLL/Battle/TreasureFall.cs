using UnityEngine;
using System.Collections;

public class TreasureFall : MonoBehaviour {

    public enum TreasureType
    {
        Coin,
        Chest,
    }

    Vector3 _direction = Vector3.right;
    float _speed;
    float _speedX;
    float _speedY;

    bool _grounded = false;

    TreasureType _treasureType = TreasureType.Coin;
    int _treasureAmount = 0;

    float _startFlyTime;
    Vector3 _startPos;
    Vector3 _endPos;
    Vector3 _toUIPos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (GUI_BattleManager.Instance.BattleUI.BoxImage == null ||
            GUI_BattleManager.Instance.BattleUI.GoldImage == null)
        {
            return;
        }

        if (!_grounded)
        {
            _speedY -= DefaultConfig.GetFloat("MonsterTreasureFallAccelerate") * Time.deltaTime;

            transform.position += Vector3.right * _speedX * GameTimer.deltaTime + Vector3.up * _speedY * GameTimer.deltaTime;

            if (transform.position.y <= 0)
            {
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                _grounded = true;
                _startFlyTime = Time.time + DefaultConfig.GetFloat("TreasureStayTime");

                _startPos = transform.position;
            }
        }

        if (_grounded && Time.time > _startFlyTime)
        {
            if (_treasureType == TreasureType.Chest)
            {
                _toUIPos = RectTransformUtility.WorldToScreenPoint(GUI_Root_DL.Instance.UICamera, GUI_BattleManager.Instance.BattleUI.BoxImage.transform.position);
            }
            else
            {
                _toUIPos = RectTransformUtility.WorldToScreenPoint(GUI_Root_DL.Instance.UICamera, GUI_BattleManager.Instance.BattleUI.GoldImage.transform.position);
            }
            _endPos = Camera.main.ScreenToWorldPoint(new Vector3(_toUIPos.x, _toUIPos.y, transform.position.z - Camera.main.transform.position.z));

            transform.position = Vector3.Lerp(_startPos, _endPos, (Time.time - _startFlyTime) / DefaultConfig.GetFloat("TreasureFlyTime"));
            if (Time.time > _startFlyTime + DefaultConfig.GetFloat("TreasureFlyTime"))
            {
                TreasureFallInterface.RaiseOnTreasuseReach(_treasureType, _treasureAmount);

                EntityPool.Destroy(gameObject);
            }
        }
    }

    //Vector3 UITo

    public static void Begin(GameObject go, float speed, Vector3 direction, TreasureType treasureType, int amount)
    {
        TreasureFall comp = go.GetComponent<TreasureFall>();
        if (comp == null) comp = go.AddComponent<TreasureFall>();

        comp._speed = speed;
        comp._speedX = speed * direction.x;
        comp._speedY = speed * direction.y;
        comp._grounded = false;
        comp._treasureType = treasureType;
        comp._treasureAmount = amount;
    }
}
