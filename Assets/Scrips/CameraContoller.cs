using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 100f;
    public float distance = 4f;
    public float yMin = -20f;
    public float yMax = 60f;
    public float smoothSpeed = 10f;
    public float cameraRadius = 0.2f;
    public bool camaraActiva = true;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private float currentYRotation;
    private float currentXRotation;
    private float currentDistance;
    private LayerMask ignoreMask;

    void Start()
    {
        currentDistance = distance;
        ignoreMask = ~(1 << LayerMask.NameToLayer("Player"));
    }

    void LateUpdate()
    {
        if (!camaraActiva) return;
        if (player == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, yMin, yMax);

        float lerpSpeed = smoothSpeed * Time.deltaTime;

        currentYRotation = Mathf.LerpAngle(
            currentYRotation,
            yRotation,
            lerpSpeed
        );

        currentXRotation = Mathf.LerpAngle(
            currentXRotation,
            xRotation,
            lerpSpeed
        );

        Quaternion rotation =
            Quaternion.Euler(
                currentXRotation,
                currentYRotation,
                0f
            );

        Vector3 origin =
            player.position + Vector3.up * 1.5f;

        Vector3 desiredPos =
            origin +
            rotation *
            new Vector3(0f, 0f, -distance);

        Vector3 dir =
            (desiredPos - origin);

        float dist = dir.magnitude;

        dir = dir.normalized;

        RaycastHit hit;

        float targetDistance;

        if (
            Physics.SphereCast(
                origin,
                cameraRadius,
                dir,
                out hit,
                dist,
                ignoreMask
            )
        )
        {
            targetDistance =
                Mathf.Clamp(
                    hit.distance - 0.1f,
                    0.5f,
                    distance
                );
        }
        else
        {
            targetDistance = distance;
        }

        float smoothFactor =
            targetDistance < currentDistance
            ? 25f
            : 8f;

        currentDistance =
            Mathf.Lerp(
                currentDistance,
                targetDistance,
                smoothFactor * Time.deltaTime
            );

        transform.position =
            origin + dir * currentDistance;

        transform.LookAt(origin);

        player.rotation =
            Quaternion.Euler(
                0f,
                currentYRotation,
                0f
            );
    }
}