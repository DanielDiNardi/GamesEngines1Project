using UnityEngine;

public class perlin : MonoBehaviour
{
    public int depth = 10;

    public int width = 256;
    public int height = 256;

    public float scale = 20f;

    void Start(){
        Debug.Log("Started");
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain(TerrainData terrainData){
        terrainData.size = new Vector3 (width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData;
    }
    
    float[,] GenerateHeights(){
        float[,] heights = new float[width, height];
        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    float CalculateHeight(int x, int y){
        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / height * scale;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}
