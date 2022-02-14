using MainScriptt;
using UnityEngine;

public class Enemy1 : MainScript
{
    public Vector2 coord;
    private GameObject player;

    public override void Start()
    {
        base.Start();
        setCoord(coord);
        player = GameObject.FindGameObjectWithTag("Player");

        BoxCollider2D playerCollider = player.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(boxCollider2D, playerCollider);
    }
}