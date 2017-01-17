using UnityEngine;
using System.Collections;
using ACTOR;

public class CameraControl : MonoBehaviour
{
    bool _following = false;
    static CameraControl _instacne;

    public static CameraControl Instance
    {
        get
        {
            return _instacne;
        }
    }

    float _followSpeed = 15f;

    public float WorldHalfWidth { get; private set; }

    public Vector3 LogicPosition { get; private set; }

    Shake _shake = new Shake();

    void Awake()
    {
        _instacne = this;
    }

    void Start()
    {
        WorldHalfWidth = -Camera.main.transform.position.z * Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2) * Screen.width / Screen.height;

        LogicPosition = Camera.main.transform.position;
    }

    public void BeginFollow()
    {
        _following = true;
    }

    public void Shake(float degrees, float range, int count, float time)
    {
        _shake.TryToShake(degrees, range, count, time);
    }

    void LateUpdate()
    {
        if (!_following)
        {
            return;
        }

        Actor[] targets = ActorManager.Instance.Choose(null, SKILL.Camp.Comrade, SKILL.Target.Foward, float.MinValue);

        if (targets != null)
        {
            float step = _followSpeed * Time.deltaTime;
            Vector3 from = LogicPosition;
            float targetX = targets[0].ActorReference.ActorMovementEx.Position.x;
            float toX;
            float absD = Mathf.Abs(targetX - from.x);

            if (absD > step)
            {
                toX = (targetX - from.x) / absD * step + from.x;
            }
            else
            {
                toX = targetX;
            }

            if (from.x > toX &&
                toX < BattleManager_DL.Instance.LeftBound + WorldHalfWidth)
            {
                toX = BattleManager_DL.Instance.LeftBound + WorldHalfWidth;
            }
            if (from.x < toX &&
                toX > BattleManager_DL.Instance.RightBound - WorldHalfWidth)
            {
                toX = BattleManager_DL.Instance.RightBound - WorldHalfWidth;
            }

            LogicPosition = new Vector3(toX, from.y, from.z);

            Camera.main.transform.position = _shake.Update() + LogicPosition;

            GUI_BGMoveController_DL.Instance.CameraMove(LogicPosition.x);
        }
    }
}
