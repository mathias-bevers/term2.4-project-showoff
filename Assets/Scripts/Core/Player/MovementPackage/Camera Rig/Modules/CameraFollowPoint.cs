using UnityEngine;

public class CameraFollowPoint : CameraRigModule
{
    public Transform followPoint;
    [SerializeField] bool smoothFollow = false;
    [SerializeField] AnimationCurve speedCurve = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(0.2f, 0.9f), new Keyframe(0.5f, 1f), new Keyframe(0.8f, 0.9f), new Keyframe(1f, 0.5f));
    [SerializeField] float followSpeed = 1.0f;
    [SerializeField] float forceTPAt = 1.0f;

    public void SetTarget(Transform target) => followPoint = target;
    public Transform GetTarget() => followPoint;

    //Vector3 lastDirection = Vector3.zero;

    private void Update()
    {
        if(followPoint == null) return;
        if (!smoothFollow) Teleport();
        else
        {
            float distance = Vector3.Distance(transform.position, followPoint.position);
            if (distance > forceTPAt) { Teleport(); return; }
            transform.position = Vector3.MoveTowards(transform.position, followPoint.position, speedCurve.Evaluate(Utils.Map(distance, 0, forceTPAt, 0, 1)) * followSpeed * Time.deltaTime);
            //lastDirection = transform.position - followPoint.position;
        }
    }

    public void Teleport()
    {
        transform.position = followPoint.position;
        //transform.position += lastDirection;
    }
}
