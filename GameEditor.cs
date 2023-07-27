using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEditor : MonoBehaviour
{
    public GameObject Player;
    private Checkpoint check;

    private int random, randomSide;

    [Header("Attack")]
    public bool attack;
    public float attackTime;
    public ParticleSystem flamePunch;


    [Header("Swipe Animation Like Matrix")]
    public bool activeMatrix;
    public bool slowlyDodge;
    public float slowlyDodgeSpeed;
     public float animationBlendNumber, animationSlowBlendNumber;

    // Start is called before the first frame update
    void Start()
    {
        attackTime = 0.75f;
        check = GameObject.Find("Player").GetComponent<Checkpoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activeMatrix)
        {
            //Yavaşça dodge larken ki bugu düzeltmesi için
            if (Input.GetMouseButtonDown(0))
            {
                animationSlowBlendNumber = animationBlendNumber;
            }

            //Bilgisayar faresi için kontrol
            if (Input.GetMouseButton(0))
            {
                if (Input.mousePosition.x < (Screen.width / 2))
                {
                    if (animationBlendNumber > -0.6f)
                        animationBlendNumber = (1 + ((Input.mousePosition.x / (Screen.width / 2)) * -1)) * -1;
                    else
                        if (animationBlendNumber > -1)
                        animationBlendNumber -= Time.deltaTime;
                }
                else
                {
                    if (animationBlendNumber < 0.6f)
                        animationBlendNumber = (Input.mousePosition.x / (Screen.width / 2)) - 1;
                    else
                        if (animationBlendNumber < 1)
                        animationBlendNumber += Time.deltaTime;
                }

                //Animatorde Blend Tree oluştutulup içindeki sayıyı değiştiriyor
                if (!slowlyDodge)
                    Player.GetComponent<Animator>().SetFloat("Dodge", animationBlendNumber);
                else
                {
                    if (animationSlowBlendNumber < animationBlendNumber)
                    {
                        animationSlowBlendNumber += Time.deltaTime * slowlyDodgeSpeed;
                    }
                    if (animationSlowBlendNumber > animationBlendNumber)
                    {
                        animationSlowBlendNumber -= Time.deltaTime * slowlyDodgeSpeed;
                    }
                    Player.GetComponent<Animator>().SetFloat("Dodge", animationSlowBlendNumber);
                }
                //Debug.Log(animationBlendNumber);
            }
            else
            {
                if (animationBlendNumber > 0)
                {
                    animationBlendNumber -= Time.deltaTime * 2;
                }
                if (animationBlendNumber < 0)
                {
                    animationBlendNumber += Time.deltaTime * 2;
                }
                Player.GetComponent<Animator>().SetFloat("Dodge", animationBlendNumber);
            }

            if (attack)
            {
                if (check.enemy.GetComponent<Enemy>().health <= 25)
                {
                    Player.GetComponent<Animator>().SetBool("combo", true);
                }
                else
                {

                    switch (random)
                    {
                        case 0:
                            Player.GetComponent<Animator>().SetBool("punch", true);
                            break;
                        case 1:
                            Player.GetComponent<Animator>().SetBool("kick", true);
                            break;
                        case 2:
                            Player.GetComponent<Animator>().SetBool("pushkick", true);
                            break;
                    }

                    switch (randomSide)
                    {
                        case 0:
                            Player.GetComponent<Animator>().SetBool("mirror", true);
                                break;
                        case 1:
                            Player.GetComponent<Animator>().SetBool("mirror", false);
                            break;
                    }
                }
                attackTime -= Time.deltaTime;
                if (attackTime <= 0)
                {
                    attack = false;
                    random = Random.Range(0, 3);
                    randomSide = Random.Range(0, 2);
                    check.enemy.GetComponent<Enemy>().health -= 25;
                    if (check.enemy.GetComponent<Enemy>().health <= 25)
                    {
                        attackTime = 2.45f;
                    }
                    else
                    {
                        attackTime = 0.75f;
                    }
                    Player.GetComponent<Animator>().SetFloat("Dodge", 0);
                    Player.GetComponent<Animator>().SetBool("punch", false);
                    Player.GetComponent<Animator>().SetBool("combo", false);
                    Player.GetComponent<Animator>().SetBool("kick", false);
                    Player.GetComponent<Animator>().SetBool("pushkick", false);
                }
            }
        }
    }
}
