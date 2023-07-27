using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Checkpoint check;
    private GameEditor gEditor;

    public float health;
    private float deadTime;
    public ParticleSystem hit;
    public GameObject camera, head;
    private GameObject player;

    private Vector3 fp, lp;
    private float dragDistance = 100;

    [HideInInspector] public bool finish;
    [HideInInspector] public bool attackAvailable;
    private bool cameraControl, punchControl, shakeControl;

    [Header("Random Attack")]
    private float time, attackTime;
    public bool attack;
    private int random;

    [Header("Hands")]
    public GameObject leftHand;
    public GameObject rightHand;

    // Start is called before the first frame update
    void Start()
    {
        time = 2f;
        attackTime = 2.15f;
        deadTime = 1.4f;
        player = GameObject.Find("Player").gameObject;
        check = player.GetComponent<Checkpoint>();
        gEditor = GameObject.FindObjectOfType<GameEditor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!check.finish && deadTime > 0 && !camera.transform.parent)
        {
            camera.transform.SetParent(null);
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, new Vector3(player.transform.position.x - 1.15f, 2, player.transform.position.z), Time.deltaTime * 2.5f);
            Quaternion neededRotation = Quaternion.LookRotation(new Vector3(player.transform.position.x, 1.5f, player.transform.position.z) - camera.transform.position);
            camera.transform.rotation = Quaternion.RotateTowards(camera.transform.rotation, neededRotation, Time.deltaTime * 90);
        }

        if (GetComponent<Animator>().GetBool("kick"))
        {
            rightHand.GetComponent<BoxCollider>().enabled = false;
            leftHand.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            rightHand.GetComponent<BoxCollider>().enabled = true;
            leftHand.GetComponent<BoxCollider>().enabled = true;
        }

        if (health <= 0)
        {
            GetComponent<Animator>().SetBool("dead", true);
            GetComponent<Animator>().SetBool("right", false);
            GetComponent<Animator>().SetBool("left", false);


            if (!hit.isPlaying)
                hit.Play();

            attack = false;
            time = 2f;
            if (deadTime <= 0)
            {
                finish = true;
                GetComponent<Animator>().SetFloat("animspeed", 1);

                hit.Stop();
                if (!cameraControl)
                {
                    if (check.finish)
                    {
                        player.GetComponent<Animator>().SetBool("victory", true);
                        camera.transform.SetParent(null);
                        camera.transform.position = Vector3.MoveTowards(camera.transform.position, new Vector3(player.transform.position.x + 3, 2, player.transform.position.z), Time.deltaTime * 2.5f);
                        Quaternion neededRotation = Quaternion.LookRotation(player.transform.position - camera.transform.position);
                        camera.transform.rotation = Quaternion.RotateTowards(camera.transform.rotation, neededRotation, Time.deltaTime * 180);
                    }
                }
            }
            else
            {
                GetComponent<Animator>().SetFloat("animspeed", 0.25f);
                deadTime -= Time.deltaTime;
                GameObject.FindObjectOfType<Player>().gEditor.attackTime = 0.75f;

                if (check.enemies.Count <= 1)
                {
                    camera.transform.LookAt(head.transform.position);
                    camera.transform.SetParent(head.transform);
                }
                else
                {
                    if (!shakeControl)
                    {
                        GameObject.FindObjectOfType<StressReceiver>().InduceStress(0.4f);
                        shakeControl = true;
                    }
                }
            }
        }


        if (gEditor.animationBlendNumber <= -0.99f || gEditor.animationBlendNumber >= 0.99f)
        {
            if (check.enemy == gameObject && !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                GetComponent<Animator>().SetFloat("animspeed", 3f);
                GameObject.Find("Player").GetComponent<Animator>().SetFloat("animspeed", 1);
            }
            else
            {
                GetComponent<Animator>().SetFloat("animspeed", 1f);
            }
        }
        else
        {
            if (check.enemy.GetComponent<Enemy>().attack)
            {
                GetComponent<Animator>().SetFloat("animspeed", 0.5f);
                GameObject.Find("Player").GetComponent<Animator>().SetFloat("animspeed", 0.25f);
            }
            else
            {
                if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    GetComponent<Animator>().SetFloat("animspeed", 1f);
                }
            }
        }

        if (attackAvailable)
        {
            if (!attack)
            {
                if (!GameObject.FindObjectOfType<GameEditor>().attack)
                {
                    time -= Time.deltaTime;
                    if (time <= 0)
                    {
                        time = 1f;
                        attackTime = 2.15f;
                        random = Random.Range(0, 3);
                        attack = true;
                    }
                }
            }
            else
            {
                if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    punchControl = true;
                }
                if (punchControl && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    attackTime = 0;
                }

                if (GameObject.FindObjectOfType<GameEditor>().attack)
                {
                    attack = false;
                    punchControl = false;
                    return;
                }
                hit.Stop();
                attackTime -= Time.deltaTime;


                switch (random)
                {
                    case 0:
                        GetComponent<Animator>().SetBool("right", true);
                        rightHand.GetComponent<TrailRenderer>().enabled = true;
                        break;
                    case 1:
                        GetComponent<Animator>().SetBool("left", true);
                        leftHand.GetComponent<TrailRenderer>().enabled = true;
                        break;
                    case 2:
                        GetComponent<Animator>().SetBool("kick", true);
                        break;
                }

                if (random == 2)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        player.GetComponent<Animator>().SetBool("jumpkick", true);
                        GetComponent<Animator>().SetFloat("animspeed", 4f);
                    }
                    if (Input.touchCount > 0)
                    {
                        Touch touch = Input.GetTouch(0);

                        if (touch.phase == TouchPhase.Began)
                        {
                            fp = touch.position;
                            lp = touch.position;
                        }
                        else if (touch.phase == TouchPhase.Moved)
                        {
                            lp = touch.position;
                        }
                        else if (touch.phase == TouchPhase.Ended)
                        {
                            lp = touch.position;

                            if (Mathf.Abs(lp.y - fp.y) > dragDistance)
                            {
                                if (Mathf.Abs(lp.y - fp.y) > Mathf.Abs(lp.x - fp.x))
                                {
                                    if ((lp.y > fp.y))
                                    {
                                        player.GetComponent<Animator>().SetBool("jumpkick", true);
                                        GetComponent<Animator>().SetFloat("animspeed", 4f);
                                    }
                                }
                            }
                        }
                    }
                }

                if (attackTime <= 0.5f)
                {
                    GetComponent<Animator>().SetFloat("animspeed", 4f);
                    GameObject.Find("Player").GetComponent<Animator>().SetFloat("animspeed", 1);
                }
                if (attackTime <= 0)
                {
                    leftHand.GetComponent<TrailRenderer>().enabled = false;
                    rightHand.GetComponent<TrailRenderer>().enabled = false;
                    GetComponent<Animator>().SetBool("right", false);
                    GetComponent<Animator>().SetBool("left", false);
                    GetComponent<Animator>().SetBool("kick", false);
                    player.GetComponent<Animator>().SetBool("jumpkick", false);
                    if (GameObject.FindObjectOfType<Player>().hit)
                    {
                        GameObject.FindObjectOfType<Player>().hit = false;
                        GameObject.Find("Player").GetComponent<Animator>().SetBool("hit", false);
                        attack = false;
                        punchControl = false;
                        return;
                    }
                    else
                    {
                        if (!GetComponent<Animator>().GetBool("kick"))
                        {
                            GameObject.FindObjectOfType<Player>().gEditor.attack = true;
                            GameObject.FindObjectOfType<Player>().rightHand.GetComponent<TrailRenderer>().enabled = true;
                            time = 2f;
                            attack = false;
                            punchControl = false;
                        }
                        else
                        {
                            time = 2f;
                            attack = false;
                            punchControl = false;
                        }
                        return;
                    }
                }

            }
        }
    }

    void Hit()
    {
        GetComponent<Animator>().Play("Hit");
        GameObject.FindObjectOfType<Player>().rightHand.GetComponent<TrailRenderer>().enabled = false;
        GameObject.FindObjectOfType<StressReceiver>().InduceStress(0.1f);
        GetComponent<Animator>().SetBool("right", false);
        GetComponent<Animator>().SetBool("left", false);
        GetComponent<Animator>().SetBool("kick", false);
        player.GetComponent<Animator>().SetFloat("animspeed", 1f);
        attack = false;
        attackTime = 2.15f;
        player.GetComponent<Animator>().SetBool("jumpkick", false);
        hit.Play();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != this.tag && health > 0)
            Hit();
    }
}
