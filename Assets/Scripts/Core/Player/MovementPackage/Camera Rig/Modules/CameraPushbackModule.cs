using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[PreferComponent(typeof(CameraAttachPoint))]
public class CameraPushbackModule : CameraRigModule
{
    Transform farPoint;
    [Header("Camera Speed")]
    [SerializeField] float maxDistance;
    [SerializeField] float returnSpeed;
    [SerializeField] AnimationCurve speedCurve = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(0.2f, 0.9f), new Keyframe(0.5f, 1f), new Keyframe(0.8f, 0.9f), new Keyframe(1f, 0.5f));
    float currentDistance = 5;
    [SerializeField] Vector3 movePointInDir = new Vector3(0, 0, -1);

    [Header("Camera Collision Settings")]
    [SerializeField] float collisionSize = 0.5f;
    [SerializeField] LayerMask cameraCollidesWith;
    [HideInInspector] public Vector3 forward => transform.forward;

    [SerializeField] bool _clampMaxDistanceToMax = true;
    public float allowedMaxDistance = 0;

    public override void Awake()
    {
        base.Awake();
        allowedMaxDistance = maxDistance;
        if (farPoint == null) AttachFarPoint(new GameObject("farPoint").transform);
    }

    void AttachFarPoint(Transform point)
    {
        if(farPoint != null) Destroy(farPoint.gameObject);
        farPoint = point;
        farPoint.parent = transform;
    }

    private void Update()
    {
        if (_clampMaxDistanceToMax) allowedMaxDistance = maxDistance;
        allowedMaxDistance = Mathf.Clamp(allowedMaxDistance, 0, maxDistance);
        HandleCamDistance();
    }

    void HandleCamDistance()
    {
        Vector3 moveDir = movePointInDir.normalized;
        float speed = speedCurve.Evaluate(1);
        if (!(allowedMaxDistance <= float.Epsilon))
            speed = speedCurve.Evaluate(currentDistance / allowedMaxDistance) * returnSpeed * Time.deltaTime;
        currentDistance += speed;
        currentDistance = Mathf.Clamp(currentDistance, 0, allowedMaxDistance);

        farPoint.localPosition = moveDir * allowedMaxDistance;
        Vector3 direction = farPoint.position - transform.position;
        direction.Normalize();

        Ray ray = new Ray(transform.position, direction);
        RaycastHit hitInfo;

            if (Physics.SphereCast(ray, collisionSize, out hitInfo, allowedMaxDistance, cameraCollidesWith, QueryTriggerInteraction.UseGlobal))
            {
                if (hitInfo.distance < currentDistance)
                    currentDistance = hitInfo.distance;
            }
        

        Collider[] colliders = new Collider[10];
        if (Physics.OverlapSphereNonAlloc(transform.position, 0.2f, colliders, cameraCollidesWith, QueryTriggerInteraction.UseGlobal) > 0)
            currentDistance = 0;

        Vector3 hitPoint = transform.position + direction * currentDistance;
        cameraRig.cameraAttachPointTransform.position = hitPoint;
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.cyan;
        if (farPoint != null)
            Gizmos.DrawWireSphere(farPoint.position, 0.2f);
        Gizmos.DrawWireSphere(transform.position, 0.2f);
        if (cameraRig != null)
            if (cameraRig.cameraAttachPointTransform != null)
                Gizmos.DrawWireSphere(cameraRig.cameraAttachPointTransform.position, collisionSize);
    }
#endif
}
