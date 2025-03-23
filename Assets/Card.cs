using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour
{
    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    public enum Rank
    {
        Ace = 1, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8,
        Nine = 9, Ten = 10, Jack = 11, Queen = 12, King = 13
    }

    public Suit suit;
    public Rank rank;
    public bool isFaceUp = true;
    public bool isSelected = false;
    public bool isJoker = false;

    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isAnimating = false;
    private Coroutine currentAnimation;

    // 애니메이션 설정
    [Header("Animation Settings")]
    public float popScale = 1.2f;
    public float popDuration = 0.2f;
    public float returnDuration = 0.1f;
    public float hoverHeight = 0.5f;

    // 캐시된 값들
    private Vector3 hoverPosition;
    private Vector3 popScaleVector;
    private Vector3 originalScaleVector;

    private bool isDragging = false;
    private Vector3 offset;
    private float zOffset = 0f; // zOffset을 0으로 변경
    private Camera mainCamera;

    // 카드 이미지 관리
    [Header("Card Sprites")]
    public Sprite cardBackSprite; // 카드 뒷면 스프라이트
    private Sprite cardFrontSprite; // 카드 앞면 스프라이트

    // 카드 이미지 매핑을 위한 딕셔너리
    private static Dictionary<string, Sprite> cardSprites = new Dictionary<string, Sprite>();

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        mainCamera = Camera.main;
        
        // 캐시 계산
        hoverPosition = originalPosition + Vector3.up * hoverHeight;
        popScaleVector = originalScale * popScale;
        originalScaleVector = originalScale;

        // 카드 이미지 초기화
        if (cardSprites.Count == 0)
        {
            LoadCardSprites();
        }
    }

    private void LoadCardSprites()
    {
        // Resources 폴더에서 카드 스프라이트 로드
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Cards");
        foreach (Sprite sprite in allSprites)
        {
            cardSprites[sprite.name] = sprite;
        }
    }

    void OnMouseEnter()
    {
        if (!isAnimating)
        {
            StopCurrentAnimation();
            currentAnimation = StartCoroutine(HoverAnimation());
        }
    }

    void OnMouseExit()
    {
        if (!isAnimating)
        {
            StopCurrentAnimation();
            currentAnimation = StartCoroutine(ReturnToOriginal());
        }
    }

    void OnMouseDown()
    {
        if (!isAnimating)
        {
            StopCurrentAnimation();
            currentAnimation = StartCoroutine(PopAnimation());
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging)
        {
            isDragging = true;
            StopCurrentAnimation();
            
            // 마우스 위치와 카드 위치의 차이를 계산
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
            offset = transform.position - mousePos;
            
            // 카드를 앞으로 가져오기
            Vector3 newPosition = transform.position;
            newPosition.z = -mainCamera.transform.position.z;
            transform.position = newPosition;
        }

        // 카드를 마우스 위치로 이동
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        targetPosition.z = -mainCamera.transform.position.z;
        transform.position = targetPosition + offset;
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            // 카드를 원래 위치로 돌아가게 하거나, 여기에 카드 놓기 로직 추가
            StartCoroutine(ReturnToOriginal());
        }
    }

    private void StopCurrentAnimation()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
    }

    private IEnumerator PopAnimation()
    {
        isAnimating = true;
        
        // 팝업 애니메이션
        float elapsedTime = 0;
        while (elapsedTime < popDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / popDuration;
            transform.localScale = Vector3.Lerp(originalScaleVector, popScaleVector, t);
            yield return null;
        }

        // 원래 크기로 돌아가기
        elapsedTime = 0;
        while (elapsedTime < returnDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / returnDuration;
            transform.localScale = Vector3.Lerp(popScaleVector, originalScaleVector, t);
            yield return null;
        }

        transform.localScale = originalScaleVector;
        isAnimating = false;
        currentAnimation = null;
    }

    private IEnumerator HoverAnimation()
    {
        isAnimating = true;
        float elapsedTime = 0;
        
        while (elapsedTime < popDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / popDuration;
            transform.position = Vector3.Lerp(originalPosition, hoverPosition, t);
            yield return null;
        }

        isAnimating = false;
        currentAnimation = null;
    }

    private IEnumerator ReturnToOriginal()
    {
        isAnimating = true;
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;
        
        while (elapsedTime < returnDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / returnDuration;
            transform.position = Vector3.Lerp(startPosition, originalPosition, t);
            yield return null;
        }

        transform.position = originalPosition;
        isAnimating = false;
        currentAnimation = null;
    }

    public void Initialize(Suit newSuit, Rank newRank)
    {
        suit = newSuit;
        rank = newRank;
        isFaceUp = true;
        isSelected = false;
        isJoker = false;
        UpdateCardVisual();
    }

    public void FlipCard()
    {
        isFaceUp = !isFaceUp;
        UpdateCardVisual();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateCardVisual();
    }

    private void UpdateCardVisual()
    {
        if (spriteRenderer != null)
        {
            if (isFaceUp)
            {
                // 카드 앞면 스프라이트 설정
                string spriteName = GetCardSpriteName();
                if (cardSprites.TryGetValue(spriteName, out Sprite frontSprite))
                {
                    cardFrontSprite = frontSprite;
                    spriteRenderer.sprite = cardFrontSprite;
                }
                else
                {
                    Debug.LogError($"카드 스프라이트를 찾을 수 없습니다: {spriteName}");
                }
            }
            else
            {
                // 카드 뒷면 스프라이트 설정
                spriteRenderer.sprite = cardBackSprite;
            }

            // 선택된 카드의 색상 변경
            if (isSelected)
            {
                spriteRenderer.color = new Color(1f, 1f, 0.5f); // 노란색으로 하이라이트
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }

    private string GetCardSpriteName()
    {
        if (isJoker)
            return "joker";

        string suitName = suit.ToString().ToLower();
        string rankName = rank.ToString().ToLower();
        return $"{rankName}_of_{suitName}";
    }

    public string GetCardName()
    {
        if (isJoker)
            return "Joker";
        return $"{rank} of {suit}";
    }
} 