using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
  [SerializeField] private float speed; 
  [SerializeField] private float jumpPower; 
  private Rigidbody2D body;
  private Animator anim;
  private BoxCollider2D boxCollider; 
  [SerializeField] private LayerMask groundLayer;
  [SerializeField] private LayerMask wallLayer;
  private float wallJumpCooldown;
  private float horizontalInput;
  [SerializeField] private float counter = 0;

  private void Awake() {
    body = GetComponent<Rigidbody2D>();
    anim = GetComponent<Animator>();
    boxCollider = GetComponent<BoxCollider2D>();
  }

  private void Update() { 
    horizontalInput = Input.GetAxis("Horizontal");
    if (horizontalInput > 0.01f) {
        transform.localScale = Vector3.one;
    }
    if (horizontalInput < -0.01f) {
        transform.localScale = new Vector3(-1, 1, 1);
    }


    anim.SetBool("walk", horizontalInput != 0);
    anim.SetBool("grounded", isGrounded());

    if (wallJumpCooldown > 0.2f) {

        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        if(onWall() && !isGrounded()) {
            body.gravityScale = 0; 
            body.velocity = Vector2.zero;
        }
        else {
            body.gravityScale = 7;
        }
        if (Input.GetKey(KeyCode.Space)) {
            Jump();
        }
    }
    else {
        wallJumpCooldown += Time.deltaTime;
    }
  }

  private void Jump() {
      if (isGrounded()) {
        body.velocity = new Vector2(body.velocity.x, jumpPower);
        anim.SetTrigger("jump");
      }
    else if (onWall() && !isGrounded()) {
        if (horizontalInput == 0) {
            body.velocity = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else {
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
        }
        wallJumpCooldown = 0; 
    }
  }

  private void OnCollisionEnter2D(Collision2D collision) {

  }

  private bool isGrounded() {
      RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
      return raycastHit.collider != null;
  }

  private bool onWall() {
      RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.01f, wallLayer);
      return raycastHit.collider != null;
  }
}
