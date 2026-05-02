using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        List<Publicacion> biblioteca = new List<Publicacion>();
        CargarBiblioteca(biblioteca);
        int opcion;
        do
        {
            Console.Write("=== Bienvenido a la biblioteca ===\n" +
                              "1. Agregar libro\n" +
                              "2. Agregar revista\n" +
                              "3. Listar\n" +
                              "4. Prestar libro\n" +
                              "5. Salir\n" +
                              "Opcion: ");
            VerificarEntero(out opcion);

            switch(opcion)
            {
                case 1:
                    CrearLibro(biblioteca);
                    Console.WriteLine("\nLibro agregado con éxito\n");
                    break;
                case 2:
                    CrearRevista(biblioteca);
                    Console.WriteLine("\nRevista agregada con éxito\n");
                    break;
                case 3:
                    if (biblioteca != null && biblioteca.Count() > 0)
                    {
                        ListarPublicaciones(biblioteca);
                    }
                    else
                    {
                        Console.WriteLine("\nNo hay publicaciones en la lista\n");
                    }
                    break;
                case 4:
                    Console.WriteLine("Ingrese el ISBN del libro a prestar: ");
                    string ISBNAux;
                    VerificarString(out ISBNAux);
                    var libro = biblioteca.OfType<Libro>().FirstOrDefault(libro => libro.ISBN == ISBNAux);
                    if (libro != null)
                    {
                        libro.Prestar();
                    }
                    else
                    {
                        Console.WriteLine("\nNo se ha encontrado el libro\n");
                    }
                    break;
                case 5:
                    GuardarBiblioteca(biblioteca);
                    Console.WriteLine("\nSaliendo del programa...");
                    break;
                default:
                    break;
            }
        } while (opcion != 5);
    }

    static void VerificarEntero(out int numero)
    {
        bool ok;

        do
        {
            ok = int.TryParse(Console.ReadLine(), out numero);
            if (!ok)
            {
                Console.WriteLine("\nError al tratar de guardar el dato\n");
            }
        } while (!ok);
    }

    static void VerificarDecimal(out double numero)
    {
        bool ok;

        do
        {
            ok = double.TryParse(Console.ReadLine(), out numero);
            if (!ok)
            {
                Console.WriteLine("\nError al tratar de guardar el dato\n");
            }
        } while (!ok);
    }

    static void VerificarString(out string palabra)
    {
        do
        {
            palabra = Console.ReadLine() ?? "";

            if (palabra == null || palabra == "")
            {
                Console.WriteLine("\nError al ingresar la palabra deseada\n");
            }
        } while (palabra == null || palabra == "");
    }

    static void CrearLibro(List<Publicacion> listado)
    {
        string tituloAux;
        string ISBNAux;
        int anioPublicacionAux;
        string autorAux;
        string generoAux;

        Console.Write("\nTítulo del libro: ");
        VerificarString(out tituloAux);
        Console.Write("ISBN: ");
        VerificarString(out ISBNAux);
        Console.Write("Año de publicación: ");
        VerificarEntero(out anioPublicacionAux);
        Console.Write("Autor: ");
        VerificarString(out autorAux);
        Console.Write("Género: ");
        VerificarString(out generoAux);
        Libro ejemplar = new Libro(tituloAux, ISBNAux, anioPublicacionAux, autorAux, generoAux);
        listado.Add(ejemplar);
    }

    static void CrearRevista(List<Publicacion> listado)
    {
        string tituloAux;
        string ISBNAux;
        int anioPublicacionAux;
        int numeroEdicionAux;
        string tematicaAux;

        Console.Write("Título de la revista: ");
        VerificarString(out tituloAux);
        Console.Write("ISBN: ");
        VerificarString(out ISBNAux);
        Console.Write("Año de publicación: ");
        VerificarEntero(out anioPublicacionAux);
        Console.Write("Numero de edicion: ");
        VerificarEntero(out numeroEdicionAux);
        Console.Write("Temática: ");
        VerificarString(out tematicaAux);
        Revista ejemplar = new Revista(tituloAux, ISBNAux, anioPublicacionAux, numeroEdicionAux, tematicaAux);
        listado.Add(ejemplar);
    }

    static void ListarPublicaciones(List<Publicacion> listado)
    {
        var libros = listado.Where(ejemplar => ejemplar is Libro);
        if (libros != null && libros.Count() > 0)
        {
            Console.WriteLine("\n=== LIBROS ===");
            foreach (var ejemplar in libros)
            {
                Console.WriteLine(ejemplar.ObtenerResumen());
            }
        }

        var revistas = listado.Where(ejemplar => ejemplar is Revista);
        if (revistas != null && revistas.Count() > 0)
        {
            Console.WriteLine("=== REVISTAS ===");
            foreach (var ejemplar in revistas)
            {
                Console.WriteLine(ejemplar.ObtenerResumen());
            }
        }
    }

    static void GuardarBiblioteca(List<Publicacion> listado)
    {
        try
        {
            using (StreamWriter archivo = new StreamWriter("biblioteca.txt"))
            {
                foreach(Publicacion item in listado)
                {
                    archivo.WriteLine(item.ToCsv());
                }
            }
        }
        catch(IOException ex)
        {
            Console.WriteLine("Error de IO: " + ex.Message);
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error al crear el archivo: " + ex.Message);
        }
    }

    static void CargarBiblioteca(List<Publicacion> listado)
    {
        try
        {
            using (StreamReader archivo = new StreamReader("biblioteca.txt"))
            {
                string? buffer;
                string[] linea;
                while((buffer = archivo.ReadLine()) != null)
                {
                    linea = buffer.Split("|");

                    if (linea[0] == "Libro")
                    {
                        Libro ejemplar = new Libro(linea[1], linea[2], int.Parse(linea[3]), linea[4], linea[5]);
                        listado.Add(ejemplar);
                    }
                    else
                    {
                        Revista ejemplar = new Revista(linea[1], linea[2], int.Parse(linea[3]), int.Parse(linea[4]), linea[5]);
                        listado.Add(ejemplar);
                    }
                }
                
            }
        }
        catch(FileNotFoundException ex)
        {
            Console.WriteLine("Error al buscar archivo: " + ex.Message);
        }
        catch(IOException ex)
        {
            Console.WriteLine("Error por archivo usado en otro proceso: " + ex.Message);
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}

public interface IPrestable
{
    void Prestar();
    void Devolver();
}
public abstract class Publicacion
{
    public string Titulo { get; set; }
    public string ISBN { get; set; }
    public int AnioPublicacion { get; set; }

    public Publicacion(string titulo, string isbn, int anioPublicacion)
    {
        this.Titulo = titulo;
        this.ISBN = isbn;
        this.AnioPublicacion = anioPublicacion;
    }

    public virtual string ObtenerResumen()
    {
        return $"Titulo: {Titulo}\nISBN: {ISBN}\nAño de publicación: {AnioPublicacion}";
    }

    public virtual string ToCsv()
    {
        return $"Tipo: x | Titulo: {Titulo} | ISBN: {ISBN} | Año: {AnioPublicacion}";
    }
}

public class Libro : Publicacion, IPrestable
{
    public string Autor { get; set; }
    public string Genero { get; set; }

    public Libro(string titulo, string isbn, int anioPublicacion, string autor, string genero) : base(titulo, isbn, anioPublicacion)
    {
        this.Autor = autor;
        this.Genero = genero;
    }

    public void Prestar()
    {
        Console.WriteLine($"El libro {Titulo} ha sido prestado");
    }

    public void Devolver()
    {
        Console.WriteLine($"El libro {Titulo} ha sido devuelto");
    }
    public override string ObtenerResumen()
    {
        return $"Titulo: {Titulo}\nISBN: {ISBN}\nAño de publicación: {AnioPublicacion}\nAutor: {Autor}\nGénero: {Genero}";
    }

    public override string ToCsv()
    {
        return $"Libro|{Titulo}|{ISBN}|{AnioPublicacion}|{Autor}|{Genero}";
    }
}

public class Revista : Publicacion
{
    public int NumeroEdicion { get; set; }
    public string Tematica { get; set; }

    public Revista(string titulo, string isbn, int anioPublicacion, int numeroEdicion, string tematica) : base(titulo, isbn, anioPublicacion)
    {
        this.NumeroEdicion = numeroEdicion;
        this.Tematica = tematica;
    }

    public override string ObtenerResumen()
    {
        return $"Titulo: {Titulo}\nISBN: {ISBN}\nAño de publicación: {AnioPublicacion}\nNumero de edición: {NumeroEdicion}\nTemática: {Tematica}";
    }

    public override string ToCsv()
    {
        return $"Revista|{Titulo}|{ISBN}|{AnioPublicacion}|{NumeroEdicion}|{Tematica}";
    }
}