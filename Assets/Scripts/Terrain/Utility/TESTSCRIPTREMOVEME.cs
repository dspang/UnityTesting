using UnityEngine;
using System.Collections;

public class TESTSCRIPTREMOVEME : MonoBehaviour
{
    public TerrainBlock disblock1;
    public TerrainBlock disblock2;
    public TerrainBlock disblock3;
    public TerrainBlock disblock4;
    public TerrainBlock disblock5;
    public TerrainBlock disblock6;
    public TerrainBlock disblock7;
    public TerrainBlock disblock8;

    // Use this for initialization
    void Start()
    {
        TerrainBlock[] tbList = {disblock1, disblock2, disblock3, 
                                 disblock4, disblock5, disblock6, 
                                 disblock7, disblock8};
        for (int i = 0; i < 8; i++)
        {
            TerrainBlock.PHT.addBlock(tbList[i]);
        }
        for (int i = 0; i < 8; i++)
        {
            tbList[i].setupMergeBlocks("TestMergeBlock");
        }
        StartCoroutine(corout());
    }

    IEnumerator corout()
    {
        yield return new WaitForSeconds(1);
    }
}
