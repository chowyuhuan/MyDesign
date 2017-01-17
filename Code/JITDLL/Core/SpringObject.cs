using UnityEngine;


public class SpringObject : MonoBehaviour
{
    static public SpringObject current;

    public Vector3 target = Vector3.zero;

    public float strength = 10f;

    public delegate void OnSpring(GameObject go);

    public OnSpring onSpring;

    public delegate void OnFinished();

    public OnFinished onFinished;

    Transform mTrans;

    void Start()
    {
        mTrans = transform;
    }

    /// <summary>
    /// Advance toward the target position.
    /// </summary>

    void Update()
    {
        AdvanceTowardsPosition();
    }

    /// <summary>
    /// Advance toward the target position.
    /// </summary>

    protected virtual void AdvanceTowardsPosition()
    {
        float delta = GameTimer.deltaTime;

        bool trigger = false;
        Vector3 before = mTrans.localPosition;
        Vector3 after = SpringLerp(mTrans.localPosition, target, strength, delta);

        if (onSpring != null) onSpring(gameObject);

        if ((after - target).sqrMagnitude < 0.01f)
        {
            after = target;
            enabled = false;
            trigger = true;
        }

        mTrans.localPosition = after;

        Vector3 offset = after - before;

        if (trigger && onFinished != null)
        {
            current = this;
            onFinished();
            current = null;
        }
    }

    /// <summary>
    /// Start the tweening process.
    /// </summary>

    static public SpringObject Begin(GameObject go, Vector3 pos, float strength)
    {
        SpringObject so = go.GetComponent<SpringObject>();
        if (so == null) so = go.AddComponent<SpringObject>();
        so.target = pos;
        so.strength = strength;
        so.onFinished = null;
        so.enabled = true;
        return so;
    }

    static public Vector3 SpringLerp(Vector3 from, Vector3 to, float strength, float deltaTime)
    {
        return Vector3.Lerp(from, to, SpringLerp(strength, deltaTime));
    }

    static public float SpringLerp(float strength, float deltaTime)
    {
        if (deltaTime > 1f) deltaTime = 1f;
        int ms = Mathf.RoundToInt(deltaTime * 1000f);
        deltaTime = 0.001f * strength;
        float cumulative = 0f;
        for (int i = 0; i < ms; ++i) cumulative = Mathf.Lerp(cumulative, 1f, deltaTime);
        return cumulative;
    }
}
