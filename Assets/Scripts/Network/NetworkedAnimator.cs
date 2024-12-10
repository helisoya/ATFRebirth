using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class NetworkedAnimator : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool isServerBased = false;
    private Dictionary<string, float> fValues = new Dictionary<string, float>();
    private Dictionary<string, bool> bValues = new Dictionary<string, bool>();


    public void ChangeRuntimeAnimator(RuntimeAnimatorController controller)
    {
        animator.runtimeAnimatorController = controller;
        animator.Rebind();
        animator.Update(0.0f);

        foreach (string key in fValues.Keys)
        {
            animator.SetFloat(key, fValues[key]);
        }

        foreach (string key in bValues.Keys)
        {
            animator.SetBool(key, bValues[key]);
        }
    }

    private T GetValue<T>(Dictionary<string, T> dic, string id, T defaultValue)
    {
        if (dic.ContainsKey(id))
        {
            return dic[id];
        }

        dic[id] = defaultValue;
        return defaultValue;
    }

    public void SetTrigger(string name)
    {
        if (isServerBased) SetTriggerRpc(name);
        else SetTriggerCommand(name);
    }

    [Command(requiresAuthority = false)]
    void SetTriggerCommand(string name)
    {
        SetTriggerRpc(name);
    }

    [ClientRpc(includeOwner = false)]
    void SetTriggerRpc(string name)
    {
        animator.SetTrigger(name);
    }



    public void SetFloat(string name, float value)
    {
        if (GetValue(fValues, name, 0) != value)
        {
            fValues[name] = value;
            animator.SetFloat(name, value);
            if (isServerBased) SetFloatRpc(name, value);
            else SetFloatCommand(name, value);
        }
    }

    [Command(requiresAuthority = false)]
    void SetFloatCommand(string name, float value)
    {
        SetFloatRpc(name, value);
    }

    [ClientRpc(includeOwner = false)]
    void SetFloatRpc(string name, float value)
    {
        animator.SetFloat(name, value);
    }



    public void SetBool(string name, bool value)
    {
        if (GetValue(bValues, name, false) != value)
        {
            bValues[name] = value;
            animator.SetBool(name, value);
            if (isServerBased) SetBoolRpc(name, value);
            else SetBoolCommand(name, value);
        }
    }


    [Command(requiresAuthority = false)]
    void SetBoolCommand(string name, bool value)
    {
        SetBoolRpc(name, value);
    }

    [ClientRpc(includeOwner = false)]
    void SetBoolRpc(string name, bool value)
    {
        animator.SetBool(name, value);
    }
}
