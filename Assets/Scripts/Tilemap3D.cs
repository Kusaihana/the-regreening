using UnityEngine;

public class Tilemap3D : MonoBehaviour
{
    public GameObject tilePrefab;
    public int gridSize = 10;

    void Start()
    {
        GenerateSoilMap();
    }

    private void GenerateSoilMap()
    {
        for (var x = 0; x < gridSize; x += 2)
        {
            for (var y = 0; y < gridSize; y += 2)
            {
                var tilePosition = new Vector3(x, y, 0);
                var soilTileObject = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                var soilTile = soilTileObject.GetComponent<SoilTile>();

                if (soilTile != null)
                {
                    //everything starts as dessert
                    soilTile.SetSoilProperties(LandType.Dessert);
                    soilTileObject.transform.parent = transform;
                }
            }
        }
    }
}