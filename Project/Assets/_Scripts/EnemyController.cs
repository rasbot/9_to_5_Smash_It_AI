using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    private NavMeshAgent agent;
    private PSMeshRendererUpdater psmru;
    private ParticleSystem ps;
    public bool useNavMesh = false;
    //public float speed;
    public static float _speed;
    public static int cEnemyControllerID = 0;
    public int id;
    //public GameObject glove; // this is NOT glove+spring but only the glove
    public bool isAlive;
    public float currentHealth;
    public float maxHealth;
    public Slider healthBar;
    public GameObject canvas;
    public float baseHitLevel;
    public GameObject robotParts;
    public GameObject[] glasses;
    public GameObject effects;
    public float baseScore;
    public float[] puDamage;
    public GameObject[] visualEffects;
    [Header("Audio Clips")]
    public AudioClip[] audioSpawn;
    public AudioClip[] audioVoices;
    public AudioClip[] audioExplosions;

	//private float failSafeDestroyTime;
    private Vector3 origin;
    private Quaternion originROT;
    private GameObject player;
    private Transform playerTrans;
    private Rigidbody rb;
    private Collider[] colChildren;
    private Rigidbody[] rigidChildren;
    private Collider[] botCol;
    private List<Transform> explosionLevelTrans_1 = new List<Transform>();    // used to created different explosion intensities
    private List<Transform> explosionLevelTrans_2 = new List<Transform>();
    private List<Transform> explosionLevelTrans_3 = new List<Transform>();
    private List<Transform> explosionLevelTrans_4 = new List<Transform>();
    private List<Rigidbody> explosionLevelRigid_1 = new List<Rigidbody>();    // used to created different explosion intensities
    private List<Rigidbody> explosionLevelRigid_2 = new List<Rigidbody>();
    private List<Rigidbody> explosionLevelRigid_3 = new List<Rigidbody>();
    private List<Rigidbody> explosionLevelRigid_4 = new List<Rigidbody>();
    private AudioSource audiosource;
    private int audioClipsLength;
    private int index;
    private int scoreValue;
    private bool waitCheck;
    private float robotHeight;
    private float[] expLevel;
    private float explosionMag;
    private bool gloveClose;
    private bool thermoPunched;
    private bool freezePunched;
    private bool oneMesh;
    private bool dieOnce;
    private bool femPunched;
    private float speed;        // this will control the navmesh agent speed
    private bool playerHit;
    private bool nudgeBot;
    private bool repathBot;
    private bool updateOnce = true;
    private GameObject glove;
    private Vector3 nudgeForce;
    private bool tutScene = false;
    enum BotID
    {
        mailBot,
        clampBot,
        coffeeBot,
        femBot,
        destructoBot,
        thermoBot
    }
    BotID botID = 0;
    private GameObject enemyTarget;
    //public float forceOffset = 0.25f;
    //public float forceRadius = 0.25f;
    public float knockBackStrength = 30;
    public float ragTime = 0.5f;
    public float nudgeStrength;
    public float waitTimeMin;
    public float waitTimeMax;
    public bool nudgeTowardPlayer = false;
    public float repathTimeMin;
    public float repathTimeMax;

    [SerializeField] float timeToGetUp = 1f; //expose to inspector, but no need to be public

    [Space(20)] //TODO: place all public vars which should NOT be changed in inspector below....

    public bool isKnockingBack = false;

    Coroutine knockBackRoutine;

    //private float speed_i;      // store the initial speed at start
    //private PSMeshRendererUpdater psmru;

    public static int botsDestroyed;
    public static float hitMultiplier;

    NavBot navBot;
    float navBotVel;

    private void Awake()
    {
        cEnemyControllerID++;
        id = cEnemyControllerID;
    }

    // Use this for initialization
    void Start()
    {
		UIController.s.totSpawnBot += 1;
		print("(EC) bot spawned: " + UIController.s.totSpawnBot);

		//failSafeDestroyTime = 0;
        navBot = GetComponent<NavBot>();
        if (UIController.s.currentSceneName != "L0_Tutorial")
        {
        navBotVel = navBot.maxVel; // cache the velocity of the navBot only if the scene is not the tutorial
        useNavMesh = navBot == null ? true : false;
        }
        botID = (BotID)PlayerController.s.botTypeIndex;


        foreach (GameObject go in GameObject.FindGameObjectsWithTag("ExpLvl_1"))
        {
            if (go.transform.IsChildOf(gameObject.transform))
            {
                explosionLevelTrans_1.Add(go.GetComponent<Transform>());
                explosionLevelRigid_1.Add(go.GetComponent<Rigidbody>());
            }
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("ExpLvl_2"))
        {
            if (go.transform.IsChildOf(gameObject.transform))
            {
                explosionLevelTrans_2.Add(go.GetComponent<Transform>());
                explosionLevelRigid_2.Add(go.GetComponent<Rigidbody>());
            }
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("ExpLvl_3"))
        {
            if (go.transform.IsChildOf(gameObject.transform))
            {
                explosionLevelTrans_3.Add(go.GetComponent<Transform>());
                explosionLevelRigid_3.Add(go.GetComponent<Rigidbody>());
            }
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("ExpLvl_4"))
        {
            if (go.transform.IsChildOf(gameObject.transform))
            {
                explosionLevelTrans_4.Add(go.GetComponent<Transform>());
                explosionLevelRigid_4.Add(go.GetComponent<Rigidbody>());
            }
        }
        isAlive = true;
        nudgeBot = true;
        repathBot = true;
        if (PlayerController.s.currentSceneName != "L0_Tutorial" && useNavMesh)
        {
            agent = GetComponent<NavMeshAgent>();
            speed = GetComponent<NavMeshAgent>().speed;
        }
        enemyTarget = GameObject.FindGameObjectWithTag("EnemyTarget"); // LOOK AT
        //print("Do I need to find the enemy target every time?");
        if (useNavMesh && !PlayerController.s.holeBot)
        {
            agent.enabled = true;
            agent.SetDestination(enemyTarget.transform.position);
        }
        glove = GameObject.FindGameObjectWithTag("Glove");
        playerHit = false;
        dieOnce = false;
        botsDestroyed = 0;
        //if (agent.enabled) speed = GetComponent<NavMeshAgent>().speed;

        _speed = speed;  // cache the initial speed
        oneMesh = true;
        femPunched = false;
        thermoPunched = false;
        gloveClose = false;
        rb = GetComponent<Rigidbody>();
        audiosource = GetComponent<AudioSource>();
        index = Random.Range(0, audioSpawn.Length); // Play a random clip when the bot spawns
        audiosource.PlayOneShot(audioSpawn[index]);
        player = GameObject.FindGameObjectWithTag("MainCamera");
        colChildren = robotParts.GetComponentsInChildren<Collider>();
        UpdateHealth(maxHealth);

        expLevel = new float[] { 1.25f, 1.5f, 1.75f, 2.0f };
    }

    void EnableColliders()
    {
        if (playerHit)
        {
            explosionMag = expLevel[2]; // when the bot runs into the player, have the explosion be at level 3
        }
        else explosionMag = hitMultiplier;     // get the value of the last punch strength

        if (Glove.coffeePowerUp)
        {
            explosionMag = expLevel[3];     // if the coffee powerup is active, punches should be strong!
        }


        List<Rigidbody> rbs = new List<Rigidbody>();


        if (explosionMag < expLevel[0] && explosionMag != 0)
        {
            // level 1
            audiosource.PlayOneShot(audioExplosions[0]);
            foreach (Transform tran in explosionLevelTrans_1)
            {
                tran.GetComponent<Collider>().enabled = true;
                //print("level 1 col enabled");
            }


            rbs = explosionLevelRigid_1;
        }
        else if (explosionMag >= expLevel[0] && explosionMag < expLevel[1])
        {
            // level 2
            audiosource.PlayOneShot(audioExplosions[1]);
            foreach (Transform tran in explosionLevelTrans_2)
            {
                tran.GetComponent<Collider>().enabled = true;
                //print("level 2 col enabled");
            }
            rbs = explosionLevelRigid_2;
        }
        else if (explosionMag >= expLevel[1] && explosionMag < expLevel[2])
        {
            // level 3
            audiosource.PlayOneShot(audioExplosions[2]);
            foreach (Transform tran in explosionLevelTrans_3)
            {
                tran.GetComponent<Collider>().enabled = true;
                //print("level 3 col enabled");
            }
            rbs = explosionLevelRigid_3;

        }
        else if (explosionMag >= expLevel[2])
        {
            // level 4
            audiosource.PlayOneShot(audioExplosions[3]);
            foreach (Transform tran in explosionLevelTrans_4)
            {
                tran.GetComponent<Collider>().enabled = true;
                //print("level 4 col enabled");
            }
            rbs = explosionLevelRigid_4;
        }


        foreach (Rigidbody rig in rbs)
        {
            rig.isKinematic = false;
            rig.useGravity = true;

            rig.transform.parent = null;
            DelayDestroy dd = rig.gameObject.AddComponent<DelayDestroy>();
            dd.delay = 10f;
        }

    }

    bool alreadyExploding = false;

    IEnumerator RobotExplosionDelay()
    {
        if (alreadyExploding) yield break;
        alreadyExploding = true;
        EnableColliders();
        DisableAnimators();

        if (agent) agent.enabled = false; // disable the navmesh agent
		if (UIController.s.currentSceneName == "L0_Tutorial")
		{
		yield return new WaitForSeconds(5);
		}
		else yield return new WaitForSeconds(PlayerController.s.instantiateTime);
		Destroy(gameObject);
		hitMultiplier = 0;      // reset the hit multiplier
        alreadyExploding = false;
    }

	public void DestroyStuckBot()
	{
		playerHit = true;
		isAlive = false;
        if (healthBar) Destroy(healthBar.gameObject);

        if (knockBackRoutine != null) StopCoroutine(knockBackRoutine);

        StartCoroutine(RobotExplosionDelay());
		PlayerController.s.totalBotsSpawned -= 1;
		print("Stuck bot explode - (PC) totSpn: " + PlayerController.s.totalBotsSpawned);
	}

    void DisableAnimators()
    {
        foreach (Animator anim in gameObject.GetComponentsInChildren<Animator>())
        {
            anim.enabled = false;
        }
    }

    void EnableAnimators()
    {
        foreach (Animator anim in gameObject.GetComponentsInChildren<Animator>())
        {
            anim.enabled = true;
        }
    }

    void UpdateHeathScore()
    {
        AddHealth(-baseHitLevel * hitMultiplier);
        PlayerController.s.AddScore(scoreValue);
    }

	

    IEnumerator BotHitOnce()
    {
        if (updateOnce)
        {
            updateOnce = false;
            StartCoroutine(KnockBack(knockBackStrength, ForceMode.Impulse));
            //KnockBack(knockBackStrength);
            UpdateHeathScore();
            yield return new WaitForSeconds(0.4f);
            updateOnce = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerCol")
        {
            playerHit = true;
            StartCoroutine(RobotExplosionDelay());
        }
        if (other.tag == "elecGlove")
        {
            //currentHealth -= puDamage[0]; // Take damage when hit by electricity
            //healthBar.value = currentHealth;
            AddHealth(-puDamage[0]);
        }

        if (currentHealth > 0 && other.gameObject.tag == "GloveSpring")
        {
            StartCoroutine(BotHitOnce());

            if (Glove.thermPowerUp)
            {
                thermoPunched = true;
            }
            if (Glove.freezePowerUp)
            {
                freezePunched = true;
            }
            //else thermoPunched = false;
            if (glasses.Length >= 1 && !femPunched)  // check if we are punching the fembot
            {
                AngryFemBot();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "elecGlove")
        {
            gloveClose = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        gloveClose = false;
    }

    public float RandomNum(float min, float max)
    {
        return Random.Range(min, max);
    }


    IEnumerator RandomNudge(float strength, float waitTime_min, float waitTime_max)
    {
        nudgeBot = false; // already nudging
        //print("nudge initiated");
        Vector3 dir = player.transform.position - transform.position;
        Vector3 dirNorm = dir.normalized;
        nudgeForce = strength * dirNorm * RandomNum(5, 20);
        //if (nudgeTowardPlayer)
        //{
        //    Vector3 dir = player.transform.position - transform.position;
        //    Vector3 dirNorm = dir.normalized;
        //    nudgeForce = strength * dirNorm * RandomNum(1, 5);

        //}
        //else
        //{
        //    nudgeForce = new Vector3(strength * RandomNum(-5, 5), 0, strength * RandomNum(-5, 5));
        //}
        float delay = RandomNum(waitTime_min, waitTime_max);
        if (useNavMesh && agent.enabled)
        {
            print("navMesh nudge");
            agent.velocity = nudgeForce;
            agent.SetDestination(enemyTarget.transform.position);
            yield return new WaitForSeconds(delay);
        }
        else
        {
            //Vector3 _vel = rb.velocity;
            //print("navBot nudge");
            yield return new WaitForSeconds(delay);
            navBot.enabled = false;
            rb.velocity = nudgeForce;
            //print("delay time: " + delay);
            yield return new WaitForSeconds(1);
            navBot.enabled = true;
            print("navBot should be enabled now: " + navBot.enabled);
            //rb.velocity = _vel;
        }
        nudgeBot = true;
    }

    IEnumerator RepathAgent(float waitTime_min, float waitTime_max)
    {
        repathBot = false; // behavior already initialized
        float delay = RandomNum(waitTime_min, waitTime_max);
        yield return new WaitForSeconds(delay);
        if (agent.enabled) agent.SetDestination(enemyTarget.transform.position);
        repathBot = true;
    }

    IEnumerator KnockBack(float strength, ForceMode mode)
    {
        if (isKnockingBack)
        {
            yield break;
        }

        isKnockingBack = true;
        Quaternion origRot = transform.rotation;
        Vector3 dir = transform.position - player.transform.position;
        Vector3 dirNorm = dir.normalized;
        //print("dirNorm = " + dirNorm);
        if (useNavMesh)
        {
            agent.enabled = false;
        }
        else if(navBot) navBot.enabled = false;
        Vector3 forceVec = dirNorm * strength;
        rb.AddForce(forceVec, mode); // doesn't seem to work right
        //print("forceVec = " + forceVec);
        yield return new WaitForSeconds(ragTime);
        //lerp back to original rotation
        Quaternion fallenRot = transform.rotation;
        float t = 0f;

        while (t < timeToGetUp)
        {
            t += Time.fixedDeltaTime;
            transform.rotation = Quaternion.Slerp(fallenRot, origRot, t / timeToGetUp);
            yield return new WaitForFixedUpdate();
        }

        if (useNavMesh)
        {
            agent.enabled = true;
            agent.SetDestination(enemyTarget.transform.position);
        }
        else if(navBot) navBot.enabled = true;

        isKnockingBack = false;
    }

    //void KnockBack(float strengthMultiplier) // change navmesh agent velocity, no rotational correction
    //{
    //    Vector3 dir = transform.position - player.transform.position;
    //    //Vector3 dir = glove.      // I need to get the z direction of the glove to do it this way...not sure yet
    //    Vector3 dirNorm = dir.normalized;
    //    Vector3 velocityVec = dirNorm * strengthMultiplier * hitMultiplier;
    //    agent.velocity = velocityVec;
    //}

		public MonoBehaviour[] activateOnAnger;

    void AngryFemBot() // fembot will look mad and move faster
    {
        femPunched = true;
        if (useNavMesh)
        {
            speed = 2 * _speed;
            agent.speed = speed;
        }
        else 
		{
		navBot.maxVel *= 2.5f;
		navBot.forwardDrive = 1.7f;
		}
        nudgeTowardPlayer = true;
        glasses[0].SetActive(false);
        glasses[1].SetActive(true);
		foreach (MonoBehaviour component in activateOnAnger) component.enabled = true;
        //EnableAnimators();
    }

    IEnumerator FrozenBot(float freezeTime)
    {
        var psUpdater = visualEffects[2].GetComponentInChildren<PSMeshRendererUpdater>();
        psUpdater.UpdateMeshEffect(gameObject);
        //psUpdater.IsActive = true;
        DisableAnimators();
        effects.SetActive(false); // turn off any effects like the flame thrower
        float _nudgeStrength = nudgeStrength;
        nudgeStrength = 0;
        if (useNavMesh) agent.speed = 0;
        else navBot.maxVel = 0;
        yield return new WaitForSeconds(freezeTime);
        psmru = visualEffects[2].GetComponent<PSMeshRendererUpdater>();
        psmru.OnDestroy();
        if (useNavMesh) agent.speed = _speed;
        else navBot.maxVel = navBotVel;
        //psUpdater.IsActive = false;
        effects.SetActive(true); // turn on effects
        EnableAnimators();
        nudgeStrength = _nudgeStrength;
        freezePunched = false;
    }

    void MoveBot()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, origin, step);
        //var q = Quaternion.LookRotation(player.position - transform.position);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, q, speed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, playerTrans.rotation, step);
    }



    void AddHealth(float add)
    {
        UpdateHealth(currentHealth + add);
    }

    void UpdateHealth(float newHealth)
    {
        if (currentHealth < 0)
        {
            currentHealth = 0; //print("Already Dead");
            return;
        }

        currentHealth = newHealth;//the only place on earth where health can actually get changed

        healthBar.value = currentHealth;

        if (isAlive && currentHealth <= 0) Die();

    }

    public void PlayerUpdateHealth(float amount)
    {
        PlayerController.s.AddHealth(amount);
    }



    void Die()
    {

        isAlive = false;
        if (healthBar) Destroy(healthBar.gameObject);

        PlayerController.s.AddScore(5 * scoreValue);  // add more to score when the bot is dead
        //db.label(transform.position + Vector3.up, "DEAD");

        if (knockBackRoutine != null) StopCoroutine(knockBackRoutine);

        StartCoroutine(RobotExplosionDelay());
        //TimeManager.s.PauseUnity();
        //TimeManager.s.SetGameSpeed(0);
        UIController.s.botsDestroyed++;
		print("increment bots punched and destroyed: " + UIController.s.botsDestroyed);
    }

    void FixedUpdate()
    {
#if UNITY_EDITOR
        hitMultiplier = PlayerController.s.mouseTimeDown;   // CHK - this can probably be changed to gloveAnimSpeed so there doesn't need to be unity vs android stuff
#elif UNITY_ANDROID
                hitMultiplier = PlayerController.s.gloveAnimSpeed;
#endif

    }
    void Update()
    {
		//failSafeDestroyTime += Time.deltaTime;
		//if (failSafeDestroyTime >= 20)
		//{
		//	DestroyStuckBot();
		//}
        //if (currentHealth <= 0f && !dieOnce)
        //{
        //    healthBar.value = 0f;

        //    playercontroller.AddScore(5 * scoreValue);  // add more to score when the bot is dead
        //    currentHealth = 1f; //stop adding to the score every frame!
        //    botDead = true;
        //    StartCoroutine(RobotExplosionDelay());
        //    //StopCoroutine(KnockBack(knockBackStrength));
        //    UIController.s.botsDestroyed++;
        //    dieOnce = true;
        //}
        //print("navBot? " + navBot.enabled);
        if (nudgeBot)
        {
            //StartCoroutine(RandomNudge(nudgeStrength, waitTimeMin, waitTimeMax));
            //print("nudgeBot coroutine called");
        }

        //if (repathBot)
        //{
        //    if(useNavMesh) StartCoroutine(RepathAgent(repathTimeMin, repathTimeMax));
        //    print("repath coroutine called");
        //}

        if (thermoPunched)
        {
            var psUpdater = visualEffects[1].GetComponent<PSMeshRendererUpdater>();
            psUpdater.UpdateMeshEffect(gameObject);

            //currentHealth -= puDamage[1] * Time.deltaTime;   // drain health
            //healthBar.value = currentHealth;
            AddHealth(-puDamage[1] * Time.deltaTime);
            //print("health = " + currentHealth);
            //blarg
        }

        if (freezePunched)
        {
            if (oneMesh)
            {
                oneMesh = false;
                StartCoroutine(FrozenBot(PlayerController.s.pupTime));
            }

            //currentHealth -= puDamage[1] * Time.deltaTime;   // drain health
            //healthBar.value = currentHealth;
            AddHealth(-puDamage[1] * Time.deltaTime);     
        }

		if (UIController.s.currentSceneName != "L0_Tutorial" && Glove.s.elecPowerUp && gloveClose)
        {
            visualEffects[0].SetActive(true);
        }
        else visualEffects[0].SetActive(false);

        scoreValue = Mathf.RoundToInt(baseScore * hitMultiplier);

        //if (!waitCheck)
        //{
        //    MoveBot();
        //}
    }

}
