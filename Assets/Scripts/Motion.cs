using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using TMPro;


public class Motion : MonoBehaviourPunCallbacks
{
    #region Variables
    public float speed;
    public float jumpforce;
    public float acc;
    public Camera normalCam;
    public Transform groundDetector;
    public LayerMask ground;
    public Transform weanponParent;
    public int max_health;
    public Transform spawnPoint;

    private Rigidbody rig;
    private float baseFOV;
    private float FOVmodifier = 1.5f;
    private bool isJumping;
    private int current_health;
    private Transform ui_healthbar;

    #endregion

    #region calls
    void Start()
    {
        current_health = max_health;
        baseFOV = normalCam.fieldOfView;
        Camera.main.gameObject.SetActive(false);
        rig = GetComponent<Rigidbody>();
        ui_healthbar = GameObject.Find("HUD/Health/Bar").transform;
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            //RefreshMultiplayerState();
            return;
        }

        bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rig.AddForce(Vector3.up * jumpforce);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            current_health -= 10;
        }
        RefreshPlayer();
        RefreshHealthBar();


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            //RefreshMultiplayerState();
            return;
        }

        float t_hmove = Input.GetAxisRaw("Horizontal");
        float t_vmove = Input.GetAxisRaw("Vertical");
        Vector3 t_direction = new Vector3(t_hmove, 0, t_vmove);
        t_direction.Normalize();

        bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isSprinting = sprint && t_vmove > 0;
        float speedModifier = 1f;
        if (isSprinting)
        {
            speedModifier = acc;
            normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * FOVmodifier, Time.deltaTime * 8f);
        }
        else normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV, Time.deltaTime * 8f);

        Vector3 t_playerV = transform.TransformDirection(t_direction * speed * speedModifier * Time.deltaTime);
        t_playerV.y = rig.velocity.y;
        rig.velocity = t_playerV;
    }
    #endregion

    #region PrivateMethods
    void RefreshHealthBar()
    {
        float t_health_ratio = (float)current_health / (float)max_health;
        ui_healthbar.localScale = Vector3.Lerp(ui_healthbar.localScale, new Vector3(t_health_ratio, 1, 1), Time.deltaTime * 8f);
    }

    void RefreshPlayer()
    {
        if (current_health <= 0)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            current_health = max_health;
            Debug.Log("You died!");
        }
    }
    #endregion
}
