using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidBehavior : MonoBehaviour
{

    Renderer rend;
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;
    Vector3 angularVelocity;
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;
    float fill;
    public string FillName;

    public GameObject LiquidSpawner;


    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        fill = rend.material.GetFloat(FillName);
        //rend.material.SetFloat("_fill2", fill);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Angle(Vector3.down, LiquidSpawner.transform.forward) <= 90f && fill >= -1f)
        {
            //dropping.Play();
            Debug.Log("entered");
            LiquidSpawner.GetComponent<WaterDropsSpawner>().StartDrop();
            float FillDroped = 0.3f * Time.deltaTime;
            fill -= FillDroped;
            LiquidSpawner.GetComponent<WaterDropsSpawner>().IncreaseFillValue(FillDroped);
            rend.material.SetFloat(FillName, fill);
        }
        else
        {
            //dropping.Stop();
            LiquidSpawner.GetComponent<WaterDropsSpawner>().EndDrop();
        }

        time += Time.deltaTime;
        // decrease wobble over time
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));

        // make a sine wave of the decreasing wobble
        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

        // send it to the shader
        rend.material.SetFloat("_WobbleX", wobbleAmountX);
        rend.material.SetFloat("_WobbleZ", wobbleAmountZ);

        // velocity
        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;


        // add clamped velocity to wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        // keep last position
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }

    public void FillLiquidContainer(float addfill)
    {
        fill += addfill;
        rend.material.SetFloat(FillName, fill);
    }

}