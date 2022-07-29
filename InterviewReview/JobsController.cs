using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace InterviewReview
{
    [Route("jobs")]
    public class JobsController : ControllerBase
    {
        [HttpGet]
        [Route("{jobId}")]
        public IActionResult GetJob()
        {
            var i = Request.Path.Value.LastIndexOf("/");
            var id = Request.Path.Value.Substring(i + 1);

            var job = GetById(id);
            if (job == null)
            {
                return Ok(new { error = true, message = "not found" });
            }

            return Ok(job);
        }

        private JobInfo GetById(string id)
        {
            using var dbCommand = DbUtils.Connection.CreateCommand();
            dbCommand.CommandText = "select * from jobs where jobid = '" + id + "'";

            for (int n = 1; n <= 3; n++)
            {
                try
                {
                    using var adapter = DbUtils.CreateAdapter(dbCommand);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return JobInfo.FromDbRow(dataTable.Rows[0]);
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }

            return null;
        }

        [HttpGet]
        public IActionResult GetJobsByUser()
        {
            var userId = Request.Query["userid"];
            Console.WriteLine($"getting jobs for {userId}");

            var jobIds = GetIdsByUser(userId);
            var jobs = jobIds.Select(id => GetById(id));
            Console.WriteLine($"got {jobs.Count()} for {userId}");

            return Ok(jobs);
        }

        private IEnumerable<string> GetIdsByUser(string id)
        {
            using var dbCommand = DbUtils.Connection.CreateCommand();
            dbCommand.CommandText = "select jobid from jobstousers where userid = '" + id + "'";

            for (int n = 1; n <= 3; n++)
            {
                try
                {
                    using var adapter = DbUtils.CreateAdapter(dbCommand);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable.Rows.Cast<DataRow>().Select(r => r["jobid"].ToString());
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }

            return null;
        }
    }
}