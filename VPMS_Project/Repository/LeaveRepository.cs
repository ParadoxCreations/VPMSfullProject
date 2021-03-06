using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using VPMS_Project.Data;
using VPMS_Project.Models;

namespace VPMS_Project.Repository
{
    public class LeaveRepository
    {
        private readonly EmpStoreContext _context = null;

        public LeaveRepository(EmpStoreContext context)
        {
            _context = context;
        }

        public async Task<int> AddLeave(LeaveApplyModel LeaveApplyModel)
        {
            var leaveApply = new LeaveApply()
            {

                LeaveType = LeaveApplyModel.LeaveType,
                Startdate = LeaveApplyModel.Startdate,
                EndDate = LeaveApplyModel.EndDate,
                Reason = LeaveApplyModel.Reason,
                EmpId = LeaveApplyModel.EmpId,
                AppliedDate = DateTime.Now,
                NoOfDays = LeaveApplyModel.NoOfDays,
                Status = "Waiting for Recommendation",
                Visible = "Show",
                PdfURL = LeaveApplyModel.PdfURL

            };

            await _context.LeaveApply.AddAsync(leaveApply);
            await _context.SaveChangesAsync();

            return leaveApply.LeaveApplyId;
        }



        public async Task<LeaveApplyModel> GetEmpLeaveById(int id)
        {
            return await (from a in _context.Employees.Where(x => x.EmpId == id)
                          select new LeaveApplyModel()
                          {
                              EmpId = id,
                              FromDate = (DateTime)a.FromDate,
                              ToDate = (DateTime)a.Todate,

                          })
                   .FirstOrDefaultAsync();
        }

        public async Task<List<LeaveApplyModel>> GetAllPendingLeaveById(int id)
        {

            return await (from a in _context.LeaveApply.Where(x => x.EmpId == id && (x.Status == "Waiting for Recommendation" || x.Status == "Waiting for Approval"))
                          select new LeaveApplyModel()
                          {
                              LeaveApplyId = a.LeaveApplyId,
                              LeaveType = a.LeaveType,
                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              Reason = a.Reason,
                              AppliedDate = (DateTime)a.AppliedDate,
                              NoOfDays = a.NoOfDays,
                              EmpId = a.EmpId,
                              Status = a.Status,
                              RecommendName = a.RecommendName,
                              PdfURL = a.PdfURL

                          })
                   .ToListAsync();
        }

        public async Task<LBalanceModel> GetLeaveBalance(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
                return null;

            int totalLeaveGiven = emp.ShortLeaveAllocated + emp.HalfLeaveAllocated + emp.AnnualAllocated + emp.MedicalAllocated + emp.CasualAllocated;
            int totalLeaveTaken = _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType != "Special Leave") && (x.LeaveType != "No Pay Leave")).Select(x => x.NoOfDays).Sum();
            int medicalTaken = _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "Medical Leave")).Select(x => x.NoOfDays).Sum();
            int annualTaken = _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "Annual Leave")).Select(x => x.NoOfDays).Sum();
            int casualTaken = _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "Casual Leave")).Select(x => x.NoOfDays).Sum();
            int shortTaken = _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "Short Leave")).Select(x => x.NoOfDays).Sum();
            int halfTaken = _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "Half Days")).Select(x => x.NoOfDays).Sum();
            int specialTaken = _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "Special Leave")).Select(x => x.NoOfDays).Sum();
            int noPayTaken = _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "No Pay Leave")).Select(x => x.NoOfDays).Sum();

            return await (from a in _context.LeaveApply
                          select new LBalanceModel()
                          {
                              EmpId = id,
                              FromDate = (DateTime)emp.FromDate,
                              Todate = (DateTime)emp.Todate,
                              TotalLeaveGiven = totalLeaveGiven,
                              TotalLeaveTaken = totalLeaveTaken,
                              TotalLeaveBalance = totalLeaveGiven - totalLeaveTaken,
                              MedicalAllocated = emp.MedicalAllocated,
                              MedicalTaken = medicalTaken,
                              CasualAllocated = emp.CasualAllocated,
                              CasualTaken = casualTaken,
                              AnnualAllocated = emp.AnnualAllocated,
                              AnnualTaken = annualTaken,
                              HalfAllocated = emp.HalfLeaveAllocated,
                              Halftaken = halfTaken,
                              ShortAllocated = emp.ShortLeaveAllocated,
                              ShortTaken = shortTaken,
                              MedicalRemain = emp.MedicalAllocated - medicalTaken,
                              CasualRemain = emp.CasualAllocated - casualTaken,
                              AnnualRemain = emp.AnnualAllocated - annualTaken,
                              HalfRemain = emp.HalfLeaveAllocated - halfTaken,
                              ShortRemain = emp.ShortLeaveAllocated - shortTaken,
                              SpecialTaken = specialTaken,
                              NoPayTaken = noPayTaken

                          }).FirstOrDefaultAsync();
        }

        public async Task<List<LeaveApplyModel>> GetApprovedLeaveById(int id)
        {
            return await (from a in _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.Visible == "Show"))
                          select new LeaveApplyModel()
                          {
                              LeaveApplyId = a.LeaveApplyId,
                              LeaveType = a.LeaveType,
                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              Reason = a.Reason,
                              AppliedDate = (DateTime)a.AppliedDate,
                              NoOfDays = a.NoOfDays,
                              EmpId = a.EmpId,
                              Status = a.Status,
                              RecommendName = a.RecommendName,
                              ApproverName = a.ApproverName,
                              PdfURL = a.PdfURL


                          })
                   .ToListAsync();
        }


        public async Task<List<LeaveApplyModel>> GetRejectedLeaveById(int id)
        {
            return await (from a in _context.LeaveApply.Where(x => x.EmpId == id && (x.Status == "Not Recommended" || x.Status == "Not Approved") && (x.Visible == "Show"))
                          select new LeaveApplyModel()
                          {
                              LeaveApplyId = a.LeaveApplyId,
                              LeaveType = a.LeaveType,
                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              Reason = a.Reason,
                              AppliedDate = (DateTime)a.AppliedDate,
                              NoOfDays = a.NoOfDays,
                              EmpId = a.EmpId,
                              Status = a.Status,
                              PdfURL = a.PdfURL
                              //RecommendName = a.RecommendName,
                              //ApproverName = a.ApproverName

                          })
                   .ToListAsync();
        }
        public async Task<bool> RecommendLeave(int id, String name)
        {
            String content = "The leave request below has been recommended. Now It's waiting for getting approval";
            await EmailSend(id, content);

            var leave = await _context.LeaveApply.FindAsync(id);
            leave.Status = "Waiting for Approval";
            leave.RecommendName = name;

            _context.Entry(leave).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<bool> NotRecommendLeave(int id, String name)
        {
            String content = "Sorry! The leave request below has been unrecommeded";
            await EmailSend(id, content);
            var leave = await _context.LeaveApply.FindAsync(id);
            leave.Status = "Not Recommended";
            leave.RecommendName = name;

            _context.Entry(leave).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;

        }


        public async Task<List<LeaveApplyModel>> GetLeaveRecommend()
        {
            return await (from a in _context.LeaveApply.Where(x => x.Status == "Waiting for Recommendation")
                          join b in _context.Employees on a.EmpId equals b.EmpId
                          join c in _context.Job on b.JobTitleId equals c.JobId
                          select new LeaveApplyModel()
                          {
                              LeaveApplyId = a.LeaveApplyId,
                              LeaveType = a.LeaveType,
                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              Reason = a.Reason,
                              AppliedDate = (DateTime)a.AppliedDate,
                              NoOfDays = a.NoOfDays,
                              EmpId = a.EmpId,
                              Status = a.Status,
                              EmpName = b.EmpFName + " " + b.EmpLName,
                              Designation = c.JobName,
                              PdfURL = a.PdfURL


                          })
                   .ToListAsync();
        }

        public async Task<bool> ApproveLeave(int id, String name)
        {
            String content = "The leave request below has been approved";
            await EmailSend(id, content);
            var leave = await _context.LeaveApply.FindAsync(id);
            leave.Status = "Approved";
            leave.ApproverName = name;

            _context.Entry(leave).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            DateTime Start = (DateTime)leave.Startdate;

            for (int i = 0; i < leave.NoOfDays; i++)
            {
                var info = _context.MarkAttendence.SingleOrDefault(x => (x.EmpId == leave.EmpId) && (x.Date == Start.Date.AddDays(i)));
                if (info == null)
                {
                    var markAttendence = new MarkAttendence()
                    {
                        Date = Start.AddDays(i),
                        EmpId = leave.EmpId,
                        InTime = null,
                        OutTime = null,
                        TotalHours = 0,
                        Type = null,
                        Status = "Leave On Day"
                    };

                    await _context.MarkAttendence.AddAsync(markAttendence);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    info.InTime = null;
                    info.OutTime = null;
                    info.TotalHours = 0;
                    info.Type = null;
                    info.Status = "Leave On Day";

                    _context.Entry(info).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            return true;
        }

        public async Task<bool> NotApproveLeave(int id, String name)
        {
            String content = "Sorry! The leave request below has been not approved";
            await EmailSend(id, content);
            var leave = await _context.LeaveApply.FindAsync(id);
            leave.Status = "Not Approved";
            leave.ApproverName = name;

            _context.Entry(leave).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<List<LeaveApplyModel>> GetLeaveApprove()
        {
            return await (from a in _context.LeaveApply.Where(x => x.Status == "Waiting for Approval")
                          join b in _context.Employees on a.EmpId equals b.EmpId
                          join c in _context.Job on b.JobTitleId equals c.JobId
                          select new LeaveApplyModel()
                          {
                              LeaveApplyId = a.LeaveApplyId,
                              LeaveType = a.LeaveType,
                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              Reason = a.Reason,
                              AppliedDate = (DateTime)a.AppliedDate,
                              NoOfDays = a.NoOfDays,
                              EmpId = a.EmpId,
                              Status = a.Status,
                              EmpName = b.EmpFName + " " + b.EmpLName,
                              Designation = c.JobName,
                              RecommendName = a.RecommendName,
                              PdfURL = a.PdfURL


                          })
                   .ToListAsync();
        }

        public async Task<LeaveApplyModel> GetOneLeaveById(int id)
        {
            return await (from a in _context.LeaveApply.Where(x => x.LeaveApplyId == id)
                          select new LeaveApplyModel()
                          {
                              LeaveApplyId = a.LeaveApplyId,
                              LeaveType = a.LeaveType,
                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              Reason = a.Reason,
                              AppliedDate = (DateTime)a.AppliedDate,
                              NoOfDays = a.NoOfDays,
                              EmpId = a.EmpId,
                              Status = a.Status,
                              PdfURL = a.PdfURL
                          })
                  .FirstOrDefaultAsync();

        }


        public async Task<LeaveApplyModel> GetEmpLeaveJoinById(int id)
        {
            return await (from a in _context.LeaveApply.Where(x => x.LeaveApplyId == id)
                          join b in _context.Employees on a.EmpId equals b.EmpId
                          select new LeaveApplyModel()
                          {
                              LeaveApplyId = a.LeaveApplyId,
                              LeaveType = a.LeaveType,
                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              Reason = a.Reason,
                              AppliedDate = (DateTime)a.AppliedDate,
                              NoOfDays = a.NoOfDays,
                              EmpId = a.EmpId,
                              FromDate = (DateTime)b.FromDate,
                              ToDate = (DateTime)b.Todate

                          })
                  .FirstOrDefaultAsync();

        }

        public async Task<bool> UpdateLeave(LeaveApplyModel leaveApplyModel)
        {
            var leave = await _context.LeaveApply.FindAsync(leaveApplyModel.LeaveApplyId);

            leave.LeaveType = leaveApplyModel.LeaveType;
            leave.Startdate = leaveApplyModel.Startdate;
            leave.EndDate = leaveApplyModel.EndDate;
            leave.Reason = leaveApplyModel.Reason;
            leave.NoOfDays = leaveApplyModel.NoOfDays;
            leave.PdfURL = leaveApplyModel.PdfURL;

            _context.Entry(leave).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<bool> DeleteLeave(int id)
        {

            var leave = await _context.LeaveApply.FindAsync(id);
            _context.Entry(leave).State = EntityState.Deleted;
            await _context.SaveChangesAsync();

            return true;

        }

        public bool CheckExist(int id, DateTime date)
        {
            bool result = _context.LeaveApply.ToList().Exists(x => (x.EmpId == id) && (x.Startdate <= date) && (x.EndDate > date));
            return result;
        }


        public async Task<bool> ClearLeave(int id)
        {

            var leave = await _context.LeaveApply.FindAsync(id);

            leave.Visible = "Hide";

            _context.Entry(leave).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;

        }
        public async Task<List<LeaveApplyModel>> GetLeaveAsync(int id)
        {
            return await (from a in _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Startdate > DateTime.UtcNow) && (x.Visible == "Show"))
                          select new LeaveApplyModel()
                          {
                              EmpId = id,
                              LeaveType = a.LeaveType,
                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              NoOfDays = a.NoOfDays,
                              Status = a.Status
                          }).ToListAsync();
        }



        public async Task<List<LeaveApplyModel>> TodayLeaveAsync()
        {
            return await (from a in _context.LeaveApply.Where(x => (x.Startdate <= DateTime.UtcNow) && (x.EndDate > DateTime.UtcNow) && (x.Status == "Approved"))
                          join b in _context.Employees on a.EmpId equals b.EmpId
                          select new LeaveApplyModel()
                          {

                              EmpName = b.EmpFName + " " + b.EmpLName,
                              PhotoURL = b.ProfilePhoto,
                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              NoOfDays = a.NoOfDays,
                          }).ToListAsync();
        }

        public async Task<List<LeaveApplyModel>> UpcomingLeaveAsync()
        {
            return await (from a in _context.LeaveApply.Where(x => (x.Startdate > DateTime.UtcNow) && (x.Status == "Approved") && (x.Startdate < DateTime.UtcNow.AddDays(7)))
                          join b in _context.Employees on a.EmpId equals b.EmpId
                          select new LeaveApplyModel()
                          {

                              EmpName = b.EmpFName + " " + b.EmpLName,
                              PhotoURL = b.ProfilePhoto,
                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              NoOfDays = a.NoOfDays,
                          }).ToListAsync();
        }

        public async Task<List<LeaveApplyModel>> GetLeaveById(int id)
        {
            return await (from a in _context.LeaveApply.Where(x => (x.EmpId == id))
                          select new LeaveApplyModel()
                          {
                              LeaveApplyId = a.LeaveApplyId,
                              LeaveType = a.LeaveType,
                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              Reason = a.Reason,
                              AppliedDate = (DateTime)a.AppliedDate,
                              NoOfDays = a.NoOfDays,
                              EmpId = a.EmpId,
                              Status = a.Status,
                              RecommendName = a.RecommendName,
                              ApproverName = a.ApproverName,

                          })
                   .ToListAsync();
        }

        public async Task<List<LeaveApplyModel>> GetLeaveTakenAsync(int id)
        {
            return await (from a in _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved"))
                          select new LeaveApplyModel()
                          {

                              LeaveApplyId = a.LeaveApplyId,
                              LeaveType = a.LeaveType,
                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              Reason = a.Reason,
                              AppliedDate = (DateTime)a.AppliedDate,
                              NoOfDays = a.NoOfDays,
                              EmpId = a.EmpId,
                              RecommendName = a.RecommendName,
                              ApproverName = a.ApproverName,
                              PdfURL = a.PdfURL

                          })
                   .ToListAsync();
        }

        public async Task<List<KeyModel>> SearchLeave1(int id)
        {
            return await (from a in _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved"))
                          join b in _context.Employees on a.EmpId equals b.EmpId
                          select new KeyModel()
                          {
                              LeaveApplyId = a.LeaveApplyId,
                              EmpId = a.EmpId,
                              EmpFName = b.EmpFName,
                              EmpFullName = b.EmpFName + " " + b.EmpLName,
                              PhotoURL = b.ProfilePhoto,
                              FromDate = (DateTime)a.Startdate,
                              ToDate = (DateTime)a.EndDate,


                          })
                   .ToListAsync();
        }

        public async Task<List<KeyModel>> SearchLeave2(int id, DateTime date)
        {
            return await (from a in _context.LeaveApply.Where(x => (x.EmpId == id) && ((x.Status == "Approved") && (x.Startdate <= date.Date) && (x.EndDate > date.Date)))
                          join b in _context.Employees on a.EmpId equals b.EmpId
                          select new KeyModel()
                          {
                              LeaveApplyId = a.LeaveApplyId,
                              EmpId = a.EmpId,
                              EmpFName = b.EmpFName,
                              EmpFullName = b.EmpFName + " " + b.EmpLName,
                              PhotoURL = b.ProfilePhoto,
                              FromDate = (DateTime)a.Startdate,
                              ToDate = (DateTime)a.EndDate,


                          })
                   .ToListAsync();
        }

        public async Task<List<KeyModel>> SearchLeave3(DateTime date)
        {
            return await (from a in _context.LeaveApply.Where(x => (x.Status == "Approved") && (x.Startdate <= date.Date) && (x.EndDate > date.Date))
                          join b in _context.Employees on a.EmpId equals b.EmpId
                          select new KeyModel()
                          {
                              LeaveApplyId = a.LeaveApplyId,
                              EmpId = a.EmpId,
                              EmpFName = b.EmpFName,
                              EmpFullName = b.EmpFName + " " + b.EmpLName,
                              PhotoURL = b.ProfilePhoto,
                              FromDate = (DateTime)a.Startdate,
                              ToDate = (DateTime)a.EndDate,
                          })
                   .ToListAsync();
        }

        public async Task<LeaveApplyModel> GetSearchLeaveAsync(int id)
        {
            return await (from a in _context.LeaveApply.Where(x => x.LeaveApplyId == id)
                          join b in _context.Employees on a.EmpId equals b.EmpId
                          select new LeaveApplyModel()
                          {
                              LeaveApplyId = a.LeaveApplyId,
                              LeaveType = a.LeaveType,
                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              Reason = a.Reason,
                              AppliedDate = (DateTime)a.AppliedDate,
                              NoOfDays = a.NoOfDays,
                              EmpId = a.EmpId,
                              EmpName = b.EmpFName + " " + b.EmpLName,
                              ApproverName = a.ApproverName,
                              RecommendName = a.RecommendName,
                              PdfURL = a.PdfURL
                          })
                  .FirstOrDefaultAsync();

        }

        public async Task<RemainLeaveModel> LeaveBalanceGetAsync(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
                return null;
            int medicalTaken = _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "Medical Leave")).Select(x => x.NoOfDays).Sum();
            int annualTaken = _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "Annual Leave")).Select(x => x.NoOfDays).Sum();
            int casualTaken = _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "Casual Leave")).Select(x => x.NoOfDays).Sum();
            int shortTaken = _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "Short Leave")).Select(x => x.NoOfDays).Sum();
            int halfTaken = _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "Half Days")).Select(x => x.NoOfDays).Sum();

            return await (from a in _context.LeaveApply
                          select new RemainLeaveModel()
                          {
                              EmpId = id,
                              MedicalRemain = emp.MedicalAllocated - medicalTaken,
                              CasualRemain = emp.CasualAllocated - casualTaken,
                              AnnualRemain = emp.AnnualAllocated - annualTaken,
                              HalfRemain = emp.HalfLeaveAllocated - halfTaken,
                              ShortRemain = emp.ShortLeaveAllocated - shortTaken
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<LeaveApplyModel>> GetSpecialLeaveById(int id)
        {
            return await (from a in _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "Special Leave"))
                          select new LeaveApplyModel()
                          {

                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              AppliedDate = (DateTime)a.AppliedDate,
                              Reason = a.Reason,
                              NoOfDays = a.NoOfDays,

                          })
                   .ToListAsync();
        }

        public async Task<List<LeaveApplyModel>> GetNoPayLeaveById(int id)
        {
            return await (from a in _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "No Pay Leave"))
                          select new LeaveApplyModel()
                          {

                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              AppliedDate = (DateTime)a.AppliedDate,
                              Reason = a.Reason,
                              NoOfDays = a.NoOfDays,

                          })
                   .ToListAsync();
        }

        public async Task<List<LeaveApplyModel>> SpecialLeavePeriod(int id, DateTime start_Date, DateTime end_Date)
        {
            return await (from a in _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "Special Leave") && (x.Startdate >= start_Date.Date) && (x.Startdate < end_Date.Date))
                          select new LeaveApplyModel()
                          {

                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              AppliedDate = (DateTime)a.AppliedDate,
                              Reason = a.Reason,
                              NoOfDays = a.NoOfDays,

                          })
                   .ToListAsync();
        }

        public async Task<List<LeaveApplyModel>> NoPayLeavePeriod(int id, DateTime start_Date, DateTime end_Date)
        {
            return await (from a in _context.LeaveApply.Where(x => (x.EmpId == id) && (x.Status == "Approved") && (x.LeaveType == "No Pay Leave") && (x.Startdate >= start_Date.Date) && (x.Startdate < end_Date.Date))
                          select new LeaveApplyModel()
                          {

                              Startdate = (DateTime)a.Startdate,
                              EndDate = (DateTime)a.EndDate,
                              AppliedDate = (DateTime)a.AppliedDate,
                              Reason = a.Reason,
                              NoOfDays = a.NoOfDays,

                          })
                   .ToListAsync();
        }

        public int leaveRecomCount()
        {
            int count = _context.LeaveApply.Where(x => (x.Status == "Waiting for Recommendation")).Select(x => x.LeaveApplyId).Count();
            return count;

        }

        public int leaveAppCount()
        {
            int count = _context.LeaveApply.Where(x => (x.Status == "Waiting for Approval")).Select(x => x.LeaveApplyId).Count();
            return count;

        }

        public async Task<bool> EmailSend(int id, String content)
        {
            var leave = await _context.LeaveApply.FindAsync(id);
            var emp = await _context.Employees.FindAsync(leave.EmpId);
            string to = emp.Email;
            String subject = "Regarding Your Leave Request";

            MailMessage mm = new MailMessage();
            mm.To.Add(to);
            mm.Subject = subject;
            mm.From = new MailAddress("bellvantagecom737@gmail.com");
            mm.Body += " Hi " + emp.EmpFName + ",<br>";
            mm.Body += " " + content + "<br> ";
            mm.Body += " <html>";
            mm.Body += "<body>";
            mm.Body += "<table style='border: 1px solid black'>";
            mm.Body += "<tr>";
            mm.Body += "<td><b>Leave Type</b> :  </td><td>" + leave.LeaveType + "</td>";
            mm.Body += "</tr>";
            mm.Body += "<tr>";
            mm.Body += "<td><b>Reason</b> :  </td><td>" + leave.Reason + "</td>";
            mm.Body += "</tr>";
            mm.Body += "<tr>";
            mm.Body += "<td><b>No Of days</b> :  </td><td>" + leave.NoOfDays + "</td>";
            mm.Body += "</tr>";
            mm.Body += "<tr>";
            mm.Body += "<td><b>From </b>:  </td><td>" + leave.Startdate + "</td>";
            mm.Body += "</tr>";
            mm.Body += "<tr>";
            mm.Body += "<td><b>To</b> :  </td><td>" + leave.EndDate + "</td>";
            mm.Body += "</tr>";
            mm.Body += "</table>";
            mm.Body += "</body>";
            mm.Body += "</html>";
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;

            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("bellvantagecom737@gmail.com", "Bell@1234");
            smtp.EnableSsl = true;
            smtp.Send(mm);
            return true;
        }




    }
}
