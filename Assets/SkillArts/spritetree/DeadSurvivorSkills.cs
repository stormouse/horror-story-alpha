using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0)]
public class DeadSurvivorSkills : NetworkBehaviour {

    public float overwatchHeight = 5.0f;
    public NetworkCharacter character;
    // CameraFollow cameraFx;

    bool possessing = false;
    int currentTreeId;
    TreeControl[] trees;

    private void Start()
    {
        trees = LevelManager.Singleton.GetTrees();

        if (!isLocalPlayer)
            return;
        currentTreeId = Random.Range(0, trees.Length);
        CmdChangeTree(currentTreeId);
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            LastTree();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            NextTree();
        }
    }

    [Command]
    public void CmdChangeTree(int i)
    {
        RpcChangeTree(i);
        ChangeTree(i);
    }

    [ClientRpc]
    private void RpcChangeTree(int i)
    {
        if(!isServer)
            ChangeTree(i);
    }


    public void ChangeTree(int i)
    {
        i = (i + trees.Length) % trees.Length;
        if (possessing)
        {
            trees[currentTreeId].Leave(character);
        }
        if (currentTreeId != i) {
            trees[i].Possess(character);
            currentTreeId = i;
        }

        Vector3 pos = trees[i].transform.position;
        pos.y = overwatchHeight;
        transform.position = pos;

        possessing = true;
    }

    public void NextTree()
    {
        CmdChangeTree(currentTreeId + 1);
    }

    public void LastTree()
    {
        CmdChangeTree(currentTreeId - 1);
    }

}
