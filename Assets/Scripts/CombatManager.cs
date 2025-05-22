using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public CombatLogUI combatLog;
    public ScoreManager scoreManager;
    public BoardManager boardManager;
    public Button endTurnButton;

    private List<CardOnBoard> allCards = new List<CardOnBoard>();

    public int manaMaximo = 3;
    public int manaActual = 3;

    public void IniciarCombate()
    {
        DeckManager deckManager = FindObjectOfType<DeckManager>();
        if (deckManager != null)
            deckManager.selectedCard = null;

        if (endTurnButton != null)
            endTurnButton.gameObject.SetActive(false);

        StartCoroutine(CombatCoroutine());
    }

    IEnumerator CombatCoroutine()
    {
        allCards.Clear();

        foreach (var slot in boardManager.playerSlots)
            if (slot.HasCard()) allCards.Add(new CardOnBoard(slot.GetPlacedCard(), slot));

        foreach (var slot in boardManager.opponentSlots)
            if (slot.HasCard()) allCards.Add(new CardOnBoard(slot.GetPlacedCard(), slot));

        if (allCards.Count == 0)
        {
            Debug.LogWarning("No hay cartas en el tablero.");
            yield break;
        }

        allCards.Sort((a, b) => b.cardData.speed.CompareTo(a.cardData.speed));
        combatLog.AddLog("¡Empieza el combate!");

        while (true)
        {
            foreach (var attacker in new List<CardOnBoard>(allCards))
            {
                if (attacker == null || attacker.cardData == null) continue;
                var enemy = FindTarget(attacker);

                if (enemy != null)
                {
                    int damage = attacker.cardData.attack;

                    if (attacker.slot.EsTerrenoSlot())
                    {
                        CardData terrenoData = attacker.slot.GetPlacedCard()?.GetComponent<CardDisplayReference>()?.cardData;
                        if (terrenoData != null && terrenoData.isTerrain)
                        {
                            if (attacker.cardData.type1 == terrenoData.type1 || attacker.cardData.type2 == terrenoData.type1)
                            {
                                damage += terrenoData.buffAttackAmount;
                                attacker.cardData.health += terrenoData.buffHealthAmount;
                                if (terrenoData.buffAttackAmount != 0)
                                    combatLog.AddLog(attacker.cardData.cardName + " gana +" + terrenoData.buffAttackAmount + " ATK por " + terrenoData.cardName);
                                if (terrenoData.buffHealthAmount != 0)
                                    combatLog.AddLog(attacker.cardData.cardName + " gana +" + terrenoData.buffHealthAmount + " HP por " + terrenoData.cardName);
                            }
                        }
                    }

                    if (attacker.cardData.passiveAbility == PassiveAbilityType.DañoExtra &&
                        (enemy.cardData.type1 == attacker.cardData.abilityTarget || enemy.cardData.type2 == attacker.cardData.abilityTarget))
                    {
                        damage += 2;
                        combatLog.AddLog(attacker.cardData.cardName + " hace daño extra a " + enemy.cardData.cardName);
                    }

                    if (enemy.cardData.passiveAbility == PassiveAbilityType.ReduccionDeDaño &&
                        (attacker.cardData.type1 == enemy.cardData.abilityTarget || attacker.cardData.type2 == enemy.cardData.abilityTarget))
                    {
                        damage = Mathf.Max(0, damage - 1);
                        combatLog.AddLog(enemy.cardData.cardName + " recibe menos daño de " + attacker.cardData.cardName);
                    }

                    if (attacker.cardObject != null)
                    {
                        LeanTween.moveLocalX(attacker.cardObject, attacker.cardObject.transform.localPosition.x + 30f, 0.15f).setEaseOutQuad()
                            .setLoopPingPong(1);
                    }

                    yield return new WaitForSeconds(0.25f);

                    combatLog.AddLog(attacker.cardData.cardName + " ataca a " + enemy.cardData.cardName + " causando " + damage + " de daño.");

                    enemy.cardData.health -= damage;

                    if (enemy.cardObject != null)
                    {
                        LeanTween.moveX(enemy.cardObject, enemy.cardObject.transform.localPosition.x + 10f, 0.1f).setEaseShake();
                    }

                    yield return new WaitForSeconds(0.2f);

                    if (attacker.cardData.passiveAbility == PassiveAbilityType.UsarVariosHechizos &&
                        (enemy.cardData.type1 == attacker.cardData.abilityTarget || enemy.cardData.type2 == attacker.cardData.abilityTarget))
                    {
                        attacker.cardData.health += 2;
                        combatLog.AddLog(attacker.cardData.cardName + " se cura al atacar a " + enemy.cardData.cardName);
                    }

                    if (enemy.cardData.health <= 0)
                    {
                        if (enemy.cardData.passiveAbility == PassiveAbilityType.ResucitarCadaTurno && !enemy.cardData.haResucitado)
                        {
                            Vector3 originalScale = enemy.cardObject.transform.localScale; // Guardar escala 🔥
                            enemy.cardData.health = 2;
                            enemy.cardData.haResucitado = true;
                            combatLog.AddLog(enemy.cardData.cardName + " resucita con 2 de vida.");

                            LeanTween.scale(enemy.cardObject, originalScale, 0.3f).setFrom(Vector3.zero);
                            yield return new WaitForSeconds(0.3f);
                        }
                        else
                        {
                            yield return StartCoroutine(HandleDeathAndInvocation(enemy));
                            allCards.Remove(enemy);
                        }
                    }
                }
            }

            if (CheckCombatEnd())
            {
                manaActual = manaMaximo;
                yield return StartCoroutine(LimpiarCartasConAnimacion());
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator HandleDeathAndInvocation(CardOnBoard enemy)
    {
        if (enemy.cardObject != null)
        {
            LeanTween.scale(enemy.cardObject, Vector3.zero, 0.5f).setEaseInBack();
        }

        yield return new WaitForSeconds(0.5f);

        if (enemy.cardObject != null)
        {
            Destroy(enemy.cardObject);
            enemy.slot.ClearSlot();
        }

        if (enemy.cardData.cartaInvocada != null)
        {
            BoardSlot slotLibre = enemy.slot.slotOwner == BoardSlot.SlotOwner.Player
                ? boardManager.GetFreePlayerSlot()
                : boardManager.GetFreeOpponentSlot();

            if (slotLibre != null)
            {
                GameObject invocada = boardManager.CrearCartaEnSlot(enemy.cardData.cartaInvocada, slotLibre.transform);
                slotLibre.PlaceCardFromAI(invocada);
                combatLog.AddLog(enemy.cardData.cartaInvocada.cardName + " ha sido invocado en el tablero.");
            }
            else
            {
                combatLog.AddLog("No hay espacio para invocar " + enemy.cardData.cartaInvocada.cardName + ".");
            }
        }
    }

    CardOnBoard FindTarget(CardOnBoard attacker)
    {
        List<CardOnBoard> posibles = new List<CardOnBoard>();
        foreach (var target in allCards)
        {
            if (target.slot.slotOwner != attacker.slot.slotOwner)
                posibles.Add(target);
        }

        foreach (var target in posibles)
        {
            if (target.cardData.passiveAbility == PassiveAbilityType.Provocar)
                return target;
        }

        if (posibles.Count > 0)
            return posibles[Random.Range(0, posibles.Count)];
        return null;
    }

    bool CheckCombatEnd()
    {
        bool playerAlive = false, opponentAlive = false;

        foreach (var card in allCards)
        {
            if (card.slot.slotOwner == BoardSlot.SlotOwner.Player) playerAlive = true;
            if (card.slot.slotOwner == BoardSlot.SlotOwner.Opponent) opponentAlive = true;
        }

        if (!playerAlive || !opponentAlive)
        {
            if (playerAlive)
                combatLog.AddLog("¡El jugador gana la ronda!");
            else if (opponentAlive)
                combatLog.AddLog("¡El oponente gana la ronda!");
            else
                combatLog.AddLog("¡Empate!");

            if (playerAlive)
                scoreManager.AddPlayerPoint();
            else if (opponentAlive)
                scoreManager.AddOpponentPoint();

            return true;
        }
        return false;
    }

    IEnumerator LimpiarCartasConAnimacion()
    {
        foreach (var slot in boardManager.playerSlots)
        {
            if (slot.HasCard() && !slot.EsTerrenoSlot())
            {
                var carta = slot.GetPlacedCard();
                if (carta != null)
                {
                    LeanTween.moveY(carta, carta.transform.localPosition.y + 100f, 0.5f).setEaseInSine();
                    LeanTween.scale(carta, Vector3.zero, 0.5f).setEaseInBack().setDelay(0.3f);
                    Destroy(carta, 0.6f);
                }
            }
        }

        foreach (var slot in boardManager.opponentSlots)
        {
            if (slot.HasCard() && !slot.EsTerrenoSlot())
            {
                var carta = slot.GetPlacedCard();
                if (carta != null)
                {
                    LeanTween.moveY(carta, carta.transform.localPosition.y + 100f, 0.5f).setEaseInSine();
                    LeanTween.scale(carta, Vector3.zero, 0.5f).setEaseInBack().setDelay(0.3f);
                    Destroy(carta, 0.6f);
                }
            }
        }

        yield return new WaitForSeconds(0.7f);

        allCards.Clear();

        if (endTurnButton != null)
        {
            endTurnButton.gameObject.SetActive(true);
            endTurnButton.interactable = true;
        }
    }
}

public class CardOnBoard
{
    public GameObject cardObject;
    public BoardSlot slot;
    public CardData cardData;

    public CardOnBoard(GameObject obj, BoardSlot s)
    {
        cardObject = obj;
        slot = s;
        CardDisplayReference refComp = obj.GetComponent<CardDisplayReference>();
        cardData = refComp != null ? refComp.cardData : null;
    }
}
