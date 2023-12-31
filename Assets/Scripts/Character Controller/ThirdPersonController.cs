﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using Photon.Pun;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;
        
        [Tooltip("Crouch speed of the character in m/s")]
        public float CrouchSpeed = 0.5f;

        [Tooltip("Sets the stamina the character has before having to rest")]
        public float maxStamina = 100;

        [Tooltip("Sets the time before the stamina regenerates")]
        public float regenStaminaTime = 5;

        [Tooltip("Units at which the stamina regenerates")]
        public float staminaValueIncrement = 2;

        [Tooltip("Rate at which the stamina regenerates")]
        public float staminaTimeIncrement = 0.1f;

        [Tooltip("Rate at which stamina decreases while running")]
        public float staminaUseMultiplier = 5;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("Height of the character when crouching")]
        public float crouchHeight = 0.5f;

        [Tooltip("Height of the character when standing")]
        public float standingHeight = 1.8f;

        [Tooltip("Radius of the character crouching")]
        public float crouchRadius = 0.68f;

        [Tooltip("Radius of the character when standing")]
        public float standingRadius = 0.28f;

        [Tooltip("Time for the character to crouch")]
        public float timeToCrouch = 0.25f;

        [Tooltip("Center of the character when crouching")]
        public Vector3 crouchingCenter = new Vector3(0,0.5f,0);

        [Tooltip("Center of the character when standing")]
        public Vector3 standingCenter = new Vector3(0,0.93f,0);

        [Space(10)]
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Space(10)]
        [Tooltip("Flashlight object")]
        public Transform Flashlight;

        [Tooltip("Sets if character has infinite battery")]
        public bool infiniteBattery = false;

        [Tooltip("If flashlight is on")]
        public bool isLit = false;

        [Tooltip("Sets the battery the flashlight has before having to charge")]
        public float maxBattery = 100;

        [Tooltip("Sets the time before the battery regenerates")]
        public float regenBatteryTime = 5;

        [Tooltip("Units at which the battery regenerates")]
        public float batteryValueIncrement = 2;

        [Tooltip("Rate at which the battery regenerates")]
        public float batteryTimeIncrement = 0.1f;

        [Tooltip("Rate at which battery decreases while on")]
        public float batteryUseMultiplier = 5;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        [Header("Animation Override")]
        [Tooltip("Object that the character aim will follow")]
        public GameObject aimTarget;

        [Header("Interaction Actions")]
        public TextMeshPro useText;
        public float maxUseDistance = 5f;
        public LayerMask useLayers;

        public bool hasKey;

        GameManager gameManager;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float sprintSpeedInitial;
        public float currentStamina;

        public float CurrentStamina
        {
            get { return currentStamina; }
            set { currentStamina = value; }
        }

        public float currentBattery;
        private Coroutine regeneratingStamina;
        private Coroutine regeneratingBattery;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private bool isCrouching;
        private bool duringCrouchAnimation;
        private bool canLight = true;

        // timeout deltatime
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDCrouching;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        public GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        [PunRPC]
        public void LightNetwork(){
            StartCoroutine(WaitFlashlight());
        }


        private void Awake()
        {
            // get a reference to our main camera
        }

        private void Start()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            currentStamina = maxStamina;
            currentBattery = maxBattery;
            sprintSpeedInitial = SprintSpeed;

            Flashlight.GetChild(0).GetComponent<Light>().enabled = isLit;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            if (gameManager.renderTotal >= 100 && !XRSettings.enabled)
            {
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene(5);
            }
            _hasAnimator = TryGetComponent(out _animator);

            GravityForce();
            Crouch();
            if(!infiniteBattery)
            {
                Battery();
            }
            FlashlightSwitch();
            GroundedCheck();
            Stamina();
            Move();
            Prompt();
        }

        private void LateUpdate()
        {
            CameraRotation();
            CameraAim();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) 
        {
            if(hit.gameObject.GetComponent<exit>())
            {
                hit.gameObject.GetComponent<exit>().EndOfGame();
            }
        }

        public void Die()
        {
            if(!XRSettings.enabled)
            {
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene("GameOver");
            }
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDCrouching = Animator.StringToHash("Crouching");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void CameraAim()
        {
            Transform aimReference = _mainCamera.transform.GetChild(0).transform; 
            aimTarget.transform.position = new Vector3(aimReference.position.x, aimReference.position.y, aimReference.position.z);
        }

        [PunRPC]
        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = isCrouching ? CrouchSpeed : _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void GravityForce()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }
            else
            {
                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void Crouch()
        {
            if(_input.crouch && !duringCrouchAnimation && Grounded || _input.sprint && isCrouching && !duringCrouchAnimation && Grounded)
            {
                StartCoroutine(CrouchStand());
            }
        }

        private void FlashlightSwitch()
        {
            if(gameObject.GetComponent<PhotonView>().IsMine && _input.flashlight && canLight && currentBattery != 0)
            {
                gameObject.GetComponent<PhotonView>().RPC ("LightNetwork",RpcTarget.AllBuffered, null);
            } 
        }

        private void Stamina()
        {
            if(_input.sprint && _speed >= 1f)
            {
                if(regeneratingStamina != null)
                {
                    StopCoroutine(regeneratingStamina);
                    regeneratingStamina = null;
                }

                currentStamina -= staminaUseMultiplier * Time.deltaTime;

                if(currentStamina < 0)
                {
                    currentStamina = 0;
                }

                if(currentStamina <= 0)
                {
                    SprintSpeed = MoveSpeed;
                }
            }

            if(!_input.sprint && currentStamina < maxStamina && regeneratingStamina == null)
            {
                regeneratingStamina = StartCoroutine(RegenerateStamina());
            }
        }

        private void Battery()
        {
            if(gameObject.GetComponent<PhotonView>().IsMine && isLit && Flashlight.GetChild(0).GetComponent<Light>().enabled == true)
            {
                if(regeneratingBattery != null)
                {
                    StopCoroutine(regeneratingBattery);
                    regeneratingBattery = null;
                }

                currentBattery -= batteryUseMultiplier * Time.deltaTime;

                if(currentBattery < 0)
                {
                    currentBattery = 0;
                }

                if(currentBattery <= 0)
                {   
                    gameObject.GetComponent<PhotonView>().RPC ("LightNetwork",RpcTarget.AllBuffered, null);
                    canLight = false;
                }
            }

            if(gameObject.GetComponent<PhotonView>().IsMine && !isLit && currentBattery < maxBattery && regeneratingBattery == null)
            {
                regeneratingBattery = StartCoroutine(RegenerateBattery());
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        public void OnInteract()
        {
            if(Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out RaycastHit hit, maxUseDistance, useLayers))
            {
                if(hit.collider.TryGetComponent<Door>(out Door door) && door.needsKey && !door.isFinalDoor)
                {
                    if(door.isOpen && hasKey)
                    {
                        door.GetComponent<PhotonView>().RPC ("CloseNetwork",RpcTarget.AllBuffered, null);
                    }
                    else if(!door.isOpen && hasKey)
                    {
                        door.GetComponent<PhotonView>().RPC ("OpenNetwork",RpcTarget.AllBuffered, transform.position);
                    }
                }
                if(hit.collider.TryGetComponent<Door>(out door) && !door.needsKey && !door.isFinalDoor)
                {
                    if(door.isOpen)
                    {
                        door.GetComponent<PhotonView>().RPC ("CloseNetwork",RpcTarget.AllBuffered, null);
                    }
                    else
                    {
                        door.GetComponent<PhotonView>().RPC ("OpenNetwork",RpcTarget.AllBuffered, transform.position);
                    }
                }
                else if(hit.collider.TryGetComponent<Rendering>(out Rendering computer) && computer.enabled && !computer.isRendering)
                {
                    computer.GetComponent<PhotonView>().RPC ("StartProgress",RpcTarget.AllBuffered, null);
                }
            }
        }
        

        private void Prompt()
        {
            if(Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out RaycastHit hit, maxUseDistance, useLayers) && hit.collider.TryGetComponent<Door>(out Door door) && !door.needsKey && !door.isFinalDoor)
            {
                if(door.isOpen)
                {
                    useText.SetText("Close \"E\"");
                }
                else
                {
                    useText.SetText("Open \"E\"");
                }
                useText.gameObject.SetActive(true);
                useText.transform.position = hit.point - (hit.point - _mainCamera.transform.position).normalized * 1f;
                useText.transform.rotation = Quaternion.LookRotation((hit.point - _mainCamera.transform.position).normalized);
            }
            else if(Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, maxUseDistance, useLayers) && hit.collider.TryGetComponent<Door>(out door) && door.needsKey && !door.isFinalDoor)
            {
                if(!hasKey)
                {
                    useText.SetText("Access Card Needed");
                }
                else if(door.isOpen)
                {
                    useText.SetText("Close \"E\"");
                }
                else
                {
                    useText.SetText("Open \"E\"");
                }
                useText.gameObject.SetActive(true);
                useText.transform.position = hit.point - (hit.point - _mainCamera.transform.position).normalized * 1f;
                useText.transform.rotation = Quaternion.LookRotation((hit.point - _mainCamera.transform.position).normalized);
            }
            else if(Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, maxUseDistance, useLayers) && hit.collider.TryGetComponent<Rendering>(out Rendering computer) && computer.enabled && !computer.isRendering)
            {
                useText.SetText("Start Rendering \"E\"");
                useText.gameObject.SetActive(true);
                useText.transform.position = hit.point - (hit.point - _mainCamera.transform.position).normalized * 1f;
                useText.transform.rotation = Quaternion.LookRotation((hit.point - _mainCamera.transform.position).normalized);
            }
            else
            {
                useText.gameObject.SetActive(false);
            }
        }

        private IEnumerator CrouchStand()
        {
            if(isCrouching && Physics.Raycast(gameObject.transform.GetChild(1).position, Vector3.up, 0.7f))
            {
                yield break;
            }

            if(_hasAnimator)
            {
                _animator.SetBool(_animIDCrouching, !isCrouching);
            }

            duringCrouchAnimation = true;

            float timeElapsed = 0;
            float targetHeight = isCrouching ? standingHeight : crouchHeight;
            float currentHeight = _controller.height;
            float targetRadius = isCrouching ? standingRadius : crouchRadius;
            float currentRadius = _controller.radius;
            Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
            Vector3 currentCenter = _controller.center;

            while(timeElapsed < timeToCrouch)
            {
                _controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/timeToCrouch);
                _controller.radius = Mathf.Lerp(currentRadius, targetRadius, timeElapsed/timeToCrouch);
                _controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/timeToCrouch);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            _controller.height = targetHeight;
            _controller.center = targetCenter;
            
            isCrouching = !isCrouching;

            yield return new WaitForSeconds(0.3f);

            duringCrouchAnimation = false;
        }

        private IEnumerator WaitFlashlight()
        {
            canLight = false;

            isLit = !isLit;
            
            Flashlight.GetChild(0).GetComponent<Light>().enabled = isLit;

            yield return new WaitForSeconds(0.3f);

            canLight = true;
        }

        private IEnumerator RegenerateStamina()
        {
            yield return new WaitForSeconds(regenStaminaTime);
            WaitForSeconds timeToWait = new WaitForSeconds(staminaTimeIncrement);

            while(currentStamina < maxStamina)
            {
                if(currentStamina > 0)
                {
                    SprintSpeed = sprintSpeedInitial;
                }

                currentStamina += staminaValueIncrement;

                if(currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }

                yield return timeToWait;
            }

            regeneratingStamina = null;
        }

        private IEnumerator RegenerateBattery()
        {
            yield return new WaitForSeconds(regenBatteryTime);
            WaitForSeconds timeToWait = new WaitForSeconds(batteryTimeIncrement);

            while(currentBattery < maxBattery)
            {
                if(currentBattery > 0)
                {
                    SprintSpeed = sprintSpeedInitial;
                }

                currentBattery += batteryValueIncrement;

                if(currentBattery > maxBattery)
                {
                    currentBattery = maxBattery;
                }

                yield return timeToWait;
            }

            regeneratingBattery = null;
        }
    }
}