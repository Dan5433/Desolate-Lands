using UnityEngine;
using UnityEngine.Tilemaps;


public class Interact : MonoBehaviour
{
    GameObject activeUI;
    GameObject activeTile;
    PlayerDamage damageScript;
    [SerializeField] float reach;
    [SerializeField] Transform origin;
    [SerializeField] InventoryBase inventory;
    [SerializeField] PlayerCrafting crafting;

    public bool IsUIActive => activeUI != null;

    void Awake()
    {
        damageScript = GetComponent<PlayerDamage>();
    }

    public void DisableUI()
    {
        if (activeUI != null) 
            activeUI.SetActive(false);

        activeUI = null;
        activeTile = null;
    }

    void Update()
    {
        if (GameManager.IsGamePaused)
            return;

        if (CanDisableUI())
        {
            DisableUI();
            return;
        }

        LayerMask mask = LayerMask.GetMask("Interact");
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin.position, origin.up, reach, mask);

        Debug.DrawRay(origin.position, origin.up, Color.green, reach);

        foreach (var hit in hits)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (hit.collider.TryGetComponent<Gatherable>(out var gatherable) &&
                    gatherable.State == GatherableTileState.Replenished)
                {
                    GatherTile(gatherable);
                    break;
                }

                if (hit.collider.TryGetComponent<Container>(out var container))
                {
                    OpenContainer(container);
                    break;
                }

                if (hit.collider.TryGetComponent<CraftStation>(out var craftStation))
                {
                    OpenCraftStation(craftStation);
                    break;
                }

                if (hit.collider.TryGetComponent<PrototypeStation>(out var prototypeStation))
                {
                    OpenPrototypeStation(prototypeStation);
                    break;
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (!hit.collider.TryGetComponent<IDamageable>(out var damageable) ||
                    ItemManager.Instance.IsHoldingItem || activeUI ||
                    ItemManager.Instance.IsTooltipActive)
                    continue;

                damageScript.DealDamage(hit, damageable);
                break;
            }
        }

        if(!Input.GetMouseButton(0) || hits.Length == 0)
            damageScript.ResetCooldown();
    }

    void GatherTile(Gatherable gatherable)
    {
        gatherable.State = 0;

        GatherableTile tile = gatherable.GetComponentInParent<Tilemap>()
            .GetTile<GatherableTile>(gatherable.TilePosition);

        foreach (var drop in tile.Drops)
        {
            int count = drop.RandomCount();
            if (count == 0)
                continue;

            inventory.AddToInventory(InventoryItemFactory.Create(drop.item, count));
        }

        inventory.UpdateUI();
        inventory.SaveInventory();
    }

    void OpenCraftStation(CraftStation station)
    {
        if (station.CraftingUi.activeSelf)
            return;

        station.UpdateCraftingUI();
        station.CraftingUi.SetActive(true);
        station.UpdateUI();

        UpdateUI(station.gameObject, station.CraftingUi);

    }

    void OpenPrototypeStation(PrototypeStation station)
    {
        if (station.CraftingUi.activeSelf)
            return;

        station.UpdateAvailablePrototypesUI(inventory.Inventory, crafting.Resources);
        station.CraftingUi.SetActive(true);
        UpdateUI(station.gameObject, station.CraftingUi);
    }

    void OpenContainer(Container container)
    {
        if (container.UI.gameObject.activeSelf)
            return;

        container.UI.GetComponent<InventoryRef>().Inventory = container;
        container.GenInventory();
        container.UpdateUI();
        container.UI.gameObject.SetActive(true);

        UpdateUI(container.gameObject, container.UI.gameObject);

    }
    bool CanDisableUI()
    {
        if (activeTile == null)
            return false;

        if (Input.GetKeyDown(KeyCode.Escape))
            return true;

        if (Vector2.Distance(transform.position, activeTile.transform.position) > reach * 2)
            return true;

        if (!ItemManager.Instance.IsTooltipActive && !ItemManager.Instance.IsHoldingItem &&
            Input.GetMouseButtonDown(1))
            return true;

        return false;
    }

    public void UpdateUI(GameObject tile, GameObject ui)
    {
        activeTile = tile;
        activeUI = ui;
    }
}
