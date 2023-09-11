// This script is no longer used
// I switched to the command pattern
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
public class MobileController : MonoBehaviour
{
    Rigidbody rigidbdy;
    AudioSource AS;

    [SerializeField] AudioClip mainEngine;

    [SerializeField] float Movementspeed = 1000f;
    [SerializeField] float Rotationspeed = 1f;

    [SerializeField] ParticleSystem RocketBoostParticles;
    [SerializeField] ParticleSystem leftThrustParticles;
    [SerializeField] ParticleSystem RightThrustParticles;

    Player player;




    void Start()
    {
        player = gameObject.GetComponent<Player>();
        rigidbdy = GetComponent<Rigidbody>();
        AS = GetComponent<AudioSource>();
    }

    void Update()
    {
        ProcessThrust();
        ProcessRotation();
    }


    void ProcessThrust()
    {

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || CrossPlatformInputManager.GetButton("Thrust"))
        {
            StartThrusting();
            if (player.GetIsUsingFuel())
            {

                if (player.GetFuelCounter() > 0)
                {
                    player.FuelUsage(-1);
                }
                else
                {
                    this.enabled = false;
                    StopThrusting();
                }
            }
        }
        else
        {
            StopThrusting();
        }
    }

    void ProcessRotation()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || CrossPlatformInputManager.GetButton("Left"))
        {
            RotateLeft();
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || CrossPlatformInputManager.GetButton("Right"))
        {
            RotateRight();
        }
        else
        {
            StopRotation();
        }
    }



    //--------------------------------------------------------------------------------
    public void StartThrusting()
    {
        rigidbdy.AddRelativeForce(Vector3.up * Time.deltaTime * Movementspeed);
        if (!AS.isPlaying)
        {
            AS.PlayOneShot(mainEngine);
        }
        if (!RocketBoostParticles.isPlaying)
        {
            RocketBoostParticles.Play();
        }
    }

    private void StopThrusting()
    {
        RocketBoostParticles.Stop();
        AS.Stop();
    }

    public void RotateLeft()
    {
        ApplyRotation(1);
        if (!leftThrustParticles.isPlaying)
        {
            leftThrustParticles.Play();
        }
    }

    public void RotateRight()
    {
        ApplyRotation(-1);
        if (!RightThrustParticles.isPlaying)
        {
            RightThrustParticles.Play();
        }
    }

    private void StopRotation()
    {
        leftThrustParticles.Stop();
        RightThrustParticles.Stop();
    }


    private void ApplyRotation(float rotation)
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, 0, transform.eulerAngles.z);
        rigidbdy.freezeRotation = true;
        transform.Rotate(Vector3.forward * Time.deltaTime * Rotationspeed * rotation);
        rigidbdy.freezeRotation = false;
    }

}