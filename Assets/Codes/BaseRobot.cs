using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRobot : MonoBehaviour
{
    // Start is called before the first frame update
    public int hp = 100;

    public bool IsAlive(){
        return hp > 0;
    }

    public void GetDamage(int dmg){
        hp -= dmg;
        if(!IsAlive()){
            Die();
        }
    }

    public virtual void Die(){
        Destroy(this.gameObject);
    }

    public virtual void OpenFire(){

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
