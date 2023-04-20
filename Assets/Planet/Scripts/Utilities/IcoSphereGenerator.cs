using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IcoSphereGenerator
{
    public const int LimitLevel = 6; // 6 - 40.962   7 - 163.842 (no uv)
    private const int Cash = 16; // 16 - 65.536

    public static Mesh Generate (int level, bool uvLayout = false)
    {
        if (level < 0)
            throw new Exception("IcoSphere level cannot be negative");
        if (level > LimitLevel)
            throw new Exception(LimitLevel+" is max level for "+Cash+" bit cash");

        ZeroIcoData data = uvLayout ? ZeroIcoData.UV : ZeroIcoData.NoUV;
        List<TriangleVetex> vertices = data.vertices;
        Triangle[] triangles = data.triangles;

        triangles = LevelUp(vertices, triangles, level);

        return GetMesh(vertices.ToArray(), triangles, "IcosphereL"+level);
    }

    /*
     *          A(0)
     *         ○
     *        / \
     *       /  ↘\
     *      CA ←— \ AB
     *     ○−−−−−−−○
     *    / \ —→  / \
     *   /  ↘\↖  /  ↘\
     *  C(2)— \ BC ←— \ B(1)
     * ○−−−−−−−○−−−−−−−○
     */
    private static Triangle[] LevelUp (List<TriangleVetex> zeroVertices, Triangle[] zeroTriangles, int targetLevel)
    {
        if (targetLevel == 0)
            return zeroTriangles;
        Dictionary<int, int> midPointCache = new Dictionary<int, int>();
        for (int l = 0; l < targetLevel; l++)
        {
            zeroVertices.Capacity = zeroVertices.Capacity*3;
            Triangle[] newTriangles = new Triangle[zeroTriangles.Length*4];

            for (int i = 0; i < zeroTriangles.Length; i++)
            {
                Triangle triangle = zeroTriangles[i];

                int ai = triangle.Vertices[0];
                int bi = triangle.Vertices[1];
                int ci = triangle.Vertices[2];

                int abi = GetMidPointIndex(midPointCache, ai, bi, zeroVertices);
                int bci = GetMidPointIndex(midPointCache, bi, ci, zeroVertices);
                int cai = GetMidPointIndex(midPointCache, ci, ai, zeroVertices);

                int index = i*4;
                newTriangles[index]   = new Triangle(ai,  abi, cai);
                newTriangles[index+1] = new Triangle(abi, bci, cai);
                newTriangles[index+2] = new Triangle(cai, bci, ci);
                newTriangles[index+3] = new Triangle(abi, bi,  bci);
            }
            zeroTriangles = newTriangles;
        }
        return zeroTriangles;
    }

    private static int GetMidPointIndex (Dictionary<int, int> cache, int indexA, int indexB, List<TriangleVetex> vertices)
    {
        int smallerIndex = Mathf.Min(indexA, indexB);
        int greaterIndex = Mathf.Max(indexA, indexB);
        int key = (smallerIndex << Cash)+greaterIndex;

        if (cache.TryGetValue(key, out int ret))
            return ret;

        Vector3 position = Vector3.Lerp(vertices[indexA].position, vertices[indexB].position, 0.5f).normalized;
        Vector2 uv = Vector2.Lerp(vertices[indexA].uv, vertices[indexB].uv, 0.5f);

        ret = vertices.Count;
        vertices.Add(new TriangleVetex(position, uv));
        cache.Add(key, ret);
        return ret;
    }

    private static Mesh GetMesh (TriangleVetex[] vertices, Triangle[] triangles, string name)
    {
        int count = vertices.Length;
        Vector3[] position = new Vector3[count];
        Vector2[] uv = new Vector2[count];
        for (int i = 0; i < count; i++)
        {
            position[i] = vertices[i].position;
            uv[i] = vertices[i].uv;
        }
        Mesh mesh = new Mesh();
        mesh.name = name;
        mesh.vertices = position;
        mesh.uv = uv;
        mesh.triangles = GetTrianglesForMesh(triangles);
        mesh.normals = position;
        return mesh;
    }

    private static int[] GetTrianglesForMesh (Triangle[] triangles)
    {
        int[] meshTriangles = new int[triangles.Length*3];
        int i = 0;
        foreach (Triangle triangle in triangles)
        {
            meshTriangles[i+0] = triangle.Vertices[0];
            meshTriangles[i+1] = triangle.Vertices[1];
            meshTriangles[i+2] = triangle.Vertices[2];
            i += 3;
        }
        return meshTriangles;
    }
    private struct Triangle
    {
        /// <summary> 0-a, 1-b, 2-c </summary>
        public readonly int[] Vertices;
        
        public Triangle (int indexA, int indexB, int indexC)
        {
            Vertices = new int[] { indexA, indexB, indexC };
        }
    }

    private struct TriangleVetex
    {
        public Vector3 position;
        public Vector2 uv;

        public TriangleVetex (Vector3 position, Vector2 uv)
        {
            this.position = position;
            this.uv = uv;
        }

        public TriangleVetex (Vector3 position) : this(position, Vector2.zero) { }
    }

    private struct ZeroIcoData
    {
        public List<TriangleVetex> vertices;
        public Triangle[] triangles;

        public ZeroIcoData (TriangleVetex[] vertices, Triangle[] triangles)
        {
            this.vertices = vertices.ToList();
            this.triangles = triangles;
        }

        public static ZeroIcoData NoUV => new ZeroIcoData(GetVerticesNoUV(), GetTrianglesNoUV());
        public static ZeroIcoData UV => new ZeroIcoData(GetVerticesUV(), GetTrianglesUV());

        /* Vertices
         *      0       0       0       0       0           n       f*0.5+2 20*4^l
         *     ○       ○       ○       ○       ○            level   vertex  face   
         *    / \     / \     / \     / \     / \           0       12      20
         *   /  ↘\   /   \   /   \   /   \   /   \          1       42      80               
         *  1  ←— \ 2     \ 3     \ 4     \ 5     \ 1       2       162     320    
         * ○−−−−−−−○−−−−−−−○−−−−−−−○−−−−−−−○−−−−−−−○        3       642     1280
         *  \ —→  / \     / \     / \     / \     / \       4       2564    5120
         *   \↖  /   \   /   \   /   \   /   \   /   \      5       10242   20480
         *    \ 6     \ 7     \ 8     \ 9     \ 10    \ 6   6       40962   81920
         *     ○−−−−−−−○−−−−−−−○−−−−−−−○−−−−−−−○−−−−−−−○    7       163842  327680
         *      \     / \     / \     / \     / \     /     8       655362  1310720
         * —−-○  \   /   \   /   \   /   \   /   \   /
         * ↑↓→←   \ 11    \ 11    \ 11    \ 11    \ 11
         * ↗↙↘↖    ○       ○       ○       ○       ○
         */
        private static TriangleVetex[] GetVerticesNoUV ()
        {
            float H_ANGLE = Mathf.PI/180*72f;
            float V_ANGLE = Mathf.Atan(1.0f/2f);
            float z = Mathf.Sin(V_ANGLE);
            float xy = Mathf.Cos(V_ANGLE);
            float hAngle1 = -Mathf.PI/2f-H_ANGLE/2f;
            float hAngle2 = -Mathf.PI/2f;

            TriangleVetex[] vertices = new TriangleVetex[12];
            vertices[0] = new TriangleVetex(new Vector3(0, 1, 0));
            for (int i = 1; i < 6; ++i)
            {
                vertices[i] = new TriangleVetex(new Vector3(xy*Mathf.Cos(hAngle1), z, xy*Mathf.Sin(hAngle1)));
                vertices[i+5] = new TriangleVetex(new Vector3(xy*Mathf.Cos(hAngle2), -z, xy*Mathf.Sin(hAngle2)));

                hAngle1 += H_ANGLE;
                hAngle2 += H_ANGLE;
            }
            vertices[11] = new TriangleVetex(new Vector3(0, -1, 0));
            return vertices;
        }

        private static Triangle[] GetTrianglesNoUV ()
        {
            Triangle[] Triangles = new Triangle[20];
            for (int i = 0; i < 5; i++)
            {
                int r1a = i+1;     // 1 2 3 4 5
                int r1b = r1a%5+1; // 2 3 4 5 1
                int r2a = r1a+5;   // 6 7 8 9 10
                int r2b = r1b+5;   // 7 8 9 10 6
                Triangles[i]    = new Triangle(0,   r1b, r1a);
                Triangles[i+5]  = new Triangle(r2a, r1a, r1b);
                Triangles[i+10] = new Triangle(r1b, r2b, r2a);
                Triangles[i+15] = new Triangle(11,  r2a, r2b);
            }
            return Triangles;
        }

        /* UV
         *              4     10    16      n               20*4^l
         *             ○−−−−−○−−−−−○        level   vertex  face
         *              \   / \   / \       0       22      20
         *           3   \ 9   \ 15  \ 21   1       63      80
         *          ○−−−−−○−−−−−○−−−−−○     2       205     320
         *           \   / \   / \          3       729     1280
         *        2   \ 8   \ 14  \ 20      4       2737    5120
         *       ○−−−−−○−−−−−○−−−−−○        5       10593   20480
         *        \   / \   / \             6       41665   81920
         *     1   \ 7   \ 13  \ 19         
         *    ○−−−−−○−−−−−○−−−−−○           
         *     \   / \   / \                
         *  0   \ 6   \ 12  \ 18            
         * ○−−−−−○−−−−−○−−−−−○              0 > 0-4
         *  \   / \   / \                   1-2-3-4-5-1 > 5-10
         *   \ 5   \ 11  \ 17               6-7-8-9-10-6 > 11-16
         *    ○−−−−−○−−−−−○                 11 > 17-21
         */
        private static TriangleVetex[] GetVerticesUV ()
        {
            float h = 0.8660254f; // sqrt(3)/2 height of equilateral triangle with lenght 1
            float l = 0.2f; // scale
            float hl = h*l;
            float botV = 0.5f-hl*2.5f;
            float[] v = { botV, botV+hl, botV+hl*2f, botV+hl*3f, botV+hl*4f, botV+hl*5f };
            float[] rightU = new float[6];
            rightU[0] = 0.5f-l*2.75f;
            for (int i = 1; i < 6; i++)
                rightU[i] = rightU[0]+l*(i*0.5f);
            int[] index =
            {
                0,  0,  0,  0,  0,
                1,  2,  3,  4,  5,  1,
                6,  7,  8,  9,  10, 6,
                11, 11, 11, 11, 11
            };

            Vector2[] uv = new Vector2[22];
            for (int i = 0; i < 5; i++)
            {
                uv[i] = new Vector2(rightU[i+1], v[i+1]);
                uv[17+i] = new Vector2(rightU[i]+l*3f, v[i]);
            }
            for (int i = 0; i < 6; i++)
            {
                uv[5+i] = new Vector2(rightU[i]+l, v[i]);
                uv[11+i] = new Vector2(rightU[i]+l*2f, v[i]);
            }

            TriangleVetex[] verticesNoUV = GetVerticesNoUV();
            TriangleVetex[] vertices = new TriangleVetex[22];
            for (int i = 0; i < 22; i++)
                vertices[i] = new TriangleVetex(verticesNoUV[index[i]].position, uv[i]);
            return vertices;
        }

        private static Triangle[] GetTrianglesUV ()
        {
            Triangle[] Triangles = new Triangle[20];
            for (int i = 0; i < 5; i++)
            {
                Triangles[i]    = new Triangle(i,    i+6,  i+5);
                Triangles[i+5]  = new Triangle(i+11, i+5,  i+6);
                Triangles[i+10] = new Triangle(i+6,  i+12, i+11);
                Triangles[i+15] = new Triangle(i+17, i+11, i+12);
            }
            return Triangles;
        }
    }
}
