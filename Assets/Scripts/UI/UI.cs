using UnityEngine;
using UnityEngine.UIElements;

public abstract class UI : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;

    protected VisualElement root;

    protected virtual void OnEnable()
    {     
        root = uiDocument.rootVisualElement;
    }

    protected void Hide()
    {
        root.style.display = DisplayStyle.None;
        //gameObject.SetActive(false);
    }

    protected void Show()
    {
        root.style.display = DisplayStyle.Flex;
        //gameObject.SetActive(true);
    }
}