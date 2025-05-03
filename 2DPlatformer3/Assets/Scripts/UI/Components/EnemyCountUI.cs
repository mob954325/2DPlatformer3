using TMPro;
using UnityEngine;

public class EnemyCountUI : MonoBehaviour
{
    private TextMeshProUGUI tmp;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    public void SetCount(int value)
    {
        tmp.text = $"x {value}";
    }
}
