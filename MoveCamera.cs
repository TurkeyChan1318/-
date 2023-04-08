using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour
{
    public Transform Target;
    public float Distance = 5F;
    private float SpeedX = 240;
    private float SpeedY = 120;
    private float MinLimitY = 5;
    private float MaxLimitY = 180;
    private float mX = 0.0F;
    private float mY = 0.0F;
    private float MaxDistance = 50;
    private float MinDistance = 1.5F;
    private float ZoomSpeed = 5F;
    public bool isNeedDamping = true;
    public float Damping = 10F;
    private Quaternion mRotation;

    void Start()
    {

        mX = transform.eulerAngles.x;
        mY = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
 
        if (Target != null && Input.GetMouseButton(1))
        {
   
            mX += Input.GetAxis("Mouse X") * SpeedX * 0.02F;
            mY -= Input.GetAxis("Mouse Y") * SpeedY * 0.02F;
            mY = ClampAngle(mY, MinLimitY, MaxLimitY);

            mRotation = Quaternion.Euler(mY, mX, 0);

            if (isNeedDamping)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, mRotation, Time.deltaTime * Damping);
            }
            else
            {
                transform.rotation = mRotation;
            }

        }

        Distance -= Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
        Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);

        Vector3 mPosition = mRotation * new Vector3(0.0F, 0.0F, -Distance) + Target.position;

        if (isNeedDamping)
        {
            transform.position = Vector3.Lerp(transform.position, mPosition, Time.deltaTime * Damping);
        }
        else
        {
            transform.position = mPosition;
        }

    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}


