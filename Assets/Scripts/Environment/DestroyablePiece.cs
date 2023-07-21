using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class DestroyablePiece : MonoBehaviour
{
    private SpriteRenderer rend;
    private Rigidbody2D body;

    private float speed;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();

        speed = Random.Range(8, 10f);
        DOTween.To(() => speed, x => speed = x, 0, 0.5f).OnKill(() =>
        {
            Destroy(body);
            Destroy(GetComponent<Collider2D>());
            Destroy(this);
        });
    }

    private void FixedUpdate()
    {
        Vector2 castedUp = transform.up;
        body.MovePosition(body.position + Time.fixedDeltaTime * speed * castedUp);
    }

    private void OnCollisionEnter2D(Collision2D _)
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, 0.5f);
        Vector2 reflectVector = Vector2.Reflect(transform.up, hitInfo.normal);

        transform.rotation = Quaternion.FromToRotation(transform.position, reflectVector);
    }

    public void ApplySprite(Sprite spriteToApply)
    {
        rend.sprite = spriteToApply;
    }
}
