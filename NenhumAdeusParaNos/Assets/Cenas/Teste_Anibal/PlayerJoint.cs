using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerJoint : MonoBehaviour
{
    public float speed;
    void Update() {
    float v = Input.GetAxis("Vertical");
    float h = Input.GetAxis("Horizontal");
    float vel = speed * Time.deltaTime;
    transform.Rotate(0, h * 90 * Time.deltaTime, 0);
    transform.Translate(0, 0, v * vel);
    }

}