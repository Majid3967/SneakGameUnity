using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public Transform pathHolder;
    public float speed = 5f;
    public float waitTime = .3f;
    public float turnSpeed = 90;
    public Light spotLight;
    public float viewDistance;
    float viewAngle;
    Transform player;
    public LayerMask viewMask;
    Color original;
    public GameObject playAgain;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        original = spotLight.color;
        viewAngle = spotLight.spotAngle;
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for(int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        StartCoroutine(FollowPath(waypoints));
    }

    // Update is called once per frame
    void Update()
    {
        if (canSee())
        {
            spotLight.color = Color.red;
            playAgain.SetActive(true);
        }
        else
        {
            spotLight.color = original;
        }
    }

    bool canSee()
    {
        if(Vector3.Distance(transform.position,player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenPlayerAndGuard = Vector3.Angle(transform.forward, dirToPlayer);
            if(angleBetweenPlayerAndGuard < viewAngle / 2)
            {
                if (!Physics.Linecast(transform.forward, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }
    void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach(Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.5f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    IEnumerator FollowPath(Vector3[] way)
    {
        transform.position = way[0];

        int targetIndex = 1;
        Vector3 targetWaypoint = way[targetIndex];
        transform.LookAt(targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetIndex = (targetIndex + 1) % way.Length;
                targetWaypoint = way[targetIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(FaceTurn(targetWaypoint));
            }
            yield return null;
        }
    }
    IEnumerator FaceTurn(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }
}
