using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour {
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI messageText;
    private void Awake() {
        closeButton.onClick.AddListener(() => {
            Hide();
        });

    }
    private void Start() {
        KitchenGameMultiplayer.Instance.OnFailedToJoin += KitchenManagerMultiplayer_OnFailedToJoin;
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted += KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnJoinFailed += KitchenGameLobby_OnJoinFailed;
        KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenGameLobby_OnQuickJoinFailed;
        Hide();
    }

    private void KitchenGameLobby_OnQuickJoinFailed(object sender, EventArgs e) {
        ShowMessage("Lobby not found");
    }

    private void KitchenGameLobby_OnJoinFailed(object sender, EventArgs e) {
        ShowMessage("Failed to join lobby!");
    }

    private void KitchenGameLobby_OnJoinStarted(object sender, EventArgs e) {
        ShowMessage("Joining lobby...");
    }

    private void KitchenGameLobby_OnCreateLobbyFailed(object sender, EventArgs e) {
        ShowMessage("Failed to create Lobby!");
    }

    private void KitchenGameLobby_OnCreateLobbyStarted(object sender, System.EventArgs e) {
        ShowMessage("Creating Lobby...");
    }

    private void KitchenManagerMultiplayer_OnFailedToJoin(object sender, System.EventArgs e) {
        if (NetworkManager.Singleton.DisconnectReason == "") {
            ShowMessage("Failed to connect");
        } else {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }
    private void ShowMessage(string message) {
        Show();
        messageText.text = message;
    }
    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
    private void OnDestroy() {
        KitchenGameMultiplayer.Instance.OnFailedToJoin -= KitchenManagerMultiplayer_OnFailedToJoin;

        KitchenGameLobby.Instance.OnCreateLobbyStarted -= KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed -= KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted -= KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnJoinFailed -= KitchenGameLobby_OnJoinFailed;
        KitchenGameLobby.Instance.OnQuickJoinFailed -= KitchenGameLobby_OnQuickJoinFailed;
    }
}
