// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class Movement : MonoBehaviour
// {
//     Rigidbody rigidbdy;
//     AudioSource AS;



//     [SerializeField] AudioClip mainEngine;

//     [SerializeField] float Movementspeed= 1000f;
//     [SerializeField] float Rotationspeed = 1f;

//     [SerializeField] ParticleSystem RocketBoostParticles;
//     [SerializeField] ParticleSystem leftThrustParticles;
//     [SerializeField] ParticleSystem RightThrustParticles;


//     void Start()
//     {
//         rigidbdy = GetComponent<Rigidbody>();
//         AS = GetComponent<AudioSource>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         ProcessThrust();
//         ProcessRotation();

//     }


//     void ProcessThrust()
//     {
//         if (Input.GetKey(KeyCode.Space)|| Input.GetKey(KeyCode.UpArrow))
//         {
//             StartThrusting();
//         }
//         else
//         {
//             StopThrusting();
//         }
//     }

//     void ProcessRotation()
//     {
//         if (Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.LeftArrow))
//         {
//             RotateLeft();
//         }
//         else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
//         {
//             RotateRight();
//         }
//         else
//         {
//             StopRotation();
//         }
//     }

//     //--------------------------------------------------------------------------------
//     private void StartThrusting()
//     {
//         rigidbdy.AddRelativeForce(Vector3.up * Time.deltaTime * Movementspeed);
//         if (!AS.isPlaying)
//         {
//             AS.PlayOneShot(mainEngine);
//         }
//         if (!RocketBoostParticles.isPlaying)
//         {
//             RocketBoostParticles.Play();
//         }
//     }

//     private void StopThrusting()
//     {
//         RocketBoostParticles.Stop();
//         AS.Stop();
//     }

//     private void RotateLeft()
//     {
//         ApplyRotation(1);
//         if (!leftThrustParticles.isPlaying)
//         {
//             leftThrustParticles.Play();
//         }
//     }

//     private void RotateRight()
//     {
//         ApplyRotation(-1);
//         if (!RightThrustParticles.isPlaying)
//         {
//             RightThrustParticles.Play();
//         }
//     }

//     private void StopRotation()
//     {
//         leftThrustParticles.Stop();
//         RightThrustParticles.Stop();
//     }


//     private void ApplyRotation(float rotation)
//     {
//         rigidbdy.freezeRotation = true;
//         transform.Rotate(Vector3.forward * Time.deltaTime * Rotationspeed * rotation);
//         rigidbdy.freezeRotation = false;
//     }
// }
