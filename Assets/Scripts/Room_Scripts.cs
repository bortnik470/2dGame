using UnityEngine;

public class Room_Scripts : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private GameObject gate = null;
    [SerializeField] private Transform delete_point;
    [SerializeField] private GameObject[] nextObject;
    [SerializeField] private GameObject corridor;
    [SerializeField] private bool fromUnder;
    [SerializeField] private bool toRight;

    private Transform player_position;
    private bool isActive;

    private void Start()
    {
        isActive = false;
        player_position = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void CreateNewLoc()
    {
        if (!toRight)
        {
            int i = Random.Range(0, 2);

            Instantiate(corridor, new Vector3(delete_point.position.x, delete_point.position.y - 19f, 0f), Quaternion.Euler(Vector3.zero));
            Instantiate(nextObject[i], new Vector3(delete_point.position.x, delete_point.position.y, 0f), Quaternion.Euler(Vector3.zero));
        }
        else
        {
            int i = Random.Range(0, 2);

            Instantiate(corridor, new Vector3(delete_point.position.x - 17f, delete_point.position.y + 2f, 0f), Quaternion.Euler(Vector3.zero));
            Instantiate(nextObject[i], new Vector3(delete_point.position.x, delete_point.position.y, 0f), Quaternion.Euler(Vector3.zero));
        }
    }

    void Update()
    {
        if (player_position.position.y >= start.position.y && !isActive && fromUnder || player_position.position.x >= start.position.x && !isActive && !fromUnder)
        {
            CreateNewLoc();

            if (gate) { gate.SetActive(true); }

            isActive = true;
        }


        if (player_position.position.x >= delete_point.position.x && player_position.position.y > delete_point.position.y)
        {
            Destroy(gameObject);
        }
    }
}
