using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject[] rectangles; // 18���� ��Ȱ��ȭ�� �簢��
    void Start()
    {
        
        ActivateRandomRectangles(8); // ���� ���� �� �� ���� ����
    }
    void ActivateRandomRectangles(int count)
    {
        if (rectangles.Length < count)
        {
            Debug.LogError("�迭 ũ�Ⱑ Ȱ��ȭ�� �������� �۽��ϴ�!");
            return;
        }

        List<int> indices = new List<int>();
        while (indices.Count < count)
        {
            int randomIndex = Random.Range(0, rectangles.Length);
            if (!indices.Contains(randomIndex))
            {
                indices.Add(randomIndex);
                rectangles[randomIndex].SetActive(true); // Ȱ��ȭ
            }
        }

        // Ȱ��ȭ�� ī�� ��ġ ����
        Vector2 startPosition = new Vector2(-4.5f, -3.5f);
        float xOffset = 1.3f;
        for (int i = 0; i < indices.Count; i++)
        {
            rectangles[indices[i]].transform.position = startPosition + new Vector2(i * xOffset, 0);
        }
    }
}