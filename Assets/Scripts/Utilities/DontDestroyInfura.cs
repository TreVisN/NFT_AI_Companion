using Infura.Unity;
using UnityEngine;

namespace UIToolkitDemo
{
    public class DontDestroyInfura : MonoBehaviour
    {
        void Awake()
        {
            var objs = FindObjectsOfType<InfuraSdk>();

            if (objs.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}
