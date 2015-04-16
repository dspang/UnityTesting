using UnityEngine;
using System.Collections;

public class TestDisabledBlock : TerrainBlock
{
    void Start()
    {
        mergeEnabled = false;
    }
}
