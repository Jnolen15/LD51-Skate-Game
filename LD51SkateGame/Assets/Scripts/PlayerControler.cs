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
    [SerializeField] private float gravity;
    public float curSpeed;
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
            curSpeed -= drag * Time.deltaTime;
        }
        if (curJump >= 0)
        {
            curJump -= gravity * Time.deltaTime;
        }

        // Jumping
        spriteObj.transform.localPosition = new Vector3(0, curJump, 0);
        if (curJump <= 0)
            grounded = true;
        else
            grounded = false;
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
                    ExecuteMove(-0.6f, 2);
                    break;
                case "dl":
                    Debug.Log("Nollie");
                    ExecuteMove(-0.6f, 2);
                    break;
                default:
                    Debug.Log("Fumbled");
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
            curSpeed += speedInc;
        if (curJump < topJump)
            curJump += jumpInc;

        // Make sure speed won't go negitive
        if (curSpeed <= 0)
            curSpeed = 0;
    }
}
