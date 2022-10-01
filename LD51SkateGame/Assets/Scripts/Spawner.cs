using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> obstacles = new List<GameObject>();

    public Transform spawnPos;
    public float spawnInterval;
    public float spawnTimer;

    private PlayerControler pc;

    private void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControler>();
    }

    void Update()
    {
        if (spawnTimer < spawnInterval) spawnTimer += (pc.curSpeed) * Time.deltaTime;
        else
        {
            spawnTimer = Random.Range(-20, 20);
            SpawnObstacle();
        }
    }

    private void SpawnObstacle()
    {
        var obj = obstacles[Random.Range(0, obstacles.Count)];
        Instantiate(obj, spawnPos.transform.position, spawnPos.rotation);
    }

}
