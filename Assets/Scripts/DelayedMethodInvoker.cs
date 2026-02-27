using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayedMethodInvoker : MonoBehaviour
{
    public static DelayedMethodInvoker Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void StartMethodDelayed(UnityAction method, float delay)
    {
        StartCoroutine(InvokeMethodAfterDelay(method, delay));
    }

    private IEnumerator InvokeMethodAfterDelay(UnityAction method, float delay)
    {
        yield return new WaitForSeconds(delay);
        method.Invoke();
    }
}
