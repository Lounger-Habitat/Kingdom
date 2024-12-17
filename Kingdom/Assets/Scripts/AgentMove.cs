using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMove : MonoBehaviour
{
    public string agentName;

    public Camera playerCamera;
    private NavMeshAgent playerAgent;
    private int _animIDSpeed;
    private int _animIDMotionSpeed;
    private Animator playerAnimator;

    [Header("测试使用")] public string targetName;

    //public float dis=0f;
    //public bool isStop;
    private Vector3 targetPos;

    // Start is called before the first frame update
    void Start()
    {
        playerAgent = GetComponent<NavMeshAgent>();
        playerAnimator = GetComponent<Animator>();
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        targetPos = transform.position;
        playerAnimator.SetFloat(_animIDMotionSpeed, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            var target = EnvironmentManager.Instance.TargetTransform(targetName);
            if (target)
            {
                //gogogo
                playerAgent.SetDestination(target.position);
                playerAnimator.SetFloat(_animIDSpeed, 1.9f);
            }
        }

        if (!playerCamera)
        {
            return;
        }

        if (!playerAgent.hasPath)
        {
            playerAnimator.SetFloat(_animIDSpeed, 0f);
        }
        else
        {
            playerAnimator.SetFloat(_animIDSpeed, 1.9f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool isColide = Physics.Raycast(ray, out hit);
            if (isColide)
            {
                playerAgent.SetDestination(hit.point);
                targetPos = hit.point;
                playerAnimator.SetFloat(_animIDSpeed, 1.9f);
            }
        }

        //dis = Vector3.Distance(transform.position, targetPos);
        //isStop = playerAgent.hasPath;
    }

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.position, FootstepAudioVolume);
        }
    }
}