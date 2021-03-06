using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    //UnityEditor.GradientWrapperJSON:{"gradient":{"serializedVersion":"2","key0":{"r":0.34433960914611819,"g":0.653507649898529,"b":1.0,"a":1.0},"key1":{"r":0.6650943756103516,"g":0.8231133222579956,"b":1.0,"a":1.0},"key2":{"r":0.7219665050506592,"g":0.8531513810157776,"b":1.0,"a":0.0},"key3":{"r":0.8521851897239685,"g":1.0,"b":0.4575471878051758,"a":0.0},"key4":{"r":0.5066992044448853,"g":0.8584905862808228,"b":0.17412781715393067,"a":0.0},"key5":{"r":0.5058823823928833,"g":0.8588235378265381,"b":0.1725490242242813,"a":0.0},"key6":{"r":0.0,"g":0.0,"b":0.0,"a":0.0},"key7":{"r":0.0,"g":0.0,"b":0.0,"a":0.0},"ctime0":11951,"ctime1":17155,"ctime2":28527,"ctime3":35273,"ctime4":44911,"ctime5":65535,"ctime6":0,"ctime7":0,"atime0":0,"atime1":65535,"atime2":0,"atime3":0,"atime4":0,"atime5":0,"atime6":0,"atime7":0,"m_Mode":0,"m_NumColorKeys":6,"m_NumAlphaKeys":2}}
    //UnityEditor.GradientWrapperJSON:{"gradient":{"serializedVersion":"2","key0":{"r":0.34433960914611819,"g":0.653507649898529,"b":1.0,"a":1.0},"key1":{"r":0.6650943756103516,"g":0.8231133222579956,"b":1.0,"a":1.0},"key2":{"r":0.7219665050506592,"g":0.8531513810157776,"b":1.0,"a":0.0},"key3":{"r":0.8521851897239685,"g":1.0,"b":0.4575471878051758,"a":0.0},"key4":{"r":0.5066992044448853,"g":0.8584905862808228,"b":0.17412781715393067,"a":0.0},"key5":{"r":0.5058823823928833,"g":0.8588235378265381,"b":0.1725490242242813,"a":0.0},"key6":{"r":0.0,"g":0.0,"b":0.0,"a":0.0},"key7":{"r":0.0,"g":0.0,"b":0.0,"a":0.0},"ctime0":11951,"ctime1":17155,"ctime2":28527,"ctime3":35273,"ctime4":44911,"ctime5":65535,"ctime6":0,"ctime7":0,"atime0":0,"atime1":65535,"atime2":0,"atime3":0,"atime4":0,"atime5":0,"atime6":0,"atime7":0,"m_Mode":0,"m_NumColorKeys":6,"m_NumAlphaKeys":2}}
    [SerializeField] private Gradient color;
    [SerializeField] private GameObject tile;
    [SerializeField] private GameObject tileCollider;
    [SerializeField] private GameObject water;
    [SerializeField] private GameObject[] props;
    [Range(0, 100)] [SerializeField] private float noiseScale;
    [Range(0, 1)] [SerializeField] private float fillPercent;
    [SerializeField] private Vector2 size;
    [SerializeField] private Transform landParent;
    [SerializeField] private Transform propsParent;
    [SerializeField] private Transform waterParent;
    [SerializeField] private BoxCollider floorCollider;
    public int r = 100;

    public void Generate()
    {
        DestroyAll();

        for (int x = 0; x <= size.x; x++)
        {
            for (int y = 0; y <= size.y; y++)
            {
                CreateLandTile(x, y);
            }
        }

        for (int x = -1; x <= size.x; x++)
        {
            CreateTile(tileCollider, x, 0, -1);
            CreateTile(tileCollider, x, 0, (int) size.y + 1);
        }

        for (int y = -1; y <= size.y; y++)
        {
            CreateTile(tileCollider, -1, 0, y);
            CreateTile(tileCollider, (int) size.x + 1, 0, y);
        }

        for (int x = 0; x <= size.x / 10; x++)
        {
            for (int y = 0; y <= size.y / 10; y++)
            {
                GameObject w = PrefabUtility.InstantiatePrefab(water, waterParent) as GameObject;
                w.transform.position = new Vector3(x * 10, -0.1f, y * 10);
            }
        }

        floorCollider.size = new Vector3(size.x + 1, 0.5f, size.y + 1);
        floorCollider.center = new Vector3(size.x * 0.5f, 0, size.y * 0.5f);
    }

    void CreateLandTile(int x, int y)
    {
        float a = Mathf.PerlinNoise((float) x / noiseScale, (float) y / noiseScale);
        float h = a > 1 - (float) fillPercent ? 0 : -0.1f;

        if (a < 1 - (float) fillPercent) //WATER TILE
        {
            CreateTile(tileCollider, x, h, y);
            return;
        }

        //LAND TILE
        GameObject g = CreateTile(tile, x, h, y);
        g.GetComponent<MeshRenderer>().material.color =
            a > 1 - (float) fillPercent ? color.Evaluate(a + 0.3f) : color.Evaluate(a);

        //PROPS
        if (Random.value > 0.95f && a > 1 - (float) fillPercent)
        {
            GameObject l =
                PrefabUtility.InstantiatePrefab(props[Random.Range(0, props.Length)],
                    propsParent) as GameObject;
            l.transform.position = new Vector3(x, 0.2f, y);
            l.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }
    }

    GameObject CreateTile(GameObject tilePref, int x, float h, int y)
    {
        GameObject tile = PrefabUtility.InstantiatePrefab(tilePref, landParent) as GameObject;
        tile.transform.position = new Vector3(x, h, y);
        tile.name = $"{x} : {y}";
        return tile;
    }

    public void DestroyLand()
    {
        DestroyChildren(landParent);
    }

    public void DestroyProps()
    {
        DestroyChildren(propsParent);
    }

    public void DestroyWater()
    {
        DestroyChildren(waterParent);
    }

    public void DestroyAll()
    {
        DestroyLand();
        DestroyProps();
        DestroyWater();
    }

    private void DestroyChildren(Transform t)
    {
        while (t.childCount > 0)
        {
            DestroyImmediate(t.GetChild(0).gameObject);
        }
    }
}