using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bldg_Fodder : MonoBehaviour {

    enum State { Alive, Dying, Transcending, SafeTravel }
    State state = State.Alive;

    bool collisionDisabled = false;

    int sceneCount;

    // Use this for initialization
    void Start()
    {
        //rigidBody = GetComponent<Rigidbody>();
        //audioSource = GetComponent<AudioSource>();
        //sceneCount = SceneManager.sceneCountInBuildSettings;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionDisabled) { return; } //Ignore collisions when dead or disabled.

        switch (collision.gameObject.tag)
        {
            case "Player":
                Destroy(GetComponent<BoxCollider>());
                Destroy(gameObject, 5);
                break;
            case "TheGround":
                Destroy(gameObject);
                break;
            default:
                break;
        }

    }
}