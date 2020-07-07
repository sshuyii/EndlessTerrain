using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeTreadmill : MonoBehaviour
{    
    #region Public Reference
    
    public GameObject target;
    public GameObject ChunkExample;
    public int sizeSquare;//sizeSquare is set in this script instead of in ChunkExample
    public int _horizontalSize;
    public int _verticalSize;
    
    #endregion
    
    #region Internal Reference
    private List<GameObject> _chunks;
    private List<Vector2> _chunkGrid = new List<Vector2>();//positions of the chunks
    private List<Vector2> _chunkGenerate = new List<Vector2>();//positions of chunks that need to be generated
    private List<Vector2> _chunkDestroy = new List<Vector2>();//positions of chunks that need to be destroied
    
    private Dictionary<Vector2, GameObject> _chunkExample = new Dictionary<Vector2, GameObject>();
  
    private Vector2 _oldCameraPos;//camera position projected to grids
    private Vector2 _currentCameraPos;    
    
    private List<Vector2> _cameraViewList;
    private List<Vector2> _chunksGridList;
    private GameObject _chunkToDestroy;

    #endregion
    
    void Start()
    {
        //Generating ChunkExamples
        _chunks = new List<GameObject>();

        for (int i = 0; i < _verticalSize; i++)
        {
            for (int j = (0 - _horizontalSize)/2; j < _horizontalSize/2; j++)
            {
                _chunks.Add(Instantiate(ChunkExample, new Vector3(target.transform.position.x + j * sizeSquare, 0, target.transform.position.z + i * sizeSquare), Quaternion.identity));
            }
        }
        
        
    }

    void Update()
    {
        
        //only do calculation when camera moves from one grid to another
        _currentCameraPos = UpdateCameraPos();
        if (_oldCameraPos != _currentCameraPos)//if the camera moved
        {
            _cameraViewList = GetCameraView(_currentCameraPos);//get the grids where there should be chunks
            _chunksGridList = ConvertChunkToGrid();//convert current chunk positions to grid coordinations
            DestroyGenerateCal(_cameraViewList, _chunksGridList);//do calculation
            
            //Destroy chunks
            foreach (Vector2 v in _chunkDestroy)
            {
                _chunkExample.TryGetValue(v, out _chunkToDestroy);
            
                Destroy(_chunkToDestroy);
                _chunks.Remove(_chunkToDestroy);

            }


            //Generate chunks
            for (int i = 0; i < _chunkGenerate.Count; i++)
            {
                _chunks.Add(Instantiate(ChunkExample, new Vector3(_chunkGenerate[i].x * sizeSquare, 0, _chunkGenerate[i].y * sizeSquare), Quaternion.identity));
            }
            _chunkGenerate.Clear();
        }


        _oldCameraPos = UpdateCameraPos();

    }


    //convert camera position to grid coordinations
    private Vector2 UpdateCameraPos()
    {
        //get the coordinate of the camera
        int _currentCamCoordX = (int)(target.transform.position.x / sizeSquare); //currently sizeSquare = 8
        int _currentCamCoordZ = (int)(target.transform.position.z / sizeSquare);

        Vector2 _currentCameraPos = new Vector2(_currentCamCoordX, _currentCamCoordZ);
        return _currentCameraPos;
    }


    //convert chunks positions to grid coordinations
    private List<Vector2> ConvertChunkToGrid()
    {
        //put all the coordinations of chunks into this list
        _chunkGrid = new List<Vector2>();

        foreach (GameObject g in _chunks)
        {
            int _gridX = (int)(g.transform.position.x / sizeSquare);
            int _gridY = (int)(g.transform.position.z / sizeSquare);

            Vector2 _grid = new Vector2(_gridX, _gridY);

            _chunkGrid.Add(_grid);
           
            GameObject temp;
            
            if (!_chunkExample.TryGetValue(_grid, out temp))
            {
                _chunkExample.Add(_grid, g);
            }
        } 
        return _chunkGrid;
    }

    //get the coordinates where there should be chunks
    //according to camera position
    private List<Vector2> GetCameraView(Vector2 CamPos)
    {
        List<Vector2> _camView = new List<Vector2>();

        for (int i = (0 - _horizontalSize)/2; i < _horizontalSize/2; i++)
        {
            for (int j = 0; j < _verticalSize; j++)
            {
                
                _camView.Add(new Vector2(CamPos.x + i, CamPos.y + j));
                
            }
        }
        
        return _camView;
    }

    private void DestroyGenerateCal(List<Vector2> camView, List<Vector2> chunkGrid)
    {

        for (int i = 0; i < camView.Count; i++)
        {
            _chunkGenerate.Add(camView[i]);
        }
        for (int i = 0; i < chunkGrid.Count; i++)
        {
            _chunkDestroy.Add(chunkGrid[i]);
        }

        
        List<Vector2> temp = new List<Vector2>();
      
        //find chunkGrid that exist in camView, delete them from the list as they have already been destroied
        //find chunkGrid that exist in camView, delete them from the list as they have already been generated
        for (int i = 0; i < camView.Count; i++)
        {
            for (int j = 0; j < chunkGrid.Count; j++)
            {
                if (camView[i] == chunkGrid[j])
                {
                    temp.Add(chunkGrid[j]);
                    _chunkDestroy.Remove(chunkGrid[j]);
                    _chunkGenerate.Remove(chunkGrid[j]);
                    break;
                }
            }
        }

    }

}