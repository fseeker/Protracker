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
    public class ProjectController : Controller
    {
        private readonly ProTrackerDbContext _context;

        public ProjectController(ProTrackerDbContext context)
        {
            _context = context;
        }

        // GET: Project
        public async Task<IActionResult> Index()
        {
            var proTrackerDbContext = _context.Projects.Include(p => p.ProjectHead);
            return View(await proTrackerDbContext.ToListAsync());
        }

        // GET: Project/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.ProjectHead)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Project/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name");
            return View();
        }

        // POST: Project/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,StartDate,Author,Status,Version,EmployeeId")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", project.EmployeeId);
            return View(project);
        }

        // GET: Project/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "ContactNumber", project.EmployeeId);
            return View(project);
        }

        // POST: Project/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartDate,Author,Status,Version,EmployeeId")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //var p = _context.Projects.FirstOrDefault(p => p.Id == project.Id);
                    //p.Name = project.Name;
                    //_context.SaveChanges();
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "ContactNumber", project.EmployeeId);
            return View(project);
        }

        // GET: Project/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.ProjectHead)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }


        public IActionResult CreateProject()
        {
            ViewBag(_context.Employees);

            return View();
        }

        public async Task<IActionResult> ProjectDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.ProjectHead)
                .Include(p => p.InvolvedEmployees)
                .ThenInclude(p => p.Employee)
                .Include(p => p.InvolvedEmployees)
                .ThenInclude(p => p.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            List<string> emp = new();
            foreach (var item in project.InvolvedEmployees)
            {
                var name = item.Employee.Name;
                var position = item.Role.Role;
                string title = name + " (" + position + ")";
                emp.Add(title);
            }
            ViewBag.DevList = emp;
            ViewBag.Dev = GetEmployee(id);
            ViewBag.update = _context.UpdateProjects.Where(m => m.ProjectId == id).OrderByDescending(x => x.PushDate).Take(3).ToList();
            ViewBag.task = _context.Tasks.Where(x => x.ProjectId == id)
                .Include(e => e.CreatedBy)
                .Include(p => p.InvolvedEmployees)
                .ThenInclude(e => e.Employee)                
                .OrderBy(x => x.CreateDate).Take(5).ToList();
            return View(project);
        }

        //private dynamic GetDevelopers(int? Id)
        //{
        //    List<Employee> Dev = new();
        //    List<ProjectEmployeeRelation> PER = _context.ProjectEmployeeRelations.Where(m => m.ProjectId == Id).ToList();
        //    foreach (var item in PER)
        //    {
        //        int EId = 
        //        _context.Employees.FirstOrDefault(m => m.Id == )
        //    }

        //    return Dev;
        //}

        public async Task<IActionResult> UpdateProject(int? projectId)
        {
            if (projectId == null)
            {
                return NotFound();
            }
            var updates = await _context.UpdateProjects.Where(p => p.ProjectId == projectId).OrderBy(x => x.PushDate).ToListAsync();             
            ViewBag.project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);
            return View(updates);
        }

        public IActionResult CreateUpdate(int? projectId)
        {
            ViewBag.project = _context.Projects.FirstOrDefault(x => x.Id == projectId);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUpdate(int? projectId, string status, string version, string description, DateTime pushDate)
        {
            if(projectId == null)
            {
                return NotFound();
            }
            var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);
            if(project == null)
            {
                return NotFound();
            }


            var update = new UpdateProject
            {
                Status = status,
                PushDate = pushDate,
                ProjectId = (int)projectId,
                Version = version,
                Description = description,

            };

            project.Status = status;
            project.Version = version;

            _context.Projects.Update(project);
            _context.UpdateProjects.Add(update);

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(ProjectDetails), new { id = projectId });
        }

        public async Task<IActionResult> SetEmployee(int? Id, string usertext, string sortOrder, int page)
        {
            ViewBag.searchWord = usertext;

            ViewBag.Dev = GetEmployee(Id);
            IQueryable<Employee> emp = _context.Employees.Include(p => p.InvolvedProjects).ThenInclude(ip => ip.Project)
                                                    .Where(p => p.InvolvedProjects.All(m => m.ProjectId != Id));

            
            if (!string.IsNullOrEmpty(usertext))
            {
                usertext = usertext.ToLower();
                emp = emp.Where(e => e.Name.ToLower().Contains(usertext) || e.Designation.Contains(usertext) || e.Department.Contains(usertext));
            }
            ViewBag.count = emp.Count();
            if (page <= 0) page = 1;
            int PageSize = 10;
            ViewBag.PageSize = PageSize;

            ViewBag.staff = GetEmployee(Id);
            var Project = _context.Projects.FirstOrDefault(p => p.Id == Id);
            ViewBag.ProjectName = Project.Name;
            ViewBag.Project = Project;
            ViewBag.Roles = _context.EmployeeRoles.ToList();
            

            return View(await PaginatedList<Employee>.CreateAsync(emp, page, PageSize));
        }
        [HttpPost]
        public async Task<IActionResult> SetEmployee(int employeeId, int projectId, int roleId, string usertext, int page)
        {
            Employee staff = _context.Employees.FirstOrDefault(m => m.Id == employeeId);           
            Project proj = _context.Projects.Include(p => p.InvolvedEmployees).FirstOrDefault(m => m.Id == projectId);
            EmployeeRoles role = _context.EmployeeRoles.FirstOrDefault(m => m.RoleId == roleId);
            if (proj == null || staff == null || role == null)
            {
                return NotFound();
            }

            var ep = new ProjectEmployeeRelation
            {
                EmployeeId = staff.Id,
                Employee = staff,
                ProjectId = proj.Id,
                Project = proj,
                Role = role
                
            };
            _context.Add(ep);
            await _context.SaveChangesAsync();

            IQueryable<Employee> emp = _context.Employees.Include(p => p.InvolvedProjects).ThenInclude(ip => ip.Project)
                                                   .Where(p => p.InvolvedProjects.All(m => m.ProjectId != projectId));


            if (!string.IsNullOrEmpty(usertext))
            {
                usertext = usertext.ToLower();
                emp = emp.Where(e => e.Name.ToLower().Contains(usertext) || e.Designation.Contains(usertext) || e.Department.Contains(usertext));
            }
            ViewBag.count = emp.Count();
            int PageSize = 10;
            ViewBag.PageSize = PageSize;
            ViewBag.searchWord = usertext;
            ViewBag.ProjectId = projectId;
            ViewBag.Roles = _context.EmployeeRoles.ToList();

            return View(await PaginatedList<Employee>.CreateAsync(emp, page, PageSize));
        }

        
        public IActionResult EditEmployee(int? ProjectId)

        {
            var Project = _context.Projects.FirstOrDefault(p => p.Id == ProjectId);
            if (ProjectId == null || Project == null)
            {
                return NotFound();
            }
            var InvolvedEmp = _context.Employees.Where(p => p.InvolvedProjects.Any(s => s.ProjectId == ProjectId))
                                                .Include(e => e.InvolvedProjects.Where(p => p.ProjectId == ProjectId))
                                                .ThenInclude(e => e.Role).ToList();

            //var b = _context.Projects.Where(p => p.Id == ProjectId).Include(p => p.InvolvedEmployees).ThenInclude(ie => ie.Employee).FirstOrDefault();
            ViewBag.roles = _context.EmployeeRoles.Where(r => r.IsProjectRole == true).ToList();

            ViewBag.Project = Project;
            return View(InvolvedEmp);
        }
        [HttpPost]
        public IActionResult EditEmployeeRole(int? empId, int? projectId, int? roleId)
        {
            ViewBag.roles = _context.EmployeeRoles.ToList();
            var relation = _context.ProjectEmployeeRelations.Find(empId, projectId);
            relation.Role = _context.EmployeeRoles.FirstOrDefault(r => r.RoleId == roleId);
            _context.ProjectEmployeeRelations.Update(relation);
            _context.SaveChanges();
            return RedirectToAction(nameof(EditEmployee), new { ProjectId = projectId });
        }

        public IActionResult DeleteEmployee (int? empId, int? projectId)
        {
            var relation = _context.ProjectEmployeeRelations.FirstOrDefault(p => p.EmployeeId == empId && p.ProjectId == projectId);
            _context.ProjectEmployeeRelations.Remove(relation);
            _context.SaveChanges();

            return RedirectToAction(nameof(EditEmployee), new { ProjectId = projectId });
        }

        public List<Employee> GetEmployee(int? ProjectId)
        {
            var Project = _context.Projects.Include(p => p.InvolvedEmployees).FirstOrDefault(m => m.Id == ProjectId);
            if(Project == null)
            {
                return null;
            }

            var ListEmployees = Project.InvolvedEmployees.ToList();
            List<Employee> EmployeesIn = new List<Employee>();
            foreach (var item in ListEmployees)
            {
                Employee emp = _context.Employees.FirstOrDefault(e => e.Id == item.EmployeeId);
                EmployeesIn.Add(emp);
            }

            return EmployeesIn;
        }

        public IActionResult AddEmployee(int? ProjectId, int employeeId, int roleId)
        {//method chaining
            var Project = _context.Projects.Include(p => p.InvolvedEmployees).FirstOrDefault(m => m.Id == ProjectId);
            ViewBag.ProjectName = Project.Name;


            var ListEmployees = Project.InvolvedEmployees.ToList();
            List<Employee> EmployeesIn = null;
            foreach (var item in ListEmployees)
            {
                int pass = item.EmployeeId;
                EmployeesIn.Add(_context.Employees.FirstOrDefault(e => e.Id == item.EmployeeId));
            }

            ViewBag.WorkingEmployees = EmployeesIn;
            //var Project1q = _context.Projects.Where(m => m.Id == ProjectId).ToList();
            //var p1 = Project1q.FirstOrDefault();
            //List<Project> p2 = (List<Project>)_context.Projects.Where(p => p.Id == Id);
            // select * from projects where projectid = @id

            //***List<Employee> employees1 = _context.Employees.Include(p => p.InvolvedProjects).Where(p => p.InvolvedProjects == Project)

            Employee Employee = _context.Employees.FirstOrDefault(e => e.Id == employeeId);

            var role = _context.EmployeeRoles.FirstOrDefault(r => r.RoleId == roleId);
//            var AddedEmp = AddedEmployee(Id.Value);
            if(Employee != null && Project.InvolvedEmployees.All(e => e.EmployeeId != employeeId))
            {
                // todo save relation
                var r = new ProjectEmployeeRelation
                {
                    Employee = Employee,
                    EmployeeId = employeeId,
                    //Project = Project,
                    ProjectId = ProjectId.Value,
                    Role = role
                };
                Project.InvolvedEmployees.Add(r);
                //Employee.InvolvedProjects.Add(r);
                _context.SaveChanges();
            }
            //***var employees = Project.InvolvedEmployees.Select(ie => ie.Employee);

            return View();
        }

        public IEnumerable<ProjectEmployeeRelation> AddedEmployee(int projectId)
        {
            var project = _context.Projects.Include(p => p.InvolvedEmployees).FirstOrDefault(p => p.Id == projectId);
            var employees = project.InvolvedEmployees;
            return employees;            
        }

    }
}
