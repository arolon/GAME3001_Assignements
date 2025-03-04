using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public float moveSpeed = 3f;
    private List<Tile> path = new List<Tile>();
    private bool isMoving = false;

    public void SetPath(List<Tile> newPath)
    {
        path = newPath;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && path.Count > 0 && !isMoving)
        {
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        isMoving = true;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 targetPos = new Vector3(path[i].x, path[i].y, 0);

            // Rotate to face direction
            if (i > 0) // Ignore first tile since there's no previous position
            {
                Vector3 direction = targetPos - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + 90);
            }

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        isMoving = false;
    }
}
