using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace InterviewReview
{
    // This type is not part of the interview!
    public class Program
    {
        public static void Main()
        {
            InitializeDatabase();
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
                .Build()
                .Run();
        }

        private static void InitializeDatabase()
        {
            Exec("create table jobs (jobid primarykey, type, startedat, completedat)");
            Exec("create table jobstousers (jobid references jobs (jobid), userid)");

            Exec("insert into jobs values ('j1', 'move', '2022-03-01', null)");
            Exec("insert into jobs values ('j2', 'copy', '2022-04-07', '2022-04-08')");
            Exec("insert into jobs values ('j3', 'delete', '2022-05-19', '2022-06-19')");

            Exec("insert into jobstousers values ('j1', 'test')");
            Exec("insert into jobstousers values ('j2', 'test')");
            Exec("insert into jobstousers values ('j3', 'test')");
        }

        private static void Exec(string command)
        {
            using var cmd = DbUtils.Connection.CreateCommand();
            cmd.CommandText = command;
            cmd.ExecuteNonQuery();
        }
    }

    // This type is not part of the interview!
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}

// This type is not part of the interview!
static class DbUtils
{
    public static DbConnection Connection { get; } = Open(":memory:");

    private static SQLiteConnection Open(string dataSource)
    {
        var db = new SQLiteConnection($"Data Source = {dataSource};");
        return db.OpenAndReturn();
    }

    public static DbDataAdapter CreateAdapter(DbCommand command) => new SQLiteDataAdapter((SQLiteCommand)command);
}

// This type is not part of the interview!
class JobInfo
{
    public string Id { get; set; }
    public string Type { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public static JobInfo FromDbRow(DataRow row) => new JobInfo
    {
        Id = row["jobid"].ToString(),
        Type = row["type"].ToString(),
        StartedAt = DateTime.Parse(row["startedat"].ToString()),
        CompletedAt = DateTime.TryParse(row["completedat"].ToString(), out var dt) ? dt : (DateTime?)null,
    };
}