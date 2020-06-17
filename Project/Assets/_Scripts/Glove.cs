using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Glove : MonoBehaviour
{
	public static Glove s;

	public static bool startPunched;
	public bool elecPowerUp;         // use to active electricity on enemies
	public static bool thermPowerUp;        // burn enemies
	public static bool freezePowerUp;       // oh that is cold!
	public static bool coffeePowerUp;       // mmmm coffee...
	public static bool healthPackCollected; // did we just pick up a health pack?
	public static bool powerUpActive;       // is there an active powerup?

	//object counts for end of level
	public int healthPacksCollected; // how many health packs have been collected

	public Material[] materials;
	public GameObject glove;
	public GameObject electricityCollider;
	public GameObject[] gloveEffects;
	public GameObject collected;
	public GameObject[] visualEffects;
	public AudioClip[] audioEffects;
	public AudioClip[] punchSounds;
	public AudioClip[] electricSounds;
	public AudioSource audiosource;
	public float knockBackObjMultiplier;
	public bool coffeeCreated;

	private Rigidbody rb;
	private Collider col;
	private BoxCollider boxCol;
	private float boxColCenterZ;
	private bool gloveColliderOn;
	private PSMeshRendererUpdater psmru;
	private bool retractOnce;

	Animator anim;
	Animation ani;

	void Awake()
	{
		s = this;
	}

	void Start()
	{
		anim = GetComponent<Animator>();
		retractOnce = false;
		powerUpActive = false; // we are allowed to pick up a powerup
		healthPacksCollected = 0;
		healthPackCollected = false;
		glove.GetComponent<Renderer>().material = materials[0]; //make sure the glove is red
		electricityCollider.SetActive(false); //make sure the electricity is not initially on
		foreach (GameObject gloveEff in gloveEffects)
		{
			gloveEff.SetActive(false);
		}
		elecPowerUp = false;
		thermPowerUp = false;
		coffeePowerUp = false;
		startPunched = false;
		rb = gameObject.GetComponent<Rigidbody>();
		col = gameObject.GetComponent<Collider>();
		col.enabled = false;
		boxCol = gameObject.GetComponent<BoxCollider>();
		boxColCenterZ = boxCol.center.z;
	}

	void GloveColliderON()
	{
		col.enabled = true;
	}

	void GloveColliderOFF()
	{
		col.enabled = false;
	}


	bool isKnockingBack = false;
	void KnockBackObj(Collider col, float strength)
	{
		if (!isKnockingBack)
		{
			Vector3 dir = transform.position - col.transform.position;
			Vector3 dirNorm = dir.normalized;
			rb.AddForce(dirNorm * strength * knockBackObjMultiplier, ForceMode.Impulse);
		}
		isKnockingBack = true;
	}

		bool isRetracting = false;

	IEnumerator Retract()
	{
		if (isRetracting) yield break;

		while (anim.GetCurrentAnimatorClipInfo(0).Length == 0) yield return null;//wait for animation to play  (not sure why it is not playing already)

		anim.SetFloat("Direction", -1f);//play animation backwards

		col.enabled = false; // disable collider on glove when retracted since animation event will not get called
		//AnimationClip animationClip = anim.runtimeAnimatorController.animationClips[0]; 
		AnimatorClipInfo clipInfo = anim.GetCurrentAnimatorClipInfo(0)[0];
		AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

		while (clipInfo.clip.length * info.normalizedTime > 0f)
		{
			try{
				clipInfo = anim.GetCurrentAnimatorClipInfo(0)[0];
				info = anim.GetCurrentAnimatorStateInfo(0);
			}
			catch
			{
				print("OOPS! anim= missing clip info? "+anim);
			}

			yield return null;
		}

		anim.SetFloat("Direction", 1f);
		anim.SetBool("Punch", false);
		anim.Play("IDLE");
		col.enabled = false; // the animation event in reverse will turn this back on so it needs to be told again to turn off!

		isRetracting = false;
	}

	IEnumerator RetractOnce()
	{
		retractOnce = true;
		StartCoroutine(Retract());
		yield return new WaitForSeconds(0.5f); // this should prevent multiple calls to retract from a single punch
		retractOnce = false;
	}

	void OnTriggerEnter(Collider other)
	{
		bool usePunchSound = false;
		if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "explode")
		{
			if (!retractOnce && !coffeePowerUp) StartCoroutine(RetractOnce());
			usePunchSound = true;
		}

		if (other.gameObject.tag == "interactable")
		{
			StartCoroutine(Retract());
			KnockBackObj(other, PlayerController.s.gloveAnimSpeed);
			usePunchSound = true;
		}


		if (other.CompareTag("PU_e") && !powerUpActive)         // electric power up is punched
		{
			PlayerController.s.powerUpInGame = false;
			elecPowerUp = true;
			Destroy(other.gameObject);
			StartCoroutine(ElectricGlovePowerUp());
			Instantiate(visualEffects[0], other.transform.position, Quaternion.identity);
			audiosource.PlayOneShot(audioEffects[1]);
			PlayerController.s.powerupsCollected++;
		}

		if (other.CompareTag("PU_t") && !powerUpActive)         // thermo power up is punched
		{
			PlayerController.s.powerUpInGame = false;
			thermPowerUp = true;
			Destroy(other.gameObject);      // boom
			StartCoroutine(ThermoGlovePowerUp());
			Instantiate(visualEffects[1], other.transform.position, Quaternion.identity);   //poof
			audiosource.PlayOneShot(audioEffects[1]);  // ding
			PlayerController.s.powerupsCollected++;
		}

		if (other.CompareTag("PU_f") && !powerUpActive)         // freeze power up is punched
		{
			PlayerController.s.powerUpInGame = false;
			freezePowerUp = true;
			Destroy(other.gameObject);      // boom
			StartCoroutine(FreezeGlovePowerUp());
			Instantiate(visualEffects[1], other.transform.position, Quaternion.identity);   //poof
			audiosource.PlayOneShot(audioEffects[1]);  // ding
			PlayerController.s.powerupsCollected++;
		}

		if (other.CompareTag("PU_coffee") && !powerUpActive)    // coffee power up is punched
		{
			Destroy(other.gameObject);
			StartCoroutine(CoffeePowerUp());
			Instantiate(visualEffects[1], other.transform.position, Quaternion.identity);   //poof
			audiosource.PlayOneShot(audioEffects[1]);  // ding
			PlayerController.s.powerupsCollected++;
		}

		if (other.CompareTag("ResetSceneButton"))
		{
			if (SceneManager.GetActiveScene().name == "L0_Tutorial") UIController.s.LoadNextLevel();
			else UIController.s.ResetScene();
		}

		if (other.CompareTag("StartButton"))
		{
			startPunched = true;
			PauseMenuu.s.pauseButton.SetActive(true);
		}

		if (UIController.s.currentSceneName != "L0_Tutorial" && other.CompareTag("Pause"))
		{
			PauseMenuu.s.Pause();
		}

		if (other.CompareTag("HealthPack"))
		{
			healthPacksCollected++;
			healthPackCollected = true;
			GameObject effect = Instantiate(visualEffects[2], other.transform.position, Quaternion.identity);   //poof
			Destroy(other.gameObject);
			audiosource.PlayOneShot(audioEffects[0]);
		}

		if(usePunchSound)
		{
			if (Glove.s.elecPowerUp) AudioSource.PlayClipAtPoint(electricSounds[Random.Range(0, electricSounds.Length)], transform.position);
			else AudioSource.PlayClipAtPoint(punchSounds[Random.Range(0, punchSounds.Length)], transform.position);
		}
	}

	IEnumerator CoffeePowerUp() // mat1
	{
		powerUpActive = true;
		coffeePowerUp = true;
		glove.GetComponent<Renderer>().material = materials[1]; //change the glove to brown
		PlayerController.s.glovePunchAnim.SetBool("PunchBool", true);
		PlayerController.s.glovePunchAnim.speed = 2f;
		yield return new WaitForSeconds(PlayerController.s.pupTime);
		glove.GetComponent<Renderer>().material = materials[0];
		PlayerController.s.glovePunchAnim.SetBool("PunchBool", false);

		coffeePowerUp = false;
		powerUpActive = false;
	}

	IEnumerator ElectricGlovePowerUp()  // mat2
	{
		powerUpActive = true;
		glove.GetComponent<Renderer>().material = materials[2]; //change the glove to blue
		electricityCollider.SetActive(true); //turn on collider to make electricty bolts
		gloveEffects[0].SetActive(true);
		yield return new WaitForSeconds(PlayerController.s.pupTime);
		glove.GetComponent<Renderer>().material = materials[0];
		elecPowerUp = false;
		powerUpActive = false;
		gloveEffects[0].SetActive(false);
		electricityCollider.SetActive(false);
	}

	IEnumerator ThermoGlovePowerUp()    // mat3
	{
		powerUpActive = true;
		glove.GetComponent<Renderer>().material = materials[3]; //change the glove to orange
		gloveEffects[1].SetActive(true);
		//var psUpdater = visualEffects[1].GetComponentInChildren<PSMeshRendererUpdater>();
		//psUpdater.UpdateMeshEffect(gameObject);
		yield return new WaitForSeconds(PlayerController.s.pupTime);
		glove.GetComponent<Renderer>().material = materials[0];
		thermPowerUp = false;
		powerUpActive = false;
		gloveEffects[1].SetActive(false);
	}

	IEnumerator FreezeGlovePowerUp()    // mat4
	{
		powerUpActive = true;
		glove.GetComponent<Renderer>().material = materials[4]; //change the glove to frozen bluish look
		gloveEffects[2].SetActive(true);
		var psUpdater = gloveEffects[2].GetComponentInChildren<PSMeshRendererUpdater>();
		psUpdater.UpdateMeshEffect(gameObject);
		yield return new WaitForSeconds(PlayerController.s.pupTime);
		psmru = gloveEffects[2].GetComponent<PSMeshRendererUpdater>();
		psmru.OnDestroy();
		glove.GetComponent<Renderer>().material = materials[0];
		freezePowerUp = false;
		powerUpActive = false;
		gloveEffects[2].SetActive(false);
	}

	//IEnumerator PowerUpCollected()
	//{
	//    collected.SetActive(true);
	//    yield return new WaitForSeconds(1);
	//    collected.SetActive(false);
	//}

	//void FixedUpdate()
	//{

 //   }
}
