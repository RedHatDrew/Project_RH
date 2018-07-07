using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserTime : MonoBehaviour {

    //Change from int to float?
    [SerializeField] int gameTime = 0;
    [SerializeField] Text countdownText;

    int sceneCount;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        sceneCount = SceneManager.sceneCountInBuildSettings;
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine("TimerStart");
    }

    // Update is called once per frame
    void Update()
    {
        countdownText.text = ("Game Time: " + gameTime);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        print("Current Scene: " + currentSceneIndex);
        print("Total Scenes: " + sceneCount);
        if (currentSceneIndex == sceneCount -1)
        {
            StopAllCoroutines();
        }
        if (currentSceneIndex == 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator TimerStart()
    {
        while (true)
        {
            //Update to take fractions of seconds into account
            yield return new WaitForSeconds(1);
            gameTime++;
        }
    }
}
