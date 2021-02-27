using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    PlayerCharacterController m_PlayerController;

    public UnityAction<bool> onFightStateChange;
    
    public List<EnemyController> enemies { get; private set; }
    public int numberOfEnemiesTotal { get; private set; }
    public int numberOfEnemiesRemaining => enemies.Count;
    
    public UnityAction<EnemyController, int> onRemoveEnemy;

    private List<EnemyController> triggeredEnemies;
    
    private void Awake()
    {
        m_PlayerController = FindObjectOfType<PlayerCharacterController>();
        DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, EnemyManager>(m_PlayerController, this);

        enemies = new List<EnemyController>();
        triggeredEnemies = new List<EnemyController>();
    }

    public void RegisterEnemy(EnemyController enemy)
    {
        enemies.Add(enemy);
        
        enemy.onDetectedTarget += () =>
        {
            HandleDetectedTarget(enemy);
        };
        
        enemy.onLostTarget += () =>
        {
            HandleLostTarget(enemy);
        };

        numberOfEnemiesTotal++;
    }

    public void UnregisterEnemy(EnemyController enemyKilled)
    {
        var enemiesRemainingNotification = numberOfEnemiesRemaining - 1;
        onRemoveEnemy?.Invoke(enemyKilled, enemiesRemainingNotification);
        
        HandleLostTarget(enemyKilled);

        // removes the enemy from the list, so that we can keep track of how many are left on the map
        enemies.Remove(enemyKilled);
    }
    
    private void HandleDetectedTarget(EnemyController enemyController)
    {
        if(triggeredEnemies.Contains(enemyController)) return;
        triggeredEnemies.Add(enemyController);
        
        onFightStateChange?.Invoke(triggeredEnemies.Count > 0);
    }
    
    private void HandleLostTarget(EnemyController enemyController)
    {
        if(!triggeredEnemies.Contains(enemyController)) return;
        triggeredEnemies.Remove(enemyController);
        
        onFightStateChange?.Invoke(triggeredEnemies.Count > 0);
    }
}
