using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid;
using static BotPatrol;

public class ClassroomCinematics : MonoBehaviour
{
    [SerializeField] private GameObject[] students;
    [SerializeField] private GameObject teacher;
    [SerializeField] private List<PatrolPoint> studentHelmetPoints = new List<PatrolPoint>();
    [SerializeField] private List<GameObject> map_equipments = new List<GameObject>();
    [SerializeField] private GameObject CrossGamePanel;

    private (bool, bool) dialoguesListen = (false, false);

    public void StudentsEnterInRoom()
    {
        Destroy(teacher.GetComponent<ToolTipObject>());
        Destroy(teacher.GetComponent<Outline>());
        Destroy(teacher.GetComponent<DialogueTrigger>());
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

    public void TakeHelmet()
    {
        StartCoroutine(HideEquipment());
    }

    private IEnumerator HideEquipment()
    {
        for (int i = 0; i < map_equipments.Count; i++)
        {
            yield return new WaitForSeconds(1);
            map_equipments[i].SetActive(false);
            students[0].GetComponent<RigItemsManager>().ShowItem(i);
        }
        students[0].GetComponent<PathFollow>().GoToPoint(2);
    }

    public void ActiveGroupsInteractions()
    {
        foreach (GameObject student in students)
        {
            if (student.TryGetComponent(out ToolTipObject tooltip))
                tooltip.enabled = true;

            if (student.TryGetComponent(out DialogueTrigger dialogue))
                dialogue.enabled = true;

            if (student.TryGetComponent(out Outline outline))
                outline.enabled = true;
        }
        students[0].transform.GetChild(0).GetComponent<Animator>().SetTrigger("Vr");
        students[1].GetComponent<CharacterAnimator>().enabled = true;
    }

    public void AssignDialogListen(int id)
    {
        if (id == 0) { dialoguesListen.Item1 = true; }
        if (id == 1) { dialoguesListen.Item2 = true; }
        if (dialoguesListen == (true, true))
        {
            StartCoroutine(StartCrosswordGameWithDelay(5f));
        }
    }

    private IEnumerator StartCrosswordGameWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CrossGamePanel.SetActive(true);
        GridManager.Instance.StartGame(1);
    }



}
