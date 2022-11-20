using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;//How much time the player can hang in the air before jumping
    private float coyoteCounter;//How much time passed since the player ran off the edge

    [Header("Multiple Jump")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpX; //Horizontal wall jump force
    [SerializeField] private float wallJumpY; //Vertical wall jump force

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("SFX")]
    [SerializeField] private AudioClip jumpSound;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxcollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    private void Awake()
    {
        //grab references for animator and rigidbody from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxcollider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        //flip transform of player when moving right or left
        if(horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1,1,1);

        //Set animator parameter
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded" , isGrounded());

        /*
        //Old wall jump logic
        if(wallJumpCooldown > 0.2f)
        {

            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (onWall() && !isGrounded())
            {
                body.gravityScale = 1;
                body.velocity = Vector2.zero;
            }
            else
                body.gravityScale = 7;

            if (Input.GetKey(KeyCode.Space))
            {
                Jump();

                if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
                    SoundManager.instance.PlaySound(jumpSound);
            }
        }
        else
            wallJumpCooldown += Time.deltaTime;*/

        //New Jump
        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        //Adjustible Jump Height
        if (Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
            body.velocity = new Vector2(body.velocity.x,body.velocity.y/2);

        if(onWall())
        {
            body.gravityScale = 0;
            body.velocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 7;
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if(isGrounded())
            {
                coyoteCounter = coyoteTime; //Reset coyote counter when on the ground  
                jumpCounter = extraJumps; //Reset jumpCounter to extraJump value
            }
            else
            {
                coyoteCounter -= Time.deltaTime; //Start decreasing coyote when not on the ground
            }
        }
    }
    private void Jump()
    {
        if (coyoteCounter <= 0 && !onWall() && jumpCounter <= 0) return;
        //If coyote counter is 0 or less and not on the wall and don't have extra jumps then don't do anything 

        SoundManager.instance.PlaySound(jumpSound);
        // old code
        /*if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            //anim.SetTrigger("jump");
            SoundManager.instance.PlaySound(jumpSound);
        }
        else if(onWall() && !isGrounded())
        {
            if(horizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            wallJumpCooldown = 0;
        }*/

        if(onWall())
            WallJump();
        else
        {
            if (isGrounded())
            {
                body.velocity = new Vector2(body.velocity.x, jumpPower);
            }
            else
            {
                //If not on the groundand coyote counter bigger than 0 then do a normal jump
                if (coyoteCounter > 0)
                    body.velocity = new Vector2(body.velocity.x,jumpPower);
                else
                {
                   if(jumpCounter > 0) //if we have extra jump then jump and decrease the jumpCounter
                   {
                        body.velocity = new Vector2(body.velocity.x, jumpPower);
                        jumpCounter--;
                   }
                }

            }

            //Reset the coyote counter to 0 to avoid double jumps
            coyoteCounter = 0;
        }
    }

    private void WallJump()
    {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
        wallJumpCooldown = 0;
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxcollider.bounds.center , boxcollider.bounds.size , 0 , Vector2.down , 0.1f , groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxcollider.bounds.center, boxcollider.bounds.size, 0, new Vector2(transform.localScale.x,0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}
