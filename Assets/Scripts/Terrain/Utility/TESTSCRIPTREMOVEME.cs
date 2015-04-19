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
        PositionHashTree PHT = TerrainBlock.PHT;
        PHT.addBlock(disblock1);
        PHT.addBlock(disblock2);
        PHT.addBlock(disblock3);
        PHT.addBlock(disblock4);
        PHT.addBlock(disblock5);
        PHT.addBlock(disblock6);
        PHT.addBlock(disblock7);
        PHT.addBlock(disblock8);

        TerrainBlock[] tbList = {disblock1, disblock2, disblock3, 
                                 disblock4, disblock5, disblock6, 
                                 disblock7, disblock8};
        for (int i = 0; i < 8; i++)
        {
            tbList[i].setupMergeBlocks("TestMergeBlock");
        }
    }
}
