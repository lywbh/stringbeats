using UnityEngine;

public class Tick : MonoBehaviour
{
    public static float triggerGapX = 2F;

    public static float leadTime = 1;
    public static float dismissTime = 0.2F;

    public Line lineObject;
    public EventItem tickInfo;
    public AudioSource audioSource;

    public GameObject perfectHitPrefab;
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
        InvokeRepeating("CheckInput", leadTime, 0.05F);
        vx = tickInfo.x / leadTime;
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

    void CheckInput()
    {
        if (Input.touchSupported)
        {
            foreach (var touch in Input.touches)
            {
                var position = Camera.main.ScreenToWorldPoint(touch.position);
                if (System.Math.Abs(position.x - transform.position.x) < triggerGapX)
                {
                    accuracy = 0F;
                    Destroy(gameObject);
                }
            }
        }
        if (Input.GetMouseButton(0))
        {
            var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (System.Math.Abs(position.x - transform.position.x) < triggerGapX)
            {
                accuracy = 0F;
                Destroy(gameObject);
            }
        }
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
        if (accuracy < dismissTime)
        {
            // perfect 渲染在线上
            var hit = Instantiate(perfectHitPrefab);
            hit.transform.position = new Vector3(tickInfo.x, lineObject.transform.position.y, 0);
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
}
