using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelVisibilityController : MonoBehaviour
{
    [Header("Target to Control (defaults to self if empty)")]
    [SerializeField] private GameObject target;

    [Header("Buttons that will open this panel")]
    [SerializeField] private List<Button> openButtons;

    [Header("Buttons that will close this panel")]
    [SerializeField] private List<Button> closeButtons;

    void Awake()
    {
        if (target == null)
        {
            target = gameObject;
        }

        // Hook all open buttons
        foreach (var btn in openButtons)
        {
            if (btn != null)
            {
                btn.onClick.AddListener(Show);
            }
        }

        // Hook all close buttons
        foreach (var btn in closeButtons)
        {
            if (btn != null)
            {
                btn.onClick.AddListener(Hide);
            }
        }
    }

    public void Show()
    {
        if (target != null)
        {
            target.SetActive(true);
        }
    }

    public void Hide()
    {
        if (target != null)
        {
            target.SetActive(false);
        }
    }

    public void Toggle()
    {
        if (target != null)
        {
            target.SetActive(!target.activeSelf);
        }
    }
}
