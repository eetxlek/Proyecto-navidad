using System.Text;

internal class Program
{
    const ushort NUMCABALLOS = 8;
    const ushort LONGITUD_COLUMNAS = 80;

    public void Main(string[] args)
    {
        InicializaVentana();
        int opcion;
        float saldo = 1000;
        StringBuilder datosCarrerasAcabadas = new StringBuilder();
        do
        {
            MenuOpciones();
            opcion = int.Parse(Console.ReadLine());
            if (opcion == 1)
            {
                datosCarrerasAcabadas.Append(JuegaApuestaACarrera(ref saldo) + "\n");
            }
            if (opcion == 2)
            {
                VerPartidasJugadas(datosCarrerasAcabadas.ToString());
            }
            if (opcion == 3)
            {
                Console.WriteLine("Adios!");
                break;
            }
        } while (opcion != 3);
    }
    static void MenuOpciones()
    {
        string menu = "Elije una opción:\n1. Jugar apuesta caballos.\n2. Ver apuestas jugadas.\n3. Salir.\nPulsa una opción:";
        Console.WriteLine(menu);
    }
    public void InicializaVentana()
    {
        const int COLUMNAS_TERMINAL = 90;
        const int FILAS_TERMINAL = 30;
        Console.SetWindowSize(COLUMNAS_TERMINAL, FILAS_TERMINAL);
        Console.SetBufferSize(COLUMNAS_TERMINAL, FILAS_TERMINAL);
        Console.Clear();
    }
    static string JuegaApuestaACarrera(ref float saldo) //juega, genera registro con update saldo
    {
        (ushort caballo, float cantidadApostada, ushort eurosPagadosPorUnoApostado) ticketApuesta = Apostar(saldo, NUMCABALLOS);
        Console.Clear();
        string cabecera = CabecerasDatosCarrera();
        //datos saldo y ticket
        Console.SetCursorPosition(0, 1);
        Console.Write(saldo);
        Console.SetCursorPosition(17, 1);
        Console.Write(ticketApuesta.caballo);
        Console.SetCursorPosition(17 + 8, 1);
        Console.Write(ticketApuesta.cantidadApostada);
        Console.SetCursorPosition(17 + 8 + 17, 1);
        Console.Write(ticketApuesta.eurosPagadosPorUnoApostado + " a 1");

        //CARRERA, da ganador
        ushort caballoGanador = DisputaCarrera(NUMCABALLOS);

        //DINEROGANADO
        float ganado = DineroGanadoEnApuesta(caballoGanador, ticketApuesta);
        //Muestra C.G y ganado
        Console.SetCursorPosition(17 + 8 + 17 + 10, 1);
        Console.Write(caballoGanador);
        Console.SetCursorPosition(17 + 8 + 17 + 10 + 8, 1);
        Console.Write(ganado);
        Pausa();
        Console.SetCursorPosition(0, 25);
        Console.WriteLine("Pulsa una tecla...");
        Console.ReadKey();
        Console.Clear();
        //actualiza saldo??
        saldo = ActualizaSaldo(saldo, ticketApuesta.cantidadApostada, ganado);

        return ObtenDatosCarreraFormateados(saldo, ticketApuesta, caballoGanador.ToString(), ganado);
    }

    //devuelve ticket en tupla
    static (ushort caballo, float cantidadApostada, ushort eurosPagadosPorUnoApostado) Apostar(float saldo, ushort NUMCABALLOS)
    {
        Console.WriteLine($"Saldo actual: {saldo}?");
        Console.WriteLine("Cuanto quieres apostar? ");
        float cantidadApostada = float.Parse(Console.ReadLine());
        Console.WriteLine("Introduce el número del caballo (de 1 a 8): ");
        ushort caballo = ushort.Parse(Console.ReadLine());
        Console.WriteLine("La apuesta se paga ? a 1");
        ushort eurosPagadosPorUnoApostado = ushort.Parse(Console.ReadLine());

        return (caballo, cantidadApostada, eurosPagadosPorUnoApostado);
    }

    //DisputaCarrera, recibe caballos y devuelve ganador
    static ushort DisputaCarrera(ushort NUMCABALLOS) //devuelve ganador
    {
        Console.CursorVisible = false;
        ushort[] posiciones = new ushort[8];
        posiciones[0] = 0;
        posiciones[1] = 0;
        posiciones[2] = 0;
        posiciones[3] = 0;
        posiciones[4] = 0;
        posiciones[5] = 0;
        posiciones[6] = 0;
        posiciones[7] = 0;

        ushort ganador = 0;
        bool finCarrera = false;
        do  //CARRERA: DIBUJA pista, caballos, avanza, dibuja pista, caballos, avanza
        {
            DibujaPistas(NUMCABALLOS, LONGITUD_COLUMNAS);
            DibujaCaballos(posiciones); //dibuja posciones
            Random semillaAleatoria = new Random();
            AvanzaPosicion(posiciones, semillaAleatoria); //avanza 1
            finCarrera = HayGanador(posiciones, LONGITUD_COLUMNAS, out ganador);
            Pausa(); // Pausará 8 milisegundos.

        } while (finCarrera == false);

        Console.CursorVisible = true;
        return ganador;
    }
    //Dentro de Disputa: Dibuja dibuja avanza hay ganador
    static void AvanzaPosicion(ushort[] posiciones, Random semillaAleatoria)//updatePosiciones
    {
        //hazar elige un caballo, y solo ese avanza una posicion
        int random = semillaAleatoria.Next(0, NUMCABALLOS);
        for (int caballo = 0; caballo < NUMCABALLOS; caballo++) //recorre caballos
        {
            if (caballo == random)// cuando random coincide con caballo, ese caballo avanza
            { posiciones.SetValue(posiciones[caballo] += 1, caballo); }//posicion del random +1
            else
                posiciones.SetValue(posiciones[caballo] += 0, caballo);
        }
    }
    static void DibujaCaballos(ushort[] posiciones)//listo, dibuja caballo en posición
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        //solo 1 caballo en cada pista //empieza en 0,5
        ushort fila = 5;
        ushort caballo = 0;
        do
        {
            if (posiciones[caballo] == 0)
            {
                Console.SetCursorPosition(posiciones[caballo], fila);
                Console.Write($"~({caballo + 1})o");
            }
            else
            {
                Console.SetCursorPosition(posiciones[caballo], fila);
                Console.Write($"~({caballo + 1})o");
                Console.SetCursorPosition(posiciones[caballo] - 1, fila);
                Console.Write(" ");
            }
            fila += 2;
            caballo++;
        } while (caballo < NUMCABALLOS);

        Console.ForegroundColor = ConsoleColor.White;
    }
    static void DibujaPistas(ushort NUMCABALLOS, ushort LONGITUD_COLUMNAS)//listo, dibuja pistas y caballos
    {

        for (int i = 4; i <= 20; i += 2)
        {
            for (int j = 0; j <= LONGITUD_COLUMNAS; j++)
            {
                Console.SetCursorPosition(j, i);
                Console.Write("-");
            }
        }
    }
    //recibe ya el ganador, no tiene que calcular nada
    static bool HayGanador(ushort[] posiciones, ushort LONGITUD_COLUMNAS, out ushort ganador)
    {

        bool hayGanador = false;
        ganador = 0;
        //pide asignar, pongo para cerrar la logica del modulo que no entiendo bien lo que debe hacer.
        //el ganador viene dado por parametros segun la firma, actua como un bool normal en disputacarrera.
        for (int i = 0; i < posiciones.Length; i++)
        {
            if (posiciones[i] == LONGITUD_COLUMNAS)
            {
                hayGanador = true;
                ganador = (ushort)(i+1);
            }
        }
        return hayGanador;
    }

    //datos tras la carrera 
    static string ObtenDatosCarreraFormateados(float saldo, (ushort caballo, float cantidadApostada, ushort eurosPagadosPorUnoApostado) ticketApuesta,
        string caballoGanador, float ganado)// listo, solo ToString()
    {
        return saldo.ToString().PadRight(17) + ticketApuesta.caballo.ToString().PadRight(8) + ticketApuesta.cantidadApostada.ToString().PadRight(17) + ticketApuesta.eurosPagadosPorUnoApostado.ToString().PadRight(10) + caballoGanador.ToString().PadRight(8) + ganado.ToString().PadRight(17);
    }
    static float DineroGanadoEnApuesta(ushort caballoGanador, (ushort caballo, float cantidadApostada, ushort eurosPagadosPorUnoApostado) ticketApuesta)
    {
        float ganado;
        if (ticketApuesta.caballo == caballoGanador)
            ganado = ticketApuesta.cantidadApostada * (float)ticketApuesta.eurosPagadosPorUnoApostado;
        else
            ganado = 0;
        return ganado;
    }
    static float ActualizaSaldo(float saldo, float apostado, float ganado)
    {
        if (ganado == 0)
            saldo = saldo - apostado;
        else if (ganado > 0)
        {
            saldo += ganado;
        }
        return saldo;
    }
    static void VerPartidasJugadas(string datosCarrerasAcabadas) //mostrar carreras
    {
        //en datosCarreras tienes cadena splitable con \n cada registro y con espacio cada apartado de registro
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        CabecerasDatosCarrera();
        //saldo.ToString().PadRight(17) + ticketApuesta.caballo.ToString().PadRight(8) + ticketApuesta.cantidadApostada.ToString().PadRight(17) + ticketApuesta.eurosPagadosPorUnoApostado.ToString().PadRight(10) + caballoGanador.ToString().PadRight(8) + ganado.ToString().PadRight(17);
        string[] registros = datosCarrerasAcabadas.Split("\n");
        int fila=1;
        for (int i = 0; i < registros.Length; i++)// recorre arrayStrings
        {
            Console.SetCursorPosition(0, fila);
            Console.WriteLine(registros[i]);
            fila++;
        }
    }
    static string CabecerasDatosCarrera()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write("Saldo");
        Console.SetCursorPosition(17, 0);
        Console.Write("C.A");
        Console.SetCursorPosition(17 + 8, 0);
        Console.Write("Apuesta");
        Console.SetCursorPosition(17 + 8 + 17, 0);
        Console.Write("Se paga");
        Console.SetCursorPosition(17 + 8 + 17 + 10, 0);
        Console.Write("C.G");
        Console.SetCursorPosition(17 + 8 + 17 + 10 + 8, 0);
        Console.Write("Ganado");

        return "cabecera"; //quizá deberia ser void?
    }

    static void Pausa()
    {
        Thread.Sleep(8); // Pausará 8 milisegundos.
    }


}