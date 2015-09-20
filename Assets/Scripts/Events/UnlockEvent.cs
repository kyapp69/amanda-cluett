﻿using UnityEngine;

public class UnlockEvent : MonoBehaviour
{
    [SerializeField]
    GamePanel parentPanel = null;

    [Header("Stored Settings")]
    [SerializeField]
    string storedKey = "test";

    [Header("Stuff to unlock")]
    [SerializeField]
    GamePowerUpGroup group;
    [SerializeField]
    DialogCollection conversation;
    // FIXME: consider unlocking news

    IEventCondition[] allConditionsToMeet = null;
    bool isUnlocked = false, allConditionsPassed = false;
    int index = 0;

    System.Action<GamePanel> unitsChanged = null;

    public bool IsUnlocked
    {
        get
        {
            return isUnlocked;
        }
    }

    // Use this for initialization
    void Awake ()
    {
        // Reset events
        OnDestroy();

        // Grab all conditions
        allConditionsToMeet = GetComponentsInChildren<IEventCondition>();

        // Check to see if this event is unlocked or not
        isUnlocked = (PlayerPrefs.GetInt(storedKey, 0) != 0);
        if(isUnlocked == true)
        {
            UnlockEverything();
        }
        else
        {
            unitsChanged = new System.Action<GamePanel>(CheckIfUnlocked);
            parentPanel.OnCurrencyChanged += unitsChanged;
            parentPanel.OnSupplyChanged += unitsChanged;
        }
    }

    void OnDestroy()
    {
        if(unitsChanged != null)
        {
            parentPanel.OnCurrencyChanged -= unitsChanged;
            parentPanel.OnSupplyChanged -= unitsChanged;
            unitsChanged = null;
        }
    }

    void CheckIfUnlocked(GamePanel panel)
    {
        // Check if all conditions passed
        allConditionsPassed = true;
        for(index = 0; index < allConditionsToMeet.Length; ++index)
        {
            if (allConditionsToMeet[index].Passed(this, panel) == false)
            {
                allConditionsPassed = false;
                break;
            }
        }

        // If so, unlock everything!
        UnlockEverything();
    }

    void UnlockEverything()
    {
        if(isUnlocked == false)
        {
            // Mark as unlocked
            isUnlocked = true;
            PlayerPrefs.SetInt(storedKey, 1);

            // Disassociate with all events
            OnDestroy();

            // Check to see if there's a conversation to unlock first
            if (conversation != null)
            {
                parentPanel.Dialog.ShowDialog(conversation, UnlockGroup);
            }
            else
            {
                // Just unlock the group
                UnlockGroup(null);
            }
        }
    }

    void UnlockGroup(DialogPanel panel)
    {
        // Unlock the group
        if(group != null)
        {
            group.gameObject.SetActive(true);
        }

        // FIXME: consider unlocking news here
    }
}
