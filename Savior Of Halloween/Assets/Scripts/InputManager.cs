using UnityEngine;

public class InputManager : MonoBehaviour {

    public Rigidbody2D rb;
    public Jeremy jeremy;

    private bool jump = false;
    private bool jumpCancel = false;
    private float move = 0f;

    public void Update() {
        if (Input.GetKey("d"))
            move = 1;
        else if (Input.GetKey("a"))
            move = -1;
        else
            move = 0;
        if (Input.GetKeyDown("space"))
            jump = true;
            
        if (Input.GetKeyUp("space"))
            jumpCancel = true;
    }

    void FixedUpdate() {
        jeremy.Move(move, jump, jumpCancel);
        jump = false;
        jumpCancel = false;
    }
}