using UnityEngine;
using System.Collections;

public abstract class MergeBlock : MonoBehaviour {
    //upper blocks which generated this merge block
    protected TerrainBlock[] parents;
    //lower block on which this merge block rests
    protected TerrainBlock mergebase;

    public void dispose()
    {
        PHT.removeBlock(this);
        for (int i = 0; i < 5; i++)
        {
            if (parents[i] != null)
                parents[i].removeMergeBlock(this);
        }
        Destroy(this.gameObject);
    }

    public static PositionHashTree PHT = new PositionHashTree();

    /*Pass this function the name of a MergeBlock prefab, the desired position,
    and the set of blocks to be the new merge block's parents and base.
    Element 0 is the base, elements 1-4 are upper parents and elements 0 and 1 must be non-null.
    Element 1 must be the block trying to generate this merge block*/
    public static MergeBlock initialize(string prefabName, string[] conflictEval, Vector3 position, TerrainBlock[] parentSet)
    {
        //set up full prefab name for each merge block
        string[] prefabFullName = new string[4];
        for (int i = 0; i < 4; i++)
        {
            prefabFullName[i] = prefabName + conflictEval[i];
        }

        if (parentSet[0] == null || parentSet[1] == null)
        {
            Debug.LogError("Failed to initialize merge block. Missing base or primary parent.");
            return null;
        }

        GameObject block = (GameObject)Instantiate(Resources.Load(prefabName));
        position.x = Mathf.Floor(position.x);
        position.y = Mathf.Floor(position.y);
        position.z = Mathf.Floor(position.z);
        block.transform.position = position;
        Vector3 noYComponent = new Vector3(1, 0, 1);
        block.transform.rotation.SetFromToRotation(Vector3.Scale(position, noYComponent),
            Vector3.Scale(parentSet[1].transform.position, noYComponent));
        Component c = block.GetComponent(prefabName);
        if (!(c is MergeBlock))
        {
            Debug.LogError("Tried to instantiate a MergeBlock using a non-MergeBlock prefab (did you forget to assign the prefab a MergeBlock script?)");
            return null;
        }
        MergeBlock mb = (MergeBlock)c;
        mb.parents = parentSet;
        PHT.addBlock(mb);
        return mb;
    }
}
