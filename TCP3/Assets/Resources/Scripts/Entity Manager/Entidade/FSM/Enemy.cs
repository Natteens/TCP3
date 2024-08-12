using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : BaseEntity
{
    [Space(10f)]
    [Header("Basics Configs")]
    [SerializeField] private List<Detector> detectors;
    [SerializeField] private List<SteeringBehaviour> steeringBehaviours;
    [SerializeField] private ContextSolver contextSolver;
    [SerializeField] private AIData _aiData;
    [SerializeField] private float detectionDelay = 0.05f;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private bool hasIncreasedDetectionRange = false;


    [Space(10f)]
    [Header("States Configs")]
    [field: SerializeField] private bool showGizmos;
    [field: SerializeField] public Transform firePoint;
    [field: SerializeField] public Collider2D attackCollider;
    [field: SerializeField] public StatusEffect enemyEffect;

    [Space(10f)]
    [Header("States")]
    [SerializeField] private EnemyIdleStateSOBase idle;
    [SerializeField] private EnemyChaseStateSOBase chase;
    [SerializeField] private EnemyAttackStateSOBase attack;

    [HideInInspector] public EnemyIdleStateSOBase Idle { get; set; }
    [HideInInspector] public EnemyChaseStateSOBase Chase { get; set; }
    [HideInInspector] public EnemyAttackStateSOBase Attack { get; set; }

    [HideInInspector] public List<Detector> Detectors { get; set; }
    [HideInInspector] public List<SteeringBehaviour> SteeringBehaviours { get; set; }
    [HideInInspector] public ContextSolver ContextSolver { get; set; }
    [HideInInspector] public AIData aiData { get { return _aiData;} set { _aiData = value; } }
    [HideInInspector]  public bool attackRangeInUse => hasIncreasedDetectionRange;

  //  private Movement move;
    private EnemyStateMachine stateMachine;
    private bool IncreaseDetectionHasBuffed = false;

    public virtual void Start()
    {
      //  move = new Movement(this);
        stateMachine = new EnemyStateMachine();

        Idle = Instantiate(idle);
        Chase = Instantiate(chase);
        Attack = Instantiate(attack);

        Idle?.Initialize(gameObject, this);
        Chase?.Initialize(gameObject, this);
        Attack?.Initialize(gameObject, this);

        stateMachine.Initialize(Idle);

        InitializeScriptables();
       
        InvokeRepeating("PerformDetection", 0, detectionDelay);
    }

    protected virtual void Update()
    {
        stateMachine.CurrentEnemyState?.DoFrameUpdateLogic();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.CurrentEnemyState?.DoPhysicsLogic();
    }

    public void ChangeState(IEnemyState newState)
    {
        stateMachine.ChangeState(newState);
    }

    public void Movement(Vector2 dir)
    {
        float speed = statusComponent.GetStatus(StatusType.MoveSpeed);
        if (IsAlive)
        {
          //  move.Move(dir, speed);
        }
        else
        {
         //   move.Move(Vector2.zero, speed);
        }
    }

    private void PerformDetection()
    {
        foreach (Detector detector in Detectors)
        {
            detector.Detect(aiData);
        }
    }

    private void InitializeScriptables()
    {
        SteeringBehaviours = new List<SteeringBehaviour>();
        foreach (var behaviour in steeringBehaviours)
        {
            var newBehaviour = Instantiate(behaviour);
            SteeringBehaviours.Add(newBehaviour);
        }

        Detectors = new List<Detector>();
        foreach (var detector in detectors)
        {
            var newDetector = Instantiate(detector);
            Detectors.Add(newDetector);
        }

        ContextSolver = Instantiate(contextSolver);
        ContextSolver.Init();
    }

    protected override void OnTakeDamage(float amount)
    {
        base.OnTakeDamage(amount);
        IncreasedDetectionRange();
    }

    public void IncreasedDetectionRange()
    {
        if (!IncreaseDetectionHasBuffed)
        {
            if (!hasIncreasedDetectionRange)
            {
                foreach (Detector D in Detectors)
                {
                    D.targetDetectionRange = detectionRange;
                }
                hasIncreasedDetectionRange = true;
                IncreaseDetectionHasBuffed = true;
            } 
        }
    }

    public virtual void EventActionOnAttack() { }

    public virtual void EventActionOnDeath() { }

    private void OnDrawGizmos()
    {
        if (showGizmos && Application.isPlaying)
        {
            foreach (SteeringBehaviour steeringBehaviour in SteeringBehaviours)
            {
                steeringBehaviour.DrawGizmos(aiData);
            }

            foreach (Detector detector in Detectors)
            {
                detector.DrawGizmos(aiData);
            }

            ContextSolver.DrawGizmos(aiData);
        }
    }
}
