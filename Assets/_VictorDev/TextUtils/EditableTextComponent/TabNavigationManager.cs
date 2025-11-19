using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _VictorDev.TextUtils
{
    
public class TabNavigationManager : MonoBehaviour
{
    public List<Selectable> inputs;
    public EventSystem eventSystem;

    [Button]
    void Start()
    {
        eventSystem = EventSystem.current;

        // 抓出所有 InputField 與 TMP_InputField
        inputs = GetComponentsInChildren<Selectable>(true)
            .Where(s => s is InputField || s is TMP_InputField)
            .OrderBy(s => s.transform.GetSiblingIndex())  // 第一層排序
            .ThenBy(s => s.transform.GetHierarchyPath())  // 深度排序（保證唯一）
            .ToList();
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return;

        // Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool shift = Input.GetKeyDown(KeyCode.LeftShift);
            MoveFocus(shift ? -1 : 1);
        }

        // Enter = 下一格
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            MoveFocus(1);
        }
    }

    private void MoveFocus(int direction)
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
        
        var current = eventSystem.currentSelectedGameObject?.GetComponent<Selectable>();
        if (current == null) return;

        int idx = inputs.IndexOf(current);
        if (idx < 0) return;

        int next = idx;

        // 找下一個正常的 input（跳過 disabled）
        for (int i = 0; i < inputs.Count; i++)
        {
            next += direction;

            if (next >= inputs.Count) next = 0;
            if (next < 0) next = inputs.Count - 1;

            if (inputs[next].interactable && inputs[next].gameObject.activeInHierarchy)
            {
                Debug.Log($"next {next}");
                inputs[next].Select();
                scrollRect.ScrollToChild(inputs[next].transform as RectTransform);
                return;
            }
        }
    }

    public ScrollRect scrollRect;
}
}

