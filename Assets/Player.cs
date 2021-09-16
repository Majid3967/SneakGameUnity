using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 7f;
    public float smoothMoveTime = .1f; 
    float turnSpeed = 8f;

    float smoothInputMagnitude;
    float smoothMoveVelocity;
    float angle;
    Vector3 velocity;

    Rigidbody rigidbody;

    public GameObject playAgain;
   
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        float inputMagnitude = input.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);
        float targetAngle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle,targetAngle,turnSpeed*Time.deltaTime*inputMagnitude);
        velocity = transform.forward*speed*Time.deltaTime*smoothInputMagnitude;
    }

    void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up*angle));
        rigidbody.MovePosition(rigidbody.position + velocity);
    }
    void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Finish")
        {
            playAgain.SetActive(true);
        }
    }
}

