using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Sirenix.OdinInspector;
using static Enemy;

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
    [field: SerializeField] public Collider attackCollider;
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

    [Space(10f)]
    [Header("States")]

    //  private Movement move;
    private EnemyStateMachine stateMachine;
    private bool IncreaseDetectionHasBuffed = false;
    [Header("Drop")]
    public List<DropItem> dropItemList = new List<DropItem>();
    [Sirenix.OdinInspector.ReadOnly, SerializeField] private float dropTotalChance;

    [System.Serializable]
    public class DropItem
    {
        public Item item;

        public float dropChance;
    }

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

        dropTotalChance = GetTotalChance();
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

    public void Movement(Vector3 dir)
    {
        float speed = statusComponent.GetStatus(StatusType.MoveSpeed);
        MoveAndRotate(dir, speed);
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

    public Item GetRandomItem()
    {
        float total = 0f;
        foreach (DropItem dropItem in dropItemList)
        {
            total += dropItem.dropChance;
        }

        float randomValue = Random.Range(0, total);
        float cumulative = 0f;

        foreach (DropItem dropItem in dropItemList)
        {
            cumulative += dropItem.dropChance;
            if (randomValue < cumulative)
            {
                return dropItem.item;
            }
        }

        return null; // Nenhum item foi dropado
    }

    private float GetTotalChance()
    {
        float total = 0f;
        foreach (DropItem dropItem in dropItemList)
        {
            total += dropItem.dropChance;
        }
        return total;
    }
    public void DropEnemyItem(int myLevel)
    {
        Item dropItem = GetRandomItem();
        int dropAmount = Random.Range(1, 3); //botar conta com base no nivel ou outra coisa sla

        for (int i = 0; i < dropAmount; i++)
        {
            Spawner.Instance.SpawnItemServerRpc(transform.position, dropItem.uniqueID);
        }
    }

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
