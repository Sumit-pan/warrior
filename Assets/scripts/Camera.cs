using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    private Transform currentTarget;

    void Start()
    {
        FindPlayer();
    }

    void LateUpdate()
    {
        if (player == null)
        {
            FindPlayer(); // in case player respawns
            return;
        }

        transform.position = new Vector3(
            player.position.x + offset.x,
            player.position.y + offset.y,
            player.position.z + offset.z
        );
    }

    void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }
}
