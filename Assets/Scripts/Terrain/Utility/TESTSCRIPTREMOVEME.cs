using UnityEngine;
using System.Collections;

public class TESTSCRIPTREMOVEME : MonoBehaviour {
    public TerrainBlock disblock1;
    public TerrainBlock disblock2;
    public TerrainBlock disblock3;
    public TerrainBlock disblock4;
    public TerrainBlock disblock5;
    public TerrainBlock disblock6;
    public TerrainBlock disblock7;
    public TerrainBlock disblock8;
	// Use this for initialization
	void Start () {
        PositionHashTree HT = new PositionHashTree(disblock1);
        HT.addBlock(disblock2);
        HT.addBlock(disblock3);
        HT.addBlock(disblock4);
        Debug.Log(HT.removeBlock(disblock5.transform.position));
        Debug.Log(HT.removeBlock(disblock4.transform.position));
        HT.addBlock(disblock4);
        HT.addBlock(disblock5);
        HT.addBlock(disblock6);
        HT.addBlock(disblock7);
        HT.addBlock(disblock8);
        Debug.Log(HT.findBlock(disblock4.transform.position));
        Debug.Log(HT.findBlock(disblock6.transform.position));
	}
}
