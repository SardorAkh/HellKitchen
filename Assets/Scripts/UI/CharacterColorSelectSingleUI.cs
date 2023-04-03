using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour {
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;

    private Button button;
    private void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            KitchenGameMultiplayer.Instance.ChangePlayerColor(colorId);
        });
    }
    private void Start() {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        KitchenGameMultiplayer.Instance.OnPlayerChangeColor += KitchenGameMultiplayer_OnPlayerChangeColor;
        image.color = KitchenGameMultiplayer.Instance.GetPlayerColor(colorId);

        UpdateIsInteractable();
        UpdateIsSelected();
    }

    private void KitchenGameMultiplayer_OnPlayerChangeColor(object sender, System.EventArgs e) {
        UpdateIsInteractable();
    }
    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e) {
        UpdateIsSelected();
        UpdateIsInteractable();
    }
    private void UpdateIsInteractable() {
        if (KitchenGameMultiplayer.Instance.IsColorAvailable(colorId)) {
            button.interactable = true;
        } else {
            button.interactable = false;
        }
    }
    private void UpdateIsSelected() {
        if (KitchenGameMultiplayer.Instance.GetPlayerData().colorId == colorId) {
            selectedGameObject.SetActive(true);
        } else {
            selectedGameObject.SetActive(false);
        }
    }
    private void OnDestroy() {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        KitchenGameMultiplayer.Instance.OnPlayerChangeColor -= KitchenGameMultiplayer_OnPlayerChangeColor;
    }
}
