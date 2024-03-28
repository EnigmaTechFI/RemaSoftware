using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NLog;
using RemaSoftware.UtilityServices.Interface;
using Opc.Ua;
using Opc.Ua.Client;

namespace RemaSoftware.UtilityServices.Implementation
{
    public class MachineService : IMachineService
    {
           
        public async Task<MachineViewModel> ConnectMachine()
        {
            MachineViewModel machine = new MachineViewModel();
            try
            {
                // Imposta l'indirizzo del server OPC UA a cui connettersi
                //var serverUrl = "opc.tcp://192.168.1.250"; //in locale
                //var serverUrl = "opc.tcp://192.168.1.250:4840";
                var serverUrl = "opc.tcp://185.230.192.147:4840"; //dall'esterno ma deve essere abilitato l'ip
                
                var applicationConfiguration = new ApplicationConfiguration()
                {
                    ApplicationName = "RemaSoftware",
                    ApplicationUri = "https://mes.remacloud.it/",
                    ApplicationType = ApplicationType.Client,
                };

                var clientConfiguration = new ClientConfiguration();
                applicationConfiguration.ClientConfiguration = clientConfiguration;

                var endpointDescription = new ConfiguredEndpoint(null, new EndpointDescription(serverUrl));
                var session = await Session.Create(
                    applicationConfiguration,
                    endpointDescription,
                    true,
                    "Rema",
                    60000,
                    new UserIdentity(new AnonymousIdentityToken()),
                    null
                );

                try
                {
                    // Leggi il valore dal topic "ILLUMINAZIONE CABINA"
                    var nodeId = "ns=3;s=\"ILLUMINAZIONE CABINA\""; // NodeId del topic

                    var readValueId = new ReadValueId
                    {
                        NodeId = nodeId,
                        AttributeId = Attributes.Value
                    };

                    var response = session.Read(
                        null,
                        0,
                        TimestampsToReturn.Both,
                        new ReadValueIdCollection { readValueId },
                        out DataValueCollection results,
                        out DiagnosticInfoCollection diagnosticInfos
                    );

                    if (StatusCode.IsGood(response.ServiceResult))
                    {
                        if (results.Count > 0)
                        {
                            machine.MachineOn = (bool)results[0].Value;
                        }
                    }
                    else
                    {
                        machine.MachineOn = false;
                    }

                    var additionalNodeIds = new[]
                    {
                        "ns=3;s=\"FB_UNITA_1_RUOTA_DI_LAVORO_DB\".\"RUOTA DI LAVORO IN FUNZIONE\"",
                        "ns=3;s=\"FB_UNITA_2_RUOTA_DI_LAVORO_DB\".\"RUOTA DI LAVORO IN FUNZIONE\"",
                        "ns=3;s=\"FB_UNITA_3_RUOTA_DI_LAVORO_DB\".\"RUOTA DI LAVORO IN FUNZIONE\"",
                        "ns=3;s=\"FB_UNITA_4_RUOTA_DI_LAVORO_DB\".\"RUOTA DI LAVORO IN FUNZIONE\"",
                        "ns=3;s=\"FB_UNITA_5_RUOTA_DI_LAVORO_DB\".\"RUOTA DI LAVORO IN FUNZIONE\""
                    };

                    for (int i = 0; i < additionalNodeIds.Length; i++)
                    {
                        readValueId.NodeId = additionalNodeIds[i];

                        response = session.Read(
                            null,
                            0,
                            TimestampsToReturn.Both,
                            new ReadValueIdCollection { readValueId },
                            out results,
                            out diagnosticInfos
                        );

                        if (StatusCode.IsGood(response.ServiceResult))
                        {
                            if (results.Count > 0)
                            {
                                var propertyName = $"Brush{i + 1}_On";
                                typeof(MachineViewModel).GetProperty(propertyName)?.SetValue(machine, (bool)results[0].Value);
                            }
                        }
                        else
                        {
                            machine.MachineOn = false;
                        }
                    }

                    return machine;
                }
                catch (Exception ex)
                {
                    machine.MachineOn = false;
                    return null;
                }
                finally
                {
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                machine.MachineOn = false;
                return null;
            }
        }
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
