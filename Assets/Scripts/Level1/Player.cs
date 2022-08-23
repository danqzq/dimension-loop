using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DimensionLoop.Level1
{
    public class Player : MonoBehaviour
    {
        [FormerlySerializedAs("loop")] [SerializeField] private GameObject _loop;
        [FormerlySerializedAs("particles")] [SerializeField] private GameObject _particles;
        [FormerlySerializedAs("Hearts")] [SerializeField] private GameObject _hearts;
        [FormerlySerializedAs("Hit")] [SerializeField] private GameObject _hit;
        [FormerlySerializedAs("Enemy")] [SerializeField] private GameObject _enemy;
        [FormerlySerializedAs("FadeIn")] [SerializeField] private GameObject _fadeIn;
        
        [FormerlySerializedAs("cracks")] [SerializeField] private Sprite[] _crackSprites;
        [FormerlySerializedAs("Cracks")] [SerializeField] private SpriteRenderer _cracks;
        
        [FormerlySerializedAs("Score")] [SerializeField] private Text _scoreText;
        [FormerlySerializedAs("Dialog")] [SerializeField] private Text _dialogText;
        
        [FormerlySerializedAs("Health")] [SerializeField] private Image[] _healthDisplay;

        private int _health, _crackLevel, _score;
        
        private float _zRotation, _enemySpawnDelay;

        private bool _space, _cantJump, _immune;

        private readonly string[] _dialog = { "", "", "you", "are", "stuck", "in a loop.", "",
            "it seems like you're bored.", "let's make this game more fun...", "with colors!", "",
            "still not having fun?", "watch out for enemies!", "", "", "", "bad graphics?", "how about now?",
            "", "", "", "no music?", "this is the best i've got", "", "", "", "too easy?", "more enemies!", 
            "", "", "", "okay, what now?!", "bad game?!",
            "let's try something else then", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
            "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""};

        private AudioSource _flipSound;
        private SpriteRenderer _spriteRenderer;

        private Camera _camera;
        private Animator _cameraAnim;
        private AudioSource _music;
        private PostProcessVolume _postProcessVolume;

        private SpriteRenderer _loopSpriteRenderer;
        
        private Coroutine _enemySpawnCoroutine;
        
        private static readonly int Shake = Animator.StringToHash("shake");

        private void Start()
        {
            //TODO: remove this
            Application.targetFrameRate = 60;
            
            _space = false;
            _crackLevel = 0;
            _score = 0;
            _enemySpawnDelay = 3;
            _health = 5;
            _scoreText.gameObject.SetActive(true);

            _flipSound = GetComponent<AudioSource>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            _loopSpriteRenderer = _loop.GetComponent<SpriteRenderer>();

            _camera = Camera.main;
            if (_camera is null) return; 
            _cameraAnim = _camera.GetComponent<Animator>();
            _music = _camera.GetComponent<AudioSource>();
            _postProcessVolume = _camera.GetComponent<PostProcessVolume>();
        }

        private IEnumerator GravityFlip()
        {
            var y = transform.localPosition.y;
            var pos = new Vector3(0, y, 0);
            
            for (var i = 0; i < 50; i++)
            {
                pos.y += -y * 0.04f;
                transform.localPosition = pos;
                yield return new WaitForSeconds(0.01f);
            }
            
            _cameraAnim.SetTrigger(Shake);
            _flipSound.Play();
            _cracks.sprite = _crackSprites[_crackLevel];
            
            if (_crackLevel >= _crackSprites.Length - 1)
            {
                _crackLevel = 0;
                _score++;
                _loop.SetActive(false);

                var p = Instantiate(_particles);

                Destroy(p, 3f);
                
                if (_score > 8)
                {
                    var ps = p.GetComponent<ParticleSystem>().main;
                    ps.startColor = _loopSpriteRenderer.color;
                    _loopSpriteRenderer.color = Random.ColorHSV();
                }

                switch (_score)
                {
                    case 12:
                        _enemySpawnCoroutine = StartCoroutine(EnemySpawn());
                        _hearts.SetActive(true);
                        break;
                    case 17:
                        _postProcessVolume.enabled = true;
                        break;
                    case 22:
                        StartCoroutine(SmoothMusicVolumeIncrease());
                        break;
                    case 27:
                        _enemySpawnDelay = 1.5f;
                        break;
                    case 31:
                        StopCoroutine(_enemySpawnCoroutine);
                        break;
                    case 33:
                        _cantJump = true;
                        StartCoroutine(ZoomToBall());
                        break;
                }

                yield return new WaitForSeconds(0.01f);
                
                _loop.SetActive(true);
                _cracks.sprite = null;
            }
            else _crackLevel++;

            _space = false;
        }

        private IEnumerator EnemySpawn()
        {
            const float step = 360f / 30 * Mathf.Deg2Rad;
            var y = Mathf.Sin(step * Random.Range(0, 30)) * 30;
            var x = Mathf.Cos(step * Random.Range(0, 30)) * 30;

            var angleRad = Mathf.Atan2(0 - y, 0 - x);
            var angleDeg = 180 / Mathf.PI * angleRad + -90;
            var e = Instantiate(_enemy, new Vector3(x, y), Quaternion.Euler(0, 0, angleDeg));
            e.GetComponent<Rigidbody2D>().velocity = (Vector3.zero - e.transform.position).normalized * 3;
            e.GetComponent<UniversalScript>().updateEvents.AddListener(() =>
                e.GetComponent<UniversalScript>().ChangeColor(_loop.GetComponent<SpriteRenderer>()));

            yield return new WaitForSeconds(_enemySpawnDelay);
            _enemySpawnCoroutine = StartCoroutine(EnemySpawn());
        }

        private IEnumerator SmoothMusicVolumeIncrease()
        {
            _music.enabled = true;
            for (var i = 0; i < 100; i++)
            {
                _music.volume += 0.01f;
                yield return new WaitForSeconds(0.05f);
            }
        }

        private IEnumerator ZoomToBall()
        {
            yield return new WaitForSeconds(1f);
            
            var y = transform.localPosition.y;
            var pos = new Vector3(0, y, 0);
            
            for (var i = 0; i < 25; i++)
            {
                pos.y += -y * 0.04f;
                transform.localPosition = pos;
                yield return new WaitForSeconds(0.01f);
            }
            
            for (var i = 0; i < 50; i++)
            {
                _camera.orthographicSize -= 0.1f;
                yield return new WaitForSeconds(0.01f);
            }
            
            yield return new WaitForSeconds(0.25f);
            
            SceneManager.LoadScene(1);
        }

        private IEnumerator OnTriggerEnter2D(Collider2D collision)
        {
            if (_immune) yield break;
            
            Instantiate(_hit, transform.position, Quaternion.Euler(-90, 0, 0));
            _health--;
            _immune = true;
            if (_health <= 0)
            {
                _cantJump = true;
                _spriteRenderer.color = new Color(0, 0, 0, 0);
                yield return new WaitForSeconds(1);
                _fadeIn.SetActive(true);
                yield return new WaitForSeconds(1);
                SceneManager.LoadScene(0);
            }
            yield return new WaitForSeconds(2.5f);
            _immune = false;
        }

        private void Update()
        {
            _scoreText.text = _score.ToString();
            _dialogText.text = _dialog[_score];

            for (var i = 0; i < 5; i++)
                _healthDisplay[i].color = _health > i ? Color.red : Color.black;

            if (_space || _cantJump) return;

            _zRotation += Input.GetAxis("Horizontal");

            transform.parent.rotation = Quaternion.Euler(0, 0, _zRotation);

            if (!Input.GetButtonDown("Jump")) return;
            
            _space = true;
            StartCoroutine(GravityFlip());
        }
    }
}
