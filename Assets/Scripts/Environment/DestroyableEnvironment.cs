using UnityEngine;
using Random = UnityEngine.Random;

public class DestroyableEnvironment : MonoBehaviour, IDestroyable
{
    [SerializeField] private Sprite[] possibleSprites;
    [SerializeField] private Sprite[] paintedSprites;
    [SerializeField] private Sprite paintedParent;
    [SerializeField] private GameObject[] lootItems;
    [SerializeField] private DestroyablePiece destroyablePrefab;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private AudioClip[] impactSounds;
    
    private int maxPieces = 5;
    private int minPieces = 2;
    private bool isLootable;

    private void Awake()
    {
        int rand = Random.Range(0, 101);

        if (rand < 20)
            isLootable = true;

        if (isLootable)
        {
            GetComponent<SpriteRenderer>().sprite = paintedParent;
            particles.textureSheetAnimation.SetSprite(0, paintedSprites[0]);

            foreach (Sprite sprite in paintedSprites)
            {
                if (sprite == paintedSprites[0])
                    continue;
                
                particles.textureSheetAnimation.AddSprite(sprite);
            }
        }
        else
        {
            particles.textureSheetAnimation.SetSprite(0, possibleSprites[0]);

            foreach (Sprite sprite in possibleSprites)
            {
                if (sprite == possibleSprites[0])
                    continue;
                
                particles.textureSheetAnimation.AddSprite(sprite);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
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

            Sprite randomSprite;

            if (!isLootable)
                randomSprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
            else
                randomSprite = paintedSprites[Random.Range(0, paintedSprites.Length)];
            
            pieceToSpawn.ApplySprite(randomSprite);
        }

        if (isLootable)
        {
            int rand = Random.Range(0, lootItems.Length);
            GameObject lootToSpawn = Instantiate(lootItems[rand], transform.parent);
            Vector2 offset = new Vector2(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f));
            lootToSpawn.transform.position = (Vector2)transform.position + offset;
            lootToSpawn.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
        }
        
        Destroy(gameObject);
        AudioHandler.PlaySound(impactSounds);
    }
}
