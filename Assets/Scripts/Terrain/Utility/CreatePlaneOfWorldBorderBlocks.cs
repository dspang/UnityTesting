using UnityEngine;
using System.Collections;

public class CreatePlaneOfWorldBorderBlocks : MonoBehaviour
{

    void Start()
    {
        createBlocks();
    }
    static public void createBlocks()
    {
        for (int i = -25; i <= 25; i++)
            for (int j = -25; j <= 25; j++)
            {
                TerrainBlock.create("WorldBorderBlock", new Vector3(i, 0, j)); //get pointer to new WorldBorderBlock?
            }
    }
}
