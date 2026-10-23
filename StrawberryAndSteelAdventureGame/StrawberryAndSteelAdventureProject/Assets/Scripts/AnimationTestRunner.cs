using System;
using System.Collections;
using UnityEngine;

public class AnimationTestRunner : MonoBehaviour
{
    [Serializable]
    public class TestCharacter
    {
        public string label = "Karakter";
        [Tooltip("Zorunlu: test edilecek Animator Controller")]
        public RuntimeAnimatorController controller;
        [Tooltip("Klipler Sprite'ý animate ediyorsa zorunlu deðil; baþlangýç görüntüsü içindir")]
        public Sprite defaultSprite;
        [Tooltip("Klipleri bir child objede kaydettiysen o objenin adý (ör. Visual). Root'ta kaydettiysen boþ býrak")]
        public string spriteChildName = "";
        public Vector3 position;
        public Vector3 scale = Vector3.one;
        public int sortingOrder = 0;

        [HideInInspector] public GameObject instance;
        [HideInInspector] public Animator animator;
    }

    [Header("Test Karakterleri (2 adet)")]
    public TestCharacter[] characters = new TestCharacter[2];

    [Header("Test Ayarlarý")]
    public string[] parameterSequence = { "IsIdle", "IsRun", "IsDeath", "IsHurt", "IsAttack" };
    [Min(0.5f)] public float secondsPerState = 3f;
    public bool loop = true;
    public bool autoStart = true;
    [Tooltip("Her state'ten önce Animator sýfýrlanýr (Death gibi çýkýþý olmayan state'ler için)")]
    public bool rebindBeforeEachState = true;

    private string currentState = "-";
    private Coroutine routine;

    private void Reset()
    {
        characters = new TestCharacter[]
        {
            new TestCharacter { label = "Karakter A", position = new Vector3(-2f, 0f, 0f) },
            new TestCharacter { label = "Karakter B", position = new Vector3( 2f, 0f, 0f) }
        };
    }

    private void Start()
    {
        SpawnCharacters();
        if (autoStart) StartTest();
    }

    private void SpawnCharacters()
    {
        foreach (var c in characters)
        {
            if (c.controller == null)
            {
                Debug.LogWarning($"[AnimTest] {c.label}: Animator Controller atanmamýþ, atlandý.");
                continue;
            }

            // Root: Animator burada durur
            var root = new GameObject(c.label);
            root.transform.SetParent(transform);
            root.transform.position = transform.position + c.position;
            root.transform.localScale = c.scale;

            // SpriteRenderer: root'ta ya da child'da (klip path'ine göre)
            GameObject spriteHost = root;
            if (!string.IsNullOrEmpty(c.spriteChildName))
            {
                spriteHost = new GameObject(c.spriteChildName);
                spriteHost.transform.SetParent(root.transform, false);
            }

            var sr = spriteHost.AddComponent<SpriteRenderer>();
            sr.sprite = c.defaultSprite;
            sr.sortingOrder = c.sortingOrder;

            var anim = root.AddComponent<Animator>();
            anim.runtimeAnimatorController = c.controller;

            c.instance = root;
            c.animator = anim;
        }
    }

    public void StartTest()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(RunSequence());
    }

    public void StopTest()
    {
        if (routine != null) StopCoroutine(routine);
        routine = null;
    }

    private IEnumerator RunSequence()
    {
        do
        {
            foreach (string param in parameterSequence)
            {
                currentState = param;
                Debug.Log($"[AnimTest] >>> {param}");

                foreach (var c in characters)
                    ApplyState(c, param);

                yield return new WaitForSeconds(secondsPerState);
            }
        } while (loop);

        currentState = "Bitti";
    }

    private void ApplyState(TestCharacter c, string target)
    {
        if (c.animator == null) return;
        Animator anim = c.animator;

        if (rebindBeforeEachState)
        {
            anim.Rebind();
            anim.Update(0f);
        }

        bool found = false;

        foreach (var p in anim.parameters)
        {
            if (p.type == AnimatorControllerParameterType.Bool)
            {
                anim.SetBool(p.name, p.name == target);
                if (p.name == target) found = true;
            }
            else if (p.type == AnimatorControllerParameterType.Trigger)
            {
                anim.ResetTrigger(p.name);
                if (p.name == target)
                {
                    anim.SetTrigger(p.name);
                    found = true;
                }
            }
        }

        if (!found)
            Debug.LogWarning($"[AnimTest] {c.label}: '{target}' parametresi bulunamadý (Bool/Trigger).");
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 220, 400));
        GUILayout.Label($"Aktif State: {currentState}");

        if (GUILayout.Button("Testi Baþlat")) StartTest();
        if (GUILayout.Button("Durdur")) StopTest();

        GUILayout.Space(8);
        foreach (string param in parameterSequence)
        {
            if (GUILayout.Button(param))
            {
                StopTest();
                currentState = param;
                foreach (var c in characters) ApplyState(c, param);
            }
        }
        GUILayout.EndArea();
    }
}