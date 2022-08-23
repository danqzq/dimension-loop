using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DimensionLoop.Level3
{
    public class GameManager : MonoBehaviour
    {
        public static int score;
        
        [FormerlySerializedAs("Score")] [SerializeField] private Text _scoreText;

        private void Update() => _scoreText.text = score.ToString();
    }
}
