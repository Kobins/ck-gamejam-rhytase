using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpriteAnimation {
    public List<Sprite> m_sprites;
}

public enum PlayerAnimationType {
    NONE,
    WALK,
    RUN,
    JUMP,
    UPSTAIR,
    DOWNSTAIR,
    CLEAR,
    FAIL
}

public class Player : MonoBehaviour {
    [Header("Animations")]
    public SpriteAnimation m_walkAnimation;
    public SpriteAnimation m_runAnimation;
    public SpriteAnimation m_jumpAnimation;
    public SpriteAnimation m_upstairAnimation;
    public SpriteAnimation m_downstairAnimation;
    public SpriteAnimation m_clearAnimation;
    public SpriteAnimation m_failAnimation;

    public SpriteRenderer m_spriteRenderer;
    private Dictionary<PlayerAnimationType, SpriteAnimation> m_animationMap;
    private SpriteAnimation m_currentAnimation;
    private SpriteAnimation CurrentAnimation {
        get => m_currentAnimation;
        set {
            m_currentAnimation = value;
            m_spriteRenderer.sprite = m_currentAnimation.m_sprites[0];
            m_animationIndex = 0;
        }
    }

    private void Awake() {
        m_animationMap = new Dictionary<PlayerAnimationType, SpriteAnimation>() {
            { PlayerAnimationType.WALK, m_walkAnimation },
            { PlayerAnimationType.RUN, m_runAnimation },
            { PlayerAnimationType.JUMP, m_jumpAnimation },
            { PlayerAnimationType.UPSTAIR, m_upstairAnimation },
            { PlayerAnimationType.DOWNSTAIR, m_downstairAnimation },
            { PlayerAnimationType.CLEAR, m_clearAnimation },
            { PlayerAnimationType.FAIL, m_failAnimation },
        };
    }
    private void Start() {
        CurrentAnimation = m_walkAnimation;
        m_animationIndex = 0;
    }

    public bool SetAnimation(PlayerAnimationType key) {
        if (key == PlayerAnimationType.NONE) return false;
        var animation = m_animationMap[key];
        if (animation == null) return false;
        CurrentAnimation = animation;
        return true;
    }

    private int m_animationIndex;
    public void NextSprite() {
        var animation = CurrentAnimation;
        m_animationIndex = (m_animationIndex + 1) % animation.m_sprites.Count;
        m_spriteRenderer.sprite = animation.m_sprites[m_animationIndex];
    }

}