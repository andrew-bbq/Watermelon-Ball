using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    public Rigidbody2D rb;

    public GameObject ballVisual;
    public GameObject ourVisual;
    public Vector3 startPosition;
    public float hangTime;
    public GameObject splatterEffect;
    public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        hangTime = 0f;
        ourVisual = (GameObject)Instantiate(ballVisual, transform.position, Quaternion.identity);
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //lock rotation
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
        float relativeX = hangTime;
        float currY = transform.position.y;
        // -x^2 + 2x - 0.5 = y
        currY += -(relativeX * relativeX) + (2 * relativeX);
        ourVisual.transform.position = new Vector3(transform.position.x, currY, -11);
        // tilt accordingly
        float tiltOffset = Mathf.Abs(rb.velocity.x) * 3;
        float tiltAmount = (90f - (2*tiltOffset)) * (hangTime/2f);
        ourVisual.transform.rotation = Quaternion.Euler(0, 0, (rb.velocity.x < 0) ? 90 + tiltOffset + tiltAmount : - tiltOffset - tiltAmount);

        hangTime += Time.deltaTime;
        if (hangTime >= 2f) {
            // destroy self and ball object
            detectCatch();
            Destroy(ourVisual);
            Destroy(gameObject);
        }
    }

    void detectCatch()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 1) {
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.hasBall = true;
        } else {
            Instantiate(splatterEffect, transform.position, Quaternion.identity);
        }
    }
}
