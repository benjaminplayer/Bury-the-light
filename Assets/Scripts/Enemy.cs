using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField]
    private BoxCollider2D hitBox;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private CharacterController cc;
    [SerializeField]
    private Animator playerAnim;
    [SerializeField]
    private SpriteRenderer healthBar;
    [SerializeField]
    private Sprite[] healthBarSprites;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            Debug.Log("Collided with a player");
            player = collision.gameObject;
            playerAnim = player.GetComponent<Animator>();
            cc = player.GetComponent<CharacterController>();
            cc.health -= 25;
            handleHealthBarChange(cc.health);
            player.transform.position = new Vector3(player.transform.position.x - 2f, player.transform.position.y, player.transform.position.z);
            cc.canMove = false;
            playerAnim.SetTrigger("isHurt");
        }
    }

    private void handleHealthBarChange(int health) 
    {
        if (health == 100)
            healthBar.sprite = healthBarSprites[0];
        else if (health < 100 && health >= 75)
            healthBar.sprite = healthBarSprites[1];
        else if (health < 75 && health >= 50)
            healthBar.sprite = healthBarSprites[2];
        else if (health < 50)
            healthBar.sprite = healthBarSprites[3];
    }

}
