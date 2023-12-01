using System;
using TMPro;
using UnityEngine;

public class TileColorSetter : MonoBehaviour
{
    [SerializeField] private MeshRenderer _rainPlane;
    
    public void ShowRain()
    {
        GetComponent<MeshRenderer>().material.SetInt("_Rain", 1);
        _rainPlane.material.SetInt("_Rain", 1);
        
        Invoke("HideRain", 5f);
    }
    private void HideRain()
    {
        GetComponent<MeshRenderer>().material.SetInt("_Rain", 0);
        _rainPlane.material.SetInt("_Rain", 0);
    }
    
    public void UpdateTileColors()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        var vertices = mesh.vertices;

        var colors = new Color[vertices.Length];

        for (var i = 0; i < vertices.Length; i++)
        {
            var vertexPosition = transform.TransformPoint(vertices[i]);

            var ray = new Ray(vertexPosition + new Vector3(0, 2, 0), -transform.up);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
            {
                SoilTile tile = hit.collider.GetComponent<SoilTile>();
                if (tile != null)
                {
                    colors[i] = GetColorFromLandType(tile.landType);
                }
            }
        }

        mesh.colors = colors;
    }

    Color GetColorFromLandType(LandType landType)
    {
        switch (landType)
        {
            case LandType.Desert:
                return new Color(1,0,0,0);
            case LandType.Alive:
                return new Color(0,1,0,0);
            case LandType.Vivid:
                return new Color(0,0,1,0);
            case LandType.Lush:
                return new Color(0,0,0,1);
            default:
                return Color.white;
        }
    }
}