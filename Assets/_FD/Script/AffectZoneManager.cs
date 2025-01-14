using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AffectZoneType { Lighting, Frozen, Poison, Magnet, Cure, Fire, Dark, Aero,LightningAll,Armagdon, DefenseWall }


[Serializable]
public class AffectZoneXPConsume{
    public AffectZoneType Type;
    [Range(1,500)]
    public int XP;
}

public class AffectZoneManager : MonoBehaviour
{
    public static AffectZoneManager Instance;
    public AffectZone[] affectZoneList;
    AffectZone pickedZone;
    AffectZoneType affectType;
    [ReadOnly] public bool isChecking = false;
    [ReadOnly] public bool isAffectZoneWorking = false;
    [Header("CURE")] public float healAmount;
    public AudioClip cureSound;
    AffectZoneButton pickedBtn;
    [Header("Armagdon")]
    public float timeDifferenceBetweenFireBalls = 0.4f;

    public float disable_affectzone_countdown_cooldown = 5;
    public AffectZoneXPConsume[] XPConsumes;
    [HideInInspector]public bool isZoneUsedFirstTime = false;
    public int XPconsume(AffectZoneType type) {
        foreach (AffectZoneXPConsume item in XPConsumes) {
            if (item.Type == type)
                return item.XP;
        }
        return 0;
    }
    private void OnEnable()
    {

    }
    #region LightningGlobal

    public void LightningAll()
    {
        foreach (AffectZone zone in affectZoneList)
        {
            zone.gameObject.SetActive(true);
            zone.Active(AffectZoneType.Lighting);
        }
    }
    #endregion

    #region Armagdon

    public void Armagdon()
    {
        for (int i = 0; i < affectZoneList.Length; i++)
        {
            StartCoroutine(ActivateArmagdon(affectZoneList[i], i * timeDifferenceBetweenFireBalls));
        }
    }

    IEnumerator ActivateArmagdon(AffectZone zone,float delay)
    {
        yield return new WaitForSeconds(delay);
        zone.gameObject.SetActive(true);
        zone.ActivateArmagdon();
    }
    

    #endregion
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (var zone in affectZoneList)
        {
            zone.gameObject.SetActive(false);
        }
    }
    public IEnumerator Cure(AffectZoneButton _pickedBtn, float delay)
    {

        if (isChecking)
            yield break;

        isChecking = false;
        isAffectZoneWorking = true;
        FindObjectOfType<TheFortrest>().HealFortress(healAmount);
        GameObject[] cureAnimations =  GameObject.FindGameObjectsWithTag("CureAnimation");
        foreach (var cureAnimation in cureAnimations)
        {
            cureAnimation.GetComponent<Animator>().SetTrigger("Cure");
        }
        SoundManager.PlaySfx(cureSound);
        yield return new WaitForSeconds(1);
        _pickedBtn.StartCountingDown();
        isAffectZoneWorking = false;
    }
    void Update(){
        if (isChecking){
            if (Input.GetMouseButtonDown(0)){
                RaycastHit2D hit = new RaycastHit2D();
                RaycastHit2D[] hits = Physics2D.CircleCastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.01f, Vector2.zero);
                foreach (RaycastHit2D item in hits){
                    if (item.collider.CompareTag("Zone")){
                        hit = item;
                        break;
                    }
                }
                Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, 10), Color.blue, 5);
                if (hit){
                    var isZone = hit.collider.gameObject.GetComponent<AffectZone>();
                    if (isZone){
                        foreach (var zone in affectZoneList){
                            if (zone.gameObject.name != isZone.gameObject.name) // When The isZone deactivates OnTriggerExit2D calls and removes all enemy inside the effect and thats why some times it doesn't works. This will fix the issue
                                zone.gameObject.SetActive(false);
                        }
                        isZone.gameObject.SetActive(true);
                        isZone.Active(affectType);
                        pickedBtn.StartCountingDown();
                        isChecking = false;
                        isAffectZoneWorking = true;
                    }
                } else { // deactivating affectzone
                    foreach (var zone in affectZoneList)
                    {
                        zone.Stop();
                    }
                    pickedBtn.StartCountingDown(disable_affectzone_countdown_cooldown);
                    isAffectZoneWorking = false;
                    isChecking = false;
                }
            }
        }
    }
    public void ActiveZone(AffectZoneType _type, AffectZoneButton _pickedBtn)
    {
        if (isChecking)
            return;

        pickedBtn = _pickedBtn;
        affectType = _type;
        isChecking = true;

        foreach (var zone in affectZoneList)
        {
            zone.gameObject.SetActive(true);
        }
    }

    public void FinishAffect()
    {
        isAffectZoneWorking = false;
    }
}
