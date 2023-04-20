using UnityEngine;

namespace Planet
{
    public interface IPlanetView
    {
        void Generate (PlanetSettings settings);
        void Clear ();
    }

    public class PlanetView : MonoBehaviour
    {
        [SerializeField] private bool _generateOnStart = false;
        [SerializeField] private PlanetSettings _settings;

        public PlanetSettings Settings => _settings;

        private void Start ()
        {
            if (_generateOnStart)
                Generate();
        }

        private void OnDestroy ()
        {
            Clear();
        }

        public void Generate ()
        {
            Clear();
            IPlanetView[] views = GetComponents<IPlanetView>();
            foreach (IPlanetView view in views)
                view.Generate(_settings);
        }

        private void Clear ()
        {
            IPlanetView[] views = GetComponents<IPlanetView>();
            foreach (IPlanetView view in views)
                view.Clear();
        }
    }
}
