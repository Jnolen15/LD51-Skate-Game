using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstControl : MonoBehaviour
{
    private PlayerControler pc;
    private Transform curb;
    private float defaultY;
    private float lastPlayerPosY;

    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControler>();
        curb = GameObject.FindGameObjectWithTag("Curb").GetComponent<Transform>();
        defaultY = this.transform.localPosition.y;
    }

    void Update()
    {
        var xmov = 0f;
        if (pc.curSpeed > 0.1)
            xmov = -(pc.curSpeed * Time.deltaTime);

        transform.position += new Vector3(xmov, 0, 0);

        transform.position = new Vector3(transform.position.x, curb.position.y, 0);

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log(col.gameObject.tag);
        if (col.gameObject.tag == "Destroy")
        {
            Destroy(this.gameObject);
        }
    }
}
