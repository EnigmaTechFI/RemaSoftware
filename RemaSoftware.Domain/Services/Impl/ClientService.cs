using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Data;

namespace RemaSoftware.Domain.Services.Impl
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<MyUser> _userManager;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ClientService(ApplicationDbContext dbContext, UserManager<MyUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public Client AddClient(Client customer)
        {
            var addedClient = _dbContext.Add(customer);
            _dbContext.SaveChanges();
            return addedClient.Entity;
        }

        public List<Client> GetAllClients()
        {
            return _dbContext.Clients.ToList();
        }

        public Client GetClient(int id)
        {
            var client = _dbContext.Clients
                .Include(i=>i.UserClients)
                .ThenInclude(i=>i.MyUser)
                .SingleOrDefault(i => i.ClientID == id);
            return client;
        }

        public int GetTotalCustomerCount()
        {
            return _dbContext.Clients.Count();
        }

        public bool DeleteById(int clientId)
        {
            try
            {
                var hasProducts = _dbContext.Products.Any(a => a.ClientID == clientId);
                if (hasProducts)
                {
                    var client = _dbContext.Clients.SingleOrDefault(sd => sd.ClientID == clientId);
                    //client.IsDeleted = true;
                    _dbContext.Clients.Update(client);
                }
                else
                    _dbContext.Clients.Remove(new Client { ClientID = clientId});
                
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"Errore durante l'eliminazione del cliente: {clientId}. ", e);
                return false;
            }
        }

        public bool UpdateClient(Client clientModel)
        {
            try
            {
                _dbContext.Clients.Update(clientModel);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"Errore durante l'update del cliente: {clientModel.ClientID}. ", e);
                return false;
            }
        }

        public int GetClientIdFattureInCloudByRemaClientId(int remaClientId)
        {
            if (remaClientId <= 0) throw new ArgumentException("Parametro RemaClientId obbligaotrio.");
            return _dbContext.Clients.Single(sd => sd.ClientID == remaClientId).FC_ClientID;
        }

        public MyUser AddUserClient(MyUser clientUser, string password)
        {
            var result = _userManager.CreateAsync(clientUser, password).Result;
            var addedRole = _userManager.AddToRoleAsync(clientUser, Roles.Cliente).Result;

            return clientUser;
        }

        public async Task<MyUser> UpdateUserClient(MyUser clientUser)
        {
            var clientuserOnDb = await _userManager.FindByIdAsync(clientUser.Id);
            if (clientUser == null)
                throw new Exception($"Utente non trovato con id {clientUser.Id}");
            clientuserOnDb.Name = clientUser.Name;
            clientuserOnDb.Surname = clientUser.Surname;
            clientuserOnDb.Email = clientUser.Email;
            clientuserOnDb.UserName = clientUser.UserName;
            var res = await _userManager.UpdateAsync(clientuserOnDb);
            return clientUser;
        }

        public async Task<bool> DeleteUserClientById(string clientUserId)
        {
            var clientUser = await _userManager.FindByIdAsync(clientUserId);
            if (clientUser == null)
                throw new Exception($"Utente non trovato con id {clientUserId}");
            await _userManager.DeleteAsync(clientUser);
            return true;
        }
    }
}