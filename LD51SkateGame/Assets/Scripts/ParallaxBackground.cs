using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private PlayerControler pc;
    private Transform camTrans;
    [SerializeField] private Vector2 parallaxMult;
    private float textureSizeX;
    private float defaultY;
    private float lastPlayerPosY;

    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControler>();
        camTrans = Camera.main.transform;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureSizeX = texture.width / sprite.pixelsPerUnit;
        defaultY = this.transform.localPosition.y;
    }

    void Update()
    {
        // Parallax movement
        var xmov = 0f;
        var ymov = 0f;
        if (pc.curSpeed > 0.1)
            xmov = -(pc.curSpeed * Time.deltaTime);
        ymov = pc.gameObject.transform.position.y - lastPlayerPosY;

        transform.position += new Vector3(xmov * parallaxMult.x, -ymov * parallaxMult.y, 0);
        lastPlayerPosY = pc.gameObject.transform.position.y;

        // Screen wrapping
        if (camTrans.position.x - transform.position.x >= textureSizeX)
        {
            float offsetX = (camTrans.position.x - transform.position.x) % textureSizeX;
            transform.position = new Vector3(camTrans.position.x + offsetX, transform.position.y);
        }

        // Make sure it dosn't drift down
        if (pc.curJump <= 0.1)
        {
            transform.position = new Vector3(transform.position.x, defaultY, 0);
        }
    }
}
