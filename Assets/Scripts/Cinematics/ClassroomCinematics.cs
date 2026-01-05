using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BotPatrol;

public class ClassroomCinematics : MonoBehaviour
{
    [SerializeField] private GameObject[] students;
    [SerializeField] private GameObject teacher;
    [SerializeField] private List<PatrolPoint> studentHelmetPoints = new List<PatrolPoint>();
    [SerializeField] private List<GameObject> map_equipments = new List<GameObject>();


    void Start()
    {
        StudentsEnterInRoom();
    }

    public void StudentsEnterInRoom()
    {
        foreach (GameObject student in students)
        {
            PathFollow patrol = student.GetComponent<PathFollow>();
            if (patrol)
            {
                patrol.GoToPoint(0);
            }
        }
    }


    public void TeacherGoToIsDesk()
    {
        PathFollow patrol = teacher.GetComponent<PathFollow>();
        if (patrol)
        {
            patrol.GoToPoint(0);
        }
    }

    private void TeacherAtDesk(int teacherPathPoint, int lastPP)
    {
        if (teacherPathPoint >= lastPP) {
            StudentTakeHelmet();
        }
    }

    private void StudentsSit(int StudentPathPoint, int lastPP)
    {
        if (StudentPathPoint >= lastPP) {
        }
    }

    private void StudentTakeHelmet()
    {
        BotPatrol patrol = students[0].GetComponent<BotPatrol>();
        patrol.SetPatrolPoints(studentHelmetPoints);
        patrol.StartPatrol();
        patrol.pathEvent.AddListener(TakeHelmet);
    }

    private void TakeHelmet(int StudentPathPoint, int lastPP)
    {
        if (StudentPathPoint == 3)
        {
            StartCoroutine(HideEquipment());
        }
    }

    private IEnumerator HideEquipment()
    {
        for (int i = 0; i < map_equipments.Count; i++)
        {
            yield return new WaitForSeconds(1);
            map_equipments[i].SetActive(false);
            students[0].GetComponent<RigItemsManager>().ShowItem(i);
        }
    }
}
