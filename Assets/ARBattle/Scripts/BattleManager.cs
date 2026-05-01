using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Battle Settings")]
    public float stopDistance   = 0.5f;
    public float combatDuration = 3f;
    public float deathDuration  = 2f;

    private bool battleInProgress = false;

    public void StartBattle()
    {
        if (battleInProgress) return;

        GameObject[] all = GameObject.FindGameObjectsWithTag("Player");

        if (all.Length < 2)
        {
            Debug.Log("BattleManager: Se necesitan al menos 2 monstruos en escena.");
            return;
        }

        // Elegir 2 al azar entre todos los disponibles
        List<GameObject> pool = new List<GameObject>(all);
        int indexA = Random.Range(0, pool.Count);
        GameObject a = pool[indexA];
        pool.RemoveAt(indexA);
        int indexB = Random.Range(0, pool.Count);
        GameObject b = pool[indexB];

        StartCoroutine(BattleCoroutine(a, b));
    }

    IEnumerator BattleCoroutine(GameObject a, GameObject b)
    {
        battleInProgress = true;

        MonsterCombatant combatantA = a.GetComponentInChildren<MonsterCombatant>();
        MonsterCombatant combatantB = b.GetComponentInChildren<MonsterCombatant>();

        if (combatantA == null || combatantB == null)
        {
            Debug.LogWarning("BattleManager: Uno o ambos objetos no tienen MonsterCombatant.");
            battleInProgress = false;
            yield break;
        }

        // --- FASE 1: Acercarse ---
        combatantA.PlayWalk();
        combatantB.PlayWalk();

        while (a != null && b != null &&
               Vector3.Distance(a.transform.position, b.transform.position) > stopDistance)
        {
            combatantA.LookAt(b.transform.position);
            combatantB.LookAt(a.transform.position);
            combatantA.MoveTo(b.transform.position);
            combatantB.MoveTo(a.transform.position);
            yield return null;
        }

        // --- FASE 2: Combate ---
        combatantA.PlayAttack();
        combatantB.PlayAttack();

        float timer = 0f;
        while (timer < combatDuration)
        {
            combatantA.LookAt(b.transform.position);
            combatantB.LookAt(a.transform.position);
            timer += Time.deltaTime;
            yield return null;
        }

        // --- FASE 3: Muerte aleatoria ---
        bool aLoses = Random.value < 0.5f;
        GameObject loser  = aLoses ? a : b;
        GameObject winner = aLoses ? b : a;

        MonsterCombatant loserCombatant  = loser.GetComponentInChildren<MonsterCombatant>();
        MonsterCombatant winnerCombatant = winner.GetComponentInChildren<MonsterCombatant>();

        loserCombatant.PlayDie();
        winnerCombatant.PlayIdle();

        yield return new WaitForSeconds(deathDuration);

        // --- Eliminar al perdedor ---
        yield return new WaitForSeconds(1f);
        Destroy(loser);

        battleInProgress = false;
    }
}
