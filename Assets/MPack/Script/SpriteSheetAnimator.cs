#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

namespace MPack {
    public class SpriteSheetAnimator : MonoBehaviour
    {
        [SerializeField]
        private AnimType type;
        private enum AnimType {
            Loop,
            PingPong,
            BackwardLoop,
            OneTime,
            BackwardOneTime,
        }

        [SerializeField]
        private bool sameInterval = true;
        [SerializeField]
        private float interval = 0.2f;

        [SerializeField]
        private KeyPoint[] keyPoints;
        private int keyPointIndex;
        private bool indexForward = true;
        System.Action triggerFinished;

        private SpriteRenderer spriteRenderer;
        private Image image;

        private Timer timer;

        private Sprite sprite {
            get {
                return spriteRenderer != null? spriteRenderer.sprite: image.sprite;
            }
            set {
                if (spriteRenderer != null) spriteRenderer.sprite = value;
                else if (image != null) image.sprite = value;
                else
                {
                    spriteRenderer = GetComponent<SpriteRenderer>();
                    image = GetComponent<Image>();

                    if (spriteRenderer != null) spriteRenderer.sprite = value;
                    else if (image == null) image.sprite = value;
                }
            }
        }

        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();

            if (sameInterval) timer = new Timer(interval);
            else timer = new Timer(keyPoints[0].Interval);

            if (enabled)
            {
                switch (type) {
                    case AnimType.Loop:
                    case AnimType.PingPong:
                    case AnimType.OneTime:
                        sprite = keyPoints[0].Sprite;
                        keyPointIndex = 1;
                        break;
                    case AnimType.BackwardLoop:
                    case AnimType.BackwardOneTime:
                        sprite = keyPoints[keyPoints.Length - 1].Sprite;
                        keyPointIndex = keyPoints.Length - 2;
                        indexForward = false;
                        break;
                }
            }
        }

        private void Update() {
            if (timer.UpdateEnd) {
                timer.Reset();
                if (!sameInterval) timer.TargetTime = keyPoints[keyPointIndex].Interval;

                sprite = keyPoints[keyPointIndex].Sprite;

                if (indexForward) {
                    keyPointIndex++;
                    if (keyPointIndex >= keyPoints.Length) {
                        switch (type) {
                            case AnimType.Loop:
                                keyPointIndex = 0;
                                break;
                            case AnimType.PingPong:
                                indexForward = false;
                                keyPointIndex = keyPoints.Length - 2;
                                break;
                            case AnimType.OneTime:
                                enabled = false;
                                triggerFinished?.Invoke();
                                triggerFinished = null;
                                break;
                        }
                    }
                }
                else {
                    keyPointIndex--;
                    if (keyPointIndex < 0) {
                        switch (type)
                        {
                            case AnimType.BackwardLoop:
                                keyPointIndex = keyPoints.Length - 1;
                                break;
                            case AnimType.PingPong:
                                indexForward = true;
                                keyPointIndex = 1;
                                break;
                            case AnimType.BackwardOneTime:
                                enabled = false;
                                triggerFinished?.Invoke();
                                triggerFinished = null;
                                break;
                        }
                    }
                }
            }
        }

        public void Trigger(bool forward, bool startAtLastSprite=true, System.Action finished=null)
        {
            if (startAtLastSprite)
            {
                int index = keyPointIndex;
                for (int i = 0; i < keyPoints.Length; i++)
                {
                    if (keyPoints[i].Sprite == sprite) {
                        index = i;
                        break;
                    }
                }

                if (forward)
                {
                    keyPointIndex = index + 1;
                    if (keyPointIndex >= keyPoints.Length)
                    {
                        enabled = false;
                        finished?.Invoke();
                        return;
                    }
                }
                else
                {
                    keyPointIndex = index - 1;
                    if (keyPointIndex < 0)
                    {
                        enabled = false;
                        finished?.Invoke();
                        return;
                    }
                }
            }
            else if (forward)
            {
                keyPointIndex = 1;
                sprite = keyPoints[0].Sprite;
            }
            else
            {
                sprite = keyPoints[keyPoints.Length - 1].Sprite;
                keyPointIndex = keyPoints.Length - 2;
            }

            indexForward = forward;
            type = forward? AnimType.OneTime: AnimType.BackwardOneTime;
            triggerFinished += finished;

            enabled = true;
        }

        public void SetAnimationFrame(bool forward, int index)
        {
            if (forward)
            {
                if (index == -1)
                    index = keyPoints.Length - 1;
                
                sprite = keyPoints[index].Sprite;

                keyPointIndex = index + 1;
                if (keyPointIndex >= keyPoints.Length)
                {
                    enabled = false;
                    return;
                }
            }
            else
            {
                if (index == -1)
                    index = 0;
                
                sprite = keyPoints[index].Sprite;

                keyPointIndex = index - 1;
                if (keyPointIndex < 0)
                {
                    enabled = false;
                    return;
                }
            }
        }

        [System.Serializable]
        private struct KeyPoint {
            public Sprite Sprite;
            public float Interval;
        }
    }
}