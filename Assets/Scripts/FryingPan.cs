using UnityEngine;

public class FryingPan : MonoBehaviour
{
    public bool isFrying = false;
    public AudioSource fryingSound; // Assign in Inspector
    private Animator animator;

    public Transform fryingSpot;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("FryingPan: No Animator component found!");
        }
    }

    public void FinishFrying()
    {
        isFrying = false;
        Debug.Log("Frying done!");
    }

    void Update()
    {
        if (animator != null)
        {
            animator.SetBool("IsFrying", isFrying);
        }
        if (isFrying && !fryingSound.isPlaying)
        {
            fryingSound.Play();
        }
        else if (!isFrying && fryingSound.isPlaying)
        {
            fryingSound.Stop();
        }
    }
}
