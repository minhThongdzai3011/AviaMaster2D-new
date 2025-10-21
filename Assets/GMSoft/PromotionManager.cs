using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GMSoft
{
    [RequireComponent(typeof(Button))]
    public class PromotionManager : MonoBehaviour
    {
        public static PromotionManager Instance { get; private set; }
        
        [Header("UI Components")]
        [SerializeField] private Button promotionButton;
        [SerializeField] private Image promotionImage;
        
        [Header("Settings")]
        [SerializeField] private float checkInterval = 0.5f; // Interval to check for loaded promotions
        [SerializeField] private bool autoHideWhenNoPromotion = true;
        
        private GMPromotionData currentPromotion;
        private Coroutine checkPromotionCoroutine;
        private bool isWaitingForPromotion = false;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            // Get components if not assigned
            if (promotionButton == null)
                promotionButton = GetComponent<Button>();
            
            if (promotionImage == null)
                promotionImage = GetComponent<Image>();
            
            // Add button click listener
            if (promotionButton != null)
                promotionButton.onClick.AddListener(OnPromotionButtonClicked);
        }
        
        private void Start()
        {
            // Try to display promotion immediately
            TryDisplayPromotion();
            
            // If no promotion is available, start waiting for one
            if (currentPromotion == null)
            {
                StartWaitingForPromotion();
            }
        }
        
        private void OnEnable()
        {
            // Subscribe to GMSDK events if available
            if (GMSDK.Instance != null)
            {
                GMSDK.Instance.OnLoadedGameInfo += OnGameInfoLoaded;
            }
        }
        
        private void OnDisable()
        {
            // Unsubscribe from GMSDK events
            if (GMSDK.Instance != null)
            {
                GMSDK.Instance.OnLoadedGameInfo -= OnGameInfoLoaded;
            }
            
            // Stop checking coroutine
            StopWaitingForPromotion();
        }
        
        private void OnGameInfoLoaded(GMGameInfo gameInfo)
        {
            // When game info is loaded, try to display promotion
            TryDisplayPromotion();
        }
        
        /// <summary>
        /// Try to display a loaded promotion immediately
        /// </summary>
        public void TryDisplayPromotion()
        {
            if (GMSDK.Instance == null)
            {
                Debug.Log("[Promotion Manager] GMSDK Instance is null");
                HidePromotion();
                return;
            }
            
            GMPromotion promotion = GMSDK.Instance.GetPromotion();
            if (promotion == null)
            {
                Debug.Log("[Promotion Manager] No promotion data available");
                HidePromotion();
                return;
            }
            
            // Check if promotion is enabled
            if (promotion.enable == "no")
            {
                Debug.Log("[Promotion Manager] Promotion is disabled");
                HidePromotion();
                return;
            }
            
            // Get a random loaded promotion
            GMPromotionData loadedPromotion = promotion.GetRandomLoadedPromotionData();
            if (loadedPromotion != null)
            {
                DisplayPromotion(loadedPromotion);
                StopWaitingForPromotion(); // Stop waiting since we found one
            }
            else
            {
                Debug.Log("[Promotion Manager] No loaded promotion data available");
                HidePromotion();
            }
        }
        
        /// <summary>
        /// Start waiting for promotions to be loaded and display when available
        /// </summary>
        public void StartWaitingForPromotion()
        {
            if (isWaitingForPromotion) return;

            if (!gameObject.activeInHierarchy)
            {
                Debug.Log("[Promotion Manager] Cannot start promotion coroutine - GameObject is not active in hierarchy.");
                return;
            }

            isWaitingForPromotion = true;
            checkPromotionCoroutine = StartCoroutine(WaitForPromotionCoroutine());
        }
        
        /// <summary>
        /// Stop waiting for promotions
        /// </summary>
        public void StopWaitingForPromotion()
        {
            isWaitingForPromotion = false;
            if (checkPromotionCoroutine != null)
            {
                StopCoroutine(checkPromotionCoroutine);
                checkPromotionCoroutine = null;
            }
        }
        
        private IEnumerator WaitForPromotionCoroutine()
        {
            Debug.Log("[Promotion Manager] Started waiting for promotion to load");
            
            while (isWaitingForPromotion)
            {
                yield return new WaitForSeconds(checkInterval);
                
                if (GMSDK.Instance == null) continue;
                
                GMPromotion promotion = GMSDK.Instance.GetPromotion();
                if (promotion == null) continue;
                
                // Check if promotion is enabled
                if (promotion.enable == "no")
                {
                    Debug.Log("[Promotion Manager] Promotion is disabled, stopping wait");
                    isWaitingForPromotion = false;
                    break;
                }
                
                // Check if any promotion is loaded
                GMPromotionData loadedPromotion = promotion.GetRandomLoadedPromotionData();
                if (loadedPromotion != null)
                {
                    Debug.Log("[Promotion Manager] Found loaded promotion, displaying it");
                    DisplayPromotion(loadedPromotion);
                    isWaitingForPromotion = false; // Stop waiting
                    break;
                }
            }
            
            Debug.Log("[Promotion Manager] Stopped waiting for promotion");
        }
        
        /// <summary>
        /// Display a specific promotion
        /// </summary>
        /// <param name="promotionData">The promotion data to display</param>
        public void DisplayPromotion(GMPromotionData promotionData)
        {
            if (promotionData == null || !promotionData.isLoaded)
            {
                Debug.LogWarning("[Promotion Manager] Cannot display promotion: data is null or not loaded");
                return;
            }
            
            currentPromotion = promotionData;
            
            // Set the sprite to the promotion image
            if (promotionImage != null && promotionData.sprite != null)
            {
                promotionImage.sprite = promotionData.sprite;
                promotionImage.enabled = true;
            }
            
            // Enable the button
            if (promotionButton != null)
            {
                promotionButton.interactable = true;
            }
            
            // Show the game object
            gameObject.SetActive(true);
            
            Debug.Log($"[Promotion Manager] Displaying promotion: {promotionData.name} - URL: {promotionData.url}");
        }
        
        /// <summary>
        /// Hide the promotion
        /// </summary>
        public void HidePromotion()
        {
            Debug.Log("[Promotion Manager] Hiding promotion");
            currentPromotion = null;
            
            if (autoHideWhenNoPromotion)
            {
                gameObject.SetActive(false);
            }
            else
            {
                // Just disable the button and hide the image
                if (promotionButton != null)
                    promotionButton.interactable = false;
                
                if (promotionImage != null)
                    promotionImage.enabled = false;
            }
        }
        
        /// <summary>
        /// Force refresh promotion display
        /// </summary>
        public void RefreshPromotion()
        {
            Debug.Log("[Promotion Manager] Refreshing promotion display");
            TryDisplayPromotion();

            // If still no promotion, start waiting again
            if (currentPromotion == null)
            {
                if (!gameObject.activeInHierarchy)
                {
                    Debug.Log("[Promotion Manager] Cannot start promotion coroutine from RefreshPromotion - GameObject is not active in hierarchy.");
                    return;
                }
                StartWaitingForPromotion();
            }
        }
        
        /// <summary>
        /// Get the currently displayed promotion
        /// </summary>
        /// <returns>Current promotion data or null</returns>
        public GMPromotionData GetCurrentPromotion()
        {
            return currentPromotion;
        }
        
        /// <summary>
        /// Check if a promotion is currently being displayed
        /// </summary>
        /// <returns>True if promotion is displayed</returns>
        public bool HasPromotion()
        {
            return currentPromotion != null;
        }
        
        private void OnPromotionButtonClicked()
        {
            if (currentPromotion == null)
            {
                Debug.LogWarning("[Promotion Manager] No promotion available to open");
                return;
            }
            
            if (string.IsNullOrEmpty(currentPromotion.url))
            {
                Debug.LogWarning($"[Promotion Manager] Promotion {currentPromotion.name} has no URL");
                return;
            }
            
            Debug.Log($"[Promotion Manager] Opening promotion URL: {currentPromotion.url}");
            Application.OpenURL(currentPromotion.url);
        }
        
        private void OnDestroy()
        {
            // Clean up singleton
            if (Instance == this)
            {
                Instance = null;
            }
            
            StopWaitingForPromotion();
            
            if (promotionButton != null)
            {
                promotionButton.onClick.RemoveListener(OnPromotionButtonClicked);
            }
            
            Debug.Log("[Promotion Manager] Destroyed");
        }
    }
}
