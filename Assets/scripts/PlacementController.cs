using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using TMPro;
using System.Linq;

public class PlacementController : MonoBehaviour
{
    public static PlacementController I { get; private set; }

    [Header("Tilemap References")]
    [SerializeField] private Tilemap oceanMap;   // Hydro only
    [SerializeField] private Tilemap groundMap;  // Solar, Wind, Biomass, etc.
    [SerializeField] private Tilemap cityMap;    // Houses, Shops

    private bool canPlaceMorePowerPlants = true;

        

    [Header("Floating Text Prefab")]
    [SerializeField] private FloatTextUI floatTextPrefab;
    [SerializeField] private Canvas uiCanvas;

    [Header("Scene refs")]
    [SerializeField] private Grid grid;
    [SerializeField] private LayerMask blockedMask;
    [SerializeField] private float ghostAlpha = 0.6f;

    [Header("Placement Limit")]
    [Tooltip("0 = unlimited")]
    public int maxPlantsAllowed = 0;


    private EnergySourceSO currentSourceSO;
    private ConsumerBuildingSO currentConsumerSO;
    private GameObject ghost;

    void Awake()
    {
        if (I != null && I != this) Destroy(gameObject);
        I = this;
    }

    public void StartPlacing(EnergySourceSO so)
    {
        CancelPlacement();
        currentSourceSO = so;
        currentConsumerSO = null;

        // ─── Enforce per-type limit ───────────────────────────────────
        if (so.maxPlantsAllowed > 0)
        {
            int existing = PowerManager.I.CountType(so);
            if (existing >= so.maxPlantsAllowed)
                return;  // refuse placement (silent)
        }

        if (!GameManager.I.CanAfford(so.buildCost))
        {
            StartCoroutine(FlashMoneyLabel());
            return;
        }

        CreateGhost(so.sprite, so.prefab.transform.localScale);
    }

    public void StartPlacing(ConsumerBuildingSO cs)
    {
        CancelPlacement();
        currentConsumerSO = cs;
        currentSourceSO = null;
        CreateGhost(cs.sprite, cs.prefab.transform.localScale);
    }

    private void CreateGhost(Sprite sprite, Vector3 scale)
    {
        ghost = new GameObject("Ghost");
        var sr = ghost.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerName = "Buildings";
        sr.color = new Color(1, 1, 1, ghostAlpha);
        ghost.transform.localScale = scale;
    }

    void Update()
    {
        if (GameManager.I.tutorialActive) return;

        if (currentSourceSO != null || currentConsumerSO != null)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            var mouseW = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var cell = grid.WorldToCell(mouseW);
            var centre = grid.CellToWorld(cell) + new Vector3(0.5f, 0.5f);
            ghost.transform.position = centre;

            // Basic blocked-check
            bool invalid = Physics2D.OverlapPoint(centre, blockedMask);

            // Layer-specific rules
            if (!invalid)
            {
                if (currentSourceSO != null)
                {
                    bool isHydro = currentSourceSO.displayName.ToLower().Contains("hydro");
                    invalid = isHydro
                        ? !oceanMap.HasTile(cell)
                        : !groundMap.HasTile(cell);
                }
                else
                {
                    invalid = !cityMap.HasTile(cell);
                }
            }

            ghost.GetComponent<SpriteRenderer>().color =
                invalid ? new Color(1, 0, 0, ghostAlpha)
                        : new Color(1, 1, 1, ghostAlpha);

            // Place on click
            if (Input.GetMouseButtonDown(0) && !invalid)
            {
                if (currentSourceSO != null)
                {
                    if (currentSourceSO.maxPlantsAllowed > 0)
                    {
                        int alreadyPlaced = Object.FindObjectsByType<EnergySourceInstance>(FindObjectsSortMode.None)
                                .Count(i => i.data == currentSourceSO);
                        if (alreadyPlaced >= currentSourceSO.maxPlantsAllowed * 2)
                        { 
                        canPlaceMorePowerPlants = false;
                        }
                        if (alreadyPlaced >= currentSourceSO.maxPlantsAllowed * 2)
                        {
                            Debug.LogWarning(
                              $"Cannot place more than {currentSourceSO.maxPlantsAllowed} " +
                              $"{currentSourceSO.displayName} on the island."
                            );
                          
                        }
                    }
                    if (canPlaceMorePowerPlants)
                    {
                        var go = Instantiate(currentSourceSO.prefab, centre, Quaternion.identity);
                        GameManager.I.Spend(currentSourceSO.buildCost);
                        var src = go.AddComponent<EnergySourceInstance>();
                        src.data = currentSourceSO;
                        PowerManager.I.RegisterSource(src);

                        ResilienceManager.I.AddScores(
                            currentSourceSO.sec,
                            currentSourceSO.eq,
                            currentSourceSO.sus,
                            currentSourceSO.ada
                        );

                        // Floating text
                        Vector3 worldPos = go.transform.position + Vector3.up * 0.5f;
                        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
                        var ftxt = Instantiate(floatTextPrefab, uiCanvas.transform);
                        ftxt.GetComponent<RectTransform>().position = screenPos;
                        ftxt.Init($"{src.CurrentOutputMW():0.0} MW");
                    }
                }
                else
                {
                    var go = Instantiate(currentConsumerSO.prefab, centre, Quaternion.identity);
                    GameManager.I.Spend(currentConsumerSO.buildCost);
                    var con = go.AddComponent<ConsumerInstance>();
                    con.data = currentConsumerSO;
                    PowerManager.I.RegisterConsumer(con);
                }

                CancelPlacement();
                return;
            }

            // Cancel placement with right-click
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
                return;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // Delete mode
            var mouseW = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mouseW, Vector2.zero, 0f, blockedMask);
            if (hit.collider != null &&
                hit.collider.gameObject.layer == LayerMask.NameToLayer("Buildings"))
            {
                var src = hit.collider.GetComponent<EnergySourceInstance>();
                if (src != null)
                {
                    ResilienceManager.I.AddScores(
                        -src.data.sec,
                        -src.data.eq,
                        -src.data.sus,
                        -src.data.ada
                    );
                    GameManager.I.Spend(-src.data.buildCost);
                }
                Destroy(hit.collider.gameObject);
            }
        }
    }

    void CancelPlacement()
    {
        Destroy(ghost);
        currentSourceSO = null;
        currentConsumerSO = null;
    }



    private IEnumerator FlashMoneyLabel()
    {
        var txt = GameManager.I.moneyText;
        var orig = txt.color;
        txt.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        txt.color = orig;
    }

    public void InstantiateFromSO(ScriptableObject so, Vector3 pos, bool register)
    {
        GameObject go;
        if (so is EnergySourceSO eso)
        {
            go = Instantiate(eso.prefab, pos, Quaternion.identity);
            var inst = go.AddComponent<EnergySourceInstance>();
            inst.data = eso;
            if (register) PowerManager.I.RegisterSource(inst);
        }
        else
        {
            go = Instantiate((so as ConsumerBuildingSO).prefab, pos, Quaternion.identity);
            var inst = go.AddComponent<ConsumerInstance>();
            inst.data = (ConsumerBuildingSO)so;
            if (register) PowerManager.I.RegisterConsumer(inst);
        }
    }
}
