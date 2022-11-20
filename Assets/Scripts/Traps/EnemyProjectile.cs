using UnityEngine;

public class EnemyProjectile : EnemyDamage // will damage player every time it touches
{
    [SerializeField] private float speed;
    [SerializeField] private float resetTime;
    private float lifetime;
    private Animator anim;
    private BoxCollider2D coll;

    private bool hit;

    public void ActivateProjectile()
    {
        hit = false;
        lifetime = 0;
        gameObject.SetActive(true);
        coll.enabled = true;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (hit) return;
        
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if(lifetime > resetTime)
            gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        base.OnTriggerEnter2D(collision);// Execute logic from parent sript first 
        coll.enabled = false;

        if (anim != null)
            anim.SetTrigger("explode");// When the object is a fireball explode it
        else
            gameObject.SetActive(false);// When this hits any objects deactivate arrows
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
