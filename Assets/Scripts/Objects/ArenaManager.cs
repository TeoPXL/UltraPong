using UnityEngine;
using UnityEngine.Serialization;

namespace Objects
{
    public class ArenaManager : MonoBehaviour
    {
        [FormerlySerializedAs("ArenaParent")] public Transform arenaParent;
        private Arena _currentArena;

        public Arena SpawnArena(Arena prefab)
        {
            RemoveCurrentArena();

            _currentArena = GameObject.Instantiate(prefab, arenaParent);
            return _currentArena;
        }

        public void RemoveCurrentArena()
        {
            if (_currentArena != null)
            {
                _currentArena.ClearItems();
                GameObject.Destroy(_currentArena.gameObject);
                _currentArena = null;
            }
        }

        public Arena CurrentArena => _currentArena;
    }
}
