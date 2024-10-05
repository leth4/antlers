using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Foundation
{
    public static class Tween
    {
        public static Coroutine Delay(this MonoBehaviour behaviour, float time, Action ended = null)
        {
            return behaviour.StartCoroutine(DelayRoutine(time, ended));
        }

        public static Coroutine Translate(this MonoBehaviour behaviour, Transform transform, Vector3 target, float time, EaseType easeType = EaseType.Linear, Action ended = null)
        {
            return behaviour.StartCoroutine(TranslateRoutine(transform, target, time, easeType, ended));
        }

        public static Coroutine Translate(this MonoBehaviour behaviour, RectTransform rectTransform, Vector3 target, float time, EaseType easeType = EaseType.Linear, Action ended = null)
        {
            return behaviour.StartCoroutine(TranslateRoutine(rectTransform, target, time, easeType, ended));
        }

        public static Coroutine Scale(this MonoBehaviour behaviour, Transform transform, Vector3 target, float time, EaseType easeType = EaseType.Linear, Action ended = null)
        {
            return behaviour.StartCoroutine(ScaleRoutine(transform, target, time, easeType, ended));
        }

        public static Coroutine Rotate(this MonoBehaviour behaviour, Transform transform, Quaternion target, float time, EaseType easeType = EaseType.Linear, Action ended = null)
        {
            return behaviour.StartCoroutine(RotateRoutine(transform, target, time, easeType, ended));
        }

        public static Coroutine RotateEuler(this MonoBehaviour behaviour, Transform transform, Vector3 target, float time, EaseType easeType = EaseType.Linear, Action ended = null)
        {
            return behaviour.StartCoroutine(RotateEulerRoutine(transform, target, time, easeType, ended));
        }

        public static Coroutine Color(this MonoBehaviour behaviour, SpriteRenderer spriteRenderer, Color target, float time, EaseType easeType = EaseType.Linear, Action ended = null)
        {
            return behaviour.StartCoroutine(ColorRoutine(spriteRenderer, target, time, easeType, ended));
        }

        public static Coroutine Color(this MonoBehaviour behaviour, Image image, Color target, float time, EaseType easeType = EaseType.Linear, Action ended = null)
        {
            return behaviour.StartCoroutine(ColorRoutine(image, target, time, easeType, ended));
        }

        public static Coroutine Shake(this MonoBehaviour behaviour, Transform transform, float magnitude, float time, Action ended = null)
        {
            return behaviour.StartCoroutine(ShakeRoutine(transform, magnitude, time, ended));
        }

        private static IEnumerator DelayRoutine(float time, Action ended)
        {
            yield return new WaitForSeconds(time);
            ended?.Invoke();
        }

        private static IEnumerator TranslateRoutine(Transform transform, Vector3 target, float time, EaseType easeType, Action ended)
        {
            var initialPosition = transform.localPosition;
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                transform.localPosition = Vector3.Lerp(initialPosition, target, Ease.Sample(t / time, easeType));
                yield return null;
            }

            transform.localPosition = target;
            ended?.Invoke();
        }

        private static IEnumerator TranslateRoutine(RectTransform rectTransform, Vector3 target, float time, EaseType easeType, Action ended)
        {
            var initialPosition = rectTransform.anchoredPosition;
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                rectTransform.anchoredPosition = Vector3.Lerp(initialPosition, target, Ease.Sample(t / time, easeType));
                yield return null;
            }

            rectTransform.anchoredPosition = target;
            ended?.Invoke();
        }

        private static IEnumerator ScaleRoutine(Transform transform, Vector3 target, float time, EaseType easeType, Action ended)
        {
            var initialScale = transform.localScale;
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                transform.localScale = Vector3.Lerp(initialScale, target, Ease.Sample(t / time, easeType));
                yield return null;
            }

            transform.localScale = target;
            ended?.Invoke();
        }

        private static IEnumerator RotateRoutine(Transform transform, Quaternion target, float time, EaseType easeType, Action ended)
        {
            var initialRotation = transform.localRotation;
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                transform.localRotation = Quaternion.Lerp(initialRotation, target, Ease.Sample(t / time, easeType));
                yield return null;
            }

            transform.localRotation = target;
            ended?.Invoke();
        }

        private static IEnumerator RotateEulerRoutine(Transform transform, Vector3 target, float time, EaseType easeType, Action ended)
        {
            var initialRotation = transform.localEulerAngles;
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                transform.localEulerAngles = Vector3.Lerp(initialRotation, target, Ease.Sample(t / time, easeType));
                yield return null;
            }

            transform.localEulerAngles = target;
            ended?.Invoke();
        }

        private static IEnumerator ColorRoutine(SpriteRenderer spriteRenderer, Color target, float time, EaseType easeType, Action ended)
        {
            var initialColor = spriteRenderer.color;
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                spriteRenderer.color = UnityEngine.Color.Lerp(initialColor, target, Ease.Sample(t / time, easeType));
                yield return null;
            }

            spriteRenderer.color = target;
            ended?.Invoke();
        }

        private static IEnumerator ColorRoutine(Image image, Color target, float time, EaseType easeType, Action ended)
        {
            var initialColor = image.color;
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                image.color = UnityEngine.Color.Lerp(initialColor, target, Ease.Sample(t / time, easeType));
                yield return null;
            }

            image.color = target;
            ended?.Invoke();
        }

        private static IEnumerator ShakeRoutine(Transform transform, float magnitude, float time, Action ended)
        {
            var startPosition = transform.position;

            for (float t = 0; t < time; t += Time.deltaTime)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.position = startPosition + new Vector3(Random.Range(-1f, 1f) * magnitude, Random.Range(-1f, 1f) * magnitude, 0);
                yield return null;
            }

            transform.position = startPosition;
            ended?.Invoke();
        }
    }
}