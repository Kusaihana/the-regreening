using UnityEngine;

public class Tilemap3D : MonoBehaviour
{
    public GameObject tilePrefab;
    public int gridSize = 100;
    
    private TileColorSetter _tileColorSetter;

    private void Awake()
    {
        _tileColorSetter = FindObjectOfType<TileColorSetter>();
        GenerateSoilMap();
    }

    private void GenerateSoilMap()
    {
        for (var x = 0; x < gridSize; x += 5)
        {
            for (var z = 0; z < gridSize; z += 5)
            {
                var tilePosition = new Vector3(x, 0, z);
                var soilTileObject = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                var soilTile = soilTileObject.GetComponent<SoilTile>();

                if (soilTile != null)
                {
                    //everything starts as dessert
                    soilTile.SetSoilProperties(LandType.Desert);
                    soilTileObject.transform.parent = transform;
                    soilTileObject.transform.localPosition = tilePosition;
                }
            }
        }
        _tileColorSetter.UpdateTileColors();
    }
}