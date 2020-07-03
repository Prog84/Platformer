using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private GameObject leftBorder;
    [SerializeField] private GameObject rightBorder;
    [SerializeField] private Rigidbody2D rigidbodyEnemy;
    [SerializeField] private bool isRightDirection;
    [SerializeField] private float speed;
    [SerializeField] private GroundDetection groundDetection;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CollisionDamage collisionDamage;
   private void Update()
    {
        if (groundDetection.isGrounded)
        {
            if (transform.position.x > rightBorder.transform.position.x
                || collisionDamage.Direction < 0)
                isRightDirection = false; 
            else if (transform.position.x < leftBorder.transform.position.x
                || collisionDamage.Direction > 0)
                isRightDirection = true;
            rigidbodyEnemy.velocity = isRightDirection ? Vector2.right : Vector2.left;
            rigidbodyEnemy.velocity *= speed;
        }
            /*if (isRightDirection && groundDetection.isGrounded)
            {
                rigidbodyEnemy.velocity = Vector2.right * speed;
                if (transform.position.x > rightBorder.transform.position.x)
                    isRightDirection = !isRightDirection;
            }
            else if (!isRightDirection && groundDetection.isGrounded)
            {
                rigidbodyEnemy.velocity = Vector2.left * speed;
                if (transform.position.x < leftBorder.transform.position.x)
                    isRightDirection = !isRightDirection;
            }*/
            if (rigidbodyEnemy.velocity.x > 0)
            spriteRenderer.flipX = true;
        if (rigidbodyEnemy.velocity.x < 0)
            spriteRenderer.flipX = false;
    }
}
