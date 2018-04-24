using UnityEngine;
using UnityEngine.Networking;

public class FreeCam : NetworkBehaviour
{

    /*
	EXTENDED FLYCAM
		Desi Quintans (CowfaceGames.com), 17 August 2012.
		Based on FlyThrough.js by Slin (http://wiki.unity3d.com/index.php/FlyThrough), 17 May 2011.
 
	LICENSE
		Free as in speech, and free as in beer.
 
	FEATURES
		WASD/Arrows:    Movement
		          Q:    Climb
		          E:    Drop
                      Shift:    Move faster
                    Control:    Move slower
                        End:    Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
	*/

    public float cameraSensitivity = 90;
    public float climbSpeed = 4;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;
    public float offsetX = 10.0f;
    public float offsetY = 10.0f;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private bool m_LockOnPlayer = false;
    private int m_CurrentPlayerIndex = 0;
    private GameObject focusedPlayer;
    private GameObject[] loadedPlayers;

    void Start()
    {
        if (!isLocalPlayer)
        {
            Destroy(this);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);


        if (!m_LockOnPlayer)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
                transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
                transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
            }
            else
            {
                transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
                transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
            }


            if (Input.GetKey(KeyCode.Q)) { transform.position += transform.up * climbSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyCode.E)) { transform.position -= transform.up * climbSpeed * Time.deltaTime; }

        } else
        {
            
            if (focusedPlayer != null)
            {
                transform.position = focusedPlayer.transform.position;

                if (Input.GetKeyDown(KeyCode.A))
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    m_CurrentPlayerIndex -= 1;
                    if (m_CurrentPlayerIndex < 0)
                    {
                        m_CurrentPlayerIndex = players.Length - 1;
                    }
                    if (m_CurrentPlayerIndex >= players.Length)
                    {
                        m_CurrentPlayerIndex = 0;
                    }
                    if (loadedPlayers == null || players.Length != loadedPlayers.Length)
                    {
                        loadedPlayers = players;
                    }
                    focusedPlayer = loadedPlayers[m_CurrentPlayerIndex];
                }
                if (Input.GetKeyDown(KeyCode.D)) {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    m_CurrentPlayerIndex += 1;
                    if (m_CurrentPlayerIndex >= players.Length)
                    {
                        m_CurrentPlayerIndex = 0;
                    }
                    if (loadedPlayers == null || players.Length != loadedPlayers.Length)
                    {
                        loadedPlayers = players;
                    }
                    focusedPlayer = loadedPlayers[m_CurrentPlayerIndex];
                }
                Debug.Log(m_CurrentPlayerIndex);
            } else
            {
                m_LockOnPlayer = !m_LockOnPlayer;
            }

        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_LockOnPlayer = !m_LockOnPlayer;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (m_CurrentPlayerIndex >= players.Length)
            {
                m_CurrentPlayerIndex = 0;
            }
            if (loadedPlayers == null || players.Length != loadedPlayers.Length)
            {
                loadedPlayers = players;
            }
            focusedPlayer = loadedPlayers[m_CurrentPlayerIndex];
        }

        Camera.main.transform.position = transform.position + (offsetY * transform.up) + (offsetX * -transform.forward);
        Camera.main.transform.rotation = transform.rotation;
    }



}