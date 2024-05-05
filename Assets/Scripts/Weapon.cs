using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviourPunCallbacks
{
    #region Variables
    public Gun[] loadout;
    public Transform weanponParent;
    public GameObject bulletholePrefab;
    public LayerMask canBeShot;

    private float currentCooldown;
    private int currentIndex;
    private GameObject currentWeapon;
    #endregion

    #region Callbacks
    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) Equip(0);
        if (currentWeapon != null)
        {
            Aim(Input.GetMouseButton(1));
            if (Input.GetMouseButtonDown(0) && currentCooldown <= 0)
            {
                Shoot();
            }

            //weapon position elasticity
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

            //cooldown
            if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
        }
    }
    #endregion
    
    #region PrivateMethod
    void Equip(int i)
    {
        if (currentWeapon != null) Destroy(currentWeapon);
        GameObject t_newEquipment = Instantiate(loadout[i].prefab, weanponParent.position, weanponParent.rotation, weanponParent);
        t_newEquipment.transform.localPosition = Vector3.zero;
        t_newEquipment.transform.localEulerAngles = Vector3.zero;
        currentWeapon = t_newEquipment;
        currentIndex = i;
    }

    void Aim(bool p_isAiming)
    {
        Transform t_anchor = currentWeapon.transform.Find("Anchor");
        Transform t_state_ads = currentWeapon.transform.Find("State/Ads/Anchor (1)");
        Transform t_state_hip = currentWeapon.transform.Find("State/Hip/Anchor (1)");

        if (p_isAiming)
        {
            //aim
            t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_ads.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
        }
        else
        {
            //hip
            t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_hip.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
        }
    }
    
    void Shoot()
    {
        Transform t_spawn = transform.Find("Camera/NormalCamera");
        currentCooldown = loadout[currentIndex].firerate;
        //bloom
        Vector3 t_bloom = t_spawn.position + t_spawn.forward * 1000f;
        t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.up;
        t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.right;
        t_bloom -= t_spawn.position;
        t_bloom.Normalize();

        RaycastHit t_hit = new RaycastHit();
        if (Physics.Raycast(t_spawn.position, t_bloom, out t_hit, 1000f, canBeShot))
        {
            GameObject t_newHole = Instantiate(bulletholePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity);
            t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
            Destroy(t_newHole, 5f);
        }

        //recoil
        currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
        currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;
    }
    #endregion
}
