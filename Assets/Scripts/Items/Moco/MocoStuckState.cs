using UnityEngine;

/// <summary>
/// Stuck-in-moco state. Freezes the body in place (no gravity, no input) until
/// the victim fills the struggle bar, or the accessibility time cap releases
/// them. Kicking and moving both feed the bar (a kick is worth more than a
/// step) and the bar drains on its own, so mashing has to outpace the drain
/// instead of just repeating one button at any speed.
/// PlayerController routes kick and move presses here while active.
/// A short immunity after release keeps a lingering trap from re-catching the
/// victim on the same frame they escape.
/// </summary>
public sealed class MocoStuckState : PlayerItemState
{
    private const float ReleaseImmunitySeconds = 1f;

    private float _struggleNeeded;
    private float _struggleFilled;
    private float _drainPerSecond;
    private float _kickPoints;
    private float _movePoints;
    private Color _stuckTint;

    private float _releasedAt = float.NegativeInfinity;

    /// <summary>0–1 fill of the struggle bar. For the HUD once real UI exists.</summary>
    public float StruggleNormalized =>
        _struggleNeeded <= 0f ? 1f : Mathf.Clamp01(_struggleFilled / _struggleNeeded);

    public bool HasReleaseImmunity =>
        !IsActive && Time.time - _releasedAt < ReleaseImmunitySeconds;

    public void Activate(
        int strugglePresses,
        float kickPoints,
        float movePoints,
        float drainPerSecond,
        float maxStuckSeconds,
        Color tint)
    {
        _struggleNeeded = Mathf.Max(1, strugglePresses);
        _struggleFilled = 0f;
        _kickPoints     = kickPoints;
        _movePoints     = movePoints;
        _drainPerSecond = drainPerSecond;
        _stuckTint      = tint;

        BeginState(maxStuckSeconds, tint);
    }

    /// <summary>
    /// Called by PlayerController on each struggle input while stuck.
    /// A kick is worth more than a movement press: the kick is the intended
    /// escape, movement is the fallback so the state never feels unresponsive.
    /// </summary>
    public void OnStrugglePress(bool fromKick)
    {
        if (!IsActive) return;

        _struggleFilled += fromKick ? _kickPoints : _movePoints;

        if (_struggleFilled >= _struggleNeeded)
            EndState();
    }

    protected override void OnStateStarted()
    {
        Movement.HoldPosition = true;
        Controller.PushMovementLock();
    }

    protected override void OnTick()
    {
        // The bar drains: stop mashing and you lose ground.
        if (_drainPerSecond > 0f && _struggleFilled > 0f)
            _struggleFilled = Mathf.Max(0f, _struggleFilled - _drainPerSecond * Time.deltaTime);

        // Placeholder feedback until the bar has real UI: the tint washes out
        // as the victim gets closer to breaking free.
        SetTintColor(Color.Lerp(_stuckTint, Color.white, StruggleNormalized * 0.7f));
    }

    protected override void OnStateEnded()
    {
        Movement.HoldPosition = false;
        Controller.PopMovementLock();
        _releasedAt = Time.time;
    }
}
