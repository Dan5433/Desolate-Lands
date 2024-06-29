using CustomExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Interact : MonoBehaviour
{
    Vector3Int targetedCell;
    GameObject activeUI;
    GameObject activeTile;
    float offsetForCell = 0.01f;
    Break breakScript;
    [SerializeField] float reach;

    void Awake()
    {
        breakScript = GetComponent<Break>();
    }

    void Update()
    {
        LayerMask mask = LayerMask.GetMask("Solid");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, reach, mask);

        if (hit)
        {
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

            if (activeUI == null && Input.GetMouseButtonDown(1))
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

            if (Input.GetMouseButton(0))
            {
                BreakableTile tile = tilemap.GetTile<BreakableTile>(targetedCell);
                breakScript.Breaking(tile, targetedCell, tilemap);
            }
            else
            {
                breakScript.ResetBreaking();
            }
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
