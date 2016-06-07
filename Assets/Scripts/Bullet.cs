using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    
    


    // 射程距離
    public float fireRange;
    // 弾丸の速さ
    public float firespeed;
    // 発射方向
    Vector3 firedir;
    // カメラとのずれの補正角度
    public float collectAngle = 4.0f;
    

    // プレイヤーオブジェクト
    private GameObject parentPlyaer;

    //public enum BulletState
    //{
        
    //}


    // Use this for initialization
    void Start () {
        Vector3 cameraAngle = Camera.main.transform.rotation.eulerAngles;
        if (0 <= cameraAngle.x && cameraAngle.x < 90)
        {
            //cameraAngle.x = -1 * cameraAngle.x;
            transform.localRotation = Quaternion.Euler(cameraAngle.x - collectAngle, 0, 0);
        }
        else if (270 <= cameraAngle.x && cameraAngle.x < 360)
        {
            cameraAngle.x = cameraAngle.x - 360.0f;
            transform.localRotation = Quaternion.Euler(cameraAngle.x - collectAngle, 0, 0);
        }
        //firedir = collectDir;
        firedir = new Vector3(0, 0, 1);
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.Translate(firedir.normalized * firespeed * Time.deltaTime);
        //gameObject.transform.localPosition = new Vector3(0, 
        //   transform.localPosition.y, transform.localPosition.z);
        //gameObject..x = 0.0f;
        gameObject.transform.localPosition = new Vector3(0, transform.localPosition.y,
            transform.localPosition.z);
        if (Mathf.Pow(gameObject.transform.localPosition.z, 2) 
            + Mathf.Pow(gameObject.transform.localPosition.y, 2) > Mathf.Pow(fireRange, 2))
        {
            Destroy(gameObject);
            Debug.Log(gameObject.transform.localPosition.z);
        }
        
    }


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }


    }


}
