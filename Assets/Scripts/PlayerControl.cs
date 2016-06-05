using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    // カメラ情報
    [SerializeField]
    GameObject playerCamera;
    // 武器
    [SerializeField]
    Weapon weapon;

    // プレイヤー動作スピード
    public float speed = 6f;
    public float moveForce = 800;
    // ジャンプ力
    public float jumpForce = 500;
    // プレイヤーの移動方向
    Vector3 movement;
    // Reference to the player's rigidbody.
    Rigidbody playerRigidbody;


    // ジャンプ関連
    private bool isGrounded = true;
    private Time janpStartTime;

    // Use this for initialization
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }



    // Update is called once per frame
    void Update () {
        // Store the input axes.
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Move(h, v);

        // ジャンプスタート
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            //isGrounded = false;
            Jump();
        }

        if (isGrounded && Input.GetKeyUp(KeyCode.Space) && playerRigidbody.velocity.y > 0)
        {
            rakka();
        }

        //playerRigidbody.rotation = camera.transform.rotation;

    }

    void Move(float h, float v)
    {
        Vector3 CameraForward = playerCamera.transform.TransformDirection(Vector3.forward);
        Vector3 CameraRight = playerCamera.transform.TransformDirection(Vector3.right);

        movement = v * CameraForward + h * CameraRight;
        movement.y = 0f;
        // Set the movement vector based on the axis input.
        //movement.Set(h, 0f, v);

        // Normalise the movement vector and make it proportional to the speed per second.
        Vector3 _movement = movement.normalized * speed * Time.deltaTime;

        // Move the player to it's current position plus the movement.
        playerRigidbody.MovePosition(transform.position + _movement);
        if (movement != Vector3.zero && Input.GetAxisRaw("Fire1") == 0)
        {
            playerRigidbody.transform.rotation = Quaternion.LookRotation(movement);
        }
        // 射撃中はプレイヤーの向きはカメラの方向に固定
        //else if(Input.GetAxisRaw("Fire1") == 1.0)
        if(weapon.weaponState != Weapon.WeaponState.CanFire || Input.GetAxisRaw("Fire1") == 1.0)
        {
            Vector3 firedir = CameraForward;
            firedir.y = 0f;
            //firedir.z = 0f;
            playerRigidbody.transform.rotation = Quaternion.LookRotation(firedir);
            weapon.Shot();   
        }
        

        //playerRigidbody.velocity = Vector3.zero;
        //playerRigidbody.AddForce(movement.normalized * moveForce);
    }


    void Jump()
    {

        playerRigidbody.AddForce(Vector3.up * jumpForce);
        //isGrounded = false;
    }


    void rakka()
    {
        playerRigidbody.AddForce(Vector3.down * (jumpForce/2.0f));
    }


}
