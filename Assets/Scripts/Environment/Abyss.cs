using System.Collections.Generic;
using UnityEngine;

public class Abyss : MonoBehaviour
{
    [SerializeField] private AnimationCurve abyssCurve;

    private readonly Queue<Character> objectsInAbyss = new Queue<Character>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Character character))
        {
            if (character is Player player) 
            {
                if (player.IsDashing)
                    return;

                player.Deinitialize();
            }

            objectsInAbyss.Enqueue(character);

            StartCoroutine(Tweener.Scale(character.gameObject, Vector3.zero, abyssCurve, Fall));
        }
    }

    public void Fall()
    {
        Character objectToProcess = objectsInAbyss.Dequeue();

        if (objectToProcess is Player player)
        {
            objectToProcess.transform.localScale = Vector3.one;
            player.Initialize();
            player.TeleportToLastCheckPoint();
        }
        else
        {
            Destroy(objectToProcess);
        }
    }
}
