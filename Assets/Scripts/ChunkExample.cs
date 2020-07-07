using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class ChunkExample : MonoBehaviour
{
    #region Internal references
    private MeshFilter _myMF;
    private MeshRenderer _myMR;
    private Mesh _myMesh;

    private Vector3[] _verts;
    private int[] _tris;
    private Vector2[] _uVs;
    private Vector3[] _normals;
    
    private Vector3[] _vertsApplied;
    private int[] _trisApplied;
    private Vector2[] _uVsApplied;
    private Vector3[] _normalsApplied;
    private Vector3[] borderVertices;

    private int sizeSquare;
    private int _totalVertInd;
    private int _totalTrisInd;
    private int _totalVertIndApplied;
    private int _totalTrisIndApplied;
    
    private CubeTreadmill CubeTreadmill;
    private GameObject _treadmill;

    #endregion

    private void Awake()
    {
        _myMF = gameObject.AddComponent<MeshFilter>();
        _myMR = gameObject.AddComponent<MeshRenderer>();
        
        _treadmill = GameObject.Find("Treadmill");
        CubeTreadmill = _treadmill.GetComponent<CubeTreadmill>();
        sizeSquare = CubeTreadmill.sizeSquare + 2;

        _myMesh = new Mesh();
    }

    private void Start()
    {
        _Init();
        _CalcMesh();
        _ApplyMesh();
    }

    private void _Init()
    {
        //for the whole chunk
        _totalVertInd = (sizeSquare + 1) * (sizeSquare + 1);
        _totalTrisInd = (sizeSquare) * (sizeSquare) * 2 * 3;//number of triangle vertices
        
        //for the central of the chunk
        _totalVertIndApplied = (sizeSquare) * (sizeSquare);
        _totalTrisIndApplied = (sizeSquare - 1) * (sizeSquare - 1) * 2 * 3;
        
        _verts = new Vector3[_totalVertInd];
        _vertsApplied = new Vector3[_totalVertIndApplied];
        
        _tris = new int[_totalTrisInd];
        _trisApplied = new int[_totalTrisIndApplied];
        
        _uVs = new Vector2[_totalVertInd];

        _normals = new Vector3[_totalVertInd];
        _normalsApplied = new Vector3[_totalVertIndApplied];
    }


    private void _CalcMesh()
    {
        //put all the vertices onto a map
        int[,] vertexIndiceMap = new int[sizeSquare + 1, sizeSquare + 1];
      
        #region Vertices
        int meshVertexIndex = 1;
        int boarderVertexIndex = -1;
        
        //deciding the height of each point according to perlin noise
        for (int z = 0; z <= sizeSquare; z++)
        {
            for (int x = 0; x <= sizeSquare; x++)
            {
                _verts[(z * (sizeSquare + 1)) + x] =
                    new Vector3(x,
                         5 * Perlin.Noise(
                            10 * ((float) x + transform.position.x) / sizeSquare, 
                            10 * ((float) z + transform.position.z) / sizeSquare),
                        z);

                //find all the vertices on the border
                if (z == sizeSquare|| x == sizeSquare|| z == 0 || x == 0)
                {
                    vertexIndiceMap[x, z] = boarderVertexIndex;
                    boarderVertexIndex--;
                }
                else
                {
                    vertexIndiceMap[x, z] = meshVertexIndex;
                    meshVertexIndex++;
                }
                
                //put the vertices in the central into the applied vertex list   
                if (vertexIndiceMap[x, z] > 0)
                {
//                    print("x = " + x + " z = " + z);
//                    print("(z - 1) * (sizeSquare - 1) + x - 1 = " + ((z - 1) * (sizeSquare - 1) + x - 1));
//                    print("(z * (sizeSquare + 1)) + x = " + ((z * (sizeSquare + 1)) + x));

                    _vertsApplied[(z - 1) * (sizeSquare - 1) + x - 1] = _verts[(z * (sizeSquare + 1)) + x];

                }
            }
        }
        #endregion

        #region Tris
        //caculating triangles
        int _triInd = 0;
        int _triIndApplied = 0;

        for (int i = 0; i < sizeSquare - 1; i++)
        {
            for (int j = 0; j < sizeSquare - 1; j++)
            {
                //find all the triangles in the central area and put them into the applied list
                if (vertexIndiceMap[j, i] > 0)
                {
                    int a = j - 1;
                    int b = i - 1;
                    int bottomLeft = a + (b * (sizeSquare - 1)); // true as long as j < sizesquare - 1
                    int bottomRight = a + (b * (sizeSquare - 1)) + 1; // true as long as j < sizesquare -1
                    int topLeft = a + ((b + 1) * (sizeSquare - 1));
                    int topRight = a + ((b + 1) * (sizeSquare - 1)) + 1;

                    _trisApplied[_triIndApplied] = bottomLeft;
                    //print("bottomLeft = " + bottomLeft);
                    _triIndApplied++;
                    _trisApplied[_triIndApplied] = topLeft;
                    _triIndApplied++;
                    _trisApplied[_triIndApplied] = bottomRight;
                    _triIndApplied++;
                    _trisApplied[_triIndApplied] = topLeft;
                    _triIndApplied++;
                    _trisApplied[_triIndApplied] = topRight;
                    _triIndApplied++;
                    _trisApplied[_triIndApplied] = bottomRight;
                    _triIndApplied++;

                }
            }
        }
        
        //calculating the whole chunk's triangles
        for (int i = 0; i < sizeSquare; i++)
        {
            for (int j = 0; j < sizeSquare; j++)
            {
                int bottomLeftW = j + (i * (sizeSquare + 1)); // true as long as j < sizesquare - 1
                int bottomRightW = j + (i * (sizeSquare + 1)) + 1; // true as long as j < sizesquare -1
                int topLeftW = j + ((i + 1) * (sizeSquare + 1));
                int topRightW = j + ((i + 1) * (sizeSquare + 1)) + 1;

                _tris[_triInd] = bottomLeftW;
                _triInd++;
                _tris[_triInd] = topLeftW;
                _triInd++;
                _tris[_triInd] = bottomRightW;
                _triInd++;
                _tris[_triInd] = topLeftW;
                _triInd++;
                _tris[_triInd] = topRightW;
                _triInd++;
                _tris[_triInd] = bottomRightW;
                _triInd++;
            }
        }
        #endregion

        #region Normals
        //calculating 
        for (int i = 0; i < _totalTrisInd / 3; i++)//loop as many times as the number of triangles- geometrical triangles, not tris
        {
            
            int normalTriangleIndex = i;//
            int vertexIndexA = _tris[normalTriangleIndex * 3];
            int vertexIndexB = _tris[normalTriangleIndex * 3 + 1];
            int vertexIndexC = _tris[normalTriangleIndex * 3 + 2];
  
            //get the weighted average
            Vector3 triangleNormal1 = SurfaceNormal(vertexIndexA, vertexIndexB, vertexIndexC);
        
            _normals[vertexIndexA] += triangleNormal1;
            _normals[vertexIndexB] += triangleNormal1;
            _normals[vertexIndexC] += triangleNormal1;
            
        }
        
        for (int i = 0; i < _totalVertInd; i++)
        {
            _normals[i] = _normals[i].normalized;
        }
      
        //get the normals in the central area
        for (int i = 0; i <= sizeSquare; i++)
        {
            for (int j = 0; j <= sizeSquare; j++)
            {
                //find the central vertices and apply
                if (vertexIndiceMap[j, i] > 0)
                {
                    _normalsApplied[(i - 1) * (sizeSquare - 1) + j - 1] = _normals[(i * (sizeSquare + 1)) + j];
                }
            }
        }
        #endregion

    }

    private Vector3 SurfaceNormal(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = (indexA < 0) ? borderVertices[-indexA - 1]:_verts[indexA];
        Vector3 pointB = (indexB < 0) ? borderVertices[-indexB - 1]:_verts[indexB];
        Vector3 pointC = (indexC < 0) ? borderVertices[-indexC - 1]:_verts[indexC];
 
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        
        return Vector3.Cross(sideAB, sideAC).normalized;
    }
    
    
    private void _ApplyMesh()
    {
        _myMesh.vertices = _vertsApplied;
        _myMesh.triangles = _trisApplied;
        _myMesh.normals = _normalsApplied;
        
        _myMF.mesh = _myMesh;

        _myMR.material = Resources.Load<Material>("MyMat");
    }

}