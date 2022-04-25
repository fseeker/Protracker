using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using demo_7.Models;

namespace demo_7.Controllers
{
    public class TasksController : Controller
    {
        private readonly ProTrackerDbContext _context;

        public TasksController(ProTrackerDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            var proTrackerDbContext = _context.Tasks.Include(t => t.Project);
            return View(await proTrackerDbContext.ToListAsync());
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // for creating tasks without project
        public IActionResult Create()
        {
            ViewBag.emp = _context.Employees.ToList();
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string description, string status, int priority, DateTime createDate, DateTime endDate, int? empId, List<int> involvedEmployeesId)
        {
            if (empId == null)
            {
                empId = 1;
            }
            //List<TaskEmployeeRelation> elist = new List<TaskEmployeeRelation>();

            var emp = await _context.Employees.FirstOrDefaultAsync(x => x.Id == empId);
            var task = new Tasks
            {
                Name = name,
                Description = description,
                Status = status,
                Priority = 1,
                CreateDate = createDate,
                EndDate = endDate,
                ProjectId = null,
                CreatedBy = emp,
                //InvolvedEmployees = elist
            };

            await _context.Tasks.AddAsync(task);

            await _context.SaveChangesAsync();           

            //int tid = task.Id;

            if (involvedEmployeesId.Count != 0)
            {
                foreach (var item in involvedEmployeesId)
                {
                    var te = new TaskEmployeeRelation
                    {
                        EmployeeId = item,
                        TaskId = task.Id,
                        Role = await _context.EmployeeRoles.FirstOrDefaultAsync(x => x.RoleId == 2002)
                    };
                    await _context.TaskEmployeeRelations.AddAsync(te);
                }

                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(TaskIndex));
        }

        //for creating tasks with project
        public IActionResult CreateTask(int? projectId)
        {
            ViewBag.emp = _context.Employees.ToList();
            ViewBag.project = _context.Projects.FirstOrDefault(x => x.Id == projectId);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTask(string name, string description, string status, int priority, DateTime createDate, DateTime endDate, int? projectId, int? empId, List<int> involvedEmployeesId)
        {
            if (empId == null)
            {
                empId = 1;
            }
            //List<TaskEmployeeRelation> elist = new List<TaskEmployeeRelation>();
            
            var emp = await _context.Employees.FirstOrDefaultAsync(x => x.Id == empId);
            var task = new Tasks
            {
                Name = name,
                Description = description,
                Status = status,
                Priority = 1,
                CreateDate = createDate,
                EndDate = endDate,
                ProjectId = projectId,
                CreatedBy = emp,
                //InvolvedEmployees = elist
            };

            await _context.Tasks.AddAsync(task);
            
            await _context.SaveChangesAsync();
            //how do i get the new id of newly created task?

            int tid = task.Id;

            if(involvedEmployeesId.Count != 0) //Difference between count and count()
            {
                foreach (var item in involvedEmployeesId)
                {
                    var te = new TaskEmployeeRelation
                    {
                        EmployeeId = item,
                        TaskId = task.Id,
                        Role = await _context.EmployeeRoles.FirstOrDefaultAsync(x => x.RoleId == 2002)
                    };
                    await _context.TaskEmployeeRelations.AddAsync(te);
                }

                await _context.SaveChangesAsync();
                
            }

            return RedirectToAction(nameof(TaskIndex));
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", tasks.ProjectId);
            return View(tasks);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,Status,Priority,CreateDate,EndDate,ProjectId")] Tasks tasks)
        {
            if (id != tasks.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tasks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(tasks.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", tasks.ProjectId);
            return View(tasks);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tasks = await _context.Tasks.FindAsync(id);
            _context.Tasks.Remove(tasks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TasksExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        public IActionResult ShowTasks (int Id)
        {
            List<Tasks> tasks = _context.Tasks.Where(t => t.ProjectId == Id).ToList();
            
            var Project = _context.Projects.FirstOrDefault(m => m.Id == Id);
            ViewBag.Project = Project;
            
            return View(tasks);
        }

        public async Task<IActionResult> AssignTask(int? Id, int? ProjectId, string usertext, int page)
        {
            ViewBag.sword = usertext;
            IQueryable<Employee> employees = _context.Employees;

            if (!string.IsNullOrEmpty(usertext))
            {
                usertext = usertext.ToLower();
                employees = employees.Where(e => e.Name.ToLower().Contains(usertext) || e.Designation.ToLower().Contains(usertext) || e.Department.ToLower().Contains(usertext));
            }

            if (page <= 0) page = 1;
            int pageSize = 10;
            ViewBag.psize = pageSize;

            
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == Id);
            ViewBag.TaskId = task.Id;
            ViewBag.ProjectId = ProjectId;
            return View(await PaginatedList<Employee>.CreateAsync(employees, page, pageSize));
        }


        [HttpPost]
        public IActionResult AssignTask(int? empId, int? taskId, int? ProjectId)
        {

            if (empId == null || taskId == null)
            {
                return NotFound();
            }

            Employee emp = _context.Employees.FirstOrDefault(emp => emp.Id == empId);
            Tasks task = _context.Tasks.FirstOrDefault(t => t.Id == taskId);
            if(emp == null || task == null)
            {
                return NotFound();
            }

            var te = new TaskEmployeeRelation
            {
                EmployeeId = emp.Id,
                Employee = emp,
                TaskId = task.Id,
                Task = task,
                Role = _context.EmployeeRoles.FirstOrDefault(r => r.RoleId == 2002)
            };
            _context.Add(te);
            _context.SaveChanges();

            if(ProjectId == null)
            {
                return RedirectToAction(nameof(TaskIndex));
            }
            else
            {
                return RedirectToAction("ProjectDetails", "Project", new { id = ProjectId });
            }

        }

        public IActionResult UnassignedTasks()
        {
            var UnassignedTasks = _context.Tasks.Include(t => t.InvolvedEmployees).Where(t => t.InvolvedEmployees.Count == 0).ToList();
            return View(UnassignedTasks);
        }

        public List<Tasks> TodayTasks()
        {
            List<Tasks> todayTasks = _context.Tasks.Where(t => t.EndDate.Date <= DateTime.Today.Date && !t.Status.Contains("Completed")).OrderBy(e => e.EndDate).ToList();

            

            //if(userId == null)
            //{
            //    //return NotFound();
            //    return null;
            //}

            //var involvedTasks = _context.Tasks.Include(p => p.InvolvedEmployees).Where(p => p.InvolvedEmployees.All(p => p.EmployeeId == userId));

            return todayTasks;
        }        
        public async Task<IActionResult> TaskComplete(int? TaskId, int? ProjectId)
        {
            //bool isSuccess = false;

            if(TaskId == null)
            {
                return NotFound();
            }
            var task = await _context.Tasks.FirstOrDefaultAsync(m => m.Id == TaskId);
            if(task == null)
            {
                return NotFound();
            }
            task.Status = "Completed";
            _context.Update(task);
            await _context.SaveChangesAsync();
            if(ProjectId == null)
            {
                return RedirectToAction(nameof(TaskIndex));
            }
            else
            {
                return RedirectToAction("ProjectDetails", "Project", new { id = ProjectId });
            }
            //isSuccess = true;            
        }
        
        public IActionResult TaskUrgent(int? TaskId, int? ProjectId)
        {
            //bool isSuccess = false;
            if(TaskId == null)
            {
                return NotFound();
            }
            var task = _context.Tasks.Find(TaskId);
            if(task == null)
            {
                return NotFound();
            }
            task.Status = task.Status + " " + "Urgent";
            _context.Update(task);
            _context.SaveChanges();

            //return RedirectToAction(nameof(TaskIndex), new { page = 1, usertext = "vakh" });
            if (ProjectId == null)
            {
                return RedirectToAction(nameof(TaskIndex));
            }
            else
            {
                return RedirectToAction("ProjectDetails", "Project", new { id = ProjectId });
            }
        }

        public List<Tasks> CompletedTasks()
        {
            List<Tasks> tasks = _context.Tasks.Where(m => m.Status.Contains("Completed")).ToList();
            return tasks;
        }

        public IActionResult TaskIndex()
        {
            ViewBag.TodayTasks = TodayTasks();
            ViewBag.UrgentTasks = _context.Tasks.Where(m => m.Status.Contains("Urgent")).ToList();
            ViewBag.CompletedTasks = CompletedTasks();

            return View();
        }

        
    }
}
