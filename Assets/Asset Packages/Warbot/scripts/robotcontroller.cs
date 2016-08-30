using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class robotcontroller : MonoBehaviour {


	public Transform mycamera;
	private Transform reference;

	public float jumpHeight = 2.0f;
	public float jumpinterval = 1.5f;
	private float nextjump = 1.2f;
	private float maxhitpoints = 1000f;
	private float hitpoints = 1000f;
	public float regen = 100f;

	public AudioClip[] hurtsounds;




	public float gravity = 20.0f;
	public float rotatespeed = 4.0f;
	private float speed;
	public float normalspeed = 4.0f;
	public float runspeed = 8.0f;

	public float dampTime = 2.0f;

	public bool isaiming = false;


	public Transform target;
	private float moveAmount;
	public float smoothSpeed = 2.0f;

	private Vector3 forward = Vector3.forward;
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 right;

	private float movespeed;
	public Vector3 localvelocity;

	public bool running = false;
	public bool canrun = true;

	public AudioSource myAudioSource;
	public AudioSource moveAudioSource;
	public AudioClip moveclip;

	Vector3 targetDirection = Vector3.zero;

	private bool canrun2 = true;
	public Vector3 addvector;
	Vector3 targetVelocity;
	float addfloat = 0f;
	public Transform Deadrobot;

	void Awake ()
	{
		reference = new GameObject().transform;

	}

	void Start () 
	{
		speed = normalspeed;
		moveAudioSource.clip = moveclip;
		moveAudioSource.loop = true;
		moveAudioSource.Play();
	}
	

	void Update () 
	{
		
		Animator animator = GetComponent<Animator>();


		reference.eulerAngles = new Vector3(0, mycamera.eulerAngles.y, 0);
		forward = reference.forward;
		right = new Vector3(forward.z, 0, -forward.x);
		float hor = Input.GetAxis("Horizontal") * 5f * Time.deltaTime;
		float ver = Input.GetAxis("Vertical") * 5f * Time.deltaTime;


		CharacterController controller = GetComponent<CharacterController>();

		Vector3 velocity = controller.velocity;
		localvelocity = transform.InverseTransformDirection(velocity);

		bool ismovingforward =localvelocity.z > .5f;

		targetDirection = (hor * right) + (ver * forward);
		targetDirection = targetDirection.normalized;
		targetVelocity = targetDirection;

		if (Input.GetButton("Fire3"))
		{
			animator.SetBool("aim",true);
			isaiming = true;
			canrun = false;


			Vector3 relativePos = target.transform.position - transform.position;
			Vector3 localTarget = transform.InverseTransformPoint(target.position);
			addfloat = (Mathf.Atan2(localTarget.x, localTarget.z));
			Quaternion lookrotation = Quaternion.LookRotation(relativePos,Vector3.up);
			lookrotation.x = 0;
			lookrotation.z = 0;

			transform.rotation = Quaternion.Lerp(transform.rotation,lookrotation,Time.deltaTime * rotatespeed);
			if (Input.GetButton("Fire1"))
			{


				animator.SetBool("fire",true);
			}
			else
			{
				animator.SetBool("fire",false);

			}

		}
		else
		{
			animator.SetBool("aim",false);
			canrun = true;
			isaiming = false;
			addfloat = 0f;

			if (targetDirection != Vector3.zero)
			{
				Quaternion lookrotation2 = Quaternion.LookRotation(targetDirection,Vector3.up);
				lookrotation2.x = 0;
				lookrotation2.z = 0;
				transform.rotation = Quaternion.Lerp(transform.rotation,lookrotation2,Time.deltaTime * rotatespeed);
			}
			if (Input.GetButton("Fire1"))
			{
				animator.SetBool("attack",true);
			}
			else
			{
				animator.SetBool("attack",false);
			}
		}




	
		if (controller.isGrounded) 
		{
				

				


			if (Input.GetButton ("Jump") && Time.time > nextjump)
			{
				nextjump = Time.time + jumpinterval;
				moveDirection.y = jumpHeight;
				animator.SetBool ("jump", true);				
			} 
		}
			
		else 
		{
				



			moveDirection.y -= (gravity) * Time.deltaTime;
			nextjump = Time.time + jumpinterval;
			animator.SetBool ("jump",false);	
				
		}
		if (Input.GetButton ("Fire2") && canrun && canrun2 && ismovingforward) 
		{
			speed = runspeed;
			running = true;
				
		}
		else
		{
			speed = normalspeed;
			running = false;
				
				
		}
		targetVelocity *= speed;
		moveDirection.z = targetVelocity.z;
		moveDirection.x = targetVelocity.x;



		animator.SetFloat("hor",(localvelocity.x/normalspeed) + (addfloat * 3f), dampTime , 0.8f);
		animator.SetFloat("ver",(localvelocity.z/normalspeed), dampTime , 0.8f);
		if (hitpoints <= 0)
		{
			//die
			Instantiate(Deadrobot, transform.position, transform.rotation);
			Destroy(gameObject);
		}

		animator.SetBool("grounded",controller.isGrounded);	 
		controller.Move (moveDirection * Time.deltaTime);


		if (hitpoints < maxhitpoints)
		hitpoints += regen * Time.deltaTime;
		
			
		moveAudioSource.pitch = Mathf.Lerp(moveAudioSource.pitch,0.7f + (localvelocity.magnitude / (normalspeed / 2.5f)), 2f * Time.deltaTime);



			



	
	}
	void Damage (float damage) 
	{
		
		

		if (!myAudioSource.isPlaying && hitpoints >= 0)
		{

			int n = Random.Range(1,hurtsounds.Length);
			myAudioSource.clip = hurtsounds[n];
			myAudioSource.pitch = 0.9f + 0.1f *Random.value;
			myAudioSource.Play();
			hurtsounds[n] = hurtsounds[0];
			hurtsounds[0] = myAudioSource.clip;
		}
		//damaged = true;
		//myAudioSource.PlayOneShot(hurtsound);
		hitpoints = hitpoints - damage;
	}



}

