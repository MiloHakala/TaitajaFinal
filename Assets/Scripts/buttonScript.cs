using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(AudioSource))]
public class buttonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Vector3 hoverOffset = new Vector3(10f, 0f, 0f);
    public float moveDuration = 0.2f;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private Vector3 originalPosition;
    private RectTransform rectTransform;
    private AudioSource audioSource;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MoveTo(originalPosition + hoverOffset);
        PlaySound(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MoveTo(originalPosition);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound(clickSound);
    }

    private void MoveTo(Vector3 targetPosition)
    {
#if USE_LEANTWEEN
        LeanTween.move(rectTransform, targetPosition, moveDuration).setEaseOutQuad();
#else
        StopAllCoroutines();
        StartCoroutine(SmoothMove(targetPosition));
#endif
    }

#if !USE_LEANTWEEN
    private System.Collections.IEnumerator SmoothMove(Vector3 target)
    {
        float elapsed = 0f;
        Vector3 start = rectTransform.anchoredPosition;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector3.Lerp(start, target, elapsed / moveDuration);
            yield return null;
        }
        rectTransform.anchoredPosition = target;
    }
#endif

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
