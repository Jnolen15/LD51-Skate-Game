using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private PlayerControler pc;
    private Transform camTrans;
    [SerializeField] private Vector2 parallaxMult;
    private float textureSizeX;
    
    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControler>();
        camTrans = Camera.main.transform;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureSizeX = texture.width / sprite.pixelsPerUnit;
    }

    void Update()
    {
        var xmov = 0f;
        var ymov = 0f;
        if (pc.curSpeed > 0.1)
            xmov = -(pc.curSpeed * Time.deltaTime);
        //if (pc.curJump < -0.1 || pc.curJump > 0.1) // Fix jumping to be velocity or make this adjusted based on 1 instead of 0
        //    ymov = pc.curJump * Time.deltaTime;

        transform.position += new Vector3(xmov * parallaxMult.x, ymov * parallaxMult.y, 0);

        if(camTrans.position.x - transform.position.x >= textureSizeX)
        {
            float offsetX = (camTrans.position.x - transform.position.x) % textureSizeX;
            transform.position = new Vector3(camTrans.position.x + offsetX, transform.position.y);
        }
    }
}
