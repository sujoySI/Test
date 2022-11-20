using UnityEngine;

public class Spikehead : EnemyDamage
{
    [Header("Spikehead Attributes")]
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float checkDelay;
    [SerializeField] private LayerMask playerLayer;
    private Vector3[] directions = new Vector3[4];
    private float checkTimer;
    private Vector3 destination;

    [Header("SFX")]
    [SerializeField] private AudioClip impactSound;

    private void OnEnable()
    {
        Stop();
    }

    private bool attacking;
    private void Update()
    {
        //move spikehead only if attacing
        if(attacking)
            transform.Translate(destination * Time.deltaTime * speed);
        else
        {
            checkTimer += Time.deltaTime;
            if(checkTimer > checkDelay)
            {
                CheckForPlayer();
            }
        }
    }

    private void CheckForPlayer()
    {
        CalculateDirection();

        //check if spike head sees player
        for(int i = 0; i < directions.Length; i++)
        {
            Debug.DrawRay(transform.position, directions[i] ,Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);

            if(hit.collider != null && !attacking)
            {
                attacking = true;
                destination = directions[i];
                checkTimer = 0;
            }
        }
    }

    private void CalculateDirection()
    {
        directions[0] = transform.right * range;//Right direction
        directions[1] = -transform.right * range;//Left direction
        directions[2] = transform.up * range;//Up direction
        directions[3] = -transform.up * range;//Down direction
    }

    private void Stop()
    {
        destination =transform.position;//Set destination as current position so it doesn't move
        attacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SoundManager.instance.PlaySound(impactSound);
        base.OnTriggerEnter2D(collision);
        //Stop spikehead once it hits something
        Stop();
    }
}
