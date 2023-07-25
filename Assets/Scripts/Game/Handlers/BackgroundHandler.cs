using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class BackgroundHandler : MonoBehaviour
{
    [Space, SerializeField] private Color targetColor;
    [SerializeField] private GameBeginner beginner;
    [SerializeField] private CanvasGroup tooltip;

    private CinemachineImpulseSource shake;
    private SpriteRenderer rend;

    private bool canLerp;
    private float currentValue;
    private float eLastPressTime = 10f;

    private void Awake()
    {
        shake = GetComponent<CinemachineImpulseSource>();
        rend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canLerp)
            eLastPressTime = 0;

        eLastPressTime += Time.deltaTime;
        if (eLastPressTime < 1f && canLerp)
        {
            Vector3 randomVector = new Vector3(Random.value * 2 - 1, Random.value * 2 - 1, Random.value * 2 - 1) * rend.color.a / 4f;
            shake.GenerateImpulseWithVelocity(randomVector);
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, rend.color.a + 1f * Time.deltaTime / 4f);
        }

        if (eLastPressTime > 1f && rend.color.a > 0f && canLerp)
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, rend.color.a - 1f * Time.deltaTime / 2f);

        if (rend.color.a >= 1f && canLerp)
        {
            canLerp = false;
            rend.color = targetColor;
            StartCoroutine(GameBeginCoroutine());
        }
    }

    private IEnumerator GameBeginCoroutine()
    {
        tooltip.DOFade(0, 1.5f).OnComplete(() => Destroy(tooltip.gameObject));
        yield return new WaitForSeconds(2f);
        
        beginner.BeginGame();
        Destroy(this);
    }

    public void StartLerp()
    {
        canLerp = true;
        tooltip.DOFade(1, 1.5f);
    }
}
