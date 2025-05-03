using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private CanvasGroup cg;
    private Button returnMenuButton;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();        
    }

    private void Start()
    {
        returnMenuButton = transform.GetChild(1).Find("GoToMenu").GetComponent<Button>();
        returnMenuButton.onClick.AddListener(() => 
        {
            if (GameManager.Instacne.State == GameState.Menu) return;

            ClosePanel();
            GameManager.Instacne.SceneChange(0); 
        });

        ClosePanel();
    }

    private void OnDestroy()
    {
        inputActions.UI.Disable();
    }

    private void OnDisable()
    {
        inputActions.UI.OpenPause.performed -= OpenPause_performed;
    }

    public void OnEnable()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.OpenPause.Enable();
        inputActions.UI.OpenPause.performed += OpenPause_performed;
    }    

    private void OpenPanel()
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

    private void OpenPause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (cg == null) return;
        if (GameManager.Instacne.State != GameState.Play) return;

        if(cg.alpha > 0)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }
}
