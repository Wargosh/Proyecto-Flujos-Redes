using UnityEngine;
using TMPro;

public class VerticeController : MonoBehaviour
{
    public TextMeshProUGUI txtNombre;

    public void CambiarNombre(string nombre) {
        txtNombre.text = nombre;
    }
}
