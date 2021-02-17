using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public int chunkSize;
    public float perlinRes;
    
    public float buildingHeight;
    public float buildingThreshold;
    public float buildingStart;

    public GameObject road;
    public GameObject pavement;

    public GameObject[] buildingBase;
    public GameObject[] buildingBlock;

    private Quaternion[] axisRotation;
    private Vector3[] rotationAdjustment;

    private GameObject chunkTemplate;
    private GameObject buildingTemplate;

    private GameObject[,] mappedChunks;
    public int mappedSize;

    // Start is called before the first frame update
    void Start()
    {
        this.axisRotation = new Quaternion[]{
            Quaternion.AngleAxis(0, Vector3.up),
            Quaternion.AngleAxis(90, Vector3.up),
            Quaternion.AngleAxis(180, Vector3.up),
            Quaternion.AngleAxis(270, Vector3.up)
        };
        this.rotationAdjustment = new Vector3[]{
            Vector3.zero,
            Vector3.back,
            Vector3.back + Vector3.left,
            Vector3.left
        };
        this.chunkTemplate = new GameObject("chunk");
        this.buildingTemplate = new GameObject("building");

        this.mappedChunks = new GameObject[this.mappedSize, this.mappedSize];
        for (int x = 0; x < this.mappedSize; x++)
        {
            for (int z = 0; z < this.mappedSize; z++)
            {
                this.mappedChunks[x,z] = GenChunk(x,z);
            }
        }
    }

    GameObject GenChunk(int ChunkX, int ChunkZ)
    {   
        GameObject chunk = Instantiate(this.chunkTemplate,
            this.transform.position
            + (Vector3.right * ChunkX * this.chunkSize)
            + (Vector3.forward * ChunkZ * this.chunkSize),
            this.transform.rotation
        );
        chunk.transform.SetParent(this.transform);

        for (int z = 0; z < this.chunkSize; z++) {
            GameObject roadPiece = Instantiate(this.road,
                chunk.transform.position + (Vector3.back * z),
                chunk.transform.rotation
            );
            roadPiece.transform.SetParent(chunk.transform);
        }
        for (int x = 1; x < this.chunkSize - 1; x++)
        {
            {
                GameObject roadPiece = Instantiate(this.road,
                    chunk.transform.position + (Vector3.left * x),
                    chunk.transform.rotation
                );
                roadPiece.transform.SetParent(chunk.transform);
            }
            for (int z = 1; z < this.chunkSize - 1; z++)
            {
                GameObject building = Instantiate(this.buildingTemplate,
                    chunk.transform.position
                    + (Vector3.back * z)
                    + (Vector3.left * x),
                    chunk.transform.rotation
                );
                building.transform.SetParent(chunk.transform);

                GenBuilding(building);
            }
            {
                GameObject roadPiece = Instantiate(this.road,
                    chunk.transform.position
                    + (Vector3.back * (this.chunkSize - 1))
                    + (Vector3.left * x),
                    chunk.transform.rotation
                );
                roadPiece.transform.SetParent(chunk.transform);
            }
        }
        for (int z = 0; z < this.chunkSize; z++) {
            GameObject roadPiece = Instantiate(this.road,
                chunk.transform.position
                + (Vector3.back * z)
                + (Vector3.left * (this.chunkSize - 1)),
                chunk.transform.rotation
            );
            roadPiece.transform.SetParent(chunk.transform);
        }

        return chunk;
    }

    void GenBuilding(GameObject buildingPlot)
    {
        GameObject pavementPiece = Instantiate(this.pavement, buildingPlot.transform);

        float currentHeight = pavementPiece.GetComponentInChildren<MeshFilter>().mesh.bounds.size.y;

        float sample = 
        // Random.Range(0.0f, 1.0f);
        Mathf.PerlinNoise(
            buildingPlot.transform.position.x / this.perlinRes,
            buildingPlot.transform.position.z / this.perlinRes
            );

        float height = this.buildingStart + sample * sample * this.buildingHeight;

        if (height > this.buildingThreshold) {
            int facing = Random.Range(0,4);
            GameObject basePiece = Instantiate(
                this.buildingBase[Random.Range(0, this.buildingBase.Length)],
                buildingPlot.transform.position
                + (Vector3.up * currentHeight)
                + this.rotationAdjustment[facing],
                this.axisRotation[facing]
            );
            basePiece.transform.SetParent(buildingPlot.transform);
            currentHeight += basePiece.GetComponentInChildren<MeshFilter>().mesh.bounds.size.y;
            while (currentHeight < height) {
                facing = Random.Range(0,4);
                GameObject buildingPiece = Instantiate(
                    this.buildingBlock[Random.Range(0, this.buildingBlock.Length)],
                    buildingPlot.transform.position
                    + (Vector3.up * currentHeight)
                    + this.rotationAdjustment[facing],
                    this.axisRotation[facing]
                );
                buildingPiece.transform.SetParent(buildingPlot.transform);
                currentHeight += buildingPiece.GetComponentInChildren<MeshFilter>().mesh.bounds.size.y;
            }
        }
    }
}
