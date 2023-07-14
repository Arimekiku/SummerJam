using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Axe : MonoBehaviour
{
    [SerializeField] private float speedRotate;
    [SerializeField] private float rotateRadius;
    [SerializeField] private int totalCount;
    [SerializeField] private int indexAxe;
    [SerializeField] private LayerMask playerLayer;

    private Vector2 defaultPosition;
    private Quaternion defaultRotation;
    private bool runnerOnJump = false;
    private Collider2D axeCollider;
    private Berserker holder;

    private void Start()
    {
        holder = GetComponentInParent<Berserker>();
        axeCollider = GetComponent<Collider2D>();
        axeCollider.enabled = false;

        defaultPosition = transform.localPosition;
        defaultRotation = transform.localRotation;
        holder.OnJumpEvent += RunnerInJump;
        holder.OnJumpEvent += SetDefaultPosition;
    }

    private void FixedUpdate()
    {
        if (runnerOnJump)
            RotateAroundHolder();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
            if (other.TryGetComponent(out Player player)) 
                player.TakeDamage(1);
    }

    private void RotateAroundHolder()
    {
        float rotationAngle = speedRotate * Time.fixedDeltaTime;
        transform.RotateAround(holder.transform.position, Vector3.forward, rotationAngle);
    }

    private void RunnerInJump(bool onJump)
    {
        if (onJump)
        {
            float angle = 360 / totalCount;
            float angleInRadius = angle * indexAxe * Mathf.Deg2Rad;
            float x = rotateRadius * Mathf.Cos(angleInRadius);
            float y = rotateRadius * Mathf.Sin(angleInRadius);
            Vector2 positionKnives = new Vector2(x, y);

            transform.localPosition = positionKnives;
        }

        axeCollider.enabled = onJump;
        runnerOnJump = onJump;
    }

    private void SetDefaultPosition(bool onJump)
    {
        if (!onJump) 
            transform.SetLocalPositionAndRotation(defaultPosition, defaultRotation);
    }
}
