using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour {
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private TextMeshProUGUI lobbyNameErrorText;
    private void Awake() {
        createPublicButton.onClick.AddListener(() => {
            if (ValidateInputs()) {
                KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
            }
        });
        createPrivateButton.onClick.AddListener(() => {
            if (ValidateInputs()) {
                KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
            }
        });
        closeButton.onClick.AddListener(() => {
            lobbyNameErrorText.gameObject.SetActive(false);
            Hide();
        });
        lobbyNameErrorText.gameObject.SetActive(false);
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }
    public void Hide() {
        gameObject.SetActive(false);
    }
    private bool ValidateInputs() {
        if (lobbyNameInputField.text.Length == 0) {
            lobbyNameErrorText.gameObject.SetActive(true);
            lobbyNameErrorText.text = "\"Lobby playerName\" cannot be empty";
            return false;
        }
        return true;
    }
}
