using UnityEngine;
using TMPro;

public class AristaController : MonoBehaviour
{
    public TextMeshPro txtValor;

    LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }
    private void Start()
    {
        ObtenerPosicionEntreVertices();
    }

    public void CambiarValor(int valor)
    {
        txtValor.text = valor + "";
    }

    public void EstablecerPuntos(RectTransform en1, RectTransform en2) {
        Vector3 enlace1 = new Vector3(en1.localPosition.x / 11.5f, en1.localPosition.y / 11.5f, 0f);
        Vector3 enlace2 = new Vector3(en2.localPosition.x / 11.5f, en2.localPosition.y / 11.5f, 0f);

        line.SetPosition(0, enlace1);
        line.SetPosition(1, enlace2);

        ObtenerPosicionEntreVertices();
    }

    public void ActualizarPosicion(Vector3 pos, int posicion)
    {
        Vector3 enlaceEdit = new Vector3(pos.x / 11.5f, pos.y / 11.5f, 0f);

        line.SetPosition(posicion, enlaceEdit);

        ObtenerPosicionEntreVertices();
    }

    public void ObtenerPosicionEntreVertices() {
        if (line.positionCount > 1) {
            Vector3 verticeA = line.GetPosition(0);
            Vector3 verticeB = line.GetPosition(1);

            //var posCentral = (verticeA + verticeB).normalized;
            var posCentral = Vector3.Lerp(verticeA, verticeB, 0.5f);
            //Debug.Log(posCentral.x + ", " + posCentral.y + ", " + posCentral.z);
            txtValor.rectTransform.position = posCentral;
        }
    }
}
