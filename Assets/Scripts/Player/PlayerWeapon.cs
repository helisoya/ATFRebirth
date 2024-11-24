using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


/// <summary>
/// Handles the player's weapons
/// </summary>
public class PlayerWeapon : NetworkBehaviour
{

    [Header("Components")]
    [SerializeField] private Animator handsAnimator;
    [SerializeField] private Transform gunRoot;
    [SerializeField] private GameObject gunHitObjVFXPrefab;
    [SerializeField] private GameObject gunHitNPCVFXPrefab;
    [SerializeField] private GameObject gunHitGlassVFXPrefab;
    [SerializeField] private AnimatorOverrideController noWeaponController;
    [SerializeField] private float soundRange;
    [SerializeField] private AudioSource sfxSource;

    [Header("Network")]
    [SerializeField] private NetworkedAnimator bodyAnimator;

    private Weapon[] weapons;
    private int currentWeaponIndex;
    private Weapon currentWeapon;
    [SyncVar] private WeaponType networkedCurrentWeaponType = WeaponType.NONE;

    private Coroutine changeWeaponRoutine;
    private bool aiming = false;

    private float lastAction = 0;
    private float currentActionCooldown = 0;

    public bool InCooldown { get { return Time.time - lastAction < currentActionCooldown; } }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, soundRange);
    }

    public override void OnStartClient()
    {
        if (!isClient)
        {
            WeaponData data = GameManager.instance.GetWeaponData(networkedCurrentWeaponType);
            sfxSource.clip = data != null ? data.fireSound : null;
            bodyAnimator.ChangeRuntimeAnimator(data.bodyAnimType);
        }

    }

    void Start()
    {
        currentWeaponIndex = -1;
        currentWeapon = null;
        weapons = new Weapon[2];
        bool selectedWeapon = false;

        for (int i = 0; i < 2; i++)
        {
            WeaponType weapon = WeaponType.GLOCK; //GameManager.instance.weapons[i];
            if (weapon != WeaponType.NONE)
            {
                AddWeaponToSlot(weapon, i, !selectedWeapon);
                weapons[i].RefillAmmoToMax();
                selectedWeapon = true;
            }
        }
    }

    /// <summary>
    /// Picks up a weapon
    /// </summary>
    /// <param name="type">The weapon's type</param>
    public void PickupWeapon(WeaponType type)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
            {
                AddWeaponToSlot(type, i, true);
                return;
            }
        }

        int currentSlot = currentWeaponIndex;
        DropCurrentWeapon();
        AddWeaponToSlot(type, currentSlot, true);
    }

    /// <summary>
    /// Adds ammo to the current weapon
    /// </summary>
    /// <param name="magsToAdd">The number of mags to add</param>
    public void AddAmmoToCurrentWeapon(int magsToAdd)
    {
        if (currentWeapon != null)
        {
            currentWeapon.AddMags(magsToAdd);
        }
    }

    /// <summary>
    /// Drops the current weapon
    /// </summary>
    public void DropCurrentWeapon()
    {
        if (currentWeapon == null) return;

        Transform cameraPos = Camera.main.transform;
        SpawnWeaponCommand(currentWeapon.GetWeaponData().type.ToString(),
            cameraPos.position + cameraPos.forward * 1f,
            cameraPos.forward);

        Destroy(currentWeapon.gameObject);
        currentWeaponIndex = -1;
        currentWeapon = null;
        handsAnimator.runtimeAnimatorController = noWeaponController;
    }

    /// <summary>
    /// Adds a weapon to the specified slot
    /// </summary>
    /// <param name="type">The gun's type</param>
    /// <param name="slot">The gun's slot</param>
    /// <param name="forceChangeToWeapon">Force the change to the new weapon ?</param>
    public void AddWeaponToSlot(WeaponType type, int slot, bool forceChangeToWeapon = true)
    {
        if (slot < 0 || slot >= weapons.Length) return;

        Weapon newWeapon = Instantiate(Resources.Load<GameObject>("Guns/" + type.ToString()), gunRoot).GetComponent<Weapon>();
        newWeapon.gameObject.SetActive(false);

        if (weapons[slot] != null)
        {
            Destroy(weapons[slot].gameObject);
        }

        weapons[slot] = newWeapon;

        if (forceChangeToWeapon)
        {
            ChangeWeaponToSlot(slot);
        }
    }

    /// <summary>
    /// Changes the selected weapon slot
    /// </summary>
    /// <param name="slot">The new slot</param>
    void ChangeWeaponToSlot(int slot)
    {
        if (changeWeaponRoutine != null) return;

        if (currentWeaponIndex == slot || weapons[slot] == null) return;

        changeWeaponRoutine = StartCoroutine(Coroutine_ChangeWeaponToSlot(slot));
    }

    /// <summary>
    /// Routine for changing the weapon's slot
    /// </summary>
    /// <param name="slot">The new slot</param>
    /// <returns>IEnumerator</returns>
    IEnumerator Coroutine_ChangeWeaponToSlot(int slot)
    {
        int oldSlot = currentWeaponIndex;
        currentWeaponIndex = slot;

        currentWeapon = weapons[currentWeaponIndex];

        if (oldSlot == -1)
        {
            handsAnimator.runtimeAnimatorController = weapons[slot].GetWeaponData().animType;
            weapons[slot].gameObject.SetActive(true);
            SetActionCooldown(weapons[slot].GetWeaponData().drawTime);
            changeWeaponRoutine = null;
        }
        else
        {
            handsAnimator.SetTrigger("ChangeWeapon");
            SetActionCooldown(weapons[oldSlot].GetWeaponData().undrawTime + weapons[slot].GetWeaponData().drawTime);

            yield return new WaitForSeconds(weapons[oldSlot].GetWeaponData().undrawTime);

            handsAnimator.runtimeAnimatorController = weapons[slot].GetWeaponData().animType;
            weapons[oldSlot].gameObject.SetActive(false);
            weapons[slot].gameObject.SetActive(true);

            changeWeaponRoutine = null;
        }

        WeaponData currentData = currentWeapon.GetWeaponData();
        sfxSource.clip = currentData.fireSound;

        ChangeWeaponCommand(currentData.type);

        handsAnimator.Rebind();
        handsAnimator.Update(0f);
    }

    /// <summary>
    /// Sets the action cooldown
    /// </summary>
    /// <param name="cooldown">The new cooldown</param>
    void SetActionCooldown(float cooldown)
    {
        lastAction = Time.time;
        currentActionCooldown = cooldown;
    }


    void Update()
    {

        if (InCooldown) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeaponToSlot(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeaponToSlot(1);
        }


        if (currentWeaponIndex == -1)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && currentWeapon.canReload)
        {
            Reload();
        }
        else if ((Input.GetMouseButtonDown(0) || (currentWeapon.GetWeaponData().automatic && Input.GetMouseButton(0))) && currentWeapon.canFire)
        {
            Fire();
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            DropCurrentWeapon();
        }

        aiming = changeWeaponRoutine != null ? false : Input.GetMouseButton(1);
        handsAnimator.SetBool("Aim", aiming);
    }

    /// <summary>
    /// Reloads the weapon
    /// </summary>
    void Reload()
    {
        currentWeapon.Reload();
        handsAnimator.SetTrigger("Reload");
        SetActionCooldown(currentWeapon.GetWeaponData().reloadCooldown);
        sfxSource.PlayOneShot(currentWeapon.GetWeaponData().reloadSound);
        PlayReloadSfxCommand(currentWeapon.GetWeaponData().type);
        bodyAnimator.SetTrigger("Reload");
    }

    /// <summary>
    /// Fires the weapon
    /// </summary>
    void Fire()
    {
        WeaponData data = currentWeapon.GetWeaponData();
        currentWeapon.Fire();

        sfxSource.Stop();
        sfxSource.Play();
        PlayFireSfxCommand();

        bodyAnimator.SetTrigger("Fire");

        handsAnimator.SetTrigger("Fire");
        SetActionCooldown(data.fireCooldown);
        //SearchForTarget(data.maxRange, data.dmg, false);

        if (!data.silenced)
        {
            /*
            foreach (NPCDetection npc in PlayerManager.instance.npcs)
            {
                if (npc == null) continue;
                if (Vector3.Distance(npc.transform.position, transform.position) <= soundRange)
                {
                    npc.Alert();
                }
            }*/
        }

    }

    void SearchForTarget(float range, int dmg)
    {
        StartCoroutine(Routine_SearchForTarget(range, dmg));

    }


    IEnumerator Routine_SearchForTarget(float range, int dmg)
    {
        bool ok = true;
        RaycastHit hit;
        Transform camTrans = Camera.main.transform;

        while (ok)
        {
            ok = false;
            if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, range))
            {
                GameObject prefabChosen = gunHitObjVFXPrefab;

                hit.transform.SendMessage("OnTakeHit", dmg, SendMessageOptions.DontRequireReceiver);

                /*
                if (hit.transform.GetComponent<NPCHitPoint>() != null)
                {
                    prefabChosen = gunHitNPCVFXPrefab;
                }
                else if (hit.transform.GetComponent<BreakableGlass>() != null)
                {
                    canShowEffectWithMelee = true;
                    prefabChosen = gunHitGlassVFXPrefab;
                    ok = true;
                }
                */


                if (prefabChosen != null)
                {
                    GameObject obj = Instantiate(prefabChosen, hit.point + hit.normal * 0.001f, Quaternion.identity);
                    obj.transform.LookAt(hit.point + hit.normal);
                    Destroy(obj, 3);
                }

            }
            yield return new WaitForEndOfFrame();
        }


    }








    /*------------------------------------- Network ----------------------------------------------------------*/


    /// <summary>
    /// Rpc for spawning a droped weapon
    /// </summary>
    /// <param name="weaponName">The droped weapon's name</param>
    /// <param name="position">The droped weapon's position</param>
    /// <param name="impulse">The droped weapon's impulse</param>
    [Command(requiresAuthority = false)]
    void SpawnWeaponCommand(string weaponName, Vector3 position, Vector3 impulse)
    {
        WeaponDrop drop = Instantiate(Resources.Load<GameObject>("Drops/" + weaponName)).GetComponent<WeaponDrop>();
        drop.Init(position, impulse);
        NetworkServer.Spawn(drop.gameObject);
    }

    /// <summary>
    /// Sets the networked weapon (Server)
    /// </summary>
    /// <param name="type">The weapon's type</param>
    [Command(requiresAuthority = false)]
    void ChangeWeaponCommand(WeaponType type)
    {
        networkedCurrentWeaponType = type;
        if (!isLocalPlayer)
        {
            sfxSource.clip = GameManager.instance.GetWeaponData(type).fireSound;
            bodyAnimator.ChangeRuntimeAnimator(GameManager.instance.GetWeaponData(type).bodyAnimType);
        }
        ChangeWeaponRpc(type);
    }

    /// <summary>
    /// Sets the networked weapon (Clients)
    /// </summary>
    /// <param name="type">The weapon's type</param>
    [Command(requiresAuthority = false)]
    void ChangeWeaponRpc(WeaponType type)
    {
        sfxSource.clip = GameManager.instance.GetWeaponData(type).fireSound;
        bodyAnimator.ChangeRuntimeAnimator(GameManager.instance.GetWeaponData(type).bodyAnimType);
    }


    /// <summary>
    /// Plays the fire sound accross the network (Server)
    /// </summary>
    [Command(requiresAuthority = false)]
    void PlayFireSfxCommand()
    {
        if (!isLocalPlayer)
        {
            sfxSource.Stop();
            sfxSource.Play();
        }

        PlayFireSfxRpc();
    }

    /// <summary>
    /// Plays the fire sound accross the network (Clients)
    /// </summary>
    [ClientRpc(includeOwner = false)]
    void PlayFireSfxRpc()
    {
        sfxSource.Stop();
        sfxSource.Play();
    }

    /// <summary>
    /// Plays the reload sound accross the network (Server)
    /// </summary>
    /// <param name="type">The weapon's type</param>
    [Command(requiresAuthority = false)]
    void PlayReloadSfxCommand(WeaponType type)
    {
        if (!isLocalPlayer) sfxSource.PlayOneShot(GameManager.instance.GetWeaponData(type).reloadSound);
        PlayReloadSfxRpc(type);
    }

    /// <summary>
    /// Plays the reload sound accross the network (Clients)
    /// </summary>
    /// <param name="type">The weapon's type</param>
    [ClientRpc(includeOwner = false)]
    void PlayReloadSfxRpc(WeaponType type)
    {
        sfxSource.PlayOneShot(GameManager.instance.GetWeaponData(type).reloadSound);
    }
}
