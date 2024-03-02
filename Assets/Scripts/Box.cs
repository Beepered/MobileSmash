using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] CanvasScript cs;
    [SerializeField] Rigidbody2D rb;
    Hitbox h;

    Vector2 origPos;
    bool shaking = false, isHit = false;
    public float knockbackMult = 1.0f;

    private void Update()
    {
        if (shaking)
        {
            Vector2 startPos;
            startPos.x = origPos.x + Random.Range(-0.1f, 0.1f);
            startPos.y = origPos.y + Random.Range(-0.1f, 0.1f);
            transform.position = startPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Hitbox" && !isHit)
        {
            isHit = true;
            h = collision.gameObject.GetComponent<Hitbox>();
            StartCoroutine(Hit());
            cs.StartCoroutine(cs.BoxHit());
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Hitbox")
        {
            isHit = false;
        }
    }

    IEnumerator Hit()
    {
        knockbackMult += 0.2f;
        Vector2 knockback = h.knockback * knockbackMult;
        knockback.x *= h.gameObject.transform.parent.localScale.x;
        origPos = transform.position;
        rb.velocity = Vector2.zero;
        shaking = true;
        transform.localScale = new Vector3(-h.gameObject.transform.parent.localScale.x, 1, 1);
        yield return new WaitForSeconds(h.hitstun / 60);
        shaking = false;
        transform.position = origPos;
        rb.velocity = new Vector2(knockback.x, knockback.y);
    }
}
