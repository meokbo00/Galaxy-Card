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
    private Vector3 originalPosition;
    private Vector3 hoverPosition;
    private bool isAnimating = false;
    private Coroutine currentAnimation;

    // 애니메이션 설정
    [Header("Animation Settings")]
    public float hoverHeight = 0.3f; // 호버 시 올라가는 높이
    public float animationDuration = 0.2f; // 애니메이션 지속 시간
    private float selectedHeight = 0.5f; // 선택 시 올라가는 높이
    private Vector3 selectedPosition;

    // 카드 이미지 관리
    [Header("Card Sprites")]
    public Sprite cardBackSprite;
    private Sprite cardFrontSprite;
    private static Dictionary<string, Sprite> cardSprites = new Dictionary<string, Sprite>();

    private Vector3 originalScale;
    private Vector3 hoverScale;

    private bool isMovingToOriginal = false; // 원래 위치로 이동 중인지 확인하는 플래그
    private bool isMovingToSelected = false; // 선택된 위치로 이동 중인지 확인하는 플래그

    private DeckManager deckManager;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = new Vector3(2f, 2f, 2f); // 기본 크기를 2로 설정
        hoverScale = new Vector3(2.3f, 2.3f, 2.3f); // 호버 시 크기를 2.3으로 설정
        transform.localScale = originalScale;
        
        // DeckManager 찾기
        deckManager = FindObjectOfType<DeckManager>();
        
        // 카드 이미지 초기화
        if (cardSprites.Count == 0)
        {
            LoadCardSprites();
        }
    }

    private void LoadCardSprites()
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Cards");
        foreach (Sprite sprite in allSprites)
        {
            cardSprites[sprite.name] = sprite;
        }
    }

    void OnMouseEnter()
    {
        if (!isAnimating && !isSelected)
        {
            StopCurrentAnimation();
            currentAnimation = StartCoroutine(HoverAnimation(true));
        }
    }

    void OnMouseExit()
    {
        if (!isMovingToOriginal && !isMovingToSelected) // 원래 위치나 선택된 위치로 이동 중이 아닐 때만 호버 애니메이션 적용
        {
            StopCurrentAnimation();
            currentAnimation = StartCoroutine(HoverAnimation(false));
        }
        else if (isSelected) // 선택된 상태에서 마우스를 떼면 크기를 원래대로
        {
            transform.localScale = originalScale;
        }
    }

    void OnMouseDown()
    {
        if (!isSelected)
        {
            // 카드를 선택하려고 할 때 최대 개수 체크
            if (!deckManager.CanSelectCard())
            {
                return; // 최대 개수에 도달했으면 선택하지 않음
            }
            deckManager.AddSelectedCard(this);
        }
        else
        {
            deckManager.RemoveSelectedCard(this);
        }

        isSelected = !isSelected;
        StopCurrentAnimation();

        if (isSelected)
        {
            isMovingToOriginal = false;
            isMovingToSelected = true;
            selectedPosition = transform.position;
            selectedPosition.y = -3f; // 선택 시 y축 위치를 -3으로 설정
            currentAnimation = StartCoroutine(MoveToPosition(selectedPosition));
        }
        else
        {
            isMovingToOriginal = true;
            isMovingToSelected = false;
            Vector3 targetPosition = transform.position;
            targetPosition.y = -3.5f; // 선택 해제 시 y축 위치를 -3.5로 설정
            currentAnimation = StartCoroutine(MoveToPosition(targetPosition));
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

    private IEnumerator HoverAnimation(bool hover)
    {
        isAnimating = true;
        float elapsedTime = 0;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = hover ? hoverScale : originalScale;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            t = Mathf.SmoothStep(0, 1, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
        isAnimating = false;
        currentAnimation = null;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isAnimating = true;
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            t = Mathf.SmoothStep(0, 1, t);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        isAnimating = false;
        currentAnimation = null;
        isMovingToOriginal = false; // 애니메이션 완료 후 플래그 초기화
        isMovingToSelected = false; // 애니메이션 완료 후 플래그 초기화
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

    public void SetOriginalPosition(Vector3 position)
    {
        originalPosition = position;
        selectedPosition = originalPosition + Vector3.up * selectedHeight;
        transform.position = originalPosition;
    }

    private void UpdateCardVisual()
    {
        if (spriteRenderer != null)
        {
            if (isFaceUp)
            {
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
                spriteRenderer.sprite = cardBackSprite;
            }

            if (isSelected)
            {
                spriteRenderer.color = new Color(1f, 1f, 0.5f);
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