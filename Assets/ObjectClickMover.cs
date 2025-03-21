using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectClickMover : MonoBehaviour
{
    public float moveDistance = 0.5f; // �̵��� �Ÿ� (y�� ����)
    public float moveSpeed = 2f; // �̵� �ӵ�
    public float trashMoveSpeed = 5f; // trash ��ư Ŭ�� �� �̵� �ӵ�
    private bool isMoving = false;
    private bool isUp = false; // ���� ���� (���� �ö󰬴��� ����)
    private Vector3 originalPosition;
    private static int upCount = 0; // ���� �ö� ī�� ����
    private static int maxUpCount = 5; // �ִ� �ö� �� �ִ� ī�� ����
    public static List<ObjectClickMover> activeCards = new List<ObjectClickMover>();

    void Start()
    {
        originalPosition = transform.position;
    }

    void OnMouseDown()
    {
        if (!isMoving)
        {
            if (isUp)
            {
                StartCoroutine(Move(originalPosition)); // ���� ��ġ�� �̵�
                upCount--; // �ö� ���� ����
                activeCards.Remove(this);
                isUp = false;
            }
            else if (upCount < maxUpCount)
            {
                StartCoroutine(Move(originalPosition + new Vector3(0, moveDistance, 0))); // ���� �̵�
                upCount++; // �ö� ���� ����
                activeCards.Add(this);
                isUp = true;
            }
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime);
            elapsedTime += Time.deltaTime * moveSpeed;
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }

    public static void TrashCards()
    {
        foreach (var card in new List<ObjectClickMover>(activeCards))
        {
            card.StartCoroutine(card.FlyAwayAndDisable());
        }
        activeCards.Clear();
        upCount = 0;
    }

    IEnumerator FlyAwayAndDisable()
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(10f, 0, 0); // ���������� �̵�
        float elapsedTime = 0;

        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime);
            elapsedTime += Time.deltaTime * trashMoveSpeed;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
