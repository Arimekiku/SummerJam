using UnityEngine;
using Random = UnityEngine.Random;

public class DestroyableEnvironment : MonoBehaviour, IInteractable, IDestroyed
{
    [SerializeField] private Sprite[] possibleSprites;
    [SerializeField] private DestroyablePiece destroyablePrefab;
    [SerializeField] private ParticleSystem particles;
    
    public InteractType Type { get; } = InteractType.Trigger;

    private int maxPieces = 5;
    private int minPieces = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Type == InteractType.Trigger)
            Interact();
    }

    public void Interact()
    {
        DestroyThisObject();
    }

    public void DestroyThisObject()
    {
        particles.transform.parent = transform.parent; 
        particles.Play();

        int pieceNumber = Random.Range(minPieces, maxPieces + 1);
        for (int i = 0; i < pieceNumber; i++)
        {
            DestroyablePiece pieceToSpawn = Instantiate(destroyablePrefab, transform.parent);
            Vector2 offset = new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
            pieceToSpawn.transform.position = (Vector2)transform.position + offset;
            pieceToSpawn.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));

            Sprite randomSprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
            pieceToSpawn.ApplySprite(randomSprite);
        }
        
        Destroy(gameObject);
    }
}
