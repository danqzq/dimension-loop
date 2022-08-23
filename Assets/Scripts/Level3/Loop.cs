using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace DimensionLoop.Level3
{
    public class Loop : MonoBehaviour
    {
        [FormerlySerializedAs("crackSprites")] [SerializeField] private Sprite[] _crackSprites;
        [FormerlySerializedAs("particles")] [SerializeField] private GameObject _particles;
        [FormerlySerializedAs("loop")] [SerializeField] private GameObject _loop;
        
        private int _crackLevel = -1;
        
        private bool _canBreak = true;
        
        private SpriteRenderer _loopCracks;
        
        private void Start() => _loopCracks = transform.GetChild(0).GetComponent<SpriteRenderer>();

        private IEnumerator Cooldown()
        {
            _canBreak = false;
            yield return new WaitForSeconds(0.5f);
            _canBreak = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_canBreak || !collision.gameObject.CompareTag("Player")) return;
            
            _crackLevel++;
            collision.gameObject.GetComponent<AudioSource>().Play();
            StartCoroutine(Cooldown());
            
            if (_crackLevel == 4)
            {
                var p = Instantiate(_particles, 
                    collision.transform.position + Vector3.down * 0.25f, Quaternion.identity);
                
                Destroy(p, 3);
                
                var l = Instantiate(_loop, transform.position, Quaternion.Euler(-90,0,0));
                l.transform.localScale = transform.localScale;
                GameManager.score++;
                Destroy(gameObject);
                return;
            }
            _loopCracks.sprite = _crackSprites[_crackLevel];
        }
    }
}
