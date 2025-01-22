using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NodeGeneration : MonoBehaviour {
    [Header("Directions")]
    [SerializeField] Vector3[] directions;
    [SerializeField]Vector3 previousDirection;

    Vector3 posOfRoomToBeGenerated;
    Vector3 posOfHallwayToBeGenerated;
    Vector3 posOfStairToBeGenerated;
    
    public Vector3 lastRoomPosition;

    [Header("Objects")]
    public GameObject roomPrefab;
    public GameObject hallway;
    public GameObject stair;

    [Header("Debug")]
    public int roomsToGenerate; // doesnt include first room so -1
    public int overlapAttempts;
    [SerializeField] int roomsGenerated = 0;
    [SerializeField] List<Vector3> roomPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Start() {
        roomPositions.Add(Vector3.zero);
        directions = new Vector3[] { transform.forward, transform.right, -transform.forward, -transform.right, transform.up };
        StartCoroutine(AttemptRoomGeneration());
    }
    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            ResetScene();
        }
    }

    IEnumerator AttemptRoomGeneration() {
        while (roomsGenerated < roomsToGenerate - 1) {
            Vector3 dir = directions[Random.Range(0, directions.Length)];

            if (dir == transform.up) {
                if (previousDirection == new Vector3(0, 0, 0)) {
                    Debug.Log("Null value");
                    ResetScene();
                } 
                else {
                    Debug.Log("Generating Up");
                    posOfRoomToBeGenerated = lastRoomPosition + (dir) * 1.5f;
                }
            }
            //get positions for room spawn
            //and potential hallway/stair spawn
            posOfRoomToBeGenerated = lastRoomPosition + dir * 1.5f;
            posOfHallwayToBeGenerated = lastRoomPosition + dir * 0.75f;
            posOfStairToBeGenerated = lastRoomPosition + (dir + transform.up) * 0.5f; 

            if (CheckForOverlap(dir, posOfRoomToBeGenerated)) {
                GenerateRoom(dir);
            } 
            else {
                overlapAttempts++;
                Debug.Log("Overlap Found, Not Generating");
            }
            yield return null;
            if (overlapAttempts >= 100) {
                ResetScene();
            }
        }
    }

    Vector3 GetHallwayPosition() {
        return Vector3.one;
    }

    void GenerateRoom(Vector3 dir) {
        Debug.DrawLine(lastRoomPosition, posOfRoomToBeGenerated, Color.blue, 99f);

        if (dir == transform.up) {
            GameObject roomInstantied = Instantiate(roomPrefab, posOfRoomToBeGenerated + previousDirection, Quaternion.identity);
            roomPositions.Add(posOfRoomToBeGenerated + previousDirection);
            roomsGenerated++;
            lastRoomPosition = posOfRoomToBeGenerated + previousDirection;
            //AddStairs(dir);
        } else {
            roomPositions.Add(posOfRoomToBeGenerated);
            roomsGenerated++;
            lastRoomPosition = posOfRoomToBeGenerated;
            GameObject roomInstantied = Instantiate(roomPrefab, posOfRoomToBeGenerated, Quaternion.identity);
           // AddHallway(dir);
        }
        previousDirection = dir;
    }

    void AddHallway(Vector3 dir) {
        GameObject hallwayInstantied = Instantiate(hallway, posOfHallwayToBeGenerated, Quaternion.identity);
        hallwayInstantied.transform.forward = dir;
    }
    void AddStairs(Vector3 dir) {
        GameObject stairInstantied = Instantiate(stair, posOfStairToBeGenerated, Quaternion.identity);
        stairInstantied.transform.forward = dir;
    }

    bool CheckForOverlap(Vector3 direction, Vector3 posOfGeneration) {
        foreach (Vector3 roomPos in roomPositions) {
            if (posOfGeneration == roomPos) {
                return false;
            }
        }
        return true;
    }

    void ResetScene() {
        StopCoroutine(AttemptRoomGeneration());
        roomPositions.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        overlapAttempts = 0;
    }
}
