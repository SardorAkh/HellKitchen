using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClearCounter : BaseCounter {

    [SerializeField] KitchenObjectSO kitchenObjectSO;
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // There's no kitchenobject here
            if (player.HasKitchenObject()) {
                //Player is carrying smth
                player.GetKitchenObject().SetKitchenObjectParent(this);
            } else {
                //Player's not carrying anything
            }
        } else {
            // There's kitchenobject here
            if (player.HasKitchenObject()) {
                //Player is carrying smth
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    //Player is holding a Plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                } else {
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                        }
                    }
                }
            } else {
                //Player's not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
