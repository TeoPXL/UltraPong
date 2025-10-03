using UnityEngine;

namespace Objects
{
    public class ColdArenaScript : BasicArenaScript
    {
        public GameObject IceCecilPrefab;

        public override void SpawnObjects()
        {
            base.SpawnObjects();

            SpawnIceCecils();
        }

        private void SpawnIceCecils()
        {
            float northWallY = height / 2f;

            GameObject ice1 = Instantiate(IceCecilPrefab);
            ice1.transform.localPosition = new Vector3(-width / 4f, northWallY , 0);
            ice1.transform.localScale = new Vector3(0.5f, 2, 1);

            GameObject ice2 = Instantiate(IceCecilPrefab);
            ice2.transform.localPosition = new Vector3(width / 4f, northWallY, 0);
            ice1.transform.localScale = new Vector3(0.5f, 2, 1);
        }
    }
}
