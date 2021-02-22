using ExitGames.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace battleRoyalServer
{
    public class ClientsPool
    {
        public static readonly ClientsPool Instance = new ClientsPool();
        public List<BattleRoyalClient> Clients { get; private set; }
        private readonly ReaderWriterLockSlim readWriteLock;

        public ClientsPool() {
            Clients = new List<BattleRoyalClient>();
            readWriteLock = new ReaderWriterLockSlim();
        }

        public bool IsContain(string name)
        {
            using (ReadLock.TryEnter(this.readWriteLock, 1000))
            {
                return Clients.Exists(n => n.CharactedName.Equals(name));
            }

        }
        public void AddClient(BattleRoyalClient client)
        {
            using (WriteLock.TryEnter(this.readWriteLock, 1000))
            {
                Clients.Add(client);
            }
        }

        public BattleRoyalClient getByName(string name)
        {
            using (ReadLock.TryEnter(this.readWriteLock, 1000))
            {
                return Clients.Find(n => n.CharactedName.Equals(name));
            }
        }
        public void RemoveClient(BattleRoyalClient client)
        {
            using (ReadLock.TryEnter(this.readWriteLock, 1000))
            {
                Clients.Remove(client);
            }

        }
        ~ClientsPool()
        {
            readWriteLock.Dispose();
        }

    }
}
