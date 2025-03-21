using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectClickMover : MonoBehaviour
{
    public float moveDistance = 0.5f; // 이동할 거리 (y축 방향)
    public float moveSpeed = 2f; // 이동 속도
    public float trashMoveSpeed = 5f; // trash 버튼 클릭 시 이동 속도
    private bool isMoving = false;
    private bool isUp = false; // 현재 상태 (위로 올라갔는지 여부)
    private Vector3 originalPosition;
    private static int upCount = 0; // 위로 올라간 카드 개수
    private static int maxUpCount = 5; // 최대 올라갈 수 있는 카드 개수
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
                StartCoroutine(Move(originalPosition)); // 원래 위치로 이동
                upCount--; // 올라간 개수 감소
                activeCards.Remove(this);
                isUp = false;
            }
            else if (upCount < maxUpCount)
            {
                StartCoroutine(Move(originalPosition + new Vector3(0, moveDistance, 0))); // 위로 이동
                upCount++; // 올라간 개수 증가
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
        Vector3 targetPos = startPos + new Vector3(10f, 0, 0); // 오른쪽으로 이동
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
