using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour {
    private const string POPUP = "Popup";

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor;

    [SerializeField] private Sprite successIcon;
    [SerializeField] private Sprite failedIcon;

    private Animator animator;
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        gameObject.SetActive(false);
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, System.EventArgs e) {
        gameObject.SetActive(true);

        iconImage.sprite = successIcon;
        backgroundImage.color = successColor;
        messageText.text = "Delivery\nSuccess";

        animator.SetTrigger(POPUP);
    }
    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e) {
        gameObject.SetActive(true);

        iconImage.sprite = failedIcon;
        backgroundImage.color = failedColor;
        messageText.text = "Delivery\nFailed";
        animator.SetTrigger(POPUP);
    }
}
