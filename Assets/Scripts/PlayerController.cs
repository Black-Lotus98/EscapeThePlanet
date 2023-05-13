// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.InputSystem;

// public class PlayerController : MonoBehaviour
// {

//     [SerializeField] float Movementspeed = 1000f;
//     [SerializeField] float Rotationspeed = 1f;
//     Rigidbody rigidbdy;

//     void Start()
//     {
//         rigidbdy = GetComponent<Rigidbody>();

//     }

//     // Update is called once per frame
//     void Update()
//     {
//     }

//     void ProcessThrust()
//     {
//             StartThrusting();
//             // if (player.getUsingFuelStatus())
//             // {

//             //     if (player.getFuelCounter() > 0)
//             //     {
//             //         player.FuelUsage(-1);
//             //     }
//             //     else
//             //     {
//             //         player.setFuelCounter(0);
//             //         this.enabled = false;
//             //         StopThrusting();
//             //     }
//             // }
//     }

//     public void StartThrusting()
//     {
//         rigidbdy.AddRelativeForce(Vector3.up * Time.deltaTime * Movementspeed);
//         // if (!AS.isPlaying)
//         // {
//         //     AS.PlayOneShot(mainEngine);
//         // }
//         // if (!RocketBoostParticles.isPlaying)
//         // {
//         //     RocketBoostParticles.Play();
//         // }
//     }

//     private void StopThrusting()
//     {
//         // RocketBoostParticles.Stop();
//         // AS.Stop();
//     }

// }
