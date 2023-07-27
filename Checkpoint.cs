using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public List<GameObject> enemies;
    public float speed;
    public bool slowly;

    [HideInInspector] public GameObject enemy;
    [HideInInspector] public bool finish, came;

    [Header("Panels")]
    public GameObject left;
    public GameObject right;

    // Start is called before the first frame update
    void Start()
    {
        enemy = enemies[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.GetComponent<Enemy>().finish)
        {
            enemy.GetComponent<Enemy>().attackAvailable = false;
            GetComponent<Animator>().SetBool("Came", false);
            if (enemies.Count > 1)
            {
                came = false;
                enemies.RemoveAt(0);
                enemy = enemies[0];
            }
            else
            {
                finish = true;
            }
        }
        else
        {
            enemy.GetComponent<Enemy>().attackAvailable = true;

            if (!came)
            {
                if (transform.position.x < enemy.transform.position.x - 0.69f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(enemy.transform.position.x - 0.65f, enemy.transform.position.y, enemy.transform.position.z), Time.deltaTime * speed);
                    transform.LookAt(new Vector3(enemy.transform.position.x, transform.position.y, enemy.transform.position.z));
                    GetComponent<Animator>().Play("Running");
                    Debug.Log(transform.position.x + " , " + (enemy.transform.position.x - 0.69f));
                }
                if (transform.position.x >= enemy.transform.position.x - 0.69f)
                {
                    came = true;
                    GetComponent<Animator>().SetBool("Came", true);
                }
            }
        }

        if (enemy.GetComponent<Animator>().GetBool("right"))
            left.gameObject.SetActive(true);
        else
            left.gameObject.SetActive(false);

        if (enemy.GetComponent<Animator>().GetBool("left"))
            right.gameObject.SetActive(true);
        else
            right.gameObject.SetActive(false);
    }
}
