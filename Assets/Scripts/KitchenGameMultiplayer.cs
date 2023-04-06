using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour {
    public const int MAX_PLAYER_AMOUNT = 4;
    private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

    public static KitchenGameMultiplayer Instance { get; private set; }

    public static bool playMultiplayer = true;

    public event EventHandler OnTryingToJoin;
    public event EventHandler OnFailedToJoin;
    public event EventHandler OnPlayerDataNetworkListChanged;
    public event EventHandler OnPlayerChangeColor;
    private NetworkList<PlayerData> playerDataNetworkList;
    private string playerName;

    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;
    [SerializeField] private List<Color> playerColorList;

    private void Awake() {
        Instance = this;

        DontDestroyOnLoad(gameObject);
        playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(100, 1000));

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }
    private void Start() {
        if (!playMultiplayer) {
            //Single play
            StartHost();
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }
    public string GetPlayerName() {
        return playerName;
    }
    public void SetPlayerName(string newPlayerName) {
        playerName = newPlayerName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
    }
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }


    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectedCallback(ulong clientId) {
        for (int i = 0; i < playerDataNetworkList.Count; i++) {
            PlayerData playerdata = playerDataNetworkList[i];
            if (playerdata.clientId == clientId) {
                //Disconnected!
                playerDataNetworkList.RemoveAt(i);
            }
        }
        SetPlayerNameServerRpc(GetPlayerName());
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId) {
        playerDataNetworkList.Add(new PlayerData {
            clientId = clientId,
            colorId = GetFirstUnusedColorId(),
        });
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);

    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse) {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString()) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }

    public void StartClient() {
        OnTryingToJoin?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId) {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default) {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerdata = playerDataNetworkList[playerDataIndex];
        playerdata.playerName = playerName;
        playerDataNetworkList[playerDataIndex] = playerdata;
        OnPlayerChangeColor?.Invoke(this, EventArgs.Empty);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default) {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerdata = playerDataNetworkList[playerDataIndex];
        playerdata.playerId = playerId;
        playerDataNetworkList[playerDataIndex] = playerdata;
        OnPlayerChangeColor?.Invoke(this, EventArgs.Empty);

    }
    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId) {
        OnFailedToJoin?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkReference) {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        kitchenObjectParentNetworkReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        if (kitchenObjectParent.HasKitchenObject()) {
            //Parent already spawned an object
            return;
        }

        Transform kitchenObjectSOTransform = Instantiate(kitchenObjectSO.prefab);

        NetworkObject kitchenObjectNetworkObject = kitchenObjectSOTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectSOTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }
    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
        return kitchenObjectListSO.kitchenObjectListSO.IndexOf(kitchenObjectSO);
    }
    public KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex) {
        return kitchenObjectListSO.kitchenObjectListSO[kitchenObjectSOIndex];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject) {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }
    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);

        if (kitchenObjectNetworkObject == null) {
            //This object is already destroyed!
            return;
        }

        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        ClearKitchenObjectClientRpc(kitchenObjectNetworkObjectReference);
        kitchenObject.DestroySelf();
    }
    [ClientRpc]
    private void ClearKitchenObjectClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        kitchenObject.ClearKitchenObjectOnParent();
    }
    public bool IsPlayerIndexConnected(int playerIndex) {
        return playerIndex < playerDataNetworkList.Count;
    }
    public int GetPlayerDataIndexFromClientId(ulong clientId) {
        for (int i = 0; i < playerDataNetworkList.Count; i++) {
            if (playerDataNetworkList[i].clientId == clientId) return i;
        }
        return -1;
    }
    public PlayerData GetPlayerDataFromClientId(ulong clientId) {
        foreach (var playerData in playerDataNetworkList) {
            if (playerData.clientId == clientId) return playerData;
        }
        return default;
    }
    public PlayerData GetPlayerData() {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex) {
        return playerDataNetworkList[playerIndex];
    }
    public Color GetPlayerColor(int colorId) {
        return playerColorList[colorId];
    }
    public void ChangePlayerColor(int colorId) {
        ChangePlayerColorServerRpc(colorId);
    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default) {
        if (!IsColorAvailable(colorId)) {
            //Color is not available;
            return;
        }
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerdata = playerDataNetworkList[playerDataIndex];
        playerdata.colorId = colorId;
        playerDataNetworkList[playerDataIndex] = playerdata;
        OnPlayerChangeColor?.Invoke(this, EventArgs.Empty);
    }

    public bool IsColorAvailable(int colorId) {
        foreach (PlayerData playerData in playerDataNetworkList) {
            if (playerData.colorId == colorId) {
                return false;
            }
        }
        return true;
    }
    private int GetFirstUnusedColorId() {
        for (int i = 0; i < playerColorList.Count; i++) {
            if (IsColorAvailable(i)) {
                return i;
            }
        }
        return -1;
    }
    public void KickPlayer(ulong clientId) {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectedCallback(clientId);
    }
}
