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

    void Awake()
    {
        breakScript = GetComponent<Break>();
    }

    void Update()
    {
        LayerMask mask = LayerMask.GetMask("Interactable");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, reach, mask);

        if (!hit)
        {
            breakScript.ResetBreaking();
            return;
        }

        Vector2 newPos = hit.ExtendRaycast(offsetForCell, transform);

        if (hit.transform.TryGetComponent(out Tilemap tilemap))
        {
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

            if (activeUI == null)
            {
                GameObject containerTile = tilemap.GetInstantiatedObject(targetedCell);
                activeTile = containerTile;

                if (containerTile != null && containerTile.TryGetComponent<Container>(out var container))
                {
                    GameObject ui = container.UI.gameObject;
                    activeUI = ui;

                    bool active = ui.activeSelf;
                    if (!active)
                    {
                        ui.GetComponent<InventoryRef>().Inventory = container;
                        container.UpdateUI();
                        ui.SetActive(true);
                    }
                }
                return;
            }
        }

        if (Input.GetMouseButton(0))
        {
            BreakableTile tile = tilemap.GetTile<BreakableTile>(targetedCell);
            if (tile != null) breakScript.Breaking(tile, targetedCell, tilemap);
        }
        else
        {
            breakScript.ResetBreaking();
        }

        if (activeTile != null && !ItemManager.Instance.ItemGrabbed &&
            (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1) ||
            Vector2.Distance(transform.position, activeTile.transform.position) > reach * 2))
        {
            if (activeUI != null) activeUI.SetActive(false);
            activeUI = null;
            activeTile = null;
        }
    }
}
