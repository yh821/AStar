using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAutoRotate : MonoBehaviour
{
    public Vector3 speed;
    Vector3 cur_maincam_angel;
    void Awake()
    {
        cur_maincam_angel = transform.eulerAngles;
    }
    // Update is called once per frame
    void Update()
    {
        cur_maincam_angel += speed;
        transform.rotation = Quaternion.Euler(cur_maincam_angel);
    }
}
