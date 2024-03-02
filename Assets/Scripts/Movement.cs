using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Animator anim;

    [Header("Movement")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float speed, jumpForce, drag;

    [SerializeField] BoxCollider2D groundCheck;
    [SerializeField] LayerMask groundLayer;

    [Header("Joystick")]
    [SerializeField] Joystick joystick;
    [SerializeField] GameObject joystick_sprite;
    float lastTapTime, holdTime;
    [SerializeField] float tapThreshold, holdThreshold;
    [SerializeField] float swipeLength;

    bool input_down = false;
    bool crouching, moving;
    Vector2 startPos, endPos;
    
    void Update()
    {
        anim.SetBool("crouch", crouching);
        anim.SetBool("aerial", !isGrounded());
        anim.SetFloat("x vel", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("y vel", rb.velocity.y);
        MouseControls();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(rb.velocity.x * drag, rb.velocity.y);
    }
    /*
    void TouchControls()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startPos = Input.GetTouch(0).position;
                if (Time.time - lastTapTime <= tapThreshold)
                {
                    lastTapTime = 0;
                    anim.SetBool("attack", true);
                }
                else
                {
                    lastTapTime = Time.time;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endPos = Input.GetTouch(0).position;
                if (Time.time - lastTapTime <= tapThreshold) //tilts
                {
                    Vector2 delta = endPos - startPos;
                    if (isGrounded())
                    {
                        if (Mathf.Abs(delta.x) > swipeLength)
                        {
                            anim.SetBool("attack", true);
                            anim.Play("F-tilt");
                            
                            Debug.Log(delta.x);
                            transform.localScale = new Vector3(Mathf.Clamp(delta.x, -1, 1), 1, 1);
                            
                        }
                        else if (endPos.y < startPos.y - 40) //d-tilt
                        {
                            crouching = true;
                            anim.SetBool("attack", true);
                            anim.Play("D-tilt");
                        }
                        else if (endPos.y > startPos.y + 40) //u-tilt
                        {
                            anim.SetBool("attack", true);
                            anim.Play("U-tilt");
                        }
                    }
                }
                if(Time.time - lastTapTime >= chargeThreshold) //charge attacks
                {
                    Vector2 delta = endPos - startPos;
                    if (isGrounded())
                    {
                        if (Mathf.Abs(delta.x) > swipeLength)
                        {
                            Debug.Log("F-smash");
                            
                            anim.SetBool("attack", true);
                            anim.Play("F-smash");
                            
                        }
                        else if (endPos.y < startPos.y - 40) //d-tilt
                        {
                            Debug.Log("D-smash");
                            
                            anim.SetBool("attack", true);
                            anim.Play("D-smash");
                            
                        }
                        else if (endPos.y > startPos.y + 40) //u-tilt
                        {
                            Debug.Log("U-smash");
                            
                            anim.SetBool("attack", true);
                            anim.Play("U-smash");
                            
                        }
                    }
                }
                chargeTime = 0;
                joystick_sprite.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (touch.phase != TouchPhase.Ended)
            {
                if (Time.time - lastTapTime >= holdThreshold)
                {
                    Vector2 delta = Input.GetTouch(0).position - startPos;
                    if(Mathf.Abs(delta.x) <= startPos.x + 40 && Mathf.Abs(delta.y) <= startPos.y + 40)
                    {
                        if(joystick_sprite.transform.localScale.x < 2)
                        {
                            chargeTime += Time.deltaTime;
                            joystick_sprite.transform.localScale *= 1.005f;
                        }
                        
                    }
                }
            }
        }
    }*/

    public void MouseControls()
    {
        if (Input.GetMouseButtonDown(0))
        {
            input_down = true;
            startPos = Input.mousePosition;
            if (Time.time - lastTapTime <= tapThreshold && !anim.GetBool("attack")) //double tap jab
            {
                lastTapTime = 0;
                anim.SetBool("attack", true);
                if (!isGrounded())
                    anim.Play("N-air");
                else
                    anim.Play("Jab");
            }
            else
            {
                lastTapTime = Time.time;
                holdTime = 0;
            }
        }
        else if (Input.GetMouseButtonUp(0)) //attacks
        {
            input_down = false;
            endPos = Input.mousePosition;
            if (Time.time - lastTapTime <= tapThreshold && !anim.GetBool("attack")) // tilts and aerials
            {
                Vector2 delta = endPos - startPos;
                if (delta.x > swipeLength) // forward
                {
                    anim.SetBool("attack", true);
                    if (!isGrounded())
                    {
                        if (transform.localScale.x == 1)
                            anim.Play("F-air");
                        else
                            anim.Play("B-air");
                    }
                    else
                    {
                        anim.Play("F-tilt");
                        transform.localScale = new Vector3(Mathf.Clamp(delta.x, -1, 1), 1, 1);
                    }
                    
                }
                if (delta.x < -swipeLength) // back
                {
                    anim.SetBool("attack", true);
                    if (!isGrounded())
                    {
                        if (transform.localScale.x == 1)
                            anim.Play("B-air");
                        else
                            anim.Play("F-air");
                    }
                    else
                    {
                        anim.Play("F-tilt");
                        transform.localScale = new Vector3(Mathf.Clamp(delta.x, -1, 1), 1, 1);
                    }
                }
                else if (endPos.y < startPos.y - swipeLength) // down
                {
                    if (!isGrounded())
                        anim.Play("D-air");
                    else
                        anim.Play("D-tilt");
                    anim.SetBool("attack", true);
                }
                else if (endPos.y > startPos.y + swipeLength) // up
                {
                    if (!isGrounded())
                        anim.Play("U-air");
                    else
                        anim.Play("U-tilt");
                    anim.SetBool("attack", true);
                }
            }
            else if (anim.GetBool("charge ready") && isGrounded()) // CHARGE
            {
                endPos = Input.mousePosition;
                Vector2 delta = endPos - startPos;
                if (Mathf.Abs(delta.x) > swipeLength) // forward
                {
                    anim.Play("F-charge");
                    anim.SetBool("attack", true);
                    transform.localScale = new Vector3(Mathf.Clamp(delta.x, -1, 1), 1, 1);
                }
                else if (endPos.y < startPos.y - swipeLength) // down
                {
                    anim.Play("D-charge");
                    anim.SetBool("attack", true);
                }
                else if (endPos.y > startPos.y + swipeLength) // up
                {
                    anim.Play("U-charge");
                    anim.SetBool("attack", true);
                }
            }
            anim.SetBool("charge", false);
            crouching = false;
            joystick_sprite.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (input_down) // Movement and Charge
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 delta = mousePos - startPos;
            if(!anim.GetBool("charge") && joystick_sprite.transform.localScale.x < 2)
            {
                if(Time.time - lastTapTime >= tapThreshold)
                {
                    if (Mathf.Abs(delta.x) > 90 && !crouching && !anim.GetBool("attack")) // walk
                    {
                        rb.velocity = new Vector2(Mathf.Clamp(delta.x, -1, 1) * speed, rb.velocity.y);
                        if (isGrounded())
                        {
                            transform.localScale = new Vector3(Mathf.Clamp(delta.x, -1, 1), 1, 1);
                        }
                        joystick_sprite.transform.localScale = new Vector3(1, 1, 1);
                        moving = true; holdTime = 0;
                    }
                    else if (delta.y > 90 && isGrounded() && !anim.GetBool("attack")) // jump
                    {
                        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                        joystick_sprite.transform.localScale = new Vector3(1, 1, 1);
                        moving = true; holdTime = 0;
                    }
                    else if (delta.y < -90 && isGrounded() && !anim.GetBool("attack")) // crouch
                    {
                        joystick_sprite.transform.localScale = new Vector3(1, 1, 1);
                        crouching = true; holdTime = 0;
                    }
                    else
                    {
                        crouching = false;
                        moving = false;
                    }
                }
            }
            
            if(!crouching && isGrounded() && !moving && Mathf.Abs(delta.x) < 100 && Mathf.Abs(delta.y) < 100) // charge
            {
                holdTime += Time.deltaTime;
                if (holdTime >= holdThreshold)
                {
                    anim.SetBool("charge", true);
                    if (joystick_sprite.transform.localScale.x < 2)
                        joystick_sprite.transform.localScale *= 1.0015f;
                    else
                        anim.SetBool("charge ready", true);
                }
            }
        }
    }

    public void AddXVel(float force)
    {
        rb.velocity = new Vector2(rb.velocity.x + (transform.localScale.x * force), rb.velocity.y);
    }
    public void AddYVel(float force)
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + force);
    }

    public void FinishAttack()
    {
        anim.SetBool("attack", false);
        anim.SetBool("charge", false);
        anim.SetBool("charge ready", false);
    }

    

    public void FinishCrouch()
    {
        crouching = false;
    }

    bool isGrounded()
    {
        return Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundLayer).Length > 0;
    }
}
