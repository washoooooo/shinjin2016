using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class PlayerControl : MonoBehaviour {

    // カメラ情報
    [SerializeField]
    GameObject playerCamera;
    [SerializeField]
    GameObject bullet;
    [SerializeField]
    GameObject reticle;
    RectTransform reticleRectTransform;
    // 武器
    //[SerializeField]
    //Weapon weapon;

    // プレイヤー動作スピード
    public float speed = 6f;
    public float moveForce = 800;
    // ジャンプ力
    public float jumpForce = 500;
    // プレイヤーの移動方向
    Vector3 movement;
    // ファイアレート
    public float fireRate = 1;
    private float coolDownTime;
    // 弾丸の射程
    public float firerange = 20f;

    // プレイヤーの射線(プレイヤーの視点方向)
    private Vector3 firedir;
    public Vector3 collectDir;
    Vector3 defaultReticlePos;

    // Reference to the player's rigidbody.
    Rigidbody playerRigidbody;


    public enum ShotState
    {
        Canfire,
        Shot
    }
    public ShotState shotState;

    // ジャンプ関連
    private bool isGrounded = true;
    private Time janpStartTime;

    // Use this for initialization
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        shotState = ShotState.Canfire;

        reticleRectTransform = reticle.GetComponent<RectTransform>();
        defaultReticlePos = reticle.transform.position;
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
            Rakka();
        }

        //playerRigidbody.rotation = camera.transform.rotation;

        
        // 弾丸発射処理
        Shot();
        
       

    }

    void LateUpdate()
    {
        // レティクルの動作
        CheckReticle();
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
        // 射撃処理
        else if(coolDownTime == 0f && Input.GetAxisRaw("Fire1") == 1.0)
        {
            //弾を撃つ処理
            
            coolDownTime = fireRate;
        }
        // 射撃中はプレイヤーの向きはカメラの方向に固定
        //else if(Input.GetAxisRaw("Fire1") == 1.0)
        if(coolDownTime > 0)
        {
            
            coolDownTime -= Time.deltaTime;
            if(coolDownTime < 0f)
            {
                coolDownTime = 0f;
            }
            Vector3 firedir = CameraForward;
            firedir.y = 0f;
            //firedir.z = 0f;
            playerRigidbody.transform.rotation = Quaternion.LookRotation(firedir);
            //weapon.Shot();   
        }
        

        //playerRigidbody.velocity = Vector3.zero;
        //playerRigidbody.AddForce(movement.normalized * moveForce);
    }


    void Jump()
    {

        playerRigidbody.AddForce(Vector3.up * jumpForce);
        //isGrounded = false;
    }


    void Rakka()
    {
        playerRigidbody.AddForce(Vector3.down * (jumpForce/2.0f));
    }


    // 弾丸の発射処理
    void Shot()
    {
        if (shotState == ShotState.Shot)
        {
            coolDownTime -= Time.deltaTime;
            if(coolDownTime < 0)
            {
                shotState = ShotState.Canfire;
            }
        }
        if (shotState == ShotState.Canfire && Input.GetAxisRaw("Fire1") == 1.0f)
        {
            shotState = ShotState.Shot;
            GameObject obj = (GameObject)Instantiate(bullet, transform.position, Quaternion.identity);
            obj.transform.parent = transform;
            //obj.transform.localPosition = new Vector3(0, 0, 1f);
            coolDownTime = fireRate;
        }
    }

    // レティクルの移動
    void CheckReticle()
    {
        // プレイヤーの視線の計算
        // 左右方向の取得
        Vector3 CameraForward = playerCamera.transform.TransformDirection(Vector3.forward);
        //CameraForward.x = 0;
        //CameraForward.x = 0;
        //CameraForward.y = 0;
        // 上下方向の計算
        /*
        Vector3 cameraAngle = Camera.main.transform.rotation.eulerAngles;
        if (0 <= cameraAngle.x && cameraAngle.x < 90)
        {
            //cameraAngle.x = -1 * cameraAngle.x;
            firedir = Quaternion.Euler(0, 0, cameraAngle.x - 6f) * CameraForward;
        }
        else if (270 <= cameraAngle.x && cameraAngle.x < 360)
        {
            cameraAngle.x = cameraAngle.x - 360.0f;
            firedir = Quaternion.Euler(0, 0, cameraAngle.x - 6f) * CameraForward;
        }
        */
        //firedir = Quaternion.Euler(-6, 0, 0) * CameraForward;
        Vector3 axis = Vector3.Cross(Vector3.up, CameraForward);
        firedir = Quaternion.AngleAxis(-5f, axis) * CameraForward;
        
        // プレイヤーからレイを飛ばす
        // モデルに置き換えた時に例の出発点を置き換える必要あり
        RaycastHit hit;
        Vector3 checkPos = transform.position;
        
        Debug.DrawRay(checkPos, firedir.normalized* firerange, Color.green, 0.1f);
        //Debug.Log(firerange);
        if (Physics.Raycast(this.transform.position, firedir, out hit, firerange))
        {
            if(hit.collider.tag != "Bullet")
            {
                reticle.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, hit.point);
            }

            
            //Debug.Log(hit.collider.name);
            //if(hit.collider.name == "Ground")
            //{
            //    Debug.DrawRay(checkPos, player.position + (Vector3.up * deltaPlayerHeight) - checkPos, Color.red, relCameraPosMag);
            //}
            //if (hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
            //{
                // This position isn't appropriate.
                //return false;
            //}
        }
        else
        {
            //reticleRectTransform.position = new Vector3(0, 0, 0);
            reticle.transform.position = defaultReticlePos;
        }
    }


}
