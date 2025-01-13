using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]//编辑的模式下进行运行
public class GridMap : MonoBehaviour
{
    public MapData_SO mapData;
    public GridType gridType;
    //private Tilemap currentTilemap;

    // private void OnDisable()
    // {
    //     if (!Application.IsPlaying(this))
    //     {
    //         //currentTilemap = GetComponent<Tilemap>();

    //         if (mapData != null)
    //         {
    //             mapData.tileProperties.Clear();
    //         }
    //     }
    // }

    private void OnEnable()
    {
        //Debug.Log("qingnianjingd");
        if (!Application.IsPlaying(this))
        {
            if (mapData != null)
            {
                mapData.tileProperties.Clear();
            }
            //currentTilemap = GetComponent<Tilemap>();
            //Debug.Log("qingnianjingd");
            UpdateTileProperties();
#if UNITY_EDITOR
            if (mapData != null)
            {
                EditorUtility.SetDirty(mapData);
            }
#endif
        }
    }

    private void UpdateTileProperties()
    {
        //currentTilemap.CompressBounds();

        if (!Application.IsPlaying(this))
        {
            if (mapData != null)
            {
                //已绘制范围的左下角坐标
                //Vector3Int startPos = currentTilemap.cellBounds.min;
                //已绘制范围的右上角坐标
                //Vector3 endPos = currentTilemap.cellBounds.max;
                foreach (Transform item in transform)
                {
                    if (item != null)
                    {
                        var pos =item.transform.position;
                        TileProperty newTile = new TileProperty
                        {
                            worldPos  = new Vector3(pos.x, pos.y,pos.z),
                            gridType = this.gridType,
                            boolTypeValue = true
                        };

                        mapData.tileProperties.Add(newTile);
                    }
                }
                // for (int x = startPos.x; x < endPos.x; x++)
                // {
                //     for (int y = startPos.y; y < endPos.y; y++)
                //     {
                //         TileBase tile = currentTilemap.GetTile(new Vector3Int(x, y, 0));

                //         if (tile != null)
                //         {
                //             TileProperty newTile = new TileProperty
                //             {
                //                 tileCoordinate = new Vector2Int(x, y),
                //                 gridType = this.gridType,
                //                 boolTypeValue = true
                //             };

                //             mapData.tileProperties.Add(newTile);
                //         }
                //     }
                // }
            }
        }
    }
}
