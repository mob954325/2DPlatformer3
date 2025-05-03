using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpGauge : MonoBehaviour
{
    Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetValue(float ratio)
    {
        StartCoroutine(ValueChangeProcess(ratio));
    }

    private IEnumerator ValueChangeProcess(float ratio)
    {
        float timeElapsed = 0.0f;

        while(timeElapsed < 1f)
        {
            slider.value = Mathf.Lerp(slider.value, ratio, timeElapsed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        slider.value = ratio;
    }
}
