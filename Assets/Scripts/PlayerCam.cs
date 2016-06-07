using UnityEngine;
using System.Collections;

public class PlayerCam : MonoBehaviour {

    public Transform player;
    [SerializeField]
    public Rigidbody playerRigidbody;
    //[SerializeField]
    //public Weapon Weapon;  


    //カメラの回転の中心座標(プレイヤーとのオフセット)
    public Vector3 pivotOffset = new Vector3(0.0f, 1.0f, 0.0f);
    //カメラ位置の座標(プレイヤーとのオフセット)
    public Vector3 camOffset = new Vector3(0.0f, 0.7f, -3.0f);

    public float smooth = 10f;

    /*カメラ設定*/
    //平行感度
    public float horizontalAimingSpeed = 200f;
    //垂直感度
    public float verticalAimingSpeed = 200f;
    //最大仰角
    public float maxVerticalAngle = 30f;
    //最大伏角
    public float minVerticalAngle = -60f;
    //マウス感度
    public float mouseSensitivity = 0.3f;
    //FOV
    public float sprintFOV = 100f;

    //
    public Vector3 diffRotation;


    private PlayerControl playerControl;
    private float angleH = 0;
    private float angleV = 0;
    [SerializeField]
    private Transform cam;

    

    // カメラのプレイヤーからの相対位置
    private Vector3 relCameraPos;
    // 上のベクトルの大きさ
    private float relCameraPosMag;

    private Vector3 smoothPivotOffset;
    private Vector3 smoothCamOffset;
    private Vector3 targetPivotOffset;
    private Vector3 targetCamOffset;

    private float defaultFOV;
    private float targetFOV;


    void Awake()
    {
        cam = this.gameObject.transform;
        playerControl = player.GetComponent<PlayerControl>();

        relCameraPos = transform.position - player.position;
        relCameraPosMag = relCameraPos.magnitude;

        smoothPivotOffset = pivotOffset;
        smoothCamOffset = camOffset;

        defaultFOV = Camera.main.fieldOfView;

        // レティクルとの角度のオフセット

    }


    void LateUpdate()
    {
        //マウスの回転角を取得
        angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * horizontalAimingSpeed * Time.deltaTime;
        angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * verticalAimingSpeed * Time.deltaTime;

        //angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -0.5f, 0.5f) * horizontalAimingSpeed * Time.deltaTime;
        diffRotation = player.rotation.eulerAngles - Camera.main.transform.rotation.eulerAngles;

        

        

        // マウスが動いていない、かつ移動中にカメラを追尾
        followPlayer();
        
        

        //カメラの角度を制限
        angleV = Mathf.Clamp(angleV, minVerticalAngle, maxVerticalAngle);

        Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
        Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
        cam.rotation = aimRotation;
 
        targetPivotOffset = pivotOffset;
        targetCamOffset = camOffset;

        targetFOV = defaultFOV;

        // カメラ画角変更の際のスムージング
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime);


        // オブジェクトとの接触判定
        // プレイヤーと他のオブジェクトとの間に物体があったらカメラを前に
        // 0.5f の値はプレイヤーの身長によって調整が必要
        Vector3 baseTempPosition = player.position + camYRotation * targetPivotOffset;
        Vector3 tempOffset = targetCamOffset;
        for (float zOffset = targetCamOffset.z; zOffset <= 0; zOffset += 0.5f)
        {
            tempOffset.z = zOffset;
            if (DoubleViewingPosCheck(baseTempPosition + aimRotation * tempOffset) || zOffset == 0)
            {
                targetCamOffset.z = tempOffset.z;
                break;
            }
        }
        

        smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, smooth * Time.deltaTime);
        smoothCamOffset = Vector3.Lerp(smoothCamOffset, targetCamOffset, smooth * Time.deltaTime);

        cam.position = player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;

    }



    // concave objects doesn't detect hit from outside, so cast in both directions
    bool DoubleViewingPosCheck(Vector3 checkPos)
    {
        float playerFocusHeight = player.GetComponent<CapsuleCollider>().height * 0.0f;
        return ViewingPosCheck(checkPos, playerFocusHeight) && ReverseViewingPosCheck(checkPos, playerFocusHeight);
    }




    // カメラ→プレイヤー間にカメラが存在しているかの確認
    // false でそのカメラ位置はまずいですよという出力
    bool ViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight)
    {
        RaycastHit hit;
        //Debug.DrawRay(checkPos, player.position + (Vector3.up * deltaPlayerHeight) - checkPos, Color.green, relCameraPosMag);
        //Debug.Log(deltaPlayerHeight);
        if (Physics.Raycast(checkPos, player.position + (Vector3.up * deltaPlayerHeight) - checkPos, out hit, relCameraPosMag))
        {
            //Debug.Log(hit.collider.name);
            //if(hit.collider.name == "Ground")
            //{
            //    Debug.DrawRay(checkPos, player.position + (Vector3.up * deltaPlayerHeight) - checkPos, Color.red, relCameraPosMag);
            //}
            if (hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                // This position isn't appropriate.
                return false;
            }
        }
        // If we haven't hit anything or we've hit the player, this is an appropriate position.
        return true;
    }


    // プレイヤー→カメラ間にカメラが存在しているかの確認
    // false でそのカメラ位置はまずいですよという出力
    bool ReverseViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight)
    {
        RaycastHit hit;

        if (Physics.Raycast(player.position + (Vector3.up * deltaPlayerHeight), checkPos - player.position, out hit, relCameraPosMag))
        {
            if (hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }
        return true;
    }

    void followPlayer()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float mh = Input.GetAxis("Mouse X");
        float mv = Input.GetAxis("Mouse Y");

        if ((h != 0.0f || v != 0.0f) && (mh == 0.0f && mv == 0.0f) && !Input.GetMouseButton(0))
        {
            if(Mathf.Abs(diffRotation.y % 90) < 10)
            {
                return;
            }

            //Debug.Log(angleH);
            if (-345 <= diffRotation.y && diffRotation.y < -195)
            {
                angleH += 0.8f;
            }
            else if (-165 <= diffRotation.y && diffRotation.y < -100)
            {
                angleH -= 0.8f;
            }
            else if (100 <= diffRotation.y && diffRotation.y < 165)
            {
                angleH += 0.8f;
            }
            else if (195 <= diffRotation.y && diffRotation.y < 345)
            {
                angleH -= 0.8f;
            }
            //Debug.Log(angleH);
        }

    }







}
