using CustomClasses;
using CustomExtensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Interact : MonoBehaviour
{
    Vector3Int targetedCell;
    GameObject activeUI;
    GameObject activeTile;
    const float offsetForCell = 0.01f;
    Break breakScript;
    [SerializeField] float reach;
    [SerializeField] InventoryBase inventory;
    [SerializeField] PlayerCrafting crafting;

    void Awake()
    {
        breakScript = GetComponent<Break>();
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
            Vector2.Distance(transform.position, activeTile.transform.position) > reach * 2)) return true;

        if(!ItemManager.Instance.IsHoldingItem && Input.GetMouseButtonDown(1)) return true;

        return false;
    }

    void Update()
    {
        LayerMask mask = LayerMask.GetMask("Solid");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, reach, mask);

        if (CanDisableUI())
        {
            DisableUI();
            return;
        }

        if (!hit)
        {
            breakScript.ResetBreaking();
            return;
        }

        Vector2 newPos = hit.ExtendRaycast(offsetForCell, transform);

        if (hit.transform.TryGetComponent<Tilemap>(out var tilemap) || hit.transform.parent.GetComponent<Tilemap>() != null)
        {
            if(tilemap == null) tilemap = hit.transform.parent.GetComponent<Tilemap>();

            if (targetedCell != tilemap.WorldToCell(newPos) && targetedCell.z != -1)
            {
                breakScript.ResetBreaking();
                targetedCell.z = -1;
                return;
            }

            targetedCell = tilemap.WorldToCell(newPos);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (hit.transform.TryGetComponent<Gatherable>(out var gatherable))
            {
                if (gatherable.State != GatherableTileState.Replenished) return;

                gatherable.State = 0;

                GatherableTile tile = gatherable.GetComponentInParent<Tilemap>()
                    .GetTile<GatherableTile>(gatherable.TilePosition);

                Dictionary<Item, int> excess = new();
                foreach (var drop in tile.Drops)
                {
                    int count = Random.Range(drop.MinDropCount, drop.MaxDropCount);
                    if (count == 0) { continue; }

                    InvItem item = new(drop.Item, drop.Item.Name, count);

                    int leftover = inventory.AddToInventory(item);
                    if (leftover > 0) excess.Add(drop.Item, leftover);
                }

                inventory.UpdateUI();
                inventory.SaveInventory();

                foreach (KeyValuePair<Item, int> item in excess)
                {
                    InvItem dropItem = new(item.Key, item.Key.Name, item.Value);

                    ItemManager.SpawnGroundItem(dropItem, transform.position, true);
                }

                return;
            }

            if(hit.transform.TryGetComponent<CraftStation>(out var craftStation))
            {
                if (!craftStation.CraftingUi.activeSelf)
                {
                    craftStation.UpdateCraftingUI();
                    craftStation.CraftingUi.SetActive(true);
                    craftStation.CraftingUi.GetComponent<SlotCraftStationUI>().SetStation(craftStation);
                    craftStation.UpdateUI();

                    UpdateUI(craftStation.gameObject, craftStation.CraftingUi);
                }
                return;
            }

            if (hit.transform.TryGetComponent<PrototypeStation>(out var prototypeStation))
            {
                if (!prototypeStation.CraftingUi.activeSelf)
                {
                    prototypeStation.UpdateAvailablePrototypesUI(inventory.Inventory, crafting.Resources);
                    prototypeStation.CraftingUi.SetActive(true);
                    UpdateUI(prototypeStation.gameObject, prototypeStation.CraftingUi);
                }
                return;
            }

            if (hit.transform.TryGetComponent<Container>(out var container))
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
            if (tilemap == null) return;
            if (ItemManager.Instance.IsHoldingItem || activeUI != null)
            {
                breakScript.ResetBreaking();
                return;
            }

            BreakableTile tile = tilemap.GetTile<BreakableTile>(targetedCell);
            breakScript.Breaking(tile, targetedCell, tilemap);
        }
        else
        {
            breakScript.ResetBreaking();
        }
    }

    public void UpdateUI(GameObject tile, GameObject ui)
    {
        activeTile = tile;
        activeUI = ui;
    }
}
