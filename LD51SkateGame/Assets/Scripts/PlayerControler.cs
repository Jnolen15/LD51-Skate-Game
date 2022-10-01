using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject spriteObj;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float yOffset;

    [Header("Inputs")]
    [SerializeField] private float inputBuffer;
    [SerializeField] private float inputBufferTimer;
    [SerializeField] private bool inputHappening;
    [SerializeField]
    private List<char> inputs = new List<char>();

    [Header("Attributes")]
    [SerializeField] private bool grounded;
    [SerializeField] private bool hitRail;
    public bool onRail;
    [SerializeField] private bool hitObs;
    [SerializeField] private float hitbuffer;
    [SerializeField] private float hitTimer;
    [SerializeField] private float topSpeed;
    [SerializeField] private float drag;
    [SerializeField] private float topJump;
    public float gravity;
    public float curSpeed;
    public float jumpVel;
    public float curJump;

    void Update()
    {
        // Input buffer timer
        if (inputHappening)
        {
            // NOTE if going to use any time warping effect cahnge to unscaleddeltatime
            if (inputBufferTimer < inputBuffer) inputBufferTimer += Time.deltaTime;
            else
            {
                DecideMove();
                inputBufferTimer = 0;
            }
        }

        // If hit an obsticle stop whatever was happening and give chance to boost
        if (hitObs)
        {
            inputHappening = false;
            inputBufferTimer = 0;
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Debug.Log("Boosted");
                hitTimer = 0;
                hitObs = false;
                ExecuteMove(2, 13);
            }
        }
        // If hit a rail stop whatever was happening and give chance grind
        else if (hitRail && !onRail)
        {
            inputHappening = false;
            inputBufferTimer = 0;
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Debug.Log("Grind!");
                onRail = true;
                jumpVel = 0;
            }
        }
        // Input recording
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                inputHappening = true;
                inputs.Add('u');
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                inputHappening = true;
                inputs.Add('d');
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                inputHappening = true;
                inputs.Add('l');
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                inputHappening = true;
                inputs.Add('r');
            }
        }

        // Hit obsticle
        if (hitObs)
        {
            sprite.color = Color.red;
            if (hitTimer < hitbuffer) hitTimer += Time.deltaTime;
            else
            {
                hitTimer = 0;
                hitObs = false;
                ExecuteMove(-10, 0);
            }
        } else if (sprite.color == Color.red)
        {
            sprite.color = Color.white;
        }

        // Speed
        if (curSpeed >= 0)
        {
            var adjustedDrag = (drag * (((topSpeed + 1) - curSpeed)/8));
            curSpeed -= adjustedDrag * Time.deltaTime;
        }

        // Jumping
        if (!onRail)
        {
            if (curJump > 0.1)
            {
                curJump -= gravity * Time.deltaTime;
            }
            else
            {
                curJump = 0;
            }

            if (jumpVel > 0)
            {
                curJump += jumpVel * Time.deltaTime;
                jumpVel -= gravity * Time.deltaTime;
            }

            this.transform.position = new Vector3(this.transform.position.x, yOffset + (curJump / 2), 0);
        }else
        {
            Debug.Log("GRIND");
            ExecuteMove(0.2f, 0);
        }

        // Grounded test
        if (curJump <= 0.1)
        {
            grounded = true;
            jumpVel = 0;
        }
        else if (onRail)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Obst")
        {
            hitObs = true;
            Debug.Log("Hit obst");
        }
        else if (col.gameObject.tag == "Rail")
        {
            hitRail = true;
            Debug.Log("Hit Rail");
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Rail")
        {
            hitRail = false;
            onRail = false;
            Debug.Log("Left Rail");
        }
    }

    private void DecideMove()
    {
        inputHappening = false;

        // Compile input string
        var movelist = "";
        foreach(char input in inputs)
        {
            movelist += input;
        }

        // Determine moves
        if (onRail)
        {
            switch (movelist)
            {
                case "dr":
                    Debug.Log("Ollie");
                    hitRail = false;
                    onRail = false;
                    Debug.Log("Left Rail");
                    ExecuteMove(0, 13);
                    break;
                case "dl":
                    Debug.Log("Nollie");
                    hitRail = false;
                    onRail = false;
                    Debug.Log("Left Rail");
                    ExecuteMove(0, 13);
                    break;
                default:
                    Debug.Log("Fumbled");
                    hitRail = false;
                    onRail = false;
                    Debug.Log("Left Rail");
                    ExecuteMove(-5, 0);
                    break;
            }
        }
        else if (grounded)
        {
            switch (movelist)
            {
                case "d":
                    Debug.Log("Pushed");
                    ExecuteMove(3, 0);
                    break;
                case "dr":
                    Debug.Log("Ollie");
                    ExecuteMove(0, 15);
                    break;
                case "dl":
                    Debug.Log("Nollie");
                    ExecuteMove(0, 15);
                    break;
                default:
                    Debug.Log("Fumbled");
                    ExecuteMove(-5, 0);
                    break;
            }
        } else
        {
            switch (movelist)
            {
                case "rl":
                    Debug.Log("Spin");
                    ExecuteMove(0.2f, 0);
                    break;
                case "lr":
                    Debug.Log("Inverse Spin");
                    ExecuteMove(0.2f, 0);
                    break;
                default:
                    Debug.Log("Fumbled");
                    ExecuteMove(-3, 0);
                    break;
            }
        }

        // Clear inputs
        inputs.Clear();
    }

    private void ExecuteMove(float speedInc, float jumpInc)
    {
        // adjustspeed and jump
        if (curSpeed < topSpeed)
        {
            //Debug.Log("Boosting speed by " + (speedInc * (1 + curSpeed / 10)));
            if(speedInc > 0)
            {
                // This makes it so you gain more speed when pushing and already moving fast
                curSpeed += (speedInc * (1 + curSpeed / 10));
                if (curSpeed > topSpeed) curSpeed = topSpeed;
            } else
            {
                curSpeed += speedInc;
            }
        }
        if (curJump < topJump && jumpInc > 0.1f)
        {
            //Debug.Log("Boosting jump by " + (jumpInc * (1 + curSpeed / 100)));
            jumpVel += (jumpInc * (1 + curSpeed / 100));
            curJump += 0.2f;
        }

        // Make sure speed won't go negitive
        if (curSpeed <= 0)
            curSpeed = 0;
    }
}
