using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MoreGamesButton : MonoBehaviour
{
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnMoreGamesButtonClicked);
    }
    
    private void Start()
    {
        // Check conditions when the script starts
        CheckAndUpdateButtonVisibility();
    }
    
    private void OnEnable()
    {
        // Also check when the object becomes active
        CheckAndUpdateButtonVisibility();
    }
    
    private void CheckAndUpdateButtonVisibility()
    {
        // Check if GMSDK instance exists
        if (GMSDK.Instance == null)
        {
            HideButton();
            return;
        }
        
        // Check if EnableMoreGames is true OR GetMoreGamesUrl is not null/empty
        bool enableMoreGames = GMSDK.Instance.EnableMoreGames();
        string moreGamesUrl = GMSDK.Instance.GetMoreGamesUrl();
        
        if (enableMoreGames || !string.IsNullOrEmpty(moreGamesUrl))
        {
            ShowButton();
        }
        else
        {
            HideButton();
        }
    }
    
    private void ShowButton()
    {
        gameObject.SetActive(true);
    }
    
    private void HideButton()
    {
        gameObject.SetActive(false);
    }
    
    private void OnMoreGamesButtonClicked()
    {
        if (GMSDK.Instance == null)
        {
            Debug.LogWarning("GMSDK Instance is null when trying to open more games URL");
            return;
        }
        
        string moreGamesUrl = GMSDK.Instance.GetMoreGamesUrl();
        
        if (!string.IsNullOrEmpty(moreGamesUrl))
        {
            Debug.Log($"Opening more games URL: {moreGamesUrl}");
            Application.OpenURL(moreGamesUrl);
        }
        else
        {
            Debug.LogWarning("More games URL is null or empty");
        }
    }
    
    private void OnDestroy()
    {
        // Clean up button listener
        if (button != null)
        {
            button.onClick.RemoveListener(OnMoreGamesButtonClicked);
        }
    }
}
