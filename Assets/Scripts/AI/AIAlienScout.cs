using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAlienScout : PathFind
{
    public Light spotlight;

    public Material chaseMaterial;
    public GameObject skinObject;
    Material defaultMaterial;
    Renderer renderComp;

    private void Awake()
    {
        renderComp = skinObject.GetComponent<Renderer>();
        defaultMaterial = renderComp.material;
    }

    protected override void Wander()
    {
        renderComp.material = defaultMaterial;
        base.Wander();

        spotlight.color = Color.white;

        HandleMovementAnim();
    }

    protected override void Chase()
    {
        renderComp.material = chaseMaterial;
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
