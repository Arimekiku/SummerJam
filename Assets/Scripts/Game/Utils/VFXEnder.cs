using UnityEngine;
using UnityEngine.VFX;

public class VFXEnder : MonoBehaviour
{
    private VisualEffect VFX;

    private void Awake()
    {
        VFX = GetComponent<VisualEffect>();
        VFX.Play();
        Destroy(gameObject, 5f);
    }
}
