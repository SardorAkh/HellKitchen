using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter {

    public event EventHandler OnPlatesSpawned;
    public event EventHandler OnPlatesRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;
    private void Start() {
        spawnPlateTimer = spawnPlateTimerMax;
    }
    private void Update() {
        if (!IsServer) {
            return;
        }
        spawnPlateTimer += Time.deltaTime;
        if (GameManager.Instance.IsGamePlaying() && spawnPlateTimer > spawnPlateTimerMax) {
            spawnPlateTimer = 0;
            if (platesSpawnedAmount < platesSpawnedAmountMax) {
                SpawnPlateServerRpc();
            }
        }
    }
    [ServerRpc]
    private void SpawnPlateServerRpc() {
        SpawnPlateClientRpc();
    }
    [ClientRpc]
    private void SpawnPlateClientRpc() {
        platesSpawnedAmount++;

        OnPlatesSpawned?.Invoke(this, EventArgs.Empty);
    }
    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            // Player hand is empty
            if (platesSpawnedAmount > 0) {
                // Counter have at least 1 plate
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                InteractLogicServerRpc();
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc() {
        platesSpawnedAmount--;
        OnPlatesRemoved?.Invoke(this, EventArgs.Empty);
    }
}
