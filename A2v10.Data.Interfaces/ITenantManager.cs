
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace A2v10.Data.Interfaces
{
    public interface ITenantManager
    {
        Task SetTenantIdAsync(SqlConnection cnn, String source);
        void SetTenantId(SqlConnection cnn, String source);
    }
}
