using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UIElements;

public class GenerateRoundabout : MonoBehaviour
{
    public Material road;
    public Material yellow;

    private Vector3[] verts;  // the vertices of the mesh
    private int[] tris;       // the triangles of the mesh (triplets of integer references to vertices)
    private int ntris = 0;    // the number of triangles that have been created so far

    // Create the mesh

    void Start()
    {
        // call the routine that makes a cube (the mesh) from scratch
        Mesh my_mesh = CreateMyMesh();
        Mesh innerMesh = CreateInnerCircle();
        Mesh connectMesh = CreateConnect();
        Mesh lineMesh = CreateLine();

        // create a new GameObject and give it a MeshFilter and a MeshRenderer
        GameObject s = new GameObject("Object 0");
        s.AddComponent<MeshFilter>();
        s.AddComponent<MeshRenderer>();
        GameObject t = new GameObject("Object 1");
        t.AddComponent<MeshFilter>();
        t.AddComponent<MeshRenderer>();
        GameObject connect = new GameObject("Object 2");
        connect.AddComponent<MeshFilter>();
        connect.AddComponent<MeshRenderer>();
        GameObject line = new GameObject("Object 3");
        line.AddComponent<MeshFilter>();
        line.AddComponent<MeshRenderer>();
        float x = 0;
        float y = 0;
        float z = 0;
        s.transform.localPosition = new Vector3(x, y, z);
        t.transform.localPosition = new Vector3(x, y, z);
        connect.transform.localPosition = new Vector3(x, y, z);
        line.transform.localPosition = new Vector3(x, y, z);

        // associate the mesh with this object
        s.GetComponent<MeshFilter>().mesh = my_mesh;
        t.GetComponent<MeshFilter>().mesh = innerMesh;
        connect.GetComponent<MeshFilter>().mesh = connectMesh;
        line.GetComponent<MeshFilter>().mesh = lineMesh;

        // change the color of the object
        Renderer rend = s.GetComponent<Renderer>();
        Renderer rend1 = t.GetComponent<Renderer>();
        Renderer rend2 = connect.GetComponent<Renderer>();
        Renderer rend3 = line.GetComponent<Renderer>();

        Vector3 position = s.transform.position;
        float distance = Vector3.Distance(position, Vector3.zero);
        rend1.material.color = new Color(0.13f, 0.13f, 0.13f, 1.0f);
        rend.material = road;
        rend2.material = road;
        rend3.material = yellow;
    }

    // Create a cube that is centered at the origin (0, 0, 0) with sides of length = 2.
    //
    // Although the faces of a cube share corners, we cannot share these vertices
    // because that would mess up the surface normals at the vertices.

    Mesh CreateMyMesh()
    {

        // Create a mesh object
        Mesh mesh = new Mesh();
        verts = new Vector3[34]; // the mesh has 34 vertices, more than 32

        float radius = 0.12f;
        float bottomRadius = 0.4f;

        // vertices for the bottom
        for (int i = 0; i < 16; i++)
        {
            float angle = i * Mathf.PI / 8; // angle for the 16-a-gon
            verts[i] = new Vector3(Mathf.Cos(angle) * bottomRadius, 0.021f, Mathf.Sin(angle) * bottomRadius);
        }

        // vertices for the top
        for (int i = 16; i < 32; i++)
        {
            float angle = i * Mathf.PI / 8; // angle for the 16-a-gon
            verts[i] = new Vector3(Mathf.Cos(angle) * radius, 0.022f, Mathf.Sin(angle) * radius);
        }

        // we reuse the center point for each face, both top and bottom
        verts[32] = new Vector3(0, 0.021f, 0); // bottom
        verts[33] = new Vector3(0, 0.2f, 0); // top

        // store our triangles to construct the shape
        int[] triangles = new int[96];
        tris = triangles;
        // bottom face triangles
        for (int i = 0; i < 16; i++)
        {
            triangles[i * 3] = 32; // center point of shape, will be the same for each triangle;
            triangles[i * 3 + 1] = (i + 1) % 16; // mod so we go back to 0 at the last, since the next vertex will be for the top face
            triangles[i * 3 + 2] = i;
        }

        // use the parent's local position
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = transform.TransformPoint(verts[i]);
        }

        // Assign to mesh
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    // making the roundabout center pin
    Mesh CreateInnerCircle()
    {
        Mesh mesh = new Mesh();
        Vector3[] tempVerts = new Vector3[16];
        int[] triangles = new int[48];

        // triangles for the top face, same ordering since we want both to face up
        for (int i = 0; i < 16; i++)
        {
            tris[(16 + i) * 3] = 33; // center point of shape, will be the same for each triangle
            tris[(16 + i) * 3 + 1] = (i + 1) % 16 + 16; // mod so we go back to 0 at the last, since the next vertex will be for the top face
            tris[(16 + i) * 3 + 2] = i + 16;
        }

        for (int i = 0; i < 48; i++)
        {
            triangles[i] = tris[i + 48];
        }

        // Assign to mesh
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    // making the road connection
    Mesh CreateConnect()
    {
        Mesh mesh = new Mesh();
        verts = new Vector3[4];

        verts[0] = new Vector3(-0.5f, 0.021f, -0.25f);
        verts[1] = new Vector3(-0.5f, 0.021f, 0.25f);
        verts[2] = new Vector3(0f, 0.021f, 0.25f); 
        verts[3] = new Vector3(0f, 0.021f, -0.25f);

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

    // making the yellow line
    Mesh CreateLine()
    {
        Mesh mesh = new Mesh();
        verts = new Vector3[4];

        verts[0] = new Vector3(-0.5f, 0.022f, -0.01f);
        verts[1] = new Vector3(-0.5f, 0.022f, 0.01f);
        verts[2] = new Vector3(0f, 0.022f, 0.01f);
        verts[3] = new Vector3(0f, 0.022f, -0.01f);

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

    // make a triangle from three vertex indices (clockwise order)
    void MakeTri(int i1, int i2, int i3)
    {
        int index = ntris * 3;  // figure out the base index for storing triangle indices
        ntris++;

        tris[index] = i1;
        tris[index + 1] = i2;
        tris[index + 2] = i3;
    }

    // make a quadrilateral from four vertex indices (clockwise order)
    void MakeQuad(int i1, int i2, int i3, int i4)
    {
        MakeTri(i1, i2, i3);
        MakeTri(i1, i3, i4);
    }

    // Update is called once per frame (in this case we don't need to do anything)
    void Update()
    {
    }
}
