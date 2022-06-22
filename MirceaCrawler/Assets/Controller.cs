using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public int[] refLeg;
    public int[] refLeg2;
    public FootScript[] legs;
    public float desiredSurfDist = -1f;
    public bool grounded;

    private Rigidbody rb;
    public float speed;
    public float angSpeed;
    private float movX;
    public float movY;

    public static Controller instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        movX = Input.GetAxis("Horizontal");
        movY = Input.GetAxis("Vertical");

        if(movY != 0)
        {
            Vector3 movement = transform.right * movY * speed * Time.deltaTime;
            rb.velocity = movement;
        }

        transform.Rotate(0, movX * angSpeed * Time.deltaTime, 0);

        CalcOrientation();
    }

    public void CalcOrientation()
    {
        Vector3 up = Vector3.zero;
        Vector3 point, a, b, c;
        float avgSurfDist = 0;
        grounded = false;

        for (int i = 0; i < legs.Length; i++)
        {
            point = legs[i].newPos;
            avgSurfDist += transform.InverseTransformPoint(point).y; //see how far we are from the surf

            up += (legs[i].stepNormal == Vector3.zero ? transform.forward : legs[i].stepNormal); //if we didnt stepped on anything we go with the forwar normal otherwise stepNormal
            grounded |= legs[i].legGrounded;
        }

        up /= legs.Length;
        avgSurfDist /= legs.Length;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, up), up), 20f * Time.deltaTime); // lookrotation gives the orientation, project on plane gives the projection of forword(a point) 

        if(grounded)
        {
            transform.Translate(0, -(-avgSurfDist + desiredSurfDist), 0, Space.Self);
        }
    }
}
