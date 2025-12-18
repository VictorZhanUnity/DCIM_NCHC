using System;
using System.Collections.Concurrent;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher instance;
    private static readonly ConcurrentQueue<Action> actions = new();

    public static void Enqueue(Action action)
    {
        if (action == null) return;

        // 還沒存在就自動建立
        if (instance == null)
        {
            var go = new GameObject("[MainThreadDispatcher]");
            instance = go.AddComponent<MainThreadDispatcher>();
            DontDestroyOnLoad(go);
        }

        actions.Enqueue(action);
    }

    private void Update()
    {
        while (actions.TryDequeue(out var action))
        {
            action.Invoke();
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }
}