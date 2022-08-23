using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace DimensionLoop.Level2
{
    public class Player2 : MonoBehaviour
    {
        [FormerlySerializedAs("sprites")] [SerializeField] private Sprite[] _sprites;
        [FormerlySerializedAs("GroundLayer")] [SerializeField] private LayerMask _groundLayer;
        
        [FormerlySerializedAs("Hit")] [SerializeField] private GameObject _hit;
        [FormerlySerializedAs("FadeIn")] [SerializeField] private GameObject _fadeIn;
        
        private float _lastPosY;

        private SpriteRenderer _sr;
        private Rigidbody2D _rb;
        private Animator _anim;
        
        private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");

        private void Start()
        {
            _sr = GetComponentInChildren<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            Application.targetFrameRate = 60;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                transform.SetParent(collision.transform.parent);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                transform.SetParent(null);
            }
        }

        private IEnumerator OnTriggerEnter2D(Collider2D collision)
        {
            Instantiate(_hit, transform.position, Quaternion.Euler(-90, 0, 0));
            _sr.color = new Color(0, 0, 0, 0);
            yield return new WaitForSeconds(1);
            _fadeIn.SetActive(true);
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(1);
        }

        private void Update()
        {
            bool isGrounded = Physics2D.OverlapCircle(transform.position + Vector3.down, 0.3f, _groundLayer);

            _rb.velocity = new Vector2(Input.GetAxis("Horizontal") * 3, _rb.velocity.y);

            if (Input.GetAxis("Horizontal") > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (Input.GetAxis("Horizontal") < 0)
                transform.localScale = new Vector3(-1, 1, 1);

            if (Input.GetButtonDown("Jump") && isGrounded)
                _rb.AddForce(Vector2.up * 350);

            _anim.SetBool(IsGroundedHash, isGrounded);

            if (_lastPosY < transform.position.y && !isGrounded)
                _sr.sprite = _sprites[1];
            else if (_lastPosY > transform.position.y && !isGrounded)
                _sr.sprite = _sprites[2];
            else if (isGrounded)
                _sr.sprite = _sprites[0];

            _lastPosY = transform.position.y;
        }
    }
}
