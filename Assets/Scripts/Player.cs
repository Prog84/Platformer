using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Singleton
    public static Player Instance { get; private set; }
    #endregion

    [SerializeField] private float speed = 2.5f;
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    [SerializeField] private float force;
    [SerializeField] private Rigidbody2D rigidbodyPlayer;
    [SerializeField] private float minimalHeight;
    [SerializeField] private bool isCheatMode;
    [SerializeField] private GroundDetection groundDetection;
    [SerializeField] private Vector3 direction;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Arrow arrow;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float shootForce = 5;
    [SerializeField] private float cooldown;
    [SerializeField] private float damageForce;
    [SerializeField] private int arrowsCount = 3;
    [SerializeField] private Health health;
    [SerializeField] private BuffReciever buffReciever;
    [SerializeField] private Camera playerCamera;
    public Health Health { get { return health; } }
    private Arrow currentArrow;
    private float bonusForce;
    private float bonusHealth;
    private float bonusDamage;
    private List<Arrow> arrowsPool;
    private bool isJumping;
    private bool isCooldown;
    private bool isBlockMovement;
    private UICharacterController controller;

    private void Awake()
    {
       Instance = this; 
    }
    private void Start()
    {
        arrowsPool = new List<Arrow>();
        for (int i = 0; i < arrowsCount; i++)
        {
            var arrowTemp = Instantiate(arrow, arrowSpawnPoint);
            arrowsPool.Add(arrowTemp);
            arrowTemp.gameObject.SetActive(false);
        }
        health.OnTakeHit += TakeHit;
        buffReciever.OnBuffsChanged += ApplyBuffs; 
    }

    public void InitUIController(UICharacterController uIController)
    {
        controller = uIController;
        controller.Jump.onClick.AddListener(Jump);
        controller.Fire.onClick.AddListener(CheckShoot);
    }

    private void ApplyBuffs()
    {
        var forceBuff = buffReciever.Buffs.Find(t => t.type == BuffType.Force);
        var damageBuff = buffReciever.Buffs.Find(t => t.type == BuffType.Damage);
        var armorBuff = buffReciever.Buffs.Find(t => t.type == BuffType.Armor);
        bonusForce = forceBuff == null ? 0 : forceBuff.additiveBonus;
        bonusHealth = armorBuff == null ? 0 : armorBuff.additiveBonus;
        health.SetHealth((int)bonusHealth);
        bonusDamage = damageBuff == null ? 0 : damageBuff.additiveBonus;
    }
    private void TakeHit(int damage, GameObject attaker)
    {
        animator.SetBool("GetDamage", true);
        animator.SetTrigger("TakeHit");
        isBlockMovement = true;
        rigidbodyPlayer.AddForce(transform.position.x < attaker.transform.position.x ? 
            new Vector2(-damageForce, 0) : new Vector2(damageForce, 0), ForceMode2D.Impulse);
    }

    public void UnblockMovement()
    {
        isBlockMovement = false;
        animator.SetBool("GetDamage", false);
    }

    void FixedUpdate()
    {
        Move();

        animator.SetFloat("Speed", Mathf.Abs(direction.x));
        CheckFall();
       
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Space))
            Jump();
#endif
    }

    private void Jump()
    {
        if (groundDetection.isGrounded)
        {
            rigidbodyPlayer.AddForce(Vector2.up * (force + bonusForce), ForceMode2D.Impulse);
            animator.SetTrigger("StartJump");
            isJumping = true;
        }
    }

    private void Move()
    {
        animator.SetBool("isGrounded", groundDetection.isGrounded);
        if (!isJumping && !groundDetection.isGrounded)
            animator.SetTrigger("StartFall");
        isJumping = isJumping && !groundDetection.isGrounded;
        direction = Vector3.zero;
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.A))
            direction = Vector3.left;
        if (Input.GetKey(KeyCode.D))
            direction = Vector3.right;
#endif
        if (controller.Left.IsPressed)
        {
            //transform.Translate(Vector2.left * Time.deltaTime * speed);
            direction = Vector3.left;
        }
        if (controller.Right.IsPressed)
        {
            //transform.Translate(Vector2.right * Time.deltaTime * speed);
            direction = Vector3.right;
        }
        direction *= speed;
        direction.y = rigidbodyPlayer.velocity.y;

        if (!isBlockMovement)
         rigidbodyPlayer.velocity = direction;

        if (direction.x > 0)
            spriteRenderer.flipX = false;
        if (direction.x < 0)
            spriteRenderer.flipX = true;
    }
    void CheckShoot()
    {
        if (!isCooldown)
        {
            animator.SetTrigger("StartShoot");
            /*GameObject prefab = Instantiate(arrow, arrowSpawnPoint.position, Quaternion.identity);
            prefab.GetComponent<Arrow>().SetImpulse(Vector2.right, spriteRenderer.flipX ?
                -force * shootForce : force * shootForce, gameObject);*/
        }
    }

    public void InitArrow()
    {
        currentArrow = GetArrowFromPool();//Instantiate(arrow, arrowSpawnPoint.position, Quaternion.identity);
        currentArrow.SetImpulse(Vector2.right, 0, 0, this);
    }

    private void Shoot()
    {
        //currentArrow = Instantiate(arrow, arrowSpawnPoint.position, Quaternion.identity);
        currentArrow.SetImpulse(Vector2.right, spriteRenderer.flipX ?
            -force * shootForce : force * shootForce, (int)bonusDamage, this);
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isCooldown = false;
    }

    private Arrow GetArrowFromPool()
    {
        if (arrowsPool.Count > 0)
        {
            var arrowTemp = arrowsPool[0];
            arrowsPool.Remove(arrowTemp);
            arrowTemp.gameObject.SetActive(true);
            arrowTemp.transform.parent = null;
            arrowTemp.transform.position = arrowSpawnPoint.transform.position;
            return arrowTemp;
        }
        return Instantiate(arrow, arrowSpawnPoint.position, Quaternion.identity);
    }

    public void ReturnArrowToPool(Arrow arrowTemp)
    {
        if (!arrowsPool.Contains(arrowTemp))    
                    arrowsPool.Add(arrowTemp);
        arrowTemp.transform.parent = arrowSpawnPoint;
        arrowTemp.transform.position = arrowSpawnPoint.transform.position;
        arrowTemp.gameObject.SetActive(false);
        
    }
          
    void CheckFall()
    {
        if (transform.position.y < minimalHeight && isCheatMode)
        {
            rigidbodyPlayer.velocity = new Vector2(0, 0);
            transform.position = new Vector3(0, 0, 0);
        }
        else if (transform.position.y < minimalHeight && !isCheatMode)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        playerCamera.transform.parent = null;
        playerCamera.enabled = true;
    }
}
