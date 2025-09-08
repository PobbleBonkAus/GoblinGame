using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image fadeToBlackImage;
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float holdTime = 1f;
    [SerializeField] private float fadeOutTime = 1f;

    [SerializeField] private UnityEvent[] fadeOutEvents;

    bool isFading = false;


    public void FadeToBlack()
    {
        if (isFading) return;

        StopAllCoroutines(); // prevents overlapping fades
        StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        isFading = true;
        // fade in
        yield return Fade(Color.clear, Color.black, fadeInTime);



        // trigger events after fade
        foreach (var e in fadeOutEvents)
            e.Invoke();

        // hold black
        yield return new WaitForSeconds(holdTime);



        // fade out
        yield return Fade(Color.black, Color.clear, fadeOutTime);
        isFading = false;
    }

    private IEnumerator Fade(Color from, Color to, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            fadeToBlackImage.color = Color.Lerp(from, to, timer / duration);
            yield return null;
        }
        fadeToBlackImage.color = to; // snap to final color
    }
}
