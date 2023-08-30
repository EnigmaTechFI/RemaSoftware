using Microsoft.Extensions.Configuration;
using NLog;
using RemaSoftware.UtilityServices.Interface;
using System;
using System;
using Opc.UaFx;
using Opc.UaFx.Client;


namespace RemaSoftware.UtilityServices.Implementation
{
    public class MachineService : IMachineService
    {
        private readonly IConfiguration _configuration;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MachineService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool ConnectMachine()
        {
            // Imposta l'indirizzo del server OPC UA a cui connettersi
            var serverUrl = "opc.tcp://192.168.1.250:4840";
            //var serverUrl = "opc.tcp://185.230.192.147:4840";

            try
            {
                var client = new OpcClient(serverUrl);
            
                // Connetti al server OPC UA
                client.Connect();

                // Verifica lo stato della connessione
                if (client.State == OpcClientState.Connected)
                {
                    // se la macchina è accesa entra qui dentro
                    Console.WriteLine("Connessione stabilita con successo!");

                    var nodeId = "ns=3;s=\"ILLUMINAZIONE CABINA\"";

                    try
                    {
                        var readValue = client.ReadNode(nodeId);

                        if (readValue.Status.IsGood)
                        {
                            Console.WriteLine($"Valore letto: {readValue.Value}");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine($"Errore durante la lettura della variabile: {readValue.Status}");
                            return true;
                        }
                    }
                    catch (OpcException ex)
                    {
                        Console.WriteLine($"Errore durante la lettura del nodo: {ex.Message}");
                        return false;
                    }
                }

            }  //se la macchina è spenta va in eccezione
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la connessione: {ex.Message}");
                return false;
            }

            return false;
        }
    }
}