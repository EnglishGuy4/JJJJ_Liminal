using System.Collections;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI[] prompts; // Text prompts array
    public AudioClip[] audioClips; // Corresponding audio clips array
    public AudioSource audioSource; // Audio source for playing audio
    public float[] displayDurations; // Duration for each prompt to be displayed
    public float[] promptDelays; // Delay for each prompt before showing up

    private Animator[] animators; // Animators array to handle fade in/out
    private CanvasGroup[] canvasGroups; // Canvas group for fade effects

    private void Start()
    {
        // Initialize animators and canvas groups
        animators = new Animator[prompts.Length];
        canvasGroups = new CanvasGroup[prompts.Length];

        for (int i = 0; i < prompts.Length; i++)
        {
            animators[i] = prompts[i].GetComponent<Animator>();
            canvasGroups[i] = prompts[i].GetComponent<CanvasGroup>();
            // Ensure initial state is hidden
            canvasGroups[i].alpha = 0;
        }

        StartCoroutine(DisplayPrompts());
    }

    private IEnumerator DisplayPrompts()
    {
        for (int i = 0; i < prompts.Length; i++)
        {
            // Wait for the specific delay before displaying the next prompt
            yield return new WaitForSeconds(promptDelays[i]);

            // Set active and trigger fade-in
            prompts[i].gameObject.SetActive(true);
            animators[i].SetTrigger("FadeIn");

            // Play the corresponding audio clip
            if (audioClips[i] != null)
            {
                audioSource.clip = audioClips[i];
                audioSource.Play();
            }

            // Wait for the fade-in animation to complete (adjust duration to match fade-in animation length)
            yield return new WaitForSeconds(1f);

            // Wait for the display duration
            yield return new WaitForSeconds(displayDurations[i]);

            // Trigger fade-out
            animators[i].SetTrigger("FadeOut");

            // Wait for the fade-out animation to complete (adjust duration to match fade-out animation length)
            yield return new WaitForSeconds(1f);

            // Set inactive after fade-out
            prompts[i].gameObject.SetActive(false);
        }
    }
}
