using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using VPMS_Project.Models;
using VPMS_Project.Repository;

namespace VPMS_Project.Controllers
{
    public class AdminLeaveController : Controller

    {
        private readonly IEmpRepository _empRepository = null;
        private readonly JobRepository _jobRepository = null;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly LeaveRepository _leaveRepository = null;
        private readonly TaskRepo _taskRepository = null;

        public AdminLeaveController(TaskRepo taskRepo, IEmpRepository empRepository, IWebHostEnvironment webHostEnvironment, LeaveRepository leaveRepository, JobRepository jobRepository)
        {
            _empRepository = empRepository;
            _jobRepository = jobRepository;
            _webHostEnvironment = webHostEnvironment;
            _leaveRepository = leaveRepository;
            _taskRepository = taskRepo;
        }

        [HttpGet]
        public async Task<IActionResult> LeaveAllocation(string Search = null)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            var data = await _empRepository.GetSearchEmps(Search);
            if (data == null )
            {
                return RedirectToAction(nameof(LeaveAllocation));  
            }
            return View(data);
        }

        
         public async Task<IActionResult> HRLeaveTableSee(bool isSucceess = false, bool isUpdate = false, bool isDelete = false)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            ViewBag.IsSuccess = isSucceess;
            ViewBag.IsUpdate = isUpdate;
            ViewBag.IsDelete = isDelete;
            var data = await _jobRepository.GetJobs();
            return View(data);
        }

        [HttpGet]
        public IActionResult AddJob()
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddJob(JobModel jobModel)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            int id = await _jobRepository.AddJob(jobModel);

            if (id > 0)
            {
                return RedirectToAction(nameof(HRLeaveTableSee), new { isSucceess = true });
            }

            return View();
        }

        public async Task<IActionResult> Recomend(bool isRecommend = false,bool isNotRecommend=false)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            ViewBag.IsRecommend = isRecommend;
            ViewBag.IsNotRecommend = isNotRecommend;
            var data = await _leaveRepository.GetLeaveRecommend();
            return View(data);
        }

        public async Task<IActionResult> LeaveRecomend(int id)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
  
            String name = User.FindFirst("UserFirstName").Value+" "+ User.FindFirst("UserLastName").Value;
            bool success = await _leaveRepository.RecommendLeave(id,name);
            if (success == true)
            {
                return RedirectToAction(nameof(Recomend), new { isRecommend = true });

            }
            return View();
        }

        public async Task<IActionResult> LeaveNotRecomend(int id)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            String name = User.FindFirst("UserFirstName").Value + " " + User.FindFirst("UserLastName").Value;
            bool success = await _leaveRepository.NotRecommendLeave(id, name);
            if (success == true)
            {
                return RedirectToAction(nameof(Recomend), new { isNotRecommend = true });

            }
            return View();
        }

        public async Task<IActionResult> EditJob(int id)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            var data = await _jobRepository.GetJobById(id);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> EditJob(JobModel jobModel)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();

            if (ModelState.IsValid)
            {
               
                bool success = await _jobRepository.Updatejob(jobModel);

                if (success == true)
                {
                    return RedirectToAction(nameof(HRLeaveTableSee), new { isUpdate = true,});
                }
            }
            
            return View();
        }

        public async Task<IActionResult> DeleteJob(int id)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            bool success = await _jobRepository.DeleteJob(id);
            if (success == true)
            {
                return RedirectToAction(nameof(HRLeaveTableSee), new { isDelete = true });

            }
            return View();
        }

        
            public async Task<IActionResult> SeeLeaveAllocation(int id, bool isUpdate = false)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            ViewBag.IsUpdate = isUpdate;
            var data = await _empRepository.GetEmpById(id);
            return View(data);
        }

        public async Task<IActionResult> EditLeaveAllocation(int id)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            var data = await _empRepository.GetEmpById(id);
            return View(data);
        }



        [HttpPost]
        public async Task<IActionResult> EditLeaveAllocation(EmpModel empModel)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            bool success = await _empRepository.UpdateEmpLeave(empModel);

                if (success == true)
                {
                    return RedirectToAction(nameof(SeeLeaveAllocation),new {id= empModel.EmpId, isUpdate = true });
                }
            
            return View();
        }

        public async Task<IActionResult> Approve(bool isApprove = false, bool isNotApprove = false)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            ViewBag.IsApprove = isApprove;
            ViewBag.IsNotApprove = isNotApprove;
            var data = await _leaveRepository.GetLeaveApprove();
            return View(data);
        }
        
        public async Task<IActionResult> LeaveApprove(int id)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            String name = User.FindFirst("UserFirstName").Value + " " + User.FindFirst("UserLastName").Value;
            bool success = await _leaveRepository.ApproveLeave(id, name);
            if (success == true)
            {
                return RedirectToAction(nameof(Approve), new { isApprove = true });

            }
            return View();
        }

        public async Task<IActionResult> LeaveNotApprove(int id)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            String name = User.FindFirst("UserFirstName").Value + " " + User.FindFirst("UserLastName").Value;
            bool success = await _leaveRepository.NotApproveLeave(id, name);
            if (success == true)
            {
                return RedirectToAction(nameof(Approve), new { isNotApprove = true });

            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> leaveDetails(string Search = null)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            var data = await _empRepository.GetSearchEmps(Search);
            if (data == null)
            {
                return RedirectToAction(nameof(leaveDetails));
            }
            return View(data);
        }

        public async Task<IActionResult> SeeLeaveDetails(int id)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            var data = await _empRepository.GetEmp(id);
                return View(data);

           
    
        }

        public async Task<IActionResult> ViewLeave(int id)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            var data = await _leaveRepository.GetLeaveById(id);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> SpecialLeave(int id, DateTime start_Date, DateTime end_Date)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            if (start_Date == DateTime.MinValue && end_Date == DateTime.MinValue)
            {
                ViewBag.Id = id;
                var data = await _leaveRepository.GetSpecialLeaveById(id);
                return View(data);

        }
            else
            {
                ViewBag.Id = id;
                var data = await _leaveRepository.SpecialLeavePeriod(id, start_Date, end_Date);
                return View(data);
         }

       }

        [HttpGet]
        public async Task<IActionResult> NoPayLeave(int id, DateTime start_Date, DateTime end_Date)
        {
            ViewBag.Rcount = _leaveRepository.leaveRecomCount();
            ViewBag.Acount = _leaveRepository.leaveAppCount();
            if (start_Date == DateTime.MinValue && end_Date == DateTime.MinValue)
            {
                ViewBag.Id = id;
                var data = await _leaveRepository.GetNoPayLeaveById(id);
                return View(data);
            }
            else
            {
                ViewBag.Id = id;
                var data = await _leaveRepository.NoPayLeavePeriod(id, start_Date, end_Date);
                return View(data);
            }
        }


    }
}
