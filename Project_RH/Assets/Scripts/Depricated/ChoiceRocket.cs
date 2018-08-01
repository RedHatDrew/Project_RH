/*
ChoiceRocket.cs

Rocket script used for making choices in dialog trees.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChoiceRocket : MonoBehaviour
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

    // SBI = Scene Build Index
    [SerializeField] int Choice1SBI;
    [SerializeField] int Choice2SBI;
    [SerializeField] int Choice3SBI;

    enum State { Alive, Dying, Transcending, SafeTravel }
    State state = State.Alive;

    bool collisionDisabled = false;

    int sceneCount;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        sceneCount = SceneManager.sceneCountInBuildSettings;

    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
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
                StartSuccessSequence();
                break;
            case "Choice1":
                MakeChoice1();
                break;
            case "Choice2":
                MakeChoice2();
                break;
            case "Choice3":
                MakeChoice3();
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

    private void MakeChoice1()
    {
        SceneManager.LoadScene(Choice1SBI);
    }

    private void MakeChoice2()
    {
        SceneManager.LoadScene(Choice2SBI);
    }

    private void MakeChoice3()
    {
        SceneManager.LoadScene(Choice3SBI);
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
        SceneManager.LoadScene(1);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex > sceneCount - 1)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex); // todo allow for more than 2 levels
    }

    private void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    // DO NOT ALTER
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

    // DO NOT ALTER
    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }


    // DO NOT ALTER
    private void RespondToRotateInput()
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

    // DO NOT ALTER
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