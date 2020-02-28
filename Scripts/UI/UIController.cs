using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Inputs")]
    public TMP_InputField inputIdVertice;
    public TMP_InputField inputValorArista;
    public TMP_Dropdown cbxEnlace1;
    public TMP_Dropdown cbxEnlace2;
    public TMP_Dropdown cbxAlgoritmo;
    public TMP_Dropdown cbxMin_Max; // obtiene si desea el flujo minimo o maximo de Kruskal

    [Header("Outputs")]
    public TextMeshProUGUI txtObjEliminar; // presenta el nombre del objeto a eliminar
    public TextMeshProUGUI txtResultadoA; // presenta el valor resultado del algoritmo
    public TextMeshProUGUI txtTotalAristas; // presenta el total de aristas
    public TextMeshProUGUI txtTotalNodos; // presenta el total de nodos

    [Header("Paneles")]
    public GameObject panelEliminar;
    public GameObject panelCreator;
    public GameObject panelAlgoritmos;
    public GameObject panelResultados;
    public GameObject btnReset;
    public GameObject btnMostrarRes;
    public GameObject btnOcultarRes;

    [Header("Prefabs")]
    public GameObject prefabVertice;
    public GameObject prefabLine;
    public Transform parentVertice; // este objeto sirve para emparentar el objeto creado en la jerarquia del objeto establecido

    public List<GameObject> listObjVertices = new List<GameObject>();
    public List<GameObject> listObjAristas = new List<GameObject>();
    public List<Arista> listaAristas = new List<Arista>(); // almacena las aristas e informacion de sus enlaces y valor
    public List<Nodo> listaNodos = new List<Nodo>(); //almacena los vertices con info de todas sus aristas

    int contCbx = 0; // evita el uso innecesario de recursos en la función update

    public static UIController Instance { get; set; }
    AlgoritmoKruskal algoritmoKruskal;

    /*************************************** GENERAL ***************************************/
    private void Awake()
    {
        Instance = this;
        algoritmoKruskal = GetComponent<AlgoritmoKruskal>();
    }

    void Start()
    {
        MostrarPanelCreator();
        OcultarPanelEliminar();
    }

    private void Update()
    {
        if (listObjVertices.Count != contCbx)
        {
            contCbx = listObjVertices.Count;
            cbxEnlace1.options.Clear();
            cbxEnlace2.options.Clear();
            foreach (var i in listObjVertices)
            {
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
                data.text = i.name;
                cbxEnlace1.options.Add(data);
                cbxEnlace2.options.Add(data);
            }
        }
    }

    /*************************************** BOTONES DE ACCIÓN ***************************************/
    public void BtnCrearVertice() {
        if (inputIdVertice.text != "")
        {
            // verificar que no exista ese nombre...
            if (VerificarNombreVertice(inputIdVertice.text))
            {
                GameObject obj = (GameObject)Instantiate(prefabVertice);
                
                Nodo nodo = new Nodo();
                nodo.id = obj.name = inputIdVertice.text.ToUpper();
                obj.GetComponent<RectTransform>().localPosition = new Vector3(50f, 50f, 0f);
                obj.GetComponent<VerticeController>().CambiarNombre(inputIdVertice.text);
                obj.transform.parent = parentVertice;
                obj.transform.localScale = new Vector3(.5f, .5f, .5f);

                listObjVertices.Add(obj);
                listaNodos.Add(nodo);

                inputIdVertice.text = "";
            }
            else {
                Debug.Log("Este nombre de vertice ya existe...");
            }         
        }
    }

    public void BtnCrearArista()
    {
        if (inputValorArista.text != "")
        {
            string enlace1 = cbxEnlace1.options[cbxEnlace1.value].text;
            string enlace2 = cbxEnlace2.options[cbxEnlace2.value].text;
            if (enlace1 != enlace2) {
                GameObject obj = Instantiate(prefabLine);

                AristaController arista = obj.GetComponent<AristaController>();
                int valor = System.Convert.ToInt16(inputValorArista.text);
                arista.CambiarValor(valor);

                RectTransform en1 = GameObject.Find(enlace1).GetComponent<RectTransform>();
                RectTransform en2 = GameObject.Find(enlace2).GetComponent<RectTransform>();

                arista.EstablecerPuntos(en1, en2);
                listObjAristas.Add(obj);

                // modelo para la nueva arista
                Arista aris = new Arista();
                aris.id = enlace1 + "" + enlace2; // para identificar más rapido una arista
                aris.valor = valor;
                aris.enlace1 = enlace1;
                aris.enlace2 = enlace2;

                listaAristas.Add(aris);

                // almacenar aristas en los nodos correspondientes
                int c = 0;
                foreach (var n in listaNodos) {
                    if (n.id == aris.enlace1 || n.id == aris.enlace2) {
                        c++;
                        n.aristas.Add(aris);
                    }

                    if (c == 2) {
                        break;
                    }
                }

                inputValorArista.text = "";
            } else {
                Debug.Log("Los enlaces a conectar deben ser diferentes...");
            }
        } else {
            Debug.Log("Es necesario establecer un valor para la arista...");
        }
    }

    public void BtnEliminarVerticeSeleccionado() {
        if (txtObjEliminar.text != "")
        {
            // eliminar
            EliminarVertice_Aristas(txtObjEliminar.text);

            // volver a ocultar el panel, para evitar errores...
            OcultarPanelEliminar();
        }
    }

    public void BtnReset() {
        foreach (var oa in listObjAristas) {
            oa.SetActive(true);
        }
        OcultarBtnReset();
        BtnOcultarResultados();
    }

    /*************************************** OTRAS FUNCIONES ***************************************/
    public void ReasignarPosicionesAristas(string nombre, Vector3 posVertice)
    {
        for (int a = 0; a < listaAristas.Count; a++)
        {
            if (listaAristas[a].enlace1 == nombre)
            {
                listObjAristas[a].GetComponent<AristaController>().ActualizarPosicion(posVertice, 0);
            }
            else if (listaAristas[a].enlace2 == nombre)
            {
                listObjAristas[a].GetComponent<AristaController>().ActualizarPosicion(posVertice, 1);
            }
        }
    }

    private bool VerificarNombreVertice(string nombre) {
        foreach (var i in listObjVertices) {
            if (nombre == i.name)
                return false; // no es posible seguir, el nombre esta repetido
        }
        return true; // es posible seguir
    }

    private void EliminarVertice_Aristas(string nombre) {
        // El vertice a eliminar debe llevarse consigo a las aristas que le correspondan (Si, es cruel)
        for (int i = 0; i < listaNodos.Count; i++) {
            if (listaNodos[i].id == nombre) {
                //busca aristas relacionadas a este vertice
                for (int a = listaAristas.Count - 1; a >= 0; a--) {// bucle inverso para ir eliminando las aristas
                    if (listaAristas[a].enlace1 == nombre || listaAristas[a].enlace2 == nombre) {// elimina todas las aristas que unan el vertice
                        listaAristas.RemoveAt(a);
                        Destroy(listObjAristas[a]);
                        listObjAristas.RemoveAt(a);

                        // aqui no puede ir un break...
                    }
                }

                //elimina el vertice
                listaNodos.RemoveAt(i);
                Destroy(listObjVertices[i]);
                listObjVertices.RemoveAt(i);
                break;
            }
        }
    }

    public void PresentarResultadoAlgoritmo(string res, int tAristas) {
        txtResultadoA.text = "Valor Total: " + res;
        txtTotalAristas.text = "Total Aristas: " + tAristas;
        txtTotalNodos.text = "Total Nodos: " + listaNodos.Count;
    }

    /*************************************** LLAMADA A ALGORITMOS ***************************************/
    public void BtnEjecutarAlgoritmo() {
        string seleccion = cbxAlgoritmo.options[cbxAlgoritmo.value].text;

        switch (seleccion) {
            case "Algoritmo de Kruskal":
                algoritmoKruskal.Algoritmo(listaAristas, listaNodos, cbxMin_Max.options[cbxMin_Max.value].text);
                break;
            case "Algoritmo de Prim":

                break;
        }
        MostrarBtnReset();
        BtnOcultarResultados();
    }

    /*************************************** MOSTRAR / OCULTAR PANELES ***************************************/
    public void OcultarPanelEliminar() {
        txtObjEliminar.text = "";
        panelEliminar.SetActive(false);
    }

    public void MostrarPanelEliminar(string nombre) {
        panelEliminar.SetActive(true);
        // muestra el objeto eliminar
        txtObjEliminar.text = nombre;
    }

    public void MostrarPanelAlgoritmos() {
        panelAlgoritmos.SetActive(true);
        panelCreator.SetActive(false);
    }

    public void MostrarPanelCreator() {
        panelAlgoritmos.SetActive(false);
        panelCreator.SetActive(true);
    }

    public void OcultarBtnReset() {
        btnReset.SetActive(false);
    }

    public void MostrarBtnReset() {
        btnReset.SetActive(true);
    }

    public void BtnMostrarResultados() {
        panelResultados.SetActive(true);
        btnMostrarRes.SetActive(false);
        btnOcultarRes.SetActive(true);
    }
    
    public void BtnOcultarResultados() {
        panelResultados.SetActive(false);
        btnMostrarRes.SetActive(true);
        btnOcultarRes.SetActive(false);
    }
}
