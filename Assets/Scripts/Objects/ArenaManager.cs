using UnityEngine;

namespace Objects
{
    public class ArenaManager : MonoBehaviour
    {
        public Transform ArenaParent;
        private Arena _currentArena;

        public Arena SpawnArena(Arena prefab)
        {
            RemoveCurrentArena();

            _currentArena = GameObject.Instantiate(prefab, ArenaParent);
            return _currentArena;
        }

        public void RemoveCurrentArena()
        {
            if (_currentArena != null)
            {
                GameObject.Destroy(_currentArena.gameObject);
                _currentArena = null;
            }
        }

        public Arena CurrentArena => _currentArena;
    }
}
