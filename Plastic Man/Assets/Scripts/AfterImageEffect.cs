//using UnityEngine;

//public class AfterImageEffect : MonoBehaviour
//{
//    public float lifeTime = 0.3f;
//    public float fadeSpeed = 5f;

//    SpriteRenderer sr;

//    void Start()
//    {
//        sr = GetComponent<SpriteRenderer>();
//        Destroy(gameObject, lifeTime);
//    }

//    void Update()
//    {
//        Color c = sr.color;
//        c.a -= fadeSpeed * Time.deltaTime;
//        sr.color = c;
//    }
//}

using UnityEngine;

public class AfterImageEffect : MonoBehaviour
{
    public float lifeTime = 0.3f;
    public float fadeSpeed = 5f;

    private SpriteRenderer[] srs;

    void Start()
    {
        srs = GetComponentsInChildren<SpriteRenderer>();
        Destroy(this.gameObject, lifeTime);
    }

    void Update()
    {
        foreach (var sr in srs)
        {
            Color c = sr.color;
            c.a -= fadeSpeed * Time.deltaTime;
            sr.color = c;
        }
    }
}
