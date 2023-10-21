using UnityEngine;

namespace Randomiser;

public class ActivateMultipleBasedOnCondition : MonoBehaviour
{
    public GameObject[] Objects;
    public bool Activate;
    public Condition Condition;

    private void Awake()
    {
        Run();
    }

    private void FixedUpdate()
    {
        Run();
    }

    private void Run()
    {
        if (Condition)
        {
            var active = Condition.Validate(null) == Activate;
            foreach (var obj in Objects)
                obj.SetActive(active);
        }
    }
}
