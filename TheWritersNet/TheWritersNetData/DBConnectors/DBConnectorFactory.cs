using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWritersNetData.DBConnectors
{
    public static class DBConnectorFactory
    {
        public static IDBConnector GetDBConnector()
        {
            return new SQLServerDapperConnector();
        }
    }
}
