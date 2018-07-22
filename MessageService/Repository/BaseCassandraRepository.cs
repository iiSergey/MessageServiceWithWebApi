using Cassandra;

namespace MessageService.Repository
{
    public abstract class BaseCassandraRepository
    {
        private ICluster cluster;
        private ISession session;

        protected BaseCassandraRepository(ICluster cluster)
        {
            this.cluster = cluster;
        }

        public string KeySpace { get; set; } = "demo";

        protected ISession GetSession()
        {
            if (session == null)
            {
                Connect();
            }

            return session;
        }

        private void Connect()
        {
            session = cluster.Connect(KeySpace);
        }
    }
}