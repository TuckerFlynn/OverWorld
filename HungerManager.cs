using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerManager : MonoBehaviour
{
    public static HungerManager hungerManager;
    CharacterManager charMngr;
    public HUDBars hudBars;

    public float hunger;
    public float hungerRate = 1.0f;

    private void Awake()
    {
        if (hungerManager == null)
            hungerManager = this;
        else if (hungerManager != this)
            Destroy(this);
    }

    private void Start()
    {
        charMngr = CharacterManager.characterManager;
        hunger = charMngr.activeChar.hunger;
    }

    void Update()
    {
        // Hunger decreases according to hunger rate, which can increase or decrease by certain consumable items
        if (hunger > 0)
            hunger = Mathf.Clamp(hunger - (hungerRate * Time.deltaTime), 0f, 100f);
        hudBars.UpdateHunger(hunger);
    }

    public void HungerDiscrete (float amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0f, 100f);
    }

    public void HungerRate (float rate, float time)
    {
        StartCoroutine(UpdateHungerRate(rate, time));
    }

    IEnumerator UpdateHungerRate (float rate, float time)
    {
        hungerRate += rate;

        yield return new WaitForSeconds(time);

        hungerRate -= rate;
    }
}