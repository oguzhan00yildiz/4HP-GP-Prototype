using PlayerLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private bool abilityShoot;
    [HideInInspector]
    public static UpgradeManager instance;
    private PlayerAttackHandler playerAttackHandler;

    private void Awake()
    {
        playerAttackHandler = FindObjectOfType<PlayerAttackHandler>();
        if (playerAttackHandler == null)
        {
            Debug.LogError("UpgradeManager: PlayerAttackHandler not found!");
        }
    }
    void Update()
    {
        if (abilityShoot)
        {
            playerAttackHandler.Shoot();
        }
    }

    public void AbilityShoot()
    {
        abilityShoot = true;
    }
}
