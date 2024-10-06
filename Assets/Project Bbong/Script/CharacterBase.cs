using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Bbong123
{
    public class CharacterStats
    {
        public float currentHP;
        public float maxHP;
        public float currentSP;
        public float maxSP;
    }
    public class CharacterBase : MonoBehaviour
    {
        public CharacterStats characterStats;
        public bool IsAiming { get; set; }
        public float CurrentSP => characterStats.currentSP;
        public float CurrentHP => characterStats.currentHP;
        public float MaxSP => characterStats.maxSP;
        public float maxHP => characterStats.maxHP;
        public bool IsGrounded => IsGrounded;
        public bool IsAlive => characterStats.currentHP > 0f;
        public float maxJumpCount = 2;
        public float attackRange = 1.5f;
        public int AttackDamage = 10;
        public LayerMask enemyLayer;
        public bool IsPossibleMovement
        {
            get => isPossibleMovement;
            set => isPossibleMovement = value;
        }

        public bool IsPossibleAttack
        {
            get => isPossibleAttack;
            set
            {
                isPossibleAttack = value;
                if (false == isPossibleAttack)
                {
                    isAttacking = false;
                }
            }
        }
        public bool IsStracfe
        {
            get => isStrafe;
            set => isStrafe = value;
        }
        public bool IsWalk
        {
            get => isWalk;
            set => isWalk = value;
        }
        public System.Action<float, float> OnChangeHP;
        public System.Action<float, float> OnChangeSP;

        public float moveSpeed;
        public float rotationSpeed;
        public float JumpForce;

        public float staminaRecoveryAmount = 5f;
        public float staminaRecoveryTime = 1f;
        public float StaminaDeltaTime;

        public float groundRadius = 0.1f;
        public float groundOffset = 0.1f;
        public LayerMask groundLayer;


        protected private int currentJumpCount = 0;
        protected private float verticalVelocity;
        protected private bool isGrounded;
        protected private float jumpTimeout = 0.1f;
        protected private float jumpTimeoutDelta = 0f;

        protected private bool isWalk = false;
        protected private bool isStrafe = false;
        protected private float speed = 0f;
        protected private float targetSpeed = 0f;
        protected private float targetSpeedBlend = 0f;


        protected private bool isPossibleMovement = false;
        protected private bool isPossibleAttack = false;
        protected bool isAttacking = false;

        protected private float targetRotation;
        protected private float rotationVelocity;
        protected private float RotationSmoothTime = 0.12f;
        protected private UnityEngine.CharacterController unityCharacterController;
        protected Animator characterAnimator;

        protected virtual void Awake()
        {
            characterStats.currentHP = characterStats.maxHP;
            characterStats.currentSP = characterStats.maxSP;

            characterAnimator = GetComponent<Animator>();
            unityCharacterController = GetComponent<CharacterController>();
        }

        protected virtual void start()
        {

        }
        protected virtual void Update()
        {
            if (!isAttacking)
            {
                if (CurrentSP < MaxSP)
                {
                    if (Time.time > StaminaDeltaTime + staminaRecoveryTime)
                    {
                        StaminaDeltaTime = Time.time;
                        IncreaseStamina(staminaRecoveryAmount);
                    }
                }
            }
            GroundCheck();
            FreeFall();

            characterAnimator.SetFloat("Strafe", isStrafe ? 1f : 0f);

            speed = isWalk ? 1f : 3f;
            targetSpeedBlend = Mathf.Lerp(targetSpeedBlend, targetSpeed, Time.deltaTime * 10f);
            characterAnimator.SetFloat("Speed", targetSpeedBlend);
        }
        public virtual void Attack()
        {
            characterAnimator.SetTrigger("Attack");

            // 공격 범위 내 적 감지
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

            
        }
        public virtual void Fire(bool isFire)
        {

        }
        public virtual void SetAiming(float aiming)
        {

        }
        public void Jump()
        {
            if (currentJumpCount >= maxJumpCount)
                return;

            verticalVelocity = JumpForce;
            jumpTimeoutDelta = jumpTimeout;
            currentJumpCount++;
            characterAnimator.SetTrigger("JumpTrigger");
        }

        public void IncreaseStamina(float value)
        {
            characterStats.currentSP += value;
            characterStats.currentSP = Mathf.Clamp(characterStats.currentSP, 0, characterStats.maxSP);

            OnChangeSP?.Invoke(characterStats.currentSP, characterStats.maxSP);
        }
        
        public void DecreaseStamina(float value)
        {
            characterStats.currentSP -= value;
            characterStats.currentSP = Mathf.Clamp(characterStats.currentSP, 0, characterStats.maxSP);

            OnChangeSP?.Invoke(characterStats.currentSP, characterStats.maxSP);
        }
        public void TakeDamage(float damage)
        {
            characterStats.currentHP -= damage;

            OnChangeHP?.Invoke(characterStats.currentHP, characterStats.maxHP);
            if (characterStats.currentHP < 0)
            {
                characterAnimator.SetTrigger("DeadTrigger");
                characterAnimator.SetBool("ISDead", true);
            }
        }
        public void Move(Vector2 input, float yAxisAngle)
        {
            targetSpeed = input.magnitude > 0f ? targetSpeed = speed : 0f;
            if (IsPossibleMovement == false)
            {
                input = Vector2.zero;
            }
            float magnitude = input.magnitude;
            if (magnitude <= 0.1f && isGrounded)
                return;
            Vector3 inputDirection = new Vector3(input.x, 0, input.y).normalized;
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + yAxisAngle;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation,
                ref rotationVelocity, RotationSmoothTime);
            if (!isStrafe)
            {
                transform.rotation = Quaternion.Euler(0, rotation, 0);
            }

            Vector3 targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;
            unityCharacterController.Move(targetDirection.normalized * moveSpeed * Time.deltaTime +
                new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);

            characterAnimator.SetFloat("Horizontal", false == IsPossibleMovement ? 0 : input.x);

            characterAnimator.SetFloat("Vertical", false == isPossibleMovement ? 0 : input.y);
           
        }
        public void Rotate(Vector3 targetPoint)
        {
            if (!isStrafe) return;

            Vector3 position = transform.position;
            Vector3 direction = (targetPoint - position).normalized;
            direction.y = 0f;
            transform.forward = Vector3.Lerp(transform.forward, direction, rotationSpeed * Time.deltaTime);
        }
        public void GroundCheck()
        {
            Vector3 spherePosition = transform.position + (Vector3.down * groundOffset);
            isGrounded = Physics.CheckSphere(spherePosition, groundRadius, groundLayer, QueryTriggerInteraction.Ignore)
            ;
            if (isGrounded)
            {
                currentJumpCount = 0;
            }
            characterAnimator.SetBool("IsGrounded", isGrounded);
        }
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
        public void FreeFall()
        {
            if (isGrounded == false)
            {
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }
            else
            {
                if (jumpTimeoutDelta > 0)
                {
                    jumpTimeoutDelta -= Time.deltaTime;

                }
                else
                {
                    verticalVelocity = 0f;
                }
            }
        }
    }
}