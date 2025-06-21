using Account_Management.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Account_Management.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Account> GetChartOfAccounts()
        {
            var accounts = new List<Account>();
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("sp_ManageChartOfAccounts", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "SELECT");
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accounts.Add(new Account
                            {
                                AccountId = reader.GetInt32("AccountId"),
                                AccountName = reader.GetString("AccountName"),
                                AccountType = reader.GetString("AccountType"),
                                ParentAccountId = reader.IsDBNull("ParentAccountId") ? null : reader.GetInt32("ParentAccountId"),
                                IsActive = reader.GetBoolean("IsActive")
                            });
                        }
                    }
                }
            }
            return accounts;
        }

        public void ManageChartOfAccounts(Account account, string action)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("sp_ManageChartOfAccounts", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@AccountId", action == "INSERT" ? DBNull.Value : account.AccountId);
                    cmd.Parameters.AddWithValue("@AccountName", account.AccountName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccountType", account.AccountType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ParentAccountId", account.ParentAccountId.HasValue ? account.ParentAccountId : DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", account.IsActive);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<RolePermission> GetRolePermissions()
        {
            var permissions = new List<RolePermission>();
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("sp_ManageRolePermissions", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "SELECT");
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            permissions.Add(new RolePermission
                            {
                                RoleId = reader.GetString("RoleId"),
                                ModuleName = reader.GetString("ModuleName"),
                                CanView = reader.GetBoolean("CanView"),
                                CanEdit = reader.GetBoolean("CanEdit"),
                                CanDelete = reader.GetBoolean("CanDelete")
                            });
                        }
                    }
                }
            }
            return permissions;
        }

        public void UpdateRolePermissions(RolePermission permission)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("sp_ManageRolePermissions", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "UPDATE");
                    cmd.Parameters.AddWithValue("@RoleId", permission.RoleId);
                    cmd.Parameters.AddWithValue("@ModuleName", permission.ModuleName);
                    cmd.Parameters.AddWithValue("@CanView", permission.CanView);
                    cmd.Parameters.AddWithValue("@CanEdit", permission.CanEdit);
                    cmd.Parameters.AddWithValue("@CanDelete", permission.CanDelete);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
