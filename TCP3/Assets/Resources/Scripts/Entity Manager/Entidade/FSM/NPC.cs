using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Sirenix.OdinInspector;

public class NPC : BaseEntity
{

    [Space(10f)]
    [Header("States Configs")]
    [field: SerializeField] private bool showGizmos;

    [Space(10f)]
    [Header("States")]
    [SerializeField] private IdleStateSOBase idle;
    [SerializeField] private ChaseStateSOBase chase;
    [SerializeField] private AttackStateSOBase attack;

    [HideInInspector] public IdleStateSOBase Idle { get; set; }
    [HideInInspector] public ChaseStateSOBase Chase { get; set; }
    [HideInInspector] public AttackStateSOBase Attack { get; set; }

    [Space(10f)]
    [Header("States")]
    private StateMachine stateMachine;

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
        stateMachine = new StateMachine();

        Idle = Instantiate(idle);
        if (chase != null)
        {
            Chase = Instantiate(chase);
        }   
        if (attack != null)
        {
            Attack = Instantiate(attack);
        }

        Idle?.Initialize(gameObject);
        Chase?.Initialize(gameObject);
        Attack?.Initialize(gameObject);

        stateMachine.Initialize(Idle);
        dropTotalChance = GetTotalChance();
    }

    protected virtual void Update()
    {
        stateMachine.CurrentState?.DoFrameUpdateLogic();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.CurrentState?.DoPhysicsLogic();
    }

    public void ChangeState(IState newState)
    {
        stateMachine.ChangeState(newState);
    }

    public void Movement(Vector3 dir)
    {
        float speed = statusComponent.GetStatus(StatusType.MoveSpeed);
        MoveAndRotate(dir, speed);
    }

    protected override void OnTakeDamage(float amount)
    {
        base.OnTakeDamage(amount);
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

        return null;
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
        int dropAmount = Random.Range(1, 3);

        for (int i = 0; i < dropAmount; i++)
        {
            Spawner.Instance.SpawnItemServerRpc(transform.position, dropItem.uniqueID);
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmos && Application.isPlaying)
        {

        }
    }
}