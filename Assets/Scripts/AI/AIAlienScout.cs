using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAlienScout : PathFind
{
    public Light spotlight;

    protected override void Wander()
    {
        base.Wander();

        spotlight.color = Color.white;

        HandleMovementAnim();
    }

    protected override void Chase()
    {
        base.Chase();

        spotlight.color = Color.red;

        HandleMovementAnim();
    }

    protected override void StateReturn()
    {
        print("Scout cannot return!");
    }

    protected override void Attack()
    {
        print("Scout cannot attack!");
    }
}
