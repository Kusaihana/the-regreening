using System;
using TMPro;
using UnityEngine;

public class TileColorSetter : MonoBehaviour
{
    public void ShowRain()
    {
        GetComponent<MeshRenderer>().material.SetInt("_Rain", 1);
        Invoke("HideRain", 5f);
    }
    private void HideRain()
    {
        GetComponent<MeshRenderer>().material.SetInt("_Rain", 0);
    }
    
    public void UpdateTileColors()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        var vertices = mesh.vertices;
        var colors = new Color[vertices.Length];

        for (var i = 0; i < vertices.Length; i++)
        {
            var vertexPosition = transform.TransformPoint(vertices[i]);
            
            RaycastHit hit;
            if (Physics.Raycast(vertexPosition + Vector3.up * 10f, Vector3.down, out hit, 100f))
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