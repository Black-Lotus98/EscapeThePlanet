using UnityEngine;
using DG.Tweening;
using TMPro;
public class DoorTrigger : MonoBehaviour
{
    [SerializeField] bool isPlayerInside = false;
    // [SerializeField] bool doorDisabled = true;


    [Tooltip("Use it when you have a gate with one door //Door opens and closes up and down")]
    [Header("Normal Door Settings")]

    [SerializeField] GameObject Door;
    [SerializeField] float OpeningPosition;
    [SerializeField] float ClosingPosition;
    [SerializeField] bool DoorIsOpen;
    // [SerializeField] string DoorName = "DoorName";

    [Header("Locked Door Settings")]
    [Tooltip("Use it when you want the player to use key to open the door")]
    [SerializeField] bool useKeyToOpen;
    [SerializeField] bool playerHasKey;
    // [SerializeField] float doorCounter = 2;


    [Header("Tip Settings")]
    [SerializeField] GameObject textGO;
    TextMeshProUGUI tipText;

    [Header("Sound FX")]
    AudioSource AS;
    [SerializeField] AudioClip DoorOpeningSFX;
    [SerializeField] AudioClip DoorClosingSFX;

    void Start()
    {
        textGO.SetActive(false);
        tipText = textGO.GetComponent<TextMeshProUGUI>();
        AS = GetComponent<AudioSource>();
    }


    private void Update()
    {
        if (isPlayerInside)
        {
            if (useKeyToOpen)
            {
                if (!playerHasKey)
                {
                    tipText.text = "Key required to open";
                }
                else
                {
                    tipText.text = "Wait for the door to Open";
                    Invoke("OpenDoor", 2f);
                }
            }
            else
            {
                tipText.text = "Wait for the door to Open";
                Invoke("OpenDoor", 2f);
                // DoorController();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var KeyManager = other.gameObject.GetComponent<KeyManager>();
        if (other.gameObject.tag == "Player")
        {
            playerHasKey = KeyManager.PlayerHasKey;
            textGO.SetActive(true);
            isPlayerInside = true;
        }


    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            if (DoorIsOpen)
            {
                CloseDoor();
                AS.Stop();
                AS.PlayOneShot(DoorOpeningSFX);
            }
            textGO.SetActive(false);
            isPlayerInside = false;
        }
    }


    void OpenDoor()
    {
        Door.transform.DOLocalMoveY(OpeningPosition, DoorOpeningSFX.length);
        DoorIsOpen = true;
    }

    void CloseDoor()
    {
        Door.transform.DOLocalMoveY(ClosingPosition, DoorClosingSFX.length);
        DoorIsOpen = false;
    }

    void DoorController()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (DoorIsOpen)
            {
                AS.Stop();
                AS.PlayOneShot(DoorOpeningSFX);
                CloseDoor();
            }
            else
            {
                AS.Stop();
                AS.PlayOneShot(DoorClosingSFX);
                OpenDoor();
            }
        }
    }
}