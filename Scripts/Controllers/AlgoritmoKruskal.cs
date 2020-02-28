using System.Collections.Generic;
using UnityEngine;

public class AlgoritmoKruskal : MonoBehaviour
{
    List<Arista> aristas = new List<Arista>(); // aristas que seran modificadas durante la comprobación
    List<Arista> aristasTotal = new List<Arista>(); // total de aristas
    List<Arista> camino = new List<Arista>(); // almacena las aristas que son seleccionadas
    List<Nodo> nodos = new List<Nodo>(); // total de nodos
    string opcion = "Minimo"; // por defecto busca el menor valor (Tradicional)

    public void Algoritmo(List<Arista> listaAristas, List<Nodo> listaNodos, string opMaxMin) {
        nodos = listaNodos; // primero debe ir este...
        ResetListas(); // para luego reiniciarlos en caso de que los modelos ya hayan sido modificados
        aristasTotal = listaAristas; // y este pues se entiende
        opcion = opMaxMin;

        foreach (var i in listaAristas) {
            Arista a = new Arista();
            a.id = i.id;
            a.valor = i.valor;
            a.enlace1 = i.enlace1;
            a.enlace2 = i.enlace2;
            aristas.Add(a);
        }

        // Primero - recoge todas las aristas de menor valor/peso (evitando ciclos)
        while (camino.Count < UIController.Instance.listObjVertices.Count - 1) {
            int pos = BuscarAristaMenor();
            if (pos != -1) {
                Arista a = aristas[pos];
                aristas.RemoveAt(pos); // elimina la arista seleccionada de la lista

                BuscarEnArbol(a);
            }
        }

        //Segundo - bloquear todas las aristas en pantalla
        BloquearTodasAristas();

        //Tercero - mostrar unicamente las aristas del camino óptimo
        MostrarCamino();
    }

    public int BuscarAristaMenor() { // se encarga de buscar y devolver la arista con menor valor/peso
        if (aristas.Count > 0) {
            int menor = aristas[0].valor; // primer valor por defecto
            int posicionArista = 0; // primera posicion por defecto
            int total = aristas.Count - 1;
            for (int a = total; a >= 0; a--) {
                if (opcion == "Minimo") {
                    if (aristas[a].valor < menor) {
                        menor = aristas[a].valor;
                        posicionArista = a;
                    }
                } else {
                    if (aristas[a].valor > menor) {
                        menor = aristas[a].valor;
                        posicionArista = a;
                    }
                }
            }
            return posicionArista;
        }
        else {
            Debug.Log("No existen aristas...");
        }
        return -1; // si no se encontro nada...
    }

    public void BuscarEnArbol(Arista aris) {
        int c = 0; int[] pos = new int[2];
        for (int n = 0; n < nodos.Count; n++) { // encontrar ambos nodos unidos por la arista
            if (aris.enlace1 == nodos[n].id || aris.enlace2 == nodos[n].id) {
                pos[c] = n; //almacena la posicion de los nodos
                c++;
            }
            if (c == 2) {
                break;
            }
        }

        string arbol1 = nodos[pos[0]].arbol;
        string arbol2 = nodos[pos[1]].arbol;

        // primer caso, ninguno de los dos pertenece a un arbol...
        if (arbol1 == "" && arbol2 == "") {
            arbol1 = nodos[pos[0]].arbol = nodos[pos[1]].arbol = nodos[pos[0]].id + "," + nodos[pos[1]].id;
            camino.Add(aris); // almacena la aristas seleccionada
            Debug.Log("1 | nodo: " + nodos[pos[0]].id + " ahora pertenece a: {" + arbol1 + "}");
            Debug.Log("1 | nodo: " + nodos[pos[1]].id + " ahora pertenece a: {" + arbol1 + "}");
            return;
        }

        // segundo caso, alguno de los dos pertenece a un arbol...
        if (arbol1 == "" && arbol2 != "") {
            nodos[pos[0]].arbol = arbol2;
            camino.Add(aris); // almacena la aristas seleccionada
            Debug.Log("2 | nodo: " + nodos[pos[0]].id + " ahora pertenece a: {" + arbol2 + "}");
            return;
        } else if (arbol1 != "" && arbol2 == "") {
            nodos[pos[1]].arbol = arbol1;
            camino.Add(aris); // almacena la aristas seleccionada
            Debug.Log("2 | nodo: " + nodos[pos[1]].id + " ahora pertenece a: {" + arbol1 + "}");
            return;
        }

        // tercer caso, ambos nodos pertenecen a un arbol [ATENCION]
        // En este caso es donde se pueden presentar ciclos de no controlarse bien...
        if (arbol1 != "" && arbol2 != "") {
            // si se encuentra en el mismo arbol descartar de inmediato
            if (arbol1 == arbol2) {
                Debug.Log("ARISTA: <" + aris.id + "> NO entra por crear un ciclo.");
                return;
            } else {
                foreach (var m in nodos) {
                    if (m.arbol == arbol1) {
                        m.arbol = arbol2;
                        Debug.Log("3 | nodo: " + m.id + " ahora pertenece a: {" + m.arbol + "}");
                    }
                }
                camino.Add(aris); // almacena la aristas seleccionada
            }
        }
    }

    public void ResetListas() {
        foreach (var n in nodos) {
            n.arbol = "";
        }
        aristas.Clear();
        camino.Clear();
    }

    private void MostrarCamino() {
        int valorTotal = 0;
        foreach (var c in camino) {
            for (int i = 0; i < aristasTotal.Count; i++) {
                if (c.id == aristasTotal[i].id) {
                    UIController.Instance.listObjAristas[i].SetActive(true);
                    break;
                }
            }
            valorTotal += c.valor;
        }
        UIController.Instance.PresentarResultadoAlgoritmo(valorTotal.ToString(), camino.Count);
    }

    private void BloquearTodasAristas() {
        foreach (var a in UIController.Instance.listObjAristas) {
            a.SetActive(false);
        }
    }
}