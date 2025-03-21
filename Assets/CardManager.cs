using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject[] rectangles; // 18개의 비활성화된 사각형
    void Start()
    {
        
        ActivateRandomRectangles(8); // 게임 시작 시 한 번만 실행
    }
    void ActivateRandomRectangles(int count)
    {
        if (rectangles.Length < count)
        {
            Debug.LogError("배열 크기가 활성화할 개수보다 작습니다!");
            return;
        }

        List<int> indices = new List<int>();
        while (indices.Count < count)
        {
            int randomIndex = Random.Range(0, rectangles.Length);
            if (!indices.Contains(randomIndex))
            {
                indices.Add(randomIndex);
                rectangles[randomIndex].SetActive(true); // 활성화
            }
        }

        // 활성화된 카드 위치 정렬
        Vector2 startPosition = new Vector2(-4.5f, -3.5f);
        float xOffset = 1.3f;
        for (int i = 0; i < indices.Count; i++)
        {
            rectangles[indices[i]].transform.position = startPosition + new Vector2(i * xOffset, 0);
        }
    }
}