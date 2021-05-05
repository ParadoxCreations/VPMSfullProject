﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VPMS_Project.Data;

namespace VPMS_Project.Models
{
    public class Repo3
    {
        private readonly EmpStoreContext _context = null;

        public Repo3(EmpStoreContext context)
        {
            _context = context;
        }

        public async Task<int> AddCustomer(Customer customer)
        {
            var NewCustomer = new Customers
            {
                Name = customer.Name,
                Address = customer.Address,
                ContactNo = customer.ContactNo
            };

            await _context.Customers.AddAsync(NewCustomer);
            await _context.SaveChangesAsync();

            return NewCustomer.Id;
        }

        public async Task<List<Customer>> GetCustomers(int x)
        {
            if (x != 0)
            {
            var data = await _context.Customers.Select(x => new Customer()
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                ContactNo = x.ContactNo
            }).OrderByDescending(x => x.Id).Take(x).ToListAsync();
             return data;
            }
            else
            {
                var data = await _context.Customers.Select(x => new Customer()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    ContactNo = x.ContactNo
                }).OrderByDescending(x => x.Id).ToListAsync();
                return data;
            }
            
        }

        public async Task<String> GetCustomerNameById(int? Id)
        {
            var data = await _context.Customers.FindAsync(Id);

            return data.Name;
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer != null)
            {
                var details = new Customer()
                {
                    Id = customer.Id,
                    ContactNo = customer.ContactNo,
                    Name = customer.Name,
                    Address =customer.Address
                };
                return details;
            }
            return null;
        }


        public async Task<bool> EditCustomer(Customer customers)
        {
            var cus = await _context.Customers.FindAsync(customers.Id);

            cus.Name = customers.Name;
            cus.Address = customers.Address;
            cus.ContactNo = customers.ContactNo;
       

            _context.Entry(cus).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;

        }


        public async Task<bool> DeleteCustomer(int id)
        {
            var customers = await _context.Customers.FindAsync(id);

            _context.Customers.Remove(customers);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> AddToDeletedCustomers(int id)
        {
            var customers = await _context.Customers.FindAsync(id);
          

            var NewCustomer = new DeletedCustomers
            {
                Address = customers.Address,
                Name = customers.Name,
                ContactNo = customers.ContactNo
                
            };
            await _context.DeletedCustomers.AddAsync(NewCustomer);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
