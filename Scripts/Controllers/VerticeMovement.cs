using UnityEngine;
using UnityEngine.EventSystems;

public class VerticeMovement : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        UIController.Instance.MostrarPanelEliminar(this.gameObject.name);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        Vector3 pos = rectTransform.localPosition;

        UIController.Instance.ReasignarPosicionesAristas(this.gameObject.name, pos);
        UIController.Instance.MostrarPanelEliminar(this.gameObject.name);
    }
}
