using UnityEngine;

namespace Objects
{
    public class HotArenaScript : BasicArenaScript
    {
        public GameObject LavaPrefab;

        public override void SpawnObjects()
        {
            base.SpawnObjects();

            SpawnLavaCecils();
        }

        private void SpawnLavaCecils()
        {

            GameObject ice1 = Instantiate(LavaPrefab);
            ice1.transform.localPosition = new Vector3(-width / 4f, 0 , 0);
            ice1.transform.localScale = new Vector3(1, 1, 1);

            GameObject ice2 = Instantiate(LavaPrefab);
            ice2.transform.localPosition = new Vector3(width / 4f, 0, 0);
            ice2.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
