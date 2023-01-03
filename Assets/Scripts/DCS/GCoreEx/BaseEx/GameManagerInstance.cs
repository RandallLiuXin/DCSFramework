using UnityEngine;

namespace Galaxy
{
    public class GameManagerInstance : MonoBehaviour
    {
        private void Awake()
        {
            GalaxyEntry.InitEngine(this);
            GalaxyEntry.Init();
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            GalaxyEntry.UpdateMono(Time.deltaTime);
            //only for DOTS
            GalaxyEntry.FixedUpdateMono(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            //GalaxyEntry.FixedUpdateMono(Time.fixedDeltaTime);
        }

        private void OnApplicationQuit()
        {
            GalaxyEntry.Shutdown();
        }
    }
}
