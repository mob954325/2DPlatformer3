using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryPanel : MonoBehaviour
{
    private CanvasGroup cg;
    private Button returnMenuButton;
    private Button restartButton;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        returnMenuButton = transform.GetChild(1).Find("GoToMenu").GetComponent<Button>();
        returnMenuButton.onClick.AddListener(() =>
        {
            ClosePanel();
            GameManager.Instacne.SceneChange(0);
        });

        ClosePanel();
    }

    public void OpenPanel()
    {
        if (cg == null) return;

        cg.alpha = 1;
        cg.blocksRaycasts = true;
        cg.interactable = true;
    }

    private void ClosePanel()
    {
        if (cg == null) return;

        cg.alpha = 0;
        cg.blocksRaycasts = false;
        cg.interactable = false;
    }
}
