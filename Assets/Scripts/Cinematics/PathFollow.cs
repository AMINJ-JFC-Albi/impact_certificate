using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class PathFollow : MonoBehaviour
{
    [SerializeField, Header("Animations")] private Animator anim;
    [SerializeField] private string walkBlendTreeAnimName = "Run";
    [SerializeField] private UnityEvent onStartEvent;

    [System.Serializable]
    private class Point
    {
        public Transform[] pointTransforms;
        public bool sitTo, layTo;
        public UnityEvent OnPointReached;
    }
    [SerializeField] private List<Point> points;

    [System.Serializable]
    private class Loop
    {
        public int[] pointTransformsId;
        public int[] onPointReachedDelay;
    }
    [SerializeField] private List<Loop> loops;
    private NavMeshAgent agent;

    private bool isFollowing = false, sitTo = false, sitOnBed = false, goingToAPoint = false;
    private Point currentPoint = null;
    private int actualPointIndex = 0, pointTransformIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (anim == null) {anim = GetComponent<Animator>();};
        onStartEvent.Invoke();
    }

    public void PlayLoop(int index)
    {
        goingToAPoint = true;
        StartCoroutine(PlayloopCoroutine(index, 0));
    }

    private IEnumerator PlayloopCoroutine(int loopIndex, int pointIndex)
    {
        int[] points = loops[loopIndex].pointTransformsId;
        GoToPoint(points[pointIndex]);
        while (goingToAPoint)
        {
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(loops[loopIndex].onPointReachedDelay[pointIndex]);
        StartCoroutine(PlayloopCoroutine(loopIndex, pointIndex+1 > points.Length-1 ? 0 : pointIndex+1));
    }

    public void GoToPoint(int index)
    {
        goingToAPoint = true;
        actualPointIndex = index;
        Point point = points[index];
        // choisis une destination aléatoire si le point en possède plusieurs.
        pointTransformIndex = Random.Range(0, point.pointTransforms.Length-1);
        sitTo = point.sitTo;
        sitOnBed = point.layTo;
        Follow(point);
    }

    private void Follow(Point point)
    {
        if (agent != null)
        {
            currentPoint = point;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            if (anim != null) {
                string name = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.ToLower();
                if (name.Contains("sit")) {anim.SetTrigger("Walk");};
                anim.SetFloat(walkBlendTreeAnimName, 1f);
            }
            StartCoroutine(Stand());
        }
    }

    private IEnumerator Stand()
    {
        // Si le patient est assis, attend qu'il se lève avant de bouger.
        string name = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.ToLower(); // 0 = layer index
        while (!name.Contains("walk"))
        {
            name = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.ToLower();
            yield return new WaitForEndOfFrame();
        }
        // Definition de la destination.
        Vector3 destination = currentPoint.pointTransforms[pointTransformIndex].position;
        agent.enabled = true;
        agent.SetDestination(destination);
        isFollowing = true;
    }

    void Update()
    {
        if (isFollowing && (!agent.pathPending) && (agent.remainingDistance - 0.01f <= agent.stoppingDistance))
        {
            isFollowing = false;
            points[actualPointIndex].OnPointReached.Invoke();
            if (anim != null) anim.SetFloat(walkBlendTreeAnimName, 0f);
            if (sitTo) { sitTo = false; anim.SetTrigger("Sit"); }
            if (sitOnBed) { sitOnBed = false; anim.SetTrigger("LieOn"); }
            StartCoroutine(RotateAndPos(0.5f));
            goingToAPoint = false;
        }
    }

    private IEnumerator RotateAndPos(float duration)
    {
        agent.enabled = false;
        transform.GetPositionAndRotation(out Vector3 startPosition, out Quaternion startRotation);
        float timeElapsed = 0f;

        Vector3 destination = currentPoint.pointTransforms[pointTransformIndex].position;
        Vector3 angularAngle = currentPoint.pointTransforms[pointTransformIndex].eulerAngles;

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, destination, timeElapsed / duration);
            transform.rotation = Quaternion.Slerp(startRotation, Quaternion.Euler(angularAngle), timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.SetPositionAndRotation(destination, Quaternion.Euler(angularAngle));
    }
}
