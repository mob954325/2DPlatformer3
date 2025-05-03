using UnityEngine;

public class PlayPanel : MonoBehaviour
{
    private CanvasGroup cg;
    private EnemyCountUI enemyCountUI;
    public EnemyCountUI EnemyCountUI => enemyCountUI;

    private PlayerHpGauge playerHpGauge;
    public PlayerHpGauge PlayerHpGauge => playerHpGauge;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        enemyCountUI = GetComponentInChildren<EnemyCountUI>();
        playerHpGauge = GetComponentInChildren<PlayerHpGauge>();
    }

    public void OpenPanel()
    {
        if (cg == null) return;

        cg.alpha = 1;
        cg.blocksRaycasts = true;
        cg.interactable = true;
    }

    public void ClosePanel()
    {
        if (cg == null) return;

        cg.alpha = 0; // null reference
        cg.blocksRaycasts = false;
        cg.interactable = false;
    }
}
