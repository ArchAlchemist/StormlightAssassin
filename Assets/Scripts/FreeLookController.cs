using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CinemachineFreeLook))]
public class FreeLookController : MonoBehaviour
{

    [Range(0f, 10f)] public float LookSpeed = 1f;
    public bool InvertY = true;
    private CinemachineFreeLook m_freeLookComponent;

    // Start is called before the first frame update
    public void Start()
    {
        m_freeLookComponent = GetComponent<CinemachineFreeLook>();
    }

    void Update()
    {
        // TODO set small delay timer to reorient to 'S' direction
    }

    //public void OnLook(InputAction.CallbackContext context)
    //{
    //    //Normalize the vector to have an uniform vector in whichever form it came from (I.E Gamepad, mouse, etc)
    //    Vector2 lookMovement = context.ReadValue<Vector2>().normalized;
    //    lookMovement.y = InvertY ? -lookMovement.y : lookMovement.y;

    //    // This is because X axis is only contains between -180 and 180 instead of 0 and 1 like the Y axis
    //    lookMovement.x = lookMovement.x * 180f;

    //    //Ajust axis values using look speed and Time.deltaTime so the look doesn't go faster if there is more FPS
    //    m_freeLookComponent.m_XAxis.Value += lookMovement.x * LookSpeed * Time.deltaTime;
    //    m_freeLookComponent.m_YAxis.Value += lookMovement.y * LookSpeed * Time.deltaTime;
    //}

    public void look(InputAction.CallbackContext context)
    {
        //Normalize the vector to have an uniform vector in whichever form it came from (I.E Gamepad, mouse, etc)
        Vector2 lookMovement = context.ReadValue<Vector2>().normalized;
        lookMovement.y = InvertY ? -lookMovement.y : lookMovement.y;

        // This is because X axis is only contains between -180 and 180 instead of 0 and 1 like the Y axis
        lookMovement.x = lookMovement.x * 180f;

        //Ajust axis values using look speed and Time.deltaTime so the look doesn't go faster if there is more FPS
        m_freeLookComponent.m_XAxis.Value += lookMovement.x * LookSpeed * Time.deltaTime;
        m_freeLookComponent.m_YAxis.Value += lookMovement.y * LookSpeed * Time.deltaTime;
    }
}
