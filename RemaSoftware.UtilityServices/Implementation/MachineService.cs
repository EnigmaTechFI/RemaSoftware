
using Microsoft.Extensions.Configuration;
using NLog;
using RemaSoftware.UtilityServices.Interface;
using System;
using Opc.UaFx;
using Opc.UaFx.Client;
using RemaSoftware.Domain.Models;
using RemaSoftware.UtilityServices.Implementation.RemaSoftware.UtilityServices.Implementation;


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
        
        public MachineViewModel ConnectMachine()
        {
            MachineViewModel machine = new MachineViewModel();

            // Imposta l'indirizzo del server OPC UA a cui connettersi
            //var serverUrl = "opc.tcp://192.168.1.250"; //Last option
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

                    var MachineOnId = "ns=3;s=\"ILLUMINAZIONE CABINA\"";
                    var BrushOn_1 = "ns=3;s=\"FB_UNITA_1_RUOTA_DI_LAVORO_DB\".\"RUOTA DI LAVORO IN FUNZIONE\"";
                    var BrushOn_2 = "ns=3;s=\"FB_UNITA_2_RUOTA_DI_LAVORO_DB\".\"RUOTA DI LAVORO IN FUNZIONE\"";
                    var BrushOn_3 = "ns=3;s=\"FB_UNITA_3_RUOTA_DI_LAVORO_DB\".\"RUOTA DI LAVORO IN FUNZIONE\"";
                    var BrushOn_4 = "ns=3;s=\"FB_UNITA_4_RUOTA_DI_LAVORO_DB\".\"RUOTA DI LAVORO IN FUNZIONE\"";
                    var BrushOn_5 = "ns=3;s=\"FB_UNITA_5_RUOTA_DI_LAVORO_DB\".\"RUOTA DI LAVORO IN FUNZIONE\"";

                    try
                    {
                        machine.MachineOn = (bool)client.ReadNode(MachineOnId).Value;
                        machine.Brush1_On = (bool)client.ReadNode(BrushOn_1).Value;
                        machine.Brush2_On = (bool)client.ReadNode(BrushOn_2).Value;
                        machine.Brush3_On = (bool)client.ReadNode(BrushOn_3).Value;
                        machine.Brush4_On = (bool)client.ReadNode(BrushOn_4).Value;
                        machine.Brush5_On = (bool)client.ReadNode(BrushOn_5).Value;
                    }
                    catch (OpcException ex)
                    {
                        machine.MachineOn = false;
                    }
                }

                client.Disconnect();
            }
            catch (Exception ex)
            {
                machine.MachineOn = false;
            }

            return machine;
        }
    }
    
    namespace RemaSoftware.UtilityServices.Implementation
    {
        public class MachineViewModel
        {
            public bool MachineOn { get; set; }
            
            public bool Brush1_On { get; set; }

            public bool Brush2_On { get; set; }

            public bool Brush3_On { get; set; }

            public bool Brush4_On { get; set; }

            public bool Brush5_On { get; set; }
        }
    }
}