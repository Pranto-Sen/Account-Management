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

        public void SaveVoucher(Voucher voucher)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int voucherId;
                        using (var cmd = new SqlCommand("sp_SaveVoucher", conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Action", "INSERT_VOUCHER");
                            cmd.Parameters.AddWithValue("@VoucherType", voucher.VoucherType ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@VoucherDate", voucher.VoucherDate);
                            cmd.Parameters.AddWithValue("@ReferenceNo", voucher.ReferenceNo ?? (object)DBNull.Value);
                            var voucherIdParam = new SqlParameter("@VoucherId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(voucherIdParam);
                            cmd.ExecuteNonQuery();
                            voucherId = (int)voucherIdParam.Value;
                        }

                        foreach (var entry in voucher.Entries)
                        {
                            using (var cmd = new SqlCommand("sp_SaveVoucher", conn, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@Action", "INSERT_ENTRY");
                                cmd.Parameters.AddWithValue("@VoucherId", voucherId);
                                cmd.Parameters.AddWithValue("@AccountId", entry.AccountId);
                                cmd.Parameters.AddWithValue("@Debit", entry.Debit);
                                cmd.Parameters.AddWithValue("@Credit", entry.Credit);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<Voucher> GetVouchers()
        {
            var vouchers = new List<Voucher>();
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("sp_SaveVoucher", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "SELECT_VOUCHERS");
                    cmd.Parameters.AddWithValue("@VoucherId", DBNull.Value); // Add missing parameter
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            vouchers.Add(new Voucher
                            {
                                VoucherId = reader.GetInt32("VoucherId"),
                                VoucherType = reader.GetString("VoucherType"),
                                VoucherDate = reader.GetDateTime("VoucherDate"),
                                ReferenceNo = reader.GetString("ReferenceNo")
                            });
                        }
                    }
                }
            }
            return vouchers;
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
