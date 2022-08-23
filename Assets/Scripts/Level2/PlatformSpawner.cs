using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DimensionLoop.Level2
{
    public class PlatformSpawner : MonoBehaviour
    {
        [FormerlySerializedAs("Platform")] [SerializeField] private GameObject _platform;
        [FormerlySerializedAs("FadeIn")] [SerializeField] private GameObject _fadeIn;

        [SerializeField] private SpriteRenderer[] backgroundElements1, backgroundElements2;

        private int _index;
        
        private bool _colorfulPlatforms, _spikes;
        
        private readonly float[] _possibleYPositions = { -0.5f, -1.5f, -2.5f };

        private PostProcessVolume _postProcessVolume;

        private IEnumerator Start()
        {
            if (Camera.main != null) _postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();

            Time.timeScale = 0.01f;
            yield return new WaitForSeconds(0.01f);
            Time.timeScale = 1;
            
            StartCoroutine(Events());
            
            for (var i = 0; i < 250; i++)
            {
                var rand = Random.Range(-1, 2);
                _index += rand;
                switch (_index)
                {
                    case -1:
                    case 3:
                        _index = Random.Range(0, 2);
                        break;
                }
                var p = Instantiate(_platform, new Vector2(4.5f, _possibleYPositions[_index]), Quaternion.identity);

                if (_colorfulPlatforms)
                    p.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = Random.ColorHSV();

                if (_spikes)
                    p.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                
                Destroy(p, 6);
                yield return new WaitForSeconds(2.5f);
            }
        }

        private IEnumerator Events()
        {
            yield return new WaitForSeconds(22f);
            var c1 = backgroundElements1[0].color;
            var c2 = backgroundElements2[0].color;
            _colorfulPlatforms = true;
            
            for (int j = 0; j < 100; j++)
            {
                c1 += new Color(-0.01f, 0, -0.01f);
                c2 += new Color(0, 0, -0.01f);
                _postProcessVolume.weight += 0.01f;
                yield return new WaitForSeconds(0.01f);
                for (int i = 0; i < backgroundElements1.Length; i++) backgroundElements1[i].color = c1;
                for (int i = 0; i < backgroundElements2.Length; i++) backgroundElements2[i].color = c2;
            }
            
            yield return new WaitForSeconds(11);
            _spikes = true;
            yield return new WaitForSeconds(10);
            _spikes = false;
            yield return new WaitForSeconds(11);
            _fadeIn.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            _fadeIn.SetActive(true);
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(2);
        }
    }
}
