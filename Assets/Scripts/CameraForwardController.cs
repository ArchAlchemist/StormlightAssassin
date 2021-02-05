using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraForwardController : MonoBehaviour
{
    private Vector3 mainCameraPosition;
    private Transform playerTransform;
    private GameObject player;
    private PlayerController playerController;
    private Vector3 newTarget;
    private Vector3 worldForward;
    private Vector3 target;

    // Start is called before the first frame update
    void Start()
    {
        //playerTransform = new Vector3(0, 0, 1);
        mainCameraPosition = transform.parent.transform.position;
        //playerTransform = GameObject.Find("SzethBaseNewTest1").transform.position;
        player = GameObject.Find("Player");
        playerTransform = player.transform;
        playerController = player.GetComponent<PlayerController>();
        worldForward = Vector3.forward;

    }

    // Update is called once per frame
    void Update()
    {
        TryRotateAngle();
    }

    void TryRotateAngle()
    {
        
        //float angle = Vector3.Angle(worldForward, Vector3.right.normalized);
        //Vector3 localTarget = playerTransform.forward;
        //localTarget.Rotate(playerTransform.forward, angle);
        
        //mainCameraPosition = transform.parent.transform.position;
        Vector3 yClamp = new Vector3(transform.parent.transform.forward.x, 0, transform.parent.transform.forward.z);
        transform.position = yClamp.normalized;
        

        //Debug.DrawRay(playerTransform.position, transform.position, Color.blue);
        //Debug.DrawRay(playerTransform.position, playerTransform.forward, Color.green);
        //Debug.DrawRay(playerTransform.position, worldForward, Color.yellow);

        // time to see if we can rotate target based on camera view;

        //Vector3 target = playerController.getMoveTarget();
        //float worldAngle = Vector3.SignedAngle(worldForward, target, Vector3.up);
        ////float worldAngle = Vector3.Angle(worldForward, target);
        //if (worldAngle < 0)
        //{
        //    //worldAngle = 360.0f - worldAngle;
        //}

        //var rotation = Quaternion.AngleAxis(worldAngle, Vector3.up);

        //newTarget = rotation * transform.position;
        //Debug.Log(newTarget.normalized);

        //Debug.DrawRay(playerTransform.position, newTarget, Color.yellow);
    }

    public Vector3 getNewTarget(float x, float y)
    {
        
        target.Set(x, 0f, y);
        target.Normalize();
        float worldAngle = Vector3.SignedAngle(worldForward, target, Vector3.up);
        //float worldAngle = Vector3.Angle(worldForward, target);
        if (worldAngle < 0)
        {
            //worldAngle = 360.0f + worldAngle;
        }

        var rotation = Quaternion.AngleAxis(worldAngle, Vector3.up);
        //var rotation = Quaternion.LookRotation(target - transform.position, Vector3.up);

        Vector3 yClamp = new Vector3(transform.parent.transform.forward.x, 0, transform.parent.transform.forward.z);
        transform.position = yClamp.normalized;

        newTarget = rotation * transform.position;
        newTarget.Set(newTarget.x, 0f, newTarget.z);
        newTarget.Normalize();
        //Debug.DrawRay(playerTransform.position, target, Color.yellow);
        //Debug.Log("(X,Y) & V3: " + x + " , " + y + " & " + worldAngle);
        //Debug.Log("new " + newTarget);

        return newTarget.normalized;
    }



}
