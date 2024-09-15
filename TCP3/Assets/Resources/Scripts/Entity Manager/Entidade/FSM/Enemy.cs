using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Enemy : BaseEntity
{
    [Space(10f)]
    [Header("States Configs")]
    [field: SerializeField] private bool showGizmos;
    [field: SerializeField] public Transform firePoint;

    [Space(10f)]
    [Header("States")]
    [SerializeField] private EnemyIdleStateSOBase idle;
    [SerializeField] private EnemyChaseStateSOBase chase;
    [SerializeField] private EnemyAttackStateSOBase attack;

    [HideInInspector] public EnemyIdleStateSOBase Idle { get; set; }
    [HideInInspector] public EnemyChaseStateSOBase Chase { get; set; }
    [HideInInspector] public EnemyAttackStateSOBase Attack { get; set; }

    [Space(10f)]
    [Header("States")]
    private EnemyStateMachine stateMachine;

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
        stateMachine = new EnemyStateMachine();

        Idle = Instantiate(idle);
        Chase = Instantiate(chase);
        Attack = Instantiate(attack);

        Idle?.Initialize(gameObject, this);
        Chase?.Initialize(gameObject, this);
        Attack?.Initialize(gameObject, this);

        stateMachine.Initialize(Idle);
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
