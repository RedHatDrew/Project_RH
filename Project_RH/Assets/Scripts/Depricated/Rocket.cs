using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

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

    bool collisionDisabled = false;

    int sceneCount;

    // Use this for initialization
    void Start () 
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        sceneCount = SceneManager.sceneCountInBuildSettings;

    }
	
	// Update is called once per frame
	void Update () 
    {
        if (state == State.Alive)
            {
                RespondToThrustInput();
                RespondToRotateInputZ();
                RespondToRotateInputX();
        }
        if(Debug.isDebugBuild)
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
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }

    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        mainEngineParticles.Stop();
        levelWinParticles.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(levelWin);
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        mainEngineParticles.Stop();
        rocketCrashParticles.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(rocketCrash);
        Invoke("ReloadLevel", levelLoadDelay);
    }

    public void LoadFirstLevel()
    {
        SceneManager.LoadScene(2);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex > sceneCount - 1)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }


    private void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
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

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(Vector3.right * rotationThisFrame);
        }

        else if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(-Vector3.right * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // Resume physics control over movement
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKey(KeyCode.L))
        {
            LoadNextLevel();
        }

        else if (Input.GetKeyUp(KeyCode.C))
        {
            // Note: assigning the opposite of a bool to itself makes a toggle. Dope!
            collisionDisabled = !collisionDisabled;
            print("CollisionsDisabled " + collisionDisabled);
        }
    }
}