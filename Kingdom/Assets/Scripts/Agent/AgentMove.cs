using System;
using UnityEngine;
using UnityEngine.AI;

public class AgentMove : MonoBehaviour
{
    public string agentName;
    public float moveSpeed=2.2f;
    public Camera playerCamera;
    private NavMeshAgent playerAgent;
    private int _animIDSpeed;
    private int _animIDMotionSpeed;
    private Animator playerAnimator;

    public bool isMove = false;
    [Header("测试使用")] public string targetName;

    //public float dis=0f;
    //public bool isStop;
    private Vector3 targetPos;

    //到达目的地后回调
    private Action action;
    // Start is called before the first frame update
    void Awake()
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
        // if (Input.GetKeyDown(KeyCode.G))
        // {
        //     var target = EnvironmentManager.Instance.TargetTransform(targetName);
        //     if (target)
        //     {
        //         //gogogo
        //         playerAgent.SetDestination(target.position);
        //         playerAnimator.SetFloat(_animIDSpeed, 1.9f);
        //     }
        // }

        if (!playerAgent.hasPath)
        {
            playerAnimator.SetFloat(_animIDSpeed, 0f);
            action?.Invoke();
        }
        else
        {
            playerAnimator.SetFloat(_animIDSpeed, moveSpeed);
        }
        if (!playerCamera)
        {
            return;
        }
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //     bool isColide = Physics.Raycast(ray, out hit);
        //     if (isColide)
        //     {
        //         playerAgent.SetDestination(hit.point);
        //         targetPos = hit.point;
        //         playerAnimator.SetFloat(_animIDSpeed, 1.9f);
        //     }
        // }

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
                var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
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

    //根据目标位置，将人物移动过去
    public void MoveToTaregt(string envName)
    {
        isMove = true;//开始移动
        var target = EnvironmentManager.Instance.TargetTransform(envName);
        //Debug.Log(target.position);
        if (target)//位置存在
        {
            //gogogo
            playerAgent.SetDestination(target.position);
            playerAnimator.SetFloat(_animIDSpeed, moveSpeed);
            action = ()=>{
                Debug.Log("本次到达位置");
                isMove = false;
                action = null;
            };
        }else
        {
            //位置不存在
            Debug.Log("位置不存在");
        }
    }
    /// <summary>
    /// 根据vector3位置，将人物移动过去
    /// </summary>
    /// <param name="targetPos"></param>
    public void MoveToTaregt(Vector3 targetPos)
    {
        isMove = true;//开始移动
        //gogogo
        playerAgent.SetDestination(targetPos);
        playerAnimator.SetFloat(_animIDSpeed, moveSpeed);
        action = () =>
        {
            Debug.Log("本次到达位置");
            isMove = false;
            action = null;
        };

    }
}