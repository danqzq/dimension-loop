using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DimensionLoop.Level3
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private float _speed = 1;

        [FormerlySerializedAs("FadeIn")] [SerializeField] private Image _fadeIn;
        
        [HideInInspector] public UnityEngine.CharacterController cc;

        private AudioSource _music;

        private void Start()
        {
            cc = GetComponent<UnityEngine.CharacterController>();
            if (!(Camera.main is null)) _music = Camera.main.GetComponent<AudioSource>();
        }

        private void Update()
        {
            var t = transform;
            
            var horizontal = Input.GetAxis("Horizontal") * 5;
            var vertical = Input.GetAxis("Vertical") * 5;
            var direction = t.right * horizontal + t.forward * vertical;
            
            cc.Move(direction * Time.deltaTime * _speed);
            
            var position = t.position;
            _fadeIn.color = new Color(1, 1, 1, position.y * 0.01f);
            _music.volume = 1 - position.y * 0.01f;

            if (transform.position.y > 100) SceneManager.LoadScene(3);
        }
    }
}
