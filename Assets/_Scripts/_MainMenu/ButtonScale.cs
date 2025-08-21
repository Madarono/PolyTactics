using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float startScale = 1f;
    public float endScale = 1.1f;
    public float speedOfScale = 3f;

    [Header("Moditications")]
    public bool isContinue;

    private Coroutine scaleCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isContinue || (isContinue && MainMenu.Instance.hasMadeGame))
        {
            StartScale(Vector3.one * endScale);

        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        StartScale(Vector3.one * startScale);
    }

    private void StartScale(Vector3 target)
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        scaleCoroutine = StartCoroutine(ScaleTo(target));
    }

    private IEnumerator ScaleTo(Vector3 target)
    {
        while (Vector3.Distance(transform.localScale, target) > 0.001f)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, target, Time.deltaTime * speedOfScale);
            yield return null;
        }
        transform.localScale = target;
    }
}
