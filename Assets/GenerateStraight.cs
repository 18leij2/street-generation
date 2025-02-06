using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateStraight : MonoBehaviour
{
    public Material road;
    public Material marking;

    private Vector3[] verts;  // the vertices of the mesh
    private int[] tris;       // the triangles of the mesh (triplets of integer references to vertices)
    private int ntris = 0;    // the number of triangles that have been created so far

    void Start()
    {
        Mesh my_mesh = CreateMyMesh();
        Mesh markingOne = CreateMarkingOne();
        Mesh markingTwo = CreateMarkingTwo();

        GameObject s = new GameObject("Object 0");
        s.AddComponent<MeshFilter>();
        s.AddComponent<MeshRenderer>();

        GameObject m1 = new GameObject("Object 1");
        m1.AddComponent<MeshFilter>();
        m1.AddComponent<MeshRenderer>();

        GameObject m2 = new GameObject("Object 2");
        m2.AddComponent<MeshFilter>();
        m2.AddComponent<MeshRenderer>();

        float x = 0;
        float y = 0;
        float z = 0;
        s.transform.localPosition = new Vector3(x, y, z);
        m1.transform.localPosition = new Vector3(x, y, z);
        m2.transform.localPosition = new Vector3(x, y, z);

        s.GetComponent<MeshFilter>().mesh = my_mesh;
        m1.GetComponent<MeshFilter>().mesh = markingOne;
        m2.GetComponent<MeshFilter>().mesh = markingTwo;

        Renderer rend = s.GetComponent<Renderer>();
        rend.material = road;
        Renderer rend1 = m1.GetComponent<Renderer>();
        rend1.material = marking;
        Renderer rend2 = m2.GetComponent<Renderer>();
        rend2.material = marking;
    }

    Mesh CreateMyMesh()
    {
        Mesh mesh = new Mesh();
        verts = new Vector3[4];

        verts[0] = new Vector3(-0.5f, 0.021f, -0.25f); // bottom left
        verts[1] = new Vector3(-0.5f, 0.021f, 0.25f); // top left
        verts[2] = new Vector3(0.5f, 0.021f, 0.25f); // top right
        verts[3] = new Vector3(0.5f, 0.021f, -0.25f); // bottom right

        ntris = 0;
        tris = new int[6];

        MakeQuad(0, 1, 2, 3);

        // use the parent's local position
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = transform.TransformPoint(verts[i]);
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        return mesh;
    }

    Mesh CreateMarkingOne()
    {
        Mesh mesh = new Mesh();
        verts = new Vector3[4];

        verts[0] = new Vector3(-0.375f, 0.022f, -0.01f);
        verts[1] = new Vector3(-0.375f, 0.022f, 0.01f); 
        verts[2] = new Vector3(-0.125f, 0.022f, 0.01f);  
        verts[3] = new Vector3(-0.125f, 0.022f, -0.01f); 

        ntris = 0;
        tris = new int[6];

        MakeQuad(0, 1, 2, 3); 

        // use the parent's local position
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = transform.TransformPoint(verts[i]);
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        return mesh;
    }

    Mesh CreateMarkingTwo()
    {
        Mesh mesh = new Mesh();
        verts = new Vector3[4];

        verts[0] = new Vector3(0.125f, 0.022f, -0.01f);
        verts[1] = new Vector3(0.125f, 0.022f, 0.01f);
        verts[2] = new Vector3(0.375f, 0.022f, 0.01f); 
        verts[3] = new Vector3(0.375f, 0.022f, -0.01f);

        ntris = 0;
        tris = new int[6];

        MakeQuad(0, 1, 2, 3);

        // use the parent's local position
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = transform.TransformPoint(verts[i]);
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        return mesh;
    }

    // Make a triangle from three vertex indices (clockwise order)
    void MakeTri(int i1, int i2, int i3)
    {
        int index = ntris * 3;  // figure out the base index for storing triangle indices
        ntris++;

        tris[index] = i1;
        tris[index + 1] = i2;
        tris[index + 2] = i3;
    }

    // Make a quadrilateral from four vertex indices (clockwise order)
    void MakeQuad(int i1, int i2, int i3, int i4)
    {
        MakeTri(i1, i2, i3);
        MakeTri(i1, i3, i4);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
