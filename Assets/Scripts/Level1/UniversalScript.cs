using UnityEngine;
using UnityEngine.Events;

namespace DimensionLoop.Level1
{
    public class UniversalScript : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr;

        public UnityEvent startEvents, updateEvents;

        private void Start() => startEvents.Invoke();

        public void ChangeColor(SpriteRenderer spriteRenderer) => sr.color = spriteRenderer.color;

        private void Update() => updateEvents.Invoke();
    }
}
