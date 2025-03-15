using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Job_Tracker.Models;
using Job_Tracker.Data;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class JobApplicationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public JobApplicationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<JobApplication>> GetApplications()
    {
        return _context.JobApplications.ToList();
    }

    [HttpPost]
    public ActionResult<JobApplication> AddApplication(JobApplication application)
    {
        _context.JobApplications.Add(application);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetApplications), new { id = application.Id }, application);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateStatus(int id, JobApplication application)
    {
        var existingApplication = _context.JobApplications.Find(id);
        if (existingApplication == null)
        {
            return NotFound();
        }

        existingApplication.Status = application.Status;
        _context.SaveChanges();
        return NoContent();
    }
}
