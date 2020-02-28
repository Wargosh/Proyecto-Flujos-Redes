using System.Collections.Generic;

public class Nodo
{
    public string id;
    public List<Arista> aristas = new List<Arista>();
    public string estado = "blanco"; // [blanco, gris, negro] || sin visitas, parcialmente visitado, visitado toda sus aristas
    public string arbol = ""; // indica a que arbol pertenece
}
