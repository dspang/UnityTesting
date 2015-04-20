using UnityEngine;
using System.Collections;

public abstract class MergeBlock : MonoBehaviour
{
    //base on which this block rests (element 0) and upper parents
    //(elements 1-4, element 1 being the upper block that generated this merge block)
    protected TerrainBlock[] parents;

    //base name of the prefab set used to generate this merge block
    protected string prefabBase;

    //determines what should be done with this merge block after the destruction of a parent
    public void evaluate(TerrainBlock lostParent)
    {
        int count = 0;
        //count upper parents remaining after the loss of this one
        for (int i = 1; i < 5; i++)
        {
            if (parents[i] == lostParent) //remove the lost parent from consideration while we're at it
                parents[i] = null;

            if (parents[i] != null)
                count++;
        }
        //if the base is gone or it has no remaining upper parents, simply remove it
        if (parents[0] == null || count == 0)
        {
            dispose();
            return;
        }

        if (parents[1] == null) //if the primary parent is gone, we should re-create this block with a new primary parent
            for (int i = 2; i < 5; i++) //select one of the other non-primary parents
            {
                if (parents[i] != null)
                {
                    parents[i].setupMergeBlocks(prefabBase); //use our existing prefab type
                    return;
                }
            }

        //Primary parent and base are present, so one of the secondary upper parents has changed.
        //Select primary parent and have it re-generate its merges.
        parents[1].setupMergeBlocks(prefabBase);
    }

    //destroys this merge block
    public void dispose()
    {
        PHT.removeBlock(this);
        for (int i = 0; i < 5; i++)
        {
            if (parents[i] != null)
                parents[i].removeMergeBlock(this);
        }

        //delete this merge's gameObject
        Destroy(gameObject);
    }

    public static PositionHashTree PHT = new PositionHashTree();

    /*Pass this function the name of a MergeBlock prefab, the desired position,
    and the set of blocks to be the new merge block's parents and base.
    Element 0 is the base, elements 1-4 are upper parents and elements 0 and 1 must be non-null.
    Element 1 must be the block trying to generate this merge block*/
    public static MergeBlock create(string prefabName, string prefabFullName, Vector3 position, TerrainBlock[] parentSet)
    {
        if (parentSet[0] == null || parentSet[1] == null)
        {
            Debug.LogError("Failed to initialize merge block. Missing base or primary parent.");
            return null;
        }

        GameObject block = (GameObject)Instantiate(Resources.Load(prefabFullName));
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
        mb.initialize(prefabName, parentSet);
        return mb;
    }

    //treat this like a constructor and call it from the creation function only
    public void initialize(string p_prefabBase, TerrainBlock[] p_parents)
    {
        prefabBase = p_prefabBase;
        parents = p_parents;
        PHT.addBlock(this);
    }
}
