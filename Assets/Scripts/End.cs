using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DimensionLoop
{
    public class End : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(15);
            SceneManager.LoadScene(0);
        }
    }
}
