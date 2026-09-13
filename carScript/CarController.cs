using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CarController : MonoBehaviour
{
    private Rigidbody playerRB;
    public WheelColliders colliders;
    public WheelMeshes wheelMeshes;
    //public WheelParticles wheelParticles;
    public  float gasInput;
    public float brakeInput;
    public float steeringInput;
    public GameObject smokePrefab;
    public float motorPower;
    public float brakePower;
    public float slipAngle;
    public  float speed;
    public AnimationCurve steeringCurve;
    public Mybutton gasPedal;
    public Mybutton brakePedal;
    public Mybutton leftButton;
    public Mybutton rightButton;
    public TextMeshProUGUI SpeedT;

    [Header("Car Fuel")]
    public float TankSize = 40f;
    public float DecreaceRate;
    bool IsEmpty;
    public TextMeshProUGUI FuelText;
    public TheMoney M;
    public  bool StartFullTank = false;
    [Header("Fill Fuel Button")]
    public Button FillButton;
    public TextMeshProUGUI FillButtonText;

    //TankFillCalc
    float EmptyLiters;
    float F_price;
    int price;

    float TopSpeed;

    

    //public GameObject CarLight;
    // Start is called before the first frame update
    public void Start()
    {
        playerRB = gameObject.GetComponent<Rigidbody>();
        //InstantiateSmoke();
        ApplyWheelPositions();
        //CarLight.SetActive(false);
        TankSize = PlayerPrefs.GetFloat("TankSize", 40f);
    }

   

   
    /*void InstantiateSmoke()
    {
        wheelParticles.FRWheel = Instantiate(smokePrefab, colliders.FRWheel.transform.position-Vector3.up*colliders.FRWheel.radius, Quaternion.identity, colliders.FRWheel.transform)
            .GetComponent<ParticleSystem>();
        wheelParticles.FLWheel = Instantiate(smokePrefab, colliders.FLWheel.transform.position- Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.FLWheel.transform)
            .GetComponent<ParticleSystem>();
        wheelParticles.RRWheel = Instantiate(smokePrefab, colliders.RRWheel.transform.position- Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.RRWheel.transform)
            .GetComponent<ParticleSystem>();
        wheelParticles.RLWheel = Instantiate(smokePrefab, colliders.RLWheel.transform.position- Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.RLWheel.transform)
            .GetComponent<ParticleSystem>();
    }*/
    // Update is called once per frame

    void Update()
    {
        speed = playerRB.velocity.magnitude;
        int SSS = Mathf.FloorToInt(speed * 1.7f);
        SpeedT.text = SSS + "Km/h" ;
        CheckInput();
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        //CheckParticles();
        ApplyWheelPositions();
        ApplyTank();
        FillTankCalCulator();
        TopSpeedCalc();
    }


    void TopSpeedCalc()
    {
        TopSpeed = motorPower / 9.375f;
    }
    

    void ApplyTank()
    {
        if(StartFullTank == true)
        {
            TankSize = 40f;
            StartFullTank = false;
        }
        PlayerPrefs.SetFloat("TankSize", TankSize);
        int TTT = Mathf.FloorToInt(TankSize);
       FuelText.text = "Fuel: " + TTT + "L";

       DecreaceRate = speed / 10000;
       TankSize -= DecreaceRate;
       if(TankSize <= 0)
       {
         TankSize = 0;
         IsEmpty = true;
       }else{
         IsEmpty = false;
       }
    }

    void FillTankCalCulator()
    {
       EmptyLiters = 40 - TankSize;
       F_price = EmptyLiters * 4.5f;
       price = Mathf.FloorToInt(F_price);
       FillButtonText.text = "fill now for " + price + "$";
       if(TankSize > 35f)
       {
          FillButton.interactable = false;
       }else{
          FillButton.interactable = true;
       }

    }

    public void On_FillBD()
    {
        if(M.Money >= F_price)
        {
            M.Money -= F_price;
            TankSize = 40f;
        }else{

        }
        
    }


    void CheckInput()
    {
        if(IsEmpty != true)
        {
           gasInput = Input.GetAxis("Vertical");
           if (gasPedal.isPressed && speed * 1.7f < TopSpeed)
           {
               gasInput += gasPedal.dampenPress;
             //  CarLight.SetActive(false);
           }
           
           if (brakePedal.isPressed)
           {
               gasInput -= brakePedal.dampenPress;
             //  CarLight.SetActive(true);
           }else{
              // CarLight.SetActive(false);
           }
        }else{
            gasInput = 0;
        }
        
        steeringInput = Input.GetAxis("Horizontal");
        if (rightButton.isPressed)
        {
            steeringInput += rightButton.dampenPress;
        }
    
        if (leftButton.isPressed)
        {
            steeringInput -= leftButton.dampenPress;
        }
       /* slipAngle = Vector3.Angle(transform.forward, playerRB.velocity-transform.forward);

        if (slipAngle < 120f) {
            if (gasInput < 0)
            {
                brakeInput = Mathf.Abs( gasInput);
                gasInput = 0.01f;
            }
            else
            {
                brakeInput = 0;
            }
        }
        else
        {
            brakeInput = 0;
        }*/

        
        slipAngle = Vector3.Angle(transform.forward, playerRB.velocity-transform.forward);

                //fixed code to brake even after going on reverse 
        float movingDirection = Vector3.Dot(transform.forward, playerRB.velocity);
        if (movingDirection < -0.5f && gasInput > 0)
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else if (movingDirection > 0.5f && gasInput < 0)
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else
        {
            if(IsEmpty != true)
            {
                brakeInput = 0;
            }else
            {
                brakeInput = 1;
            }
             
        }
          


        /*
        old code
        if (slipAngle < 120f) {
            if (gasInput < 0)
            {
                brakeInput = Mathf.Abs( gasInput);
                gasInput = 0;
            }
            else
            {
                brakeInput = 0;
            }
        }
        else
        {
            brakeInput = 0;
        }*/

    }

    void ApplyBrake()
    {
        float z = 0.1f;
        float y = 0.1f;
        //colliders.FRWheel.brakeTorque = brakeInput * brakePower* 0.1f ;
        //colliders.FLWheel.brakeTorque = brakeInput * brakePower * 0.1f;
        if (z > 0)
        {
            y = 0.1f;
            if (SA == 0)
            {
                colliders.RRWheel.brakeTorque = brakeInput * brakePower * 0.3f;
                colliders.RLWheel.brakeTorque = brakeInput * brakePower * 0.3f;
            }
            else
            {
                colliders.RRWheel.brakeTorque = brakeInput * brakePower * 0.05f;
                colliders.RLWheel.brakeTorque = brakeInput * brakePower * 0.05f;
            }
            
            z -= Time.deltaTime;
        }
        if (z <= 0)
        {
            z = 0;
            colliders.RRWheel.brakeTorque = brakeInput * brakePower * 0f;
            colliders.RLWheel.brakeTorque = brakeInput * brakePower * 0f;
            y -= Time.deltaTime;
        }
        if (y <= 0)
        {
            z = 0.1f;
        }
        
        

    }
    
    void ApplyMotor() {
        if (SA < 0) // Left
        {
          colliders.RRWheel.motorTorque = motorPower * gasInput + (2*SA*speed);
          colliders.RLWheel.motorTorque = motorPower * gasInput - (2*SA-speed);
        }
        else if (SA > 0) //Right
        {
          colliders.RRWheel.motorTorque = motorPower * gasInput - (2*SA-speed);
          colliders.RLWheel.motorTorque = motorPower * gasInput + (2*SA*speed);
        }
        else
        {
          colliders.RRWheel.motorTorque = motorPower * gasInput;
          colliders.RLWheel.motorTorque = motorPower * gasInput;
        }
        
        //colliders.FRWheel.motorTorque = motorPower * gasInput;
       // colliders.FLWheel.motorTorque = motorPower * gasInput;

    }
    public float SA;
    void ApplySteering()
    {

        float steeringAngle = steeringInput* steeringCurve.Evaluate(speed);
        SA = steeringAngle;
        if (slipAngle < 120f)
        {
            steeringAngle += Vector3.SignedAngle(transform.forward, playerRB.velocity + transform.forward, Vector3.up);
        }
        
        steeringAngle = Mathf.Clamp(steeringAngle, -90f, 90f);
        colliders.FRWheel.steerAngle = steeringAngle;
        colliders.FLWheel.steerAngle = steeringAngle;
    }
    
    void ApplyWheelPositions()
    {
        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel);
    }    
   
   /* void CheckParticles() 
    {
        WheelHit[] wheelHits = new WheelHit[4];
        colliders.FRWheel.GetGroundHit(out wheelHits[0]);
        colliders.FLWheel.GetGroundHit(out wheelHits[1]);

        colliders.FRWheel.GetGroundHit(out wheelHits[0]);
        colliders.FLWheel.GetGroundHit(out wheelHits[1]);

        colliders.RRWheel.GetGroundHit(out wheelHits[2]);
        colliders.RLWheel.GetGroundHit(out wheelHits[3]);

        float slipAllowance = 0.5f;
        
        if ((Mathf.Abs(wheelHits[0].sidewaysSlip) + Mathf.Abs(wheelHits[0].forwardSlip) > slipAllowance)){
            wheelParticles.FRWheel.Play();
        }
        else
        {
            wheelParticles.FRWheel.Stop();
        }
        
        if ((Mathf.Abs(wheelHits[1].sidewaysSlip) + Mathf.Abs(wheelHits[1].forwardSlip) > slipAllowance)){
            wheelParticles.FLWheel.Play();
        }
        else
        {
            wheelParticles.FLWheel.Stop();
        }
        
        if ((Mathf.Abs(wheelHits[2].sidewaysSlip) + Mathf.Abs(wheelHits[2].forwardSlip) > slipAllowance)){
            wheelParticles.RRWheel.Play();
        }
        else
        {
            wheelParticles.RRWheel.Stop();
        }
        
        if ((Mathf.Abs(wheelHits[3].sidewaysSlip) + Mathf.Abs(wheelHits[3].forwardSlip) > slipAllowance)){
            wheelParticles.RLWheel.Play();
        }
        else
        {
            wheelParticles.RLWheel.Stop();
        }
    
        


    }    */
    void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh)
    {
        Quaternion quat;
        Vector3 position;
        coll.GetWorldPose(out position, out quat);
        wheelMesh.transform.position = position;
     wheelMesh.transform.rotation = quat;
    }
   
}
[System.Serializable]
public class WheelColliders //class WheelColliders
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider RRWheel;
    public WheelCollider RLWheel;
}
   
[System.Serializable]

public class WheelMeshes
{
    public MeshRenderer FRWheel;
    public MeshRenderer FLWheel;
    public MeshRenderer RRWheel;
    public MeshRenderer RLWheel;
}
/*[System.Serializable]
public class WheelParticles{
    public ParticleSystem FRWheel;
    public ParticleSystem FLWheel;
    public ParticleSystem RRWheel;
    public ParticleSystem RLWheel;

}*/
   
