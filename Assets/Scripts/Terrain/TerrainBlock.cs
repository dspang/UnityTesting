using UnityEngine;
using System.Collections;

public abstract class TerrainBlock : MonoBehaviour
{
    protected bool mergeEnabled = true; //only particular terrain blocks should not merge

    public static GameObject initialize(string prefabName, Vector3 position)
    {
        GameObject block = (GameObject)Instantiate(Resources.Load(prefabName));
        position.x = Mathf.Floor(position.x);
        position.y = Mathf.Floor(position.y);
        position.z = Mathf.Floor(position.z);
        block.transform.position = position;
        return block;
    }
}
