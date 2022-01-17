using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidVectorController : MonoBehaviour {
    /* General Variables and Links to GameObject */
    public GameObject playerObject; // Link to the Player
    public GameObject playerPointer; // Link to an empty object which will point toward the player
    public float speed = 1.0f; // Movement speed
    public float rotateSpeed = 15.0f;

    /* Movement Vectors */
    /* Wander Vector */
    private Vector3 wanderVec; // Generated at spawn, determines the initial path of motion for the Boid
    public float wanderWeight = 1.0f; // Wander's influence on the final motion vector
    public float wanderOffsetMin = 10.0f; // Minimum value that a Wander Vector will be offset by when generated
    public float wanderOffsetMax = 30.0f; // Maximum value that a Wander Vector will be offset by when generated

    /* Light Vector */
    private Vector3 lightVec; // Always faces toward the player, reaction to usage of lights by the player
    public float lightWeight = 0.0f; // Light's influence on the final motion vector
    public bool lightOn = false; // Is the player's Lights on?
    private bool lightThreshold = false; // Used to cause a Wander calculation when lightWeight reaches 1.0f;

    /* Sonar Vector */
    private Vector3 sonarVec; // Always faces toward the player, reaction to usage of sonar by the player
    public float sonarWeight = 0.0f; // Sonar's influence on the final motion vector

    /* Stress Vector */
    private Vector3 stressVec; // Always faces away from the player, triggers after a threshold has been reached to encourage the Boid to move away
    public float stressWeight = 0.0f; // Stress' influence on the final motion vector
    public float stressThreshold = 0.6f; // Threshold level Stress must reach before its weight begins to affect motion
    public bool stressActive = false; // Stress begins to affect motion when this variable is true

    /* True Motion Vector */
    private Vector3 trueMotionVec;

    [SerializeField] private GameManager gm;

    [SerializeField] private Animator anim;
    private int killPlayerHash;
    private bool killing = false;

    public AudioSource killSound;
    public AudioSource shipBreakSound;
    public AudioSource groanSound;
    public AudioSource[] groanList;
    private AudioSource groan;

    private int minGroanTime = 8;
    private int maxGroanTime = 15;

    // Start is called before the first frame update
    void Start() {
        /* Search the scene for the GameObject with the Player Tag */
        playerObject = GameObject.FindWithTag("Player");

        killPlayerHash = Animator.StringToHash("killPlayer");

        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();

        /* Catch for if the PlayerObject was not found in the scene */
        if (playerObject == null) {
            Debug.LogError("Boid Vector Controller: PlayerObject could not be found");
            playerObject = new GameObject("TempPlayer");
            playerObject.transform.position = Vector3.zero;
        }

        /* Catch for if the PlayerPointer is not set correctly */
        if (playerPointer == null) {
            Debug.LogError("Boid Vector Controller: PlayerPointer not properly assigned");
            playerPointer = new GameObject("TempPointer");
            playerPointer.transform.SetParent(transform);
            playerPointer.transform.position = transform.position;
        }

        CalculateWander();

        StartCoroutine("GroanLoop");

    }

    // Update is called once per frame
    void Update() {

        if (!killing)
        {
            /* Update Player Oriented Movement Vectors */
            playerPointer.transform.LookAt(playerObject.transform);
            lightVec = playerPointer.transform.forward;
            sonarVec = lightVec;
            stressVec = -lightVec;

            /* Slightly alter the direction of Wander to create a more random and erratic motion path */
            wanderVec = Quaternion.Euler((Random.Range(0, 2) == 1 ? 0.1f : -0.1f), (Random.Range(0, 2) == 1 ? 0.5f : -0.5f), 0) * wanderVec;

            /* Calculations for Player created stimuli */
            LightCheck();
            SonarCheck();
            wanderWeight -= lightWeight + sonarWeight;
            if (wanderWeight < 0.0f) wanderWeight = 0.0f;

            /* Calculation for the trueMotion Vector, the final path that the Boid will actually move */
            /* Stress check: Stress isn't factored into the trueMotion calculation when it hasn't reached the threshold */
            if (!stressActive) // Below threshold
                trueMotionVec = Vector3.Normalize(((lightVec * lightWeight) + (sonarVec * sonarWeight) + (wanderVec * wanderWeight)) / 4.0f);
            else // Above threshold
                trueMotionVec = Vector3.Normalize(((lightVec * lightWeight) + (sonarVec * sonarWeight) + (stressVec * stressWeight) + (wanderVec * wanderWeight)) / 4.0f);
            // Debug.Log(trueMotionVec);

            /* Move the Boid */
            Vector3 rotateDirection = Vector3.RotateTowards(transform.forward, trueMotionVec, speed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(rotateDirection);

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            Debug.DrawRay(transform.position, transform.forward * 20, Color.cyan);



            /* Debug rays to visualize motion vectors */
            Debug.DrawRay(transform.position, lightVec * lightWeight * 5, Color.yellow);
            Debug.DrawRay(transform.position, stressVec * stressWeight * 5, Color.blue);
            Debug.DrawRay(transform.position, wanderVec * 5, Color.green);
            Debug.DrawRay(transform.position, trueMotionVec * 20, Color.red);

            /* Reset wanderWeight for the next frame */
            wanderWeight = 1.0f;
        }
    }

    /* Perform calulations to generate a randomly offset Wander Vector */
    void CalculateWander()
    {
        /* Initialize the Movement Vectors */
        playerPointer.transform.LookAt(playerObject.transform);
        lightVec = playerPointer.transform.forward;
        sonarVec = lightVec;
        stressVec = -lightVec;

        /* Create a randomized offset for the Wander Vector */
        wanderVec = lightVec;
        float xWander = Random.Range(wanderOffsetMin, wanderOffsetMax) * (Random.Range(0, 2) == 1 ? 1.0f : -1.0f);
        float yWander = Random.Range(wanderOffsetMin, wanderOffsetMax) * (Random.Range(0, 2) == 1 ? 1.0f : -1.0f);
        wanderVec = Quaternion.Euler(xWander, yWander, 0) * wanderVec;
    }

    /* Performs calculations related to the Light stimulus from the Player */
    void LightCheck()
    {
        if (lightOn) // While Player has light enabled
        {
            /* Recalculate Wander and disable stress, this encourages the Boid to reset its path to investigate the new stimulus */
            if (!lightThreshold)
            {
                CalculateWander();
                stressWeight = stressWeight * 0.8f;
                lightThreshold = true; // Prevents variables from being set every frame
            }

            if (lightWeight < 1.0f) lightWeight += 0.15f * Time.deltaTime;
            else
            {
                /* lightWeight is set to maximum value */
                lightWeight = 1.0f;
            }
        }
        else // While Player has light disabled
        {
            /* Reset threshold variable */
            if (lightThreshold) lightThreshold = false;

            /* Light Vector Weight decays while Player's lights are turned off, to a minimum value */
            if (lightWeight > 0.0f) lightWeight -= 0.2f * Time.deltaTime;
            else lightWeight = 0.0f;
        }
    }

    void SonarCheck()
    {
        /* sonarWeight is set to maximum value */
        if (sonarWeight > 1.0f) sonarWeight = 1.0f;

        /* Sonar Vector Weight decays constantly, to a minimum value */
        if (sonarWeight > 0.0f) sonarWeight -= 0.05f * Time.deltaTime;
        else sonarWeight = 0.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        /* If the Boid gets too close, it will head straight for the player instead of wander */
        if (other.tag == "KillSphere")
        {
            Debug.Log("found you!");
            wanderVec = lightVec;
            stressWeight = 0.0f;
            KillPlayer();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        /* While inside the Stress Sphere, increase the Stress Weight */
        if (other.tag == "StressSphere")
        {
            if (stressWeight < stressThreshold && !stressActive)
            {
                stressWeight += 0.05f * Time.deltaTime;
            }
            else
            {
                stressWeight = stressThreshold;
                stressActive = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        /* Upon leaving the Exit Sphere, teleport to a new location and recalculate the Wander Vector */
        if(other.tag == "ExitSphere")
        {
            Debug.Log("exited");
            transform.position = new Vector3(
                playerObject.transform.position.x + 25.0f * (Random.Range(0, 2) == 1 ? 1.0f : -1.0f),
                playerObject.transform.position.y + 25.0f * (Random.Range(0, 2) == 1 ? 1.0f : -1.0f),
                playerObject.transform.position.z + 25.0f * (Random.Range(0, 2) == 1 ? 1.0f : -1.0f));
            stressWeight = 0.0f;
            stressActive = false;
            CalculateWander();
        }
    }

    public void KillPlayer()
    {
        gameObject.transform.LookAt(playerObject.GetComponent<MovementShipForce>().mainCamera.transform);
        anim.SetTrigger(killPlayerHash);
        killSound.Play();
        killing = true;
        playerObject.GetComponent<MovementShipForce>().Death(transform.position);
    }

    public void EndGame()
    {
        if (!gm.testing)
        {
            shipBreakSound.Play();
            gm.SceneTransition("GameOverYouDied");
        }
    }

    /* 
     * Plays the groan sound after a random period of time
     */
    private IEnumerator GroanLoop()
    {
        int randTime;
        while (true)
        {
            groan = groanList[Random.Range(0, groanList.Length)];
            // Plays the groan sound
            //groanSound.PlayOneShot(groan);
            groan.Play();
            // Selects random time between min and mac groan time
            randTime = Random.Range(minGroanTime, maxGroanTime);
            yield return new WaitForSeconds(randTime);
        }
    }
}
