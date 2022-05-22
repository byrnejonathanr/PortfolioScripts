using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer self;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ParticleSystem bloodEffect;
    [SerializeField] private SpriteRenderer slash;
    [SerializeField] private GameObject arrow;
    [SerializeField] private bool bowUser;
    [SerializeField] private int maxHealth = 1;
    private int currentHealth;
    private WalkBehaviour walkBehaviour;
    private HitBehaviour hitBehaviour;
    private DeathBehaviour deathBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        animator.SetInteger("Health", maxHealth);
        walkBehaviour = animator.GetBehaviour<WalkBehaviour>();
        hitBehaviour = animator.GetBehaviour<HitBehaviour>();
        deathBehaviour = animator.GetBehaviour<DeathBehaviour>();
        player = GameObject.FindGameObjectWithTag("Player");
        walkBehaviour.self = self;
        walkBehaviour.player = player.transform;
        walkBehaviour.rb = rb;
        hitBehaviour.self = self;
        hitBehaviour.bloodEffect = bloodEffect;
        deathBehaviour.self = self;
        deathBehaviour.bloodEffect = bloodEffect;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetInteger("Health", currentHealth);
        animator.SetTrigger("Hit");
    }

    public void Attack()
    {
        if (!bowUser)
        {
            Vector2 slashSpawnLocation = new Vector2(rb.position.x + (self.flipX ? 0.5f : -0.5f), rb.position.y);
            slash.gameObject.SetActive(true);
            slash.gameObject.transform.position = slashSpawnLocation;
            slash.flipX = !self.flipX;
        }
        else
        {
            GameObject arrowInstance = Instantiate(arrow, self.transform.position, Quaternion.identity);
            arrowInstance.GetComponent<Rigidbody2D>().AddForce((self.flipX ? Vector2.right : Vector2.left) * 10.0f, ForceMode2D.Impulse);
            arrowInstance.GetComponent<DamageSource>().targetFaction = "Player";
        }
    }

    public void EndAttack()
    {
        if (!bowUser)
        {
            slash.gameObject.SetActive(false);
        }
    }
}
