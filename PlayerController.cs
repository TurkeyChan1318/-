using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController cc;
    public GameObject Camera, RippleCamera;
    public float Speed = 10f;
    public float Gravity = -0.98f;
    private Vector3 Velocity = Vector3.zero;
    public Transform GroundCheck;
    private float CheckRadius = 0.3f;
    private bool IsGround;
    public LayerMask layerMask;
    public ParticleSystem ripple;
    public float JumpHeight = 0.03f;
    [SerializeField]private float VelocityXZ, VelocityY;
    private Vector3 PlayerPos;
    
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        VelocityXZ = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(PlayerPos.x, 0, PlayerPos.z));
        VelocityY = Vector3.Distance(new Vector3(0, transform.position.y, 0), new Vector3(0, PlayerPos.y, 0));
        PlayerPos = transform.position;

        RippleCamera.transform.position = transform.position + Vector3.up * 10;
        Shader.SetGlobalVector("_Player", transform.position);
    }

    private void Movement()
    {
        IsGround = Physics.CheckSphere(GroundCheck.position, CheckRadius, layerMask);
        if(IsGround && Velocity.y < 0) Velocity.y = 0;

        if(IsGround && Input.GetButtonDown("Jump")) Velocity.y += Mathf.Sqrt(JumpHeight * -2 * Gravity);

        Vector3 camRight = Camera.transform.right;
        Vector3 camForward = Camera.transform.forward;
        camRight.y = 0;
        camForward.y = 0;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = camForward.normalized * Speed * vertical * Time.deltaTime + camRight.normalized * Speed * horizontal * Time.deltaTime;
        move *= Input.GetKey(KeyCode.LeftShift)?2:1;
        if(move.magnitude > 0) transform.forward = move.normalized;

        Velocity.y += Gravity * Time.deltaTime; 
        move += Velocity;
        cc.Move(move);
        
    }

    void CreateRipple(int start, int end, int delta, float speed, float size, float lifetime)
    {
        Vector3 forward = ripple.transform.eulerAngles;
        forward.y = start;
        ripple.transform.eulerAngles = forward;
        for (int i = start; i < end; i += delta)
        {
            ripple.Emit(transform.position + ripple.transform.forward * 0.5f, ripple.transform.forward * speed, size, lifetime, Color.white);
            ripple.transform.eulerAngles += Vector3.up * delta;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 4 && VelocityY > 0.01f)
        {
            CreateRipple(-180, 180, 3, 4, 2, 3);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 4 && VelocityXZ > 0.005f && Time.renderedFrameCount % 3 == 0)
        {
            int y = (int)transform.eulerAngles.y;
            CreateRipple(y - 100, y + 100, 3, 5, 2, 1.5f);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 4 && VelocityY > 0.01f)
        {
            CreateRipple(-180, 180, 3, 4, 2, 3);
        }
    }
}
