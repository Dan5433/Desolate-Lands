using CustomClasses;
using CustomExtensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class Interact : MonoBehaviour
{
    GameObject activeUI;
    GameObject activeTile;
    PlayerDamage damageScript;
    [SerializeField] float reach;
    [SerializeField] Transform origin;
    [SerializeField] InventoryBase inventory;
    [SerializeField] PlayerCrafting crafting;
    [SerializeField] AudioSource interactAudio;

    void Awake()
    {
        damageScript = GetComponent<PlayerDamage>();
    }

    public void DisableUI()
    {
        if (activeUI != null) activeUI.SetActive(false);
        activeUI = null;
        activeTile = null;
    }

    bool CanDisableUI()
    {
        if(activeTile == null) return false;

        if ((Input.GetKeyDown(KeyCode.Escape) ||
            Vector2.Distance(transform.position, activeTile.transform.position) > reach * 2)) 
            return true;

        if(!ItemManager.Instance.IsHoldingItem && Input.GetMouseButtonDown(1)) 
            return true;

        return false;
    }

    void Update()
    {
        LayerMask mask = LayerMask.GetMask("Interact");
        RaycastHit2D hit = Physics2D.Raycast(origin.position, origin.up, reach, mask);

        Debug.DrawRay(origin.position, origin.up, Color.green, reach);

        if (!hit)
            return;

        if (CanDisableUI())
        {
            DisableUI();
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (hit.collider.TryGetComponent<Gatherable>(out var gatherable))
            {
                if (gatherable.State != GatherableTileState.Replenished) return;

                gatherable.State = 0;

                GatherableTile tile = gatherable.GetComponentInParent<Tilemap>()
                    .GetTile<GatherableTile>(gatherable.TilePosition);
                foreach (var drop in tile.Drops)
                {
                    int count = drop.RandomCount();
                    if (count == 0) { continue; }

                    inventory.AddToInventory(InventoryItemFactory.Create(drop.item, count));
                }

                inventory.UpdateUI();
                inventory.SaveInventory();

                return;
            }

            if(hit.collider.TryGetComponent<CraftStation>(out var craftStation))
            {
                if (!craftStation.CraftingUi.activeSelf)
                {
                    craftStation.UpdateCraftingUI();
                    craftStation.CraftingUi.SetActive(true);
                    craftStation.UpdateUI();

                    UpdateUI(craftStation.gameObject, craftStation.CraftingUi);
                }
                return;
            }

            if (hit.collider.TryGetComponent<PrototypeStation>(out var prototypeStation))
            {
                if (!prototypeStation.CraftingUi.activeSelf)
                {
                    prototypeStation.UpdateAvailablePrototypesUI(inventory.Inventory, crafting.Resources);
                    prototypeStation.CraftingUi.SetActive(true);
                    UpdateUI(prototypeStation.gameObject, prototypeStation.CraftingUi);
                }
                return;
            }

            if (hit.collider.TryGetComponent<Container>(out var container))
            {
                if (!container.UI.gameObject.activeSelf)
                {
                    container.UI.GetComponent<InventoryRef>().Inventory = container;
                    container.GenInventory();
                    container.UpdateUI();
                    container.UI.gameObject.SetActive(true);

                    UpdateUI(container.gameObject, container.UI.gameObject);
                }
                return;
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (!hit.collider.TryGetComponent<IDamageable>(out var damageable) ||
                ItemManager.Instance.IsHoldingItem)
                return;

            damageScript.DealDamage(hit, damageable);
        }
        else
        {
            damageScript.ResetCooldown();
        }
    }


    public void UpdateUI(GameObject tile, GameObject ui)
    {
        activeTile = tile;
        activeUI = ui;
    }
}
