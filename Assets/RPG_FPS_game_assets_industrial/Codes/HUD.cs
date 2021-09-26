using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private static HUD instance;
    public Image weaponIcon;
    public Text bulletNum;
    public Text hpNum;
    public static HUD GetInstance(){
        return instance;
    }
    // Start is called before the first frame update
    private void Awake() {
        instance = this;
    }

    private void Update() {
        UpdateHp(3);
    }

    public void UpdateWeaponUI(Sprite icon,int bulletNum){
        weaponIcon.sprite = icon;
        this.bulletNum.text = bulletNum.ToString();
    }
    public void UpdateHp(int hpNum){
        this.hpNum.text = hpNum.ToString();
    }
}
