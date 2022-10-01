using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject spriteObj;

    [Header("Inputs")]
    [SerializeField] private float inputBuffer;
    [SerializeField] private float inputBufferTimer;
    [SerializeField] private bool inputHappening;
    [SerializeField]
    private List<char> inputs = new List<char>();

    [Header("Attributes")]
    [SerializeField] private bool grounded;
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
        
        // Input recording
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

        // Speed and Jump dampening
        if (curSpeed >= 0)
        {
            var adjustedDrag = (drag * (((topSpeed + 1) - curSpeed)/8));
            curSpeed -= adjustedDrag * Time.deltaTime;
        }
        if (curJump > 0.1)
        {
            curJump -= gravity * Time.deltaTime;
        } else
        {
            curJump = 0;
        }

        // Jumping
        if (jumpVel > 0)
        {
            curJump += jumpVel * Time.deltaTime;
            jumpVel -= gravity * Time.deltaTime;
        }

        spriteObj.transform.localPosition = new Vector3(0, curJump/2, 0);
        if (curJump <= 0.1)
        {
            grounded = true;
            jumpVel = 0;
        }
        else
        {
            grounded = false;
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
        if (grounded)
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
