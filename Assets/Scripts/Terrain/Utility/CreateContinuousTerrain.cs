using UnityEngine;
using System.Collections;

public class CreateContinuousTerrain : MonoBehaviour
{
    void Start()
    {
        generateTerrain(100, 100, 5);
    }

    void generateTerrain(int width, int length, int height)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                int h = Random.Range(1, height+1); //min is inclusive, max is exclusive
                for (int k = 0; k < h; k++)
                {
                    TerrainBlock.create("TestDisabledBlock", new Vector3(i, k, j));
                }
            }
        }
    }
}
