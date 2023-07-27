using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float health;
    public bool hit;
    public ParticleSystem hitParticle;

    private Checkpoint check;

    [HideInInspector]public GameEditor gEditor;

    [Header("Hands")]
    public GameObject leftHand;
    public GameObject rightHand;

    // Start is called before the first frame update
    void Start()
    {
        check = GameObject.FindObjectOfType<Checkpoint>();
        gEditor = GameObject.FindObjectOfType<GameEditor>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Hit()
    {
        GameObject.Find("Player").GetComponent<Animator>().SetBool("hit", true);
        hit = true;
        hitParticle.Play();
        health -= 25;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != this.tag && !gEditor.attack && check.enemy.GetComponent<Enemy>().attack)
        {
            if (check.enemy.GetComponent<Animator>().GetBool("right") && gEditor.animationBlendNumber > -0.99f)
            {
                Hit();
                GameObject.FindObjectOfType<StressReceiver>().InduceStress(0.1f);
            }
            if (check.enemy.GetComponent<Animator>().GetBool("left") && gEditor.animationBlendNumber < 0.99f)
            {
                Hit();
                GameObject.FindObjectOfType<StressReceiver>().InduceStress(0.1f);
            }
            if (check.enemy.GetComponent<Animator>().GetBool("kick"))
            {
                Hit();
                GameObject.FindObjectOfType<StressReceiver>().InduceStress(0.1f);
            }
        }
    }
}
