using UnityEngine;
using System.Collections;

public abstract class TerrainBlock : MonoBehaviour
{
    public bool mergeEnabled = true; //blocks are mergeable by default

    //public only for testing; should be made protected once this class is complete
    public static PositionHashTree PHT = new PositionHashTree();

    //contains all merge blocks that are a child of this TerrainBlock
    public ArrayList merges = new ArrayList();

    //destroys this terrain block
    public void dispose()
    {
        PHT.removeBlock(this);
        //inform children merges that they should re-evaluate after the loss of this block
        while (merges.Count != 0)
        {
            ((MergeBlock)merges[0]).evaluate(this);
            merges.RemoveAt(0);
        }

        //delete the gameObject hosting this script
        Destroy(gameObject);
    }

    public static GameObject create(string prefabName, Vector3 position)
    {
        GameObject block = (GameObject)Instantiate(Resources.Load(prefabName));
        position.x = Mathf.Floor(position.x);
        position.y = Mathf.Floor(position.y);
        position.z = Mathf.Floor(position.z);
        block.transform.position = position;
        Component c = block.GetComponent(prefabName);
        if (!(c is TerrainBlock))
        {
            Debug.LogError("Tried to instantiate a TerrainBlock using a non-TerrainBlock prefab" +
                "(did you forget to assign the prefab a TerrainBlock script?)");
        }
        TerrainBlock tb = (TerrainBlock)c;
        tb.initialize();
        return block;
    }

    public void initialize()
    {
        PHT.addBlock(this);
    }

    //Handles creating merge blocks for this terrain block
    public void setupMergeBlocks(string prefabBase)
    {
        //don't try to evaluate merge blocks if this block can't merge
        if (!mergeEnabled)
            return;

        TerrainBlock[] mergeableSet = findMergeableBlocks(); //find set of blocks to merge with
        string[] conflictEval = evaluateMergeConflicts(mergeableSet); //figure out any conflicts
        generateMergeBlocks(mergeableSet, conflictEval, prefabBase); //generate the appropriate merge blocks
    }

    /*Returns mergeable blocks for this block in the first 4 elements of the array.
    Returns other blocks that can merge with each mergeable in the next 12 members
    such that each mergeable's potential mergers follow the pattern:
    (mergeable blocks 1, 2, 3, 4 have potential mergers a, b, c, d respectively)
    array: 1, 2, 3, 4, a, a, a, b, b, b, c, c, c, d, d, d
    Note that any of the mergeable blocks could be null (if there is no mergeable block
    in that position), and any potential merger could be null as well.
    */
    protected TerrainBlock[] findMergeableBlocks()
    {
        int x = (int)(transform.position.x);
        int y = (int)(transform.position.y);
        int z = (int)(transform.position.z);

        TerrainBlock[] blocks = new TerrainBlock[16];

        //down and to each of the four sides one unit
        int[] xCoords = { x, x, x + 1, x - 1 };
        int[] yCoords = { y - 1, y - 1, y - 1, y - 1 };
        int[] zCoords = { z + 1, z - 1, z, z };

        /**************************************************
         * Mergeable set (elements 0-3)
        **************************************************/
        for (int i = 0; i < 4; i++)
        {
            //find a block, if any, in that position
            if((blocks[i] = (TerrainBlock)PHT.findBlock(new Vector3(xCoords[i], yCoords[i], zCoords[i]))) != null)
                if (!blocks[i].mergeEnabled) //can't merge with us?
                    blocks[i] = null; //then for the purpose of merging, this block does not exist
        }

        //search above each of the mergeable blocks for a block that would get in the way
        for (int i = 0; i < 4; i++)
        {
            if (blocks[i] != null)
            {
                if (PHT.findBlock(blocks[i].transform.position + new Vector3(0, 1, 0)) != null)
                    blocks[i] = null; //remove this mergeable from consideration if so
            }
        }

        /**************************************************
         * Conflict set (elements 4-15)
        **************************************************/
        //search for other blocks that could merge with this mergeable
        int offset = 0;
        for (int i = 0; i < 4; i++)
        {
            if (blocks[i] == null)
                continue;
            Vector3[] positions = calcOtherMergerPositions(blocks[i]);
            for (int j = 0; j < 3; j++)
            {
                offset = (i * 3) + 4 + j;
                if ((blocks[offset] = (TerrainBlock)PHT.findBlock(positions[j])) != null)
                    if (!blocks[offset].mergeEnabled) //can't conflict with a block that cannot merge
                        blocks[offset] = null; //so don't consider it
            }
        }

        return blocks;
    }

    //pass mergeable TerrainBlock to this function
    //returns positions of other blocks than "this" that can merge with the passed one
    protected Vector3[] calcOtherMergerPositions(TerrainBlock b)
    {
        int x = (int)(b.transform.position.x);
        int y = (int)(b.transform.position.y);
        int z = (int)(b.transform.position.z);
        float rel = Mathf.Atan2(z, x) - Mathf.Atan2(transform.position.z, transform.position.x);
        Vector3[] positions = new Vector3[3];
        /*start out looking straight across (0 degrees relatively speaking),
         * then looking to the left (+90 degrees relative),
         * then to the right (+180 degrees relative, 270 degrees absolute) */
        float[] offset = { 0.50f * Mathf.PI, 1.0f * Mathf.PI, 0f };
        for (int i = 0; i < 3; i++)
        {
            positions[i] = new Vector3(x + Mathf.Cos(rel), y + 1, z + Mathf.Sin(rel));
            rel += offset[i];
        }

        return positions;
    }

    //pass this function a mergeable block evaluation (array of 16 TerrainBlocks)
    //returns string detailing which directions the conflicts come from
    //(center, left, and right are the possible directions)
    protected static string[] evaluateMergeConflicts(TerrainBlock[] blocks)
    {
        string[] evaluations = new string[4];
        for (int i = 0; i < 4; i++)
        {
            if (blocks[i] == null)
                continue;
            int offset = 4 + (i * 3);
            if (blocks[offset] != null)
                evaluations[i] += "center";
            if (blocks[offset + 1] != null)
                evaluations[i] += "left";
            if (blocks[offset + 2] != null)
                evaluations[i] += "right";
        }

        return evaluations;
    }

    //pass this function a mergeable block evaluation, a conflict evaluation, and the desired type of merge block
    protected void generateMergeBlocks(TerrainBlock[] mergeableEval, string[] conflictEval, string prefabBase)
    {
        for (int i = 0; i < 4; i++)
        {
            TerrainBlock[] parents = new TerrainBlock[5];
            parents[0] = mergeableEval[i];
            parents[1] = this;
            int offset = (i * 3) + 4;
            for (int j = 0; j < 3; j++)
            {
                parents[j+2] = mergeableEval[offset + j];
            }

            if (mergeableEval[i] == null)
                continue;
            Vector3 pos = mergeableEval[i].transform.position;
            pos.y += 1;
            MergeBlock priorBlock;
            //destroy any existing merge blocks in this location
            if ((priorBlock = (MergeBlock)MergeBlock.PHT.findBlock(pos)) != null)
            {
                priorBlock.dispose();
            }
            //create the new merge block and add it to the list of child blocks for this block
            MergeBlock newBlock = (MergeBlock)MergeBlock.create(prefabBase, prefabBase + conflictEval[i], pos, parents);
            if (newBlock == null) //failed to create this merge block. 
            { //error that caused the failure should be logged in the merge block creation code; don't do it here
                continue;
            }
            merges.Add(newBlock);
            //as well as any other blocks that should be made aware of this
            for (int j = 4; j < 16; j++)
            {
                if (mergeableEval[j] == null || mergeableEval[j].merges.Contains(newBlock))
                    continue;
                mergeableEval[j].merges.Add(newBlock);
            }
        }
    }

    public void removeMergeBlock(MergeBlock block)
    {
        merges.Remove(block);
    }
}
