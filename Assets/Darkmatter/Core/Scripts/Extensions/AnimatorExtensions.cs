using UnityEngine;
using Cysharp.Threading.Tasks;

public static class AnimatorExtensions
{
    /// <summary>
    /// Plays a state by hash and waits until it finishes (normalizedTime >= 1f)
    /// </summary>
    public static async UniTask PlayAndWaitAsync(this Animator animator, int stateHash, int layer = 0, float normalizedTime = 0f)
    {
        animator.Play(stateHash, layer, normalizedTime);

        // Wait until the animator actually enters that state
        await UniTask.WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == stateHash);

        // Wait until the animation finishes and is not in transition
        await UniTask.WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(layer).normalizedTime >= 1f &&
            !animator.IsInTransition(layer));
    }

    /// <summary>
    /// Crossfades into a state (by hash) and waits until it finishes.
    /// </summary>
    public static async UniTask CrossFadeAndWaitAsync(this Animator animator, int stateHash, float transitionDuration, int layer = 0)
    {
        animator.CrossFadeInFixedTime(stateHash, transitionDuration, layer);

        await UniTask.WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == stateHash);

        await UniTask.WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(layer).normalizedTime >= 1f &&
            !animator.IsInTransition(layer));
    }

    /// <summary>
    /// Waits for the current state's animation to finish.
    /// </summary>
    public static async UniTask WaitCurrentStateAsync(this Animator animator, int layer = 0)
    {
        var state = animator.GetCurrentAnimatorStateInfo(layer).shortNameHash;

        await UniTask.WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == state &&
            animator.GetCurrentAnimatorStateInfo(layer).normalizedTime >= 1f &&
            !animator.IsInTransition(layer));
    }
}
