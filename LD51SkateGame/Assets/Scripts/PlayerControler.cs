using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject spriteObj;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private Score scoreKeeper;
    [SerializeField] private GameObject splat;
    [SerializeField] private float yOffset;

    [Header("Inputs")]
    [SerializeField] private float inputBuffer;
    [SerializeField] private float inputBufferTimer;
    [SerializeField] private bool inputHappening;
    [SerializeField]
    private List<char> inputs = new List<char>();

    [Header("Attributes")]
    public bool grounded;
    public bool onRail;
    [SerializeField] private bool dead;
    [SerializeField] private bool hitRail;
    [SerializeField] private bool hitObs;
    [SerializeField] private bool boostInput;
    [SerializeField] private float boostbuffer;
    [SerializeField] private float hitbuffer;
    [SerializeField] private float hitTimer;
    [SerializeField] private float grindCD;
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

        // Buffer to allow for easier boosting off obsticles
        if (boostbuffer > 0) boostbuffer -= Time.deltaTime;
        else
        {
            boostInput = false;
            boostbuffer = 0;
        }

        // Boost input buffer
        if (!grounded && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)))
        {
            boostbuffer = 0.4f;
            boostInput = true;
        }
        // If hit an obsticle stop whatever was happening and give chance to boost
        if (hitObs)
        {
            //inputHappening = false;
            //inputBufferTimer = 0;
            if (boostInput)
            {
                boostInput = false;
                boostbuffer = 0;
                Debug.Log("Boosted");
                hitTimer = 0;
                hitObs = false;
                jumpVel = 0;
                ExecuteMove(2, 13, 50, "Boosted");
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Boosted");
                hitTimer = 0;
                hitObs = false;
                jumpVel = 0;
                ExecuteMove(2, 13, 50, "Boosted");
            }
        }
        // If hit a rail stop whatever was happening and give chance grind
        else if (hitRail && !onRail)
        {
            inputHappening = false;
            inputBufferTimer = 0;
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Grind!");
                onRail = true;
                jumpVel = 0;
                grindCD = 0;
            }
        }
        // Input recording
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                inputHappening = true;
                inputs.Add('u');
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                inputHappening = true;
                inputs.Add('d');
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                inputHappening = true;
                inputs.Add('l');
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
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
                animator.SetTrigger("Fumble");
                ExecuteMove(-10, 0, 0, "Fumble");
            }
        } else if (sprite.color == Color.red)
        {
            sprite.color = Color.white;
        }

        // Speed
        if (curSpeed > 0)
        {
            var adjustedDrag = (drag * (((topSpeed + 1) - curSpeed)/8));
            curSpeed -= adjustedDrag * Time.deltaTime;
        } else
        {
            curSpeed = 0;
        }

        // Update animation based on speed
        if (curSpeed >= 5)
        {
            animator.SetBool("GoingFast", true);
        }
        else
        {
            animator.SetBool("GoingFast", false);
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
            ExecuteMove(0.2f, 0, 0, "Grind Boost");
            if (grindCD > 0) grindCD -= Time.deltaTime;
            else
            {
                Debug.Log("GRIND");
                ExecuteMove(0, 0, 25, "Goo Grind");
                grindCD = 0.5f;
            }
        }

        // Grounded test
        if (curJump <= 0.1)
        {
            animator.SetBool("InAir", false);
            animator.SetBool("InGrind", false);
            grounded = true;
            jumpVel = 0;
        }
        else if (onRail)
        {
            animator.SetBool("InGrind", true);
            animator.SetBool("InAir", false);
            grounded = true;
        }
        else
        {
            animator.SetBool("InAir", true);
            animator.SetBool("InGrind", false);
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
        // On Rail
        if (onRail)
        {
            switch (movelist)
            {
                case "d":
                    hitRail = false;
                    onRail = false;
                    ExecuteMove(2, 0, 0, "Left Rail");
                    break;
                case "dr":
                    hitRail = false;
                    onRail = false;
                    animator.SetTrigger("Nollie");
                    ExecuteMove(0, 13, 150, "Un-Floop");
                    break;
                case "dl":
                    hitRail = false;
                    onRail = false;
                    animator.SetTrigger("Ollie");
                    ExecuteMove(0, 13, 150, "Floop");
                    break;
                default:
                    hitRail = false;
                    animator.SetTrigger("Fumble");
                    ExecuteMove(-5, 0, -10, "Fumble");
                    break;
            }
        }
        // On Ground
        else if (grounded)
        {
            switch (movelist)
            {
                case "d":
                    animator.SetTrigger("Push");
                    ExecuteMove(3, 0, 0, "Push");
                    break;
                case "dr":
                    animator.SetTrigger("Nollie");
                    ExecuteMove(0, 15, 100, "Un-Floop");
                    break;
                case "dl":
                    animator.SetTrigger("Ollie");
                    ExecuteMove(0, 15, 100, "Floop");
                    break;
                default:
                    animator.SetTrigger("Fumble");
                    ExecuteMove(-5, 0, -10, "Fumble");
                    break;
            }
        } 
        // In air
        else
        {
            switch (movelist)
            {

                case "d":
                    break;
                case "rl":
                    animator.SetTrigger("BoardSpin");
                    ExecuteMove(0.2f, 0, 250, "Slime Spin");
                    break;
                case "lr":
                    animator.SetTrigger("BoardSpin");
                    ExecuteMove(0.2f, 0, 250, "Slime Spin");
                    break;
                case "du":
                    animator.SetTrigger("BoardFlip");
                    ExecuteMove(0.2f, 0, 250, "Freak Flip");
                    break;
                case "ud":
                    animator.SetTrigger("BoardFlip");
                    ExecuteMove(0.2f, 0, 250, "Freak Flip");
                    break;
                default:
                    animator.SetTrigger("Fumble");
                    ExecuteMove(-3, 0, -10, "Fumble");
                    break;
            }
        }

        // Clear inputs
        inputs.Clear();
    }

    private void ExecuteMove(float speedInc, float jumpInc, int numPoints, string moveName)
    {
        //Debug.Log(moveName);

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

        // Send to scoreKeeper
        if(numPoints != 0)
            scoreKeeper.AddScore(numPoints, moveName);

        // Make sure speed won't go negitive
        if (curSpeed <= 0)
            curSpeed = 0;
    }

    public void Explode()
    {
        if (!dead)
        {
            dead = true;
            Instantiate(splat, transform.position, transform.rotation);
            curSpeed = 0;
            gravity = 0;
            jumpVel = 0;
            this.gameObject.SetActive(false);
        }

    }
}
