﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VPMS_Project.Repository;

namespace VPMS_Project.Components
{
    public class WorkHoursViewComponent : ViewComponent
    {
        private readonly AttendenceRepo _attendenceRepo;

        public WorkHoursViewComponent(AttendenceRepo attendenceRepo)
        {
            _attendenceRepo = attendenceRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = await _attendenceRepo.GetStandardWorkHours();
            return View(data);
        }
    }
}