using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    [SerializeField]
    [ReadOnly]
    private Vector3 originalPosition;
    public float Intensity = 0.2f;
    public float Duration = 0.5f;
    private float endtime = 0f;

    private void Start() {
        originalPosition = transform.position;
    }

    private void FixedUpdate() {

        if (Time.time < endtime) {

            Vector3 shakeOffset = Random.insideUnitSphere * Intensity;

            transform.position = originalPosition + shakeOffset;

        } else {

            // when shake is over, position should be reset
            transform.position = originalPosition;
        }
    }

    [Button]
    public void Shake() {
        endtime = Time.time + Duration;
    }

    /* another way but with DoTween Shake
    public float ShakeDelay = 0;
    public float ShakeDuration = 1;
    public float ShakeStrength = 1;
    public int ShakeVibrato = 1;
    public float ShakeRandomness = 1;

    private Sequence shakeImageSequence;

    [Button]
    public void DoTweenShake() {
        StopCoroutine("IE_DoTweenShake");
        StartCoroutine("IE_DoTweenShake");
    }

    IEnumerator IE_DoTweenShake() {
        yield return new WaitForSeconds(ShakeDelay);

        shakeImageSequence.Append(
            transform.DOShakePosition(
                duration: ShakeDuration,
                strength: ShakeStrength,
                vibrato: ShakeVibrato,
                randomness: ShakeRandomness,
                snapping: false
            ).OnComplete(
                () => {
                    transform.DOLocalMove(originalPosition, 0.5f);
                }
            )

        );

    }
    */
}