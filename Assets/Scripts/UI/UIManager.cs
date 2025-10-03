using Shared;
using UnityEngine;

namespace UI
{
    public class UIManager : Singleton<UIManager>
    {
        public MenuUI menuUIPrefab;
        public IdleUI idleUIPrefab;
        public PauseUI pauseUIPrefab;
        public PlayingUI playingUIPrefab;
        public WinUI winUIPrefab;


        public T SpawnUI<T>(T prefab) where T : Component
        {
            if (prefab == null) throw new System.ArgumentNullException(nameof(prefab));
            var inst = Instantiate(prefab, this.transform);
            return inst;
        }

        public void DestroyUI(Component ui)
        {
            if (ui == null) return;
            Destroy(ui.gameObject);
        }
    }
}