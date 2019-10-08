using UnityEngine;

public class Line : MonoBehaviour
{
    public static float triggerGapY = 4F;

    public static float leadTime = 2;
    public static float dismissTime = 1;

    public LineItem lineInfo;
    public AudioSource audioSource;
    public Note notePrefab;
    public Tick tickPrefab;

    private Animator animator;

    private int eventClip = 0;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        transform.position = new Vector3(0, lineInfo.y, 0);
        animator.Play("Show");
        Invoke("Dismiss", leadTime + lineInfo.duration);
    }

    void Dismiss()
    {
        animator.Play("Dismiss");
        Destroy(gameObject, dismissTime);
    }

    void Update()
    {
        CheckAndGenNote(lineInfo.events, audioSource.time);
    }

    void CheckAndGenNote(EventItem[] events, float audioTime)
    {
        if (eventClip >= 0 && eventClip < events.Length)
        {
            var currentEvent = events[eventClip];
            if (audioTime >= currentEvent.offset - Note.leadTime)
            {
                switch (currentEvent.type)
                {
                    case "NOTE":
                        Note noteObject = Instantiate(notePrefab);
                        noteObject.lineObject = this;
                        noteObject.noteInfo = currentEvent;
                        noteObject.audioSource = audioSource;
                        noteObject.transform.SetAsLastSibling();
                        noteObject.transform.parent = transform.parent;
                        break;
                    case "TICK":
                        Tick tickObject = Instantiate(tickPrefab);
                        tickObject.lineObject = this;
                        tickObject.tickInfo = currentEvent;
                        tickObject.audioSource = audioSource;
                        tickObject.transform.SetAsLastSibling();
                        tickObject.transform.parent = transform.parent;
                        break;
                }
                eventClip++;
            }
        }
    }

    public void OnTouchDown(Vector2 position)
    {
        Transform nearlyNote = null;
        foreach (Transform child in transform.parent)
        {
            if ("Note(Clone)" == child.name
                && System.Math.Abs(position.x - child.position.x) < Note.triggerGapX)
            {
                nearlyNote = child;
                break;
            }
        }
        if (nearlyNote != null)
        {
            nearlyNote.GetComponent<Note>().OnTouchDown(position);
        }
    }

}
