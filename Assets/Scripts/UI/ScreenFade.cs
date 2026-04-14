using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private DialoguePanel dialoguePanel;
    [SerializeField] private DialogueTree cutsceneDialogue;

    public void TriggerCutscene()
    {
        StartCoroutine(FadeAndPlay());
    }

    private IEnumerator FadeAndPlay()
    {
        // Disable player input
        GameServices.GetInputHandler()?.DisableInput();

        // Fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // Play dialogue
        DialogueData selected = cutsceneDialogue.SelectBestOption();
        if (selected != null)
            dialoguePanel.StartDialogue(selected);

        // Wait for dialogue to finish then fade back in
        yield return new WaitUntil(() => !dialoguePanel.isOpen);
        yield return StartCoroutine(Fade(1f, 0f));

        // Re-enable player input
        GameServices.GetInputHandler()?.EnableInput();
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = to;
        fadeImage.color = c;
    }
}