using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event System.Action OnReachedEndOfLevel;
    public float smoothMoveTime = .1f;
    public float moveSpeed = 10;
    public float turnSpeed = 8;

    bool disabled;
    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;

    Rigidbody rigidBody;

    void Start() {
      rigidBody = GetComponent<Rigidbody> ();
      Enemy.OnGuardHasSpottedPlayer += Disable;
    }

    void Update() {
        Vector3 inputDirection = Vector3.zero;

        if(!disabled){
          inputDirection = new Vector3(Input.GetAxisRaw(Axis.HORIZONTAL_AXIS), 0, Input.GetAxisRaw(Axis.VERTICAL_AXIS)).normalized;
        }

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        float inputMagnitude = inputDirection.magnitude;

        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);
        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
    }

    void OnTriggerEnter(Collider hitCollider) {
      if(hitCollider.tag == Tags.FINISH_TAG) {
         Disable();
         if(OnReachedEndOfLevel != null){
           OnReachedEndOfLevel();
         }
      }
    }

    void Disable() {
      disabled = true;
    }

    void FixedUpdate() {
      rigidBody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
      rigidBody.MovePosition(rigidBody.position + velocity * Time.deltaTime);
    }

    void OnDestroy() {
      Enemy.OnGuardHasSpottedPlayer -= Disable;
    }
}
