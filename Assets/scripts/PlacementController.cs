using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PlacementController : MonoBehaviour
{
    public static PlacementController Instance { get; private set; }
    public static PlacementController I { get; private set; }

    [Header("Tilemap References")]
    [SerializeField] private Tilemap oceanMap;   // Hydro only
    [SerializeField] private Tilemap groundMap;  // Solar, Wind, Biomass, etc.
    [SerializeField] private Tilemap cityMap;    // Houses, Shops

    [Header("Scene refs")]
    [SerializeField] private Grid grid;
    [SerializeField] private LayerMask blockedMask;
    [SerializeField] private float ghostAlpha = 0.6f;

    private EnergySourceSO currentSourceSO;
    private ConsumerBuildingSO currentConsumerSO;
    private GameObject ghost;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    public void StartPlacing(EnergySourceSO so)
    {
        CancelPlacement();
        currentSourceSO = so;
        currentConsumerSO = null;

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
        // PLACEMENT MODE
        if (currentSourceSO != null || currentConsumerSO != null)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = grid.WorldToCell(mouseWorld);
            Vector3 centre = grid.CellToWorld(cell) + new Vector3(0.5f, 0.5f);
            ghost.transform.position = centre;

            // Basic blocked-check
            bool invalid = Physics2D.OverlapPoint(centre, blockedMask);

            // Then layer‐specific rules
            if (!invalid)
            {
                if (currentSourceSO != null)
                {
                    bool isHydro = currentSourceSO.displayName.ToLower().Contains("hydro");
                    if (isHydro)
                        invalid = oceanMap == null || !oceanMap.HasTile(cell);
                    else
                        invalid = groundMap == null || !groundMap.HasTile(cell);
                }
                else
                {
                    invalid = cityMap == null || !cityMap.HasTile(cell);
                }
            }

            var sr = ghost.GetComponent<SpriteRenderer>();
            sr.color = invalid
                ? new Color(1, 0, 0, ghostAlpha)
                : new Color(1, 1, 1, ghostAlpha);

            // Place on left-click
            if (Input.GetMouseButtonDown(0) && !invalid)
            {
                if (currentSourceSO != null)
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

            // Cancel placement
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
                return;
            }
        }
        // DELETE MODE
        else if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero, 0f, blockedMask);
            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Buildings"))
            {
                var target = hit.collider.gameObject;
                var src = target.GetComponent<EnergySourceInstance>();
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
                Destroy(target);
            }
        }
    }

    void CancelPlacement()
    {
        currentSourceSO = null;
        currentConsumerSO = null;
        if (ghost) Destroy(ghost);
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
        // pick prefab
        GameObject go;
        if (so is EnergySourceSO es)
            go = Instantiate(es.prefab, pos, Quaternion.identity);
        else // Consumer
            go = Instantiate((so as ConsumerBuildingSO).prefab, pos, Quaternion.identity);

        // get grid coords
        var cell = grid.WorldToCell(pos);
        int x = cell.x, y = cell.y;

        // add correct component, store CellX/Y, register
        if (so is EnergySourceSO eso)
        {
            var inst = go.AddComponent<EnergySourceInstance>();
            inst.data = eso;
            inst.SetCell(x, y);
            if (register) PowerManager.I.RegisterSource(inst);
        }
        else if (so is ConsumerBuildingSO cso)
        {
            var inst = go.AddComponent<ConsumerInstance>();
            inst.data = cso;
            inst.SetCell(x, y);
            if (register) PowerManager.I.RegisterConsumer(inst);
        }
    }
}
