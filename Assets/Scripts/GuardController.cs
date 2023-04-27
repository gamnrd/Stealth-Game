using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuardController : MonoBehaviour
{
    //Patrolling
    [Header("Guard Patrolling")]
    [SerializeField] private bool isPatrolling = true;
    [SerializeField] private bool isWaiting = false;
    [SerializeField] private Pathfinder PatrolPath;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float distanceToPoint = 0.6f;
    private Transform nextPoint = null;

    //Vision
    [Header("Guard Vision")]
    [SerializeField] private Light vision;
    [SerializeField] private float visionDistance;
    [SerializeField] private float visionAngle;
    public LayerMask visionMask;



    //Pursuing
    [Header("Guard Pursuing")]
    [SerializeField] private bool isPursuing = false;
    [SerializeField] private bool playerVisible = false;
    [SerializeField] private bool alertable = true;
    [SerializeField] private Vector3 playerLastPos;
    [SerializeField] private float pursuitSpeed = 7.5f;
    [SerializeField] private float playerDistance;
    [SerializeField] private float distanceToCapture;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject alertPopup;
 

    //Hearing
    [Header("Guard Hearing")]
    [SerializeField] private bool isInvestigating = false;
    [SerializeField] private Vector3 soundPos;
    [SerializeField] private float investigateSpeed = 4f;
    [SerializeField] private float soundThreashold = 5f;


    //Audio & other
    private AudioSource src;
    private Vector3 lookDirection;
    public int timeToWait = 3;
    private NavMeshAgent navAgent;
    private Animator animator;



    private void Awake()
    {
        //Get random survivor model
        transform.GetChild(Random.Range(1, 25)).gameObject.SetActive(true);

        src = GetComponent<AudioSource>();
    }



    // Start is called before the first frame update
    void Start()
    {
        visionAngle = vision.spotAngle;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        navAgent = this.GetComponent<NavMeshAgent>();   //Get nav mesh
        navAgent.speed = patrolSpeed;  //Set speed when using nav mesh

        animator = GetComponentInParent<Animator>();
        animator.SetFloat("MotionSpeed", 1);
        animator.SetBool("Grounded", true);
    }



    // Update is called once per frame
    void Update()
    {
        //If patrolling and a patrol path exists
        if (isPatrolling && PatrolPath != null && !isWaiting)
        {
            Patrol();
        }

        //If the player is alive
        if (PlayerLives.Instance.isAlive)
        {
            //Check if player is visible
            CheckPursuing();

            //If chasing the player
            if (isPursuing)
            {
                //Is player caught
                CheckCaught();
                //move to player pos
                MoveToNextPoint(pursuitSpeed, playerLastPos);
            }

            //If investigating sound
            if (isInvestigating)
            {
                MoveToNextPoint(investigateSpeed, soundPos);
            }
        }
    }



    //Walk from point to point from patrol path, waiting at each point
    private void Patrol()
    {
        //If starting patrol get first waypoint
        if (nextPoint == null)
        {
            nextPoint = PatrolPath.GetPoint(null);
        }

        //Once back in patrol mode the guard may be alerted again
        if (!alertable) alertable = true;

        MoveToNextPoint(patrolSpeed, nextPoint);

        //if at the next waypoint
        if (Vector3.Distance(transform.position, nextPoint.position) < distanceToPoint)
        {
            StartCoroutine(GetNextPoint());
        }
    }



    //Hearing
    //Using a sphere colider as a trigger to represent the guards hearing range
    private void OnTriggerStay(Collider other)
    {
        //If player is within hearing range
        if (other.gameObject.tag == "Player")
        {
            //Check if player sound level is over threshold
            if (PlayerNoise.Instance.GetNoiseLevel() > soundThreashold)
            {
                //Get position of sound source and investigate
                //Debug.Log("I HEAR YOU");
                soundPos = player.position;
                if (!isInvestigating) SetGuardMode("investigating");
            }

            //If investigating Go to last known sound position
            if ((isInvestigating) && (Vector3.Distance(transform.position, soundPos) < distanceToPoint))
            {
                //Once there wait then return to patrol
                //Debug.Log("Must have been the wind");
                StartCoroutine(GetNextPoint());
                if (!isPatrolling) SetGuardMode("patrolling");
            }
        }

        //If distraction is within hearing range
        if (other.gameObject.tag == "Distraction")
        {
            //get oise script from the hazard
            Noise otherNoise = other.GetComponent<Noise>();

            //Check if hazard sound level is over threshold
            if (otherNoise.GetNoiseLevel() > soundThreashold)
            {
                //Get position of sound source and investigate
                //Debug.Log("I Heard something");
                soundPos = other.gameObject.transform.position;
                if (!isInvestigating) SetGuardMode("investigating");
            }

            //If investigating Go to last known sound position
            if ((isInvestigating) && (Vector3.Distance(transform.position, soundPos) < distanceToPoint))
            {
                //Once there wait then return to patrol
                //Debug.Log("Must have been the wind");
                StartCoroutine(GetNextPoint());
                if (!isPatrolling) SetGuardMode("patrolling");
            }
        }
    }



    //Check if player is visible
    public void CheckPursuing()
    {
        //If player is within view distance
        if (Vector3.Distance(transform.position, player.position) < visionDistance)
        {
            Vector3 playerDirection = (player.position - transform.position).normalized;
            float angleBetween = Vector3.Angle(transform.forward, playerDirection);
            
            //If player is within viewangle
            if (angleBetween < visionAngle / 2f)
            {
                //If there are no obstacles blocking view of player
                if (!Physics.Linecast(transform.position, player.position, visionMask))
                {
                    //When player is first spotted
                    if (playerVisible == false && alertable == true)
                    {
                        //Guard is alerted
                        src.Play();
                        GameObject alertPrefab = Instantiate(alertPopup, this.transform).gameObject;
                        alertable = false;
                    }

                    //Debug.Log("I SEE YOU");
                    playerVisible = true;
                    if (!isPursuing) SetGuardMode("pursuing");
                    playerLastPos = player.position;

                    return;
                }
            }
        }

        Debug.Log("I Lost Them");
        playerVisible = false;

        //If the guard was pursuing but lost the player, proceed to last know area
        if (isPursuing && !playerVisible)
        {
            //Debug.Log("Checking last sighting");
            //wait at area
            //if at the next waypoint
            if (Vector3.Distance(transform.position, playerLastPos) < distanceToPoint)
            {
                //Debug.Log("They got away");
                StartCoroutine(GetNextPoint());
                if(!isPatrolling) SetGuardMode("patrolling");
            }
        }
    }



    //Check if the player is catchable
    public void CheckCaught()
    {
        //figure out how far the player is
        playerDistance = Vector3.Distance(transform.position, player.gameObject.transform.position);
        //If player is close enough to be captured
        if (playerDistance < distanceToCapture)
        {
            //Debug.Log("I have you now");
            PlayerLives.Instance.PlayerCaught();
            StartCoroutine(GetNextPoint());
            if (!isPatrolling) SetGuardMode("patrolling");
        }
    }



    //Change guard mode
    //patrolling - loop along patrol path
    //pursuing - The player is/was visible, go to the last seen position
    //investigating - There was a loud sound, head to the sounds position
    private void SetGuardMode(string mode)
    {
        isPatrolling = false;
        isPursuing = false;
        isInvestigating = false;

        switch (mode)
        {
            case "patrolling":
                isPatrolling = true;
                break;

            case "pursuing":
                isPursuing = true;
                break;

            case "investigating":
                isInvestigating = true;
                break;

            default:
                isPatrolling = true;
                break;
        }
    }



    //Wait and then get the position of the next waypoint
    IEnumerator GetNextPoint()
    {
        isWaiting = true;
        animator.SetFloat("Speed", 0);
        yield return new WaitForSeconds(timeToWait);
        nextPoint = PatrolPath.GetPoint(nextPoint);
        isWaiting = false;
    }



    //Set nav speed and assign new nav destination
    private void MoveToNextPoint(float speed, Transform nextPos)
    {
        //move towards the next waypoint
        if (navAgent.speed != speed) navAgent.speed = speed;
        if (navAgent.destination != nextPos.position) navAgent.destination = nextPos.position;
        animator.SetFloat("Speed", speed);

        //Look at next patrol point
        lookDirection = new Vector3(nextPos.position.x, transform.position.y, nextPos.position.z);
        transform.LookAt(lookDirection);
    }



    //Set nav speed and assign new nav destination
    private void MoveToNextPoint(float speed, Vector3 nextPos)
    {
        //move towards the next waypoint
        if (navAgent.speed != speed) navAgent.speed = speed;
        if (navAgent.destination != nextPos) navAgent.destination = nextPos;
        animator.SetFloat("Speed", speed);

        //Look at next patrol point
        lookDirection = new Vector3(nextPos.x, transform.position.y, nextPos.z);
        transform.LookAt(lookDirection);
    }
}
