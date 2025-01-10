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
    [SerializeField] private float soundRange;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private Camera playerCam;

    [Header("VFX")]

    [SerializeField] private GameObject gunHitObjVFXPrefab;
    [SerializeField] private GameObject gunHitNPCVFXPrefab;

    [Header("Network")]
    [SerializeField] private NetworkedAnimator bodyAnimator;
    [SerializeField] private Transform tpsWeaponRoot;
    private TPSWeapon tpsWeapon;

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
        if (!isLocalPlayer)
        {
            WeaponData data = GameManager.instance.GetWeaponData(networkedCurrentWeaponType);
            if (data == null) return;

            sfxSource.clip = data != null ? data.fireSound : null;
            bodyAnimator.ChangeRuntimeAnimator(data.bodyAnimType);
            tpsWeapon = Instantiate(Resources.Load<TPSWeapon>("Guns/TPS/" + networkedCurrentWeaponType.ToString()), tpsWeaponRoot);
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
            WeaponType weapon = LocalPlayerData.GetWeapon(i);
            if (weapon != WeaponType.NONE)
            {
                AddWeaponToSlot(weapon, i, !selectedWeapon);
                GameGUI.instance.SetGUIWeaponName(i, weapons[i].GetWeaponData().weaponName);

                weapons[i].SetupAmmo();
                GameGUI.instance.SetGUIWeaponAmmo(i, weapons[i].ammoInMag, weapons[i].ammoInBag);
                selectedWeapon = true;
            }
        }
    }


    /// <summary>
    /// Adds ammo to the current weapon
    /// </summary>
    /// <param name="magsToAdd">The number of mags to add</param>
    /// <returns>Was ammo added ?</returns>
    public bool AddAmmoToCurrentWeapon(int magsToAdd)
    {
        if (currentWeapon != null)
        {
            bool value = currentWeapon.AddMags(magsToAdd);
            GameGUI.instance.SetGUIWeaponAmmo(currentWeaponIndex, currentWeapon.ammoInMag, currentWeapon.ammoInBag);
            return value;
        }

        return false;
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

        Weapon newWeapon = Instantiate(Resources.Load<GameObject>("Guns/FPS/" + type.ToString()), gunRoot).GetComponent<Weapon>();
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
        GameGUI.instance.SetCurrentGUIWeapon(currentWeaponIndex);

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

        if (GameGUI.instance.inMenu || InCooldown || !PlayerNetwork.localPlayer.health.Alive || !PlayerNetwork.localPlayer.CanMove) return;

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

        GameGUI.instance.SetGUIWeaponAmmo(currentWeaponIndex, currentWeapon.ammoInMag, currentWeapon.ammoInBag);
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

        handsAnimator.SetTrigger("Fire");
        SetActionCooldown(data.fireCooldown);

        GameGUI.instance.SetGUIWeaponAmmo(currentWeaponIndex, currentWeapon.ammoInMag, currentWeapon.ammoInBag);
        SearchForTarget(data.maxRange, data.dmg, playerCam.transform.forward);

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









    /*------------------------------------- Network ----------------------------------------------------------*/


    /// <summary>
    /// Searches for a target in front of the player
    /// </summary>
    /// <param name="range">Max Range</param>
    /// <param name="dmg">Weapon damage</param>
    [Command]
    void SearchForTarget(float range, int dmg, Vector3 forward)
    {
        RaycastHit hit;
        Transform camTrans = playerCam.transform;

        if (Physics.Raycast(camTrans.position, forward, out hit, range))
        {
            print(hit.transform);
            hit.transform.SendMessage("OnTakeHit", dmg, SendMessageOptions.DontRequireReceiver);

            /*
            GameObject prefabChosen = gunHitObjVFXPrefab;

            if (hit.transform.GetComponent<NPCHitPoint>() != null)
            {
                prefabChosen = gunHitNPCVFXPrefab;
            }

            if (prefabChosen != null)
            {
                GameObject obj = Instantiate(prefabChosen, hit.point + hit.normal * 0.001f, Quaternion.identity);
                obj.transform.LookAt(hit.point + hit.normal);
                Destroy(obj, 3);
            }
            */
        }
    }


    /// <summary>
    /// Sets the networked weapon (Server)
    /// </summary>
    /// <param name="type">The weapon's type</param>
    [Command(requiresAuthority = false)]
    void ChangeWeaponCommand(WeaponType type)
    {
        networkedCurrentWeaponType = type;
        ChangeWeaponRpc(type);
    }

    /// <summary>
    /// Sets the networked weapon (Clients)
    /// </summary>
    /// <param name="type">The weapon's type</param>
    [ClientRpc(includeOwner = false)]
    void ChangeWeaponRpc(WeaponType type)
    {
        WeaponData data = GameManager.instance.GetWeaponData(type);

        sfxSource.clip = data.fireSound;
        bodyAnimator.ChangeRuntimeAnimator(data.bodyAnimType);
        if (tpsWeapon) Destroy(tpsWeapon.gameObject);
        tpsWeapon = Instantiate(Resources.Load<TPSWeapon>("Guns/TPS/" + type.ToString()), tpsWeaponRoot);
    }


    /// <summary>
    /// Plays the fire sound accross the network (Server)
    /// </summary>
    [Command(requiresAuthority = false)]
    void PlayFireSfxCommand()
    {
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

        if (tpsWeapon) tpsWeapon.ActivateMuzzleFlare();
    }

    /// <summary>
    /// Plays the reload sound accross the network (Server)
    /// </summary>
    /// <param name="type">The weapon's type</param>
    [Command(requiresAuthority = false)]
    void PlayReloadSfxCommand(WeaponType type)
    {
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
