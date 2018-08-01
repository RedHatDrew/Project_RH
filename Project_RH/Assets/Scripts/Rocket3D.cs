using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO: Set axis rotation limiters? (x to 180, Y locked, Z has none/is free)

public class Rocket3D : MonoBehaviour
{

    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField] float rcsThrust = 10f;
    [SerializeField] float mainThrust = 5f;
    [SerializeField] float levelLoadDelay = 3f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip rocketCrash;
    [SerializeField] AudioClip levelWin;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem levelWinParticles;
    [SerializeField] ParticleSystem rocketCrashParticles;

    enum State { Alive, Dying, Transcending, SafeTravel }
    State state = State.Alive;

    //float rocketSpeed = rigidbody.velocity.magnitude

    bool collisionDisabled = false;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        print(rigidBody.velocity.magnitude);
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInputZ();
            RespondToRotateInputX();
            RespondToRotateInputY();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionDisabled) { return; } //Ignore collisions when dead or disabled.

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                break;
            case "TheGround":
                break;
            default:
                break;
        }

    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            ApplyThrust();
        }

        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInputZ()
    {
        rigidBody.freezeRotation = true; // Take manual control over rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // Resume physics control over movement
    }

    private void RespondToRotateInputX()
    {
        rigidBody.freezeRotation = true; // Take manual control over rotation

        float rotationThisFrame = rcsThrust *  Time.deltaTime;

        if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(Vector3.right * rotationThisFrame);
        }

        else if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(-Vector3.right * rotationThisFrame);
        }

        //Add controller analog stick support below.
        //transform.Rotate(Input.)

        rigidBody.freezeRotation = false; // Resume physics control over movement
    }

    private void RespondToRotateInputY()
    {
        rigidBody.freezeRotation = true; // Take manual control over rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up * rotationThisFrame);
        }

        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(-Vector3.up * rotationThisFrame);
        }

        //Add controller analog stick support below.
        //transform.Rotate(Input.)

        rigidBody.freezeRotation = false; // Resume physics control over movement
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            // Note: assigning the opposite of a bool to itself makes a toggle. Dope!
            collisionDisabled = !collisionDisabled;
            print("CollisionsDisabled " + collisionDisabled);
        }
    }
}