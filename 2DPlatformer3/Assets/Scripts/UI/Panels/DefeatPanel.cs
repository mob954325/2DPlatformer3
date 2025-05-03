using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DefeatPanel : MonoBehaviour
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

        restartButton = transform.GetChild(1).Find("Restart").GetComponent<Button>();
        restartButton.onClick.AddListener(() =>
        {
            ClosePanel();
            GameManager.Instacne.SceneChange(SceneManager.GetActiveScene().buildIndex);
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

        cg.alpha = 0; // null reference
        cg.blocksRaycasts = false;
        cg.interactable = false;
    }
}
