using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace SimpleSqliteDisposeTest
{
    [TestFixture]
    public class SqliteDisposeTest
    {
        [Test]
        public async Task PureSqliteTest() {
        
            var db = "testdb";
            var file = $"{db}.db";
            var connectionString = $"Data Source={file}";
            
            await TestContext.Progress.WriteLineAsync($"Deleting db file {file} to start fresh");
            File.Delete(file);
            File.Exists(file).Should().BeFalse();

            await TestContext.Progress.WriteLineAsync($"Creating connection with connectionString {connectionString}");

            await using (var conn = new SqliteConnection(connectionString))
            {
                conn.Disposed += OnDisposed;
            
                await TestContext.Progress.WriteLineAsync($"Opening connection with connectionString {connectionString}");
                await conn.OpenAsync();

                await TestContext.Progress.WriteLineAsync($"Using cmd = conn.createCommand");
                await using (var cmd = conn.CreateCommand())
                {
                    cmd.Disposed += OnDisposed;
                    cmd.CommandText = "CREATE TABLE jalla(name VARCHAR(40))";
                    await cmd.ExecuteNonQueryAsync();
                }
                await TestContext.Progress.WriteLineAsync($"After using statement - cmd should have been disposed.");
            
                File.Exists(file).Should().BeTrue();
           
            }
            await TestContext.Progress.WriteLineAsync($"After using statement - conn should have been disposed.");

            await TestContext.Progress.WriteLineAsync($"Calling File.Delete({file})");
            // This works on versions up to and including 6.0.0-preview.7.21378.4, but fails on rc1 and rc2, on Windows (works on Linux).
            File.Delete(file);
            File.Exists(file).Should().BeFalse();
        }

        private static void OnDisposed(object? sender, EventArgs e)
        {
            TestContext.Progress.WriteLine($"Disposing " + sender);
        }
    }
}
