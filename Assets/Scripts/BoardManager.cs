using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
        public CellObject ContainedObject;
    }

    private CellData[,] m_BoardData;
    private Tilemap m_Tilemap;
    private Grid m_Grid;
    private List<Vector2Int> m_EmptyCellsList;

    // Object pools
    private ObjectPool<WallObject> m_WallPool;
    private ObjectPool<ExitCellObject> m_ExitPool;
    private ObjectPool<Enemy> m_EnemyPool;
    private List<ObjectPool<FoodObject>> m_FoodPools;
    private bool m_PoolsInitialized;

    [SerializeField, Range(4, 20), Tooltip("Board width in cells")]
    private int m_Width = 8;
    [SerializeField, Range(4, 20), Tooltip("Board height in cells")]
    private int m_Height = 8;
    [SerializeField, Tooltip("Ground tile variants")]
    private Tile[] m_GroundTiles;
    [SerializeField, Tooltip("Wall tile variants")]
    private Tile[] m_WallTiles;
    [SerializeField, Tooltip("Food prefab variants")]
    private GameObject[] m_FoodPrefabs;
    [SerializeField, Tooltip("Destructible wall prefab")]
    private WallObject m_WallPrefab;
    [SerializeField, Tooltip("Exit cell prefab")]
    private ExitCellObject m_ExitCellPrefab;
    [SerializeField, Tooltip("Enemy prefab")]
    private Enemy m_EnemyPrefab;

    public int Width => m_Width;
    public int Height => m_Height;

    private void InitPools()
    {
        if (m_PoolsInitialized) return;

        m_WallPool = new ObjectPool<WallObject>(m_WallPrefab, 10, transform);
        m_ExitPool = new ObjectPool<ExitCellObject>(m_ExitCellPrefab, 1, transform);
        m_EnemyPool = new ObjectPool<Enemy>(m_EnemyPrefab, 3, transform);

        m_FoodPools = new List<ObjectPool<FoodObject>>();
        foreach (var prefab in m_FoodPrefabs)
        {
            var foodComp = prefab.GetComponent<FoodObject>();
            m_FoodPools.Add(new ObjectPool<FoodObject>(foodComp, 3, transform));
        }

        m_PoolsInitialized = true;
    }

    public void Init()
    {
        InitPools();

        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();
        m_EmptyCellsList = new List<Vector2Int>();

        m_BoardData = new CellData[m_Width, m_Height];

        for (int y = 0; y < m_Height; ++y)
        {
            for (int x = 0; x < m_Width; ++x)
            {
                Tile tile;
                m_BoardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == m_Width - 1 || y == m_Height - 1)
                {
                    tile = m_WallTiles[Random.Range(0, m_WallTiles.Length)];
                    m_BoardData[x, y].Passable = false;
                }
                else
                {
                    tile = m_GroundTiles[Random.Range(0, m_GroundTiles.Length)];
                    m_BoardData[x, y].Passable = true;
                    m_EmptyCellsList.Add(new Vector2Int(x, y));
                }

                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        m_EmptyCellsList.Remove(new Vector2Int(1, 1));

        Vector2Int endCoord = new Vector2Int(m_Width - 2, m_Height - 2);
        AddObject(m_ExitPool.Get(), endCoord);
        m_EmptyCellsList.Remove(endCoord);

        GenerateWall();
        GenerateFood();
        GenerateEnemies();
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= m_Width
            || cellIndex.y < 0 || cellIndex.y >= m_Height)
        {
            return null;
        }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    void GenerateWall()
    {
        int wallCount = Random.Range(6, 10);
        for (int i = 0; i < wallCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            WallObject newWall = m_WallPool.Get();
            AddObject(newWall, coord);
        }
    }

    void GenerateFood()
    {
        int foodCount = 5;
        for (int i = 0; i < foodCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            int poolIndex = Random.Range(0, m_FoodPools.Count);
            FoodObject newFood = m_FoodPools[poolIndex].Get();
            AddObject(newFood, coord);
        }
    }

    void GenerateEnemies()
    {
        int enemyCount = Random.Range(1, 3);
        for (int i = 0; i < enemyCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            Enemy newEnemy = m_EnemyPool.Get();
            AddObject(newEnemy, coord);
        }
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return m_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }

    void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }

    public void Clean()
    {
        if (m_BoardData == null)
            return;

        for (int y = 0; y < m_Height; ++y)
        {
            for (int x = 0; x < m_Width; ++x)
            {
                var cellData = m_BoardData[x, y];

                if (cellData.ContainedObject != null)
                {
                    cellData.ContainedObject.gameObject.SetActive(false);
                    cellData.ContainedObject = null;
                }

                SetCellTile(new Vector2Int(x, y), null);
            }
        }
    }
}
