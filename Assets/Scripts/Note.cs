using UnityEngine;

public class Note : MonoBehaviour
{
    public static float triggerGapX = 2F;

    public static float leadTime = 1;
    public static float dismissTime = 0.2F;

    public Line lineObject;
    public EventItem noteInfo;
    public AudioSource audioSource;

    public GameObject perfectHitPrefab;
    public GameObject greatHitPrefab;
    public GameObject goodHitPrefab;
    public GameObject missHitPrefab;

    private Animator animator;

    private readonly float h = 9F;
    private float vx;
    private float vy;
    private float g;

    private float accuracy;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        animator.Play("Show");
        Invoke("Dismiss", leadTime);
        vx = noteInfo.x / leadTime;
        var temp = System.Math.Pow(2, 1.5) + 3;
        g = (float)temp * h;
        vy = (float)System.Math.Sqrt(2 * temp) * h;
        transform.position = new Vector3(0, lineObject.transform.position.y - h / 2, 0);
    }

    void Dismiss()
    {
        vx = 0F;
        accuracy = dismissTime;
        animator.Play("Dismiss");
        Destroy(gameObject, dismissTime);
    }

    void Update()
    {
        var dv = g * Time.deltaTime / 2;
        vy -= dv;
        transform.Translate(new Vector3(vx, vy, 0) * Time.deltaTime, Space.World);
        vy -= dv;
    }

    void OnDestroy()
    {
        if (accuracy < 0.05)
        {
            // 完美判定 渲染在线上
            var hit = Instantiate(perfectHitPrefab);
            hit.transform.position = new Vector3(noteInfo.x, lineObject.transform.position.y, 0);
            hit.transform.parent = lineObject.transform.parent;
            hit.GetComponent<ParticleSystem>().Play();
        }
        else if (accuracy < 0.1)
        {
            // great 渲染在note处
            var hit = Instantiate(greatHitPrefab);
            hit.transform.position = transform.position;
            hit.transform.parent = lineObject.transform.parent;
            hit.GetComponent<ParticleSystem>().Play();

        }
        else if (accuracy < 0.15)
        {
            // good 渲染在note处
            var hit = Instantiate(goodHitPrefab);
            hit.transform.position = transform.position;
            hit.transform.parent = lineObject.transform.parent;
            hit.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            // miss 渲染在note处
            var hit = Instantiate(missHitPrefab);
            hit.transform.position = transform.position;
            hit.transform.parent = lineObject.transform.parent;
            hit.GetComponent<ParticleSystem>().Play();
        }
    }

    public void OnTouchDown(Vector2 position)
    {
        var acc = System.Math.Abs(audioSource.time - noteInfo.offset);
        if (acc < 0.2)
        {
            accuracy = acc;
            Destroy(gameObject);
        }
    }

}
