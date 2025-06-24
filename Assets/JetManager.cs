using Unity.VisualScripting;
using UnityEngine;

public class JetManager : MonoBehaviour
{
    public GameObject waterJetPrefab;
    public AudioAnalyzer analyzer;
    public int count = 8;
    public float spacing = 2f;

    void Start()
    {
        //for (int i = 0; i < count; i++)
        //{
        //    vector3 pos = new vector3(i * spacing, 0, 0);
        //    gameobject jet = instantiate(waterjetprefab, transform.position + pos, quaternion.identity, transform);

        //    var reactive = jet.addcomponent<audioresponsivejet>();
        //    reactive.analyzer = analyzer;
        //    reactive.bandindex = i;
        //}
    }
}
