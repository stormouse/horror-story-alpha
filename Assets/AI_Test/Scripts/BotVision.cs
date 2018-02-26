using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotVision : MonoBehaviour {

    #region Setup

    private void Start()
    {
        InitVariables();

        BindMastermind();

        BindMeshFilter();

        StartDetection();
    }

    void InitVariables()
    {
        objectsInSight = new List<GameObject>();
    }

    #endregion Setup


    #region Sensor_Logic
    /* ---------------------- Sensor Logic ----------------------------*/
    [Header("Sensor Property")]
    public float viewAngle;
    public float distance;
    
    public bool hostSensible;
    private ISensible mastermind;

    private List<GameObject> objectsInSight;
    private Coroutine detectionCoroutine = null;

    private void BindMastermind()
    {
        mastermind = GetComponent<ISensible>();
        if (mastermind == null)
            hostSensible = false;
        else
            hostSensible = true;
    }

    public void StartDetection()
    {
        if(detectionCoroutine == null)
            detectionCoroutine = StartCoroutine(DetectionCoroutine());
    }

    public void StopDetection()
    {
        StopCoroutine(detectionCoroutine);
        detectionCoroutine = null;
    }

    private IEnumerator DetectionCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if(hostSensible)
                Detect();
        }
    }

    private void Detect()
    {
        var playerList = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in playerList)
        {
            if (player == gameObject) continue;

            var pos = player.transform.position;
            bool possiblyVisible = true;

            if (Vector3.Angle(pos - transform.position, transform.forward) > viewAngle * 0.5f)
            {
                possiblyVisible = false;
            }

            if (Vector3.Distance(pos, transform.position) > distance)
            {
                possiblyVisible = false;
            }

            if (possiblyVisible)
            {
                bool blockTest = Physics.Raycast(
                    transform.position,
                    pos - transform.position,
                    Vector3.Distance(transform.position, pos),
                    ~(1 << LayerMask.NameToLayer("Player")));
                if (blockTest)
                {
                    possiblyVisible = false;
                }
            }

            if (!possiblyVisible && objectsInSight.Contains(player.gameObject))
            {
                objectsInSight.Remove(player.gameObject);
                mastermind.LoseSight(player.gameObject);
            }
            else if (possiblyVisible && !objectsInSight.Contains(player.gameObject))
            {
                objectsInSight.Add(player.gameObject);
                mastermind.See(player.gameObject);
            }
        }
    }
    #endregion Sensor_Logic

    #region Visualization_Logic
    /* ------------------- Visualization Logic -------------------- */

    [Header("Visualization")]
    public int numOfPoints;
    public MeshFilter viewMeshFilter;


    private void BindMeshFilter()
    {
        if (viewMeshFilter)
        {
            var viewMesh = MakeSemicircle(numOfPoints > 1 ? numOfPoints : 1);
            viewMeshFilter.mesh = viewMesh;
        }
    }


    Mesh MakeSemicircle(int numOfPoints)
    {
        float angleStep = viewAngle / (float)numOfPoints;
        List<Vector3> vertexList = new List<Vector3>();
        List<int> triangleList = new List<int>();
        Quaternion quaternion = Quaternion.Euler(0.0f, angleStep, 0.0f);
        Quaternion initial = Quaternion.Euler(0.0f, -viewAngle * 0.5f, 0.0f);

        vertexList.Add(new Vector3(0.0f, 0.0f, 0.0f));
        vertexList.Add(initial * new Vector3(0.0f, 0.0f, distance));
        vertexList.Add(quaternion * vertexList[1]);
        triangleList.Add(0);
        triangleList.Add(1);
        triangleList.Add(2);

        for(int i = 0; i < numOfPoints - 1; i++)
        {
            triangleList.Add(0);
            triangleList.Add(vertexList.Count - 1);
            triangleList.Add(vertexList.Count);
            vertexList.Add(quaternion * vertexList[vertexList.Count - 1]);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertexList.ToArray();
        mesh.triangles = triangleList.ToArray();
        mesh.name = "Procedural Semicircle";

        return mesh;
    }
    #endregion Visualization_Logic
}
