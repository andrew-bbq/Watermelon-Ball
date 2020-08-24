using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // player move shit
    public float baseTilt = 1.0f;
    public float baseMoveSpeed = 1.0f;
    public float snapbackSpeed = 4.0f;

    // throwing shit
    public float ballThrowSpeed = 3.0f;
    public float ballChargeLevel = 0f;
    public float ballMaxChargeLevel = 10f;
    public float ballChargeSpeed = 0.03f;

    // ball move shit
    public float ballPercentSpeed = 0f;
    
    public bool hasBall = true;
    public Animator animator;
    public Animator handAnimation;
    public Rigidbody2D rb;
    public GameObject hands;
    public GameObject ball;

    // camera controller
    public CameraController camera;
    // Start is called before the first frame update
    void Start()
    {
        camera.Setup(() => transform.position);
        Vector3 target = transform.position;
        target.y = target.y - 0.45f;
        hands.transform.position = target;
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
        tiltGhost();
        handFollowGhost();
        handleThrow();
        lockHandRotation();
        handSnapback();
        updateHandAnimation();
    }

    // bounce off wall?
    void OnCollisionEnter2D(Collision2D col)
    {
        
    }

    // Handle player movement
    void handleMovement()
    {
        if (Input.GetButtonDown("Fire1") && rb.velocity.magnitude < 0.5) {
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 movementDirection = transform.position - wp;
            rb.AddForce(movementDirection * baseMoveSpeed);
        }
    }

    // tilt for aesthetic
    void tiltGhost()
    {
        if (Mathf.Abs(rb.velocity.x) > 0) {
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, rb.velocity.x * baseTilt);
        } else {
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
        }
    }

    void handFollowGhost()
    {
        Vector3 target = transform.position;
        target.y = target.y - 0.45f;
        if (ballPercentSpeed < 1) {
            ballPercentSpeed += 0.2f;
        }
        // get vector towards target
        Vector3 movementDirection = target - hands.transform.position;
        Rigidbody2D handRb = hands.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        handRb.velocity = movementDirection * Mathf.Max(rb.velocity.magnitude, 6f) * ballPercentSpeed;
        // reset percent speed if we hit the player
        if (target.y == hands.transform.position.y && target.x == hands.transform.position.x) {
            ballPercentSpeed = 0f;
        }
        // set z value depending on height
        hands.transform.position = new Vector3(
            hands.transform.position.x,
            hands.transform.position.y,
            hands.transform.position.y - target.y - 10.4f
        );
    }

    void handleThrow()
    {
        if (Input.GetButton("Fire2") && hasBall) {
           ballChargeLevel = (ballChargeLevel < ballMaxChargeLevel) ? ballChargeLevel + ballChargeSpeed : ballMaxChargeLevel;
        }
        if (Input.GetButtonUp("Fire2") && hasBall) {
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 movementDirection = wp - hands.transform.position;
            Vector3 playerMove = rb.velocity;
            movementDirection.Normalize();
            movementDirection *= ballChargeLevel * ballThrowSpeed;
            GameObject newBall = (GameObject)Instantiate(ball, hands.transform.position, Quaternion.identity);
            newBall.GetComponent<BallMove>().rb.velocity = movementDirection;
            newBall.GetComponent<BallMove>().player = gameObject;
            ballChargeLevel = 0f;
            hasBall = false;
        }
    }
    // dumbass comment
    void lockHandRotation(){
        hands.transform.rotation = Quaternion.Euler(hands.transform.rotation.x, hands.transform.rotation.y, 0);
    }

    // jump back towards hand if positions are too different or player velocity is 0 and they're not close enough by other standards
    void handSnapback(){
        Vector3 target = transform.position;
        target.y = target.y - 0.45f;
        if (Vector3.Distance(target, hands.transform.position) > 1.5 || (rb.velocity.magnitude < 0.5 && Vector3.Distance(target, hands.transform.position) > 1)) {
            Vector3 movementDirection = hands.transform.position - target;
            rb.AddForce(movementDirection * snapbackSpeed);
        }
    }

    void updateHandAnimation(){
        handAnimation.SetBool("HasBall", hasBall);
    }
}
