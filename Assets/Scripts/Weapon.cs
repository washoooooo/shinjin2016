using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

    [SerializeField]
    public Transform player;
    [SerializeField]
    public Transform camera;


    // 武器の射程距離
    public float fireRange;
    // 射撃可能のクールタイム
    public float cooltime = 0.8f;
    // 弾丸の速度
    public float firespeed = 20.0f;
    // 発射方向 カメラとのずれ
    Vector3 firedir;
    public Vector3 collectDir;

    // 最大射程 or 戻ってくるか

    public enum WeaponState
    {
        CanFire,
        Fire,
        Hit,
        Retrun,
    }
    public WeaponState weaponState = WeaponState.CanFire;

    

	// Use this for initialization
	void Start () {
        

	}
	
	// Update is called once per frame
	void Update () {


        // カメラの向きに追従



        // 弾の発射ステート管理
        ManageFireState();
        
        

        
        
    }


    public void Shot()
    {
        if (weaponState == WeaponState.CanFire)
        {
            weaponState = WeaponState.Fire;

            
            //Vector3 CameraForward = camera.transform.TransformDirection(Vector3.forward);
            //Vector3 CameraUp = camera.transform.TransformDirection(Vector3.up);
            // カメラの回転角のx方向のみを抽出、弾の発射方向のy方向(上下方向)に加算
            Vector3 cameraAngle = camera.transform.rotation.eulerAngles;
            if(0 <= cameraAngle.x && cameraAngle.x < 90)
            {
                //cameraAngle.x = -1 * cameraAngle.x;
                transform.localRotation = Quaternion.Euler(cameraAngle.x, 0, 0);
            }
            else if(270 <= cameraAngle.x && cameraAngle.x < 360)
            {
                cameraAngle.x = cameraAngle.x - 360.0f;
                transform.localRotation = Quaternion.Euler(cameraAngle.x, 0, 0);
            }
            Debug.Log(cameraAngle.x);
            
            //Quaternion.Euler(collectDir);
            //Debug.Log(cameraAngle);
            //cameraAngle.y = cameraAngle.x;
            //cameraAngle.x = 0.0f;
            //cameraAngle.z = 0.0f;
            //Debug.Log(cameraAngle);
            firedir = collectDir;// + cameraAngle;
            //firedir = camera.transform.rotation.eulerAngles;
            //firedir = camera.TransformDirection(Vector3.forward);
            
        }
    }

    public void ManageFireState()
    {
        if (weaponState == WeaponState.Fire)
        {
            gameObject.transform.Translate(firedir.normalized * firespeed * Time.deltaTime);
            if (transform.localPosition.z > fireRange)
            {
                weaponState = WeaponState.Retrun;
            }
        }
        else if (weaponState == WeaponState.Retrun)
        {
            gameObject.transform.Translate(-1 * firedir.normalized * firespeed * Time.deltaTime);
            if (transform.localPosition.z < 1f)
            {
                transform.localPosition = new Vector3(0f, 0f, 0.3f);
                weaponState = WeaponState.CanFire;
            }
        }
    }


}
