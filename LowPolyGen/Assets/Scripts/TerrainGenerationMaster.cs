using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerationMaster : MonoBehaviour
{ 
    public const string TerrainTag = "Terrain";

    void Start()
    {
        GameObject[] terrainsGobjs = GameObject.FindGameObjectsWithTag(TerrainTag);
        List<Terrain> terrains = new List<Terrain>();
        foreach (GameObject go in terrainsGobjs)
        {
            Terrain terrain = go.GetComponent<Terrain>();
            if(terrain != null)
            {
                terrains.Add(terrain);
            }
        }

        foreach (Terrain terrain in terrains)
        {
            terrain.GenerateTerrain();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
