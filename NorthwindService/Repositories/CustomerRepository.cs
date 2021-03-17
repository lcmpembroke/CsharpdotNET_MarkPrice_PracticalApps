using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Packt.Shared;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace NorthwindService.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        // thread-safe dictionary to cache the Customers - it's a small repo here, but in real life, use a distributed cache like Redis...high performance to take load off database server
        private static ConcurrentDictionary<string, Customer> customersCache;
        
        // use an instance data context field as should not be cached due to their internal caching
        private readonly Northwind _db;

        public CustomerRepository(Northwind injectedDbContext)
        {
            this._db = injectedDbContext;

            // pre-load customers from database as normal Dictionary (CustomerID as key), then convert to thread-safe ConcurrentDictionary
            if (customersCache == null)
            {
                customersCache = new ConcurrentDictionary<string, Customer>(_db.Customers.ToDictionary(c => c.CustomerID));
            }
        }



        public async Task<Customer> CreatAsync(Customer c)
        {
            // normalize CustomerID to uppercase
            c.CustomerID = c.CustomerID.ToUpper();

            // add customer to database using EF Core
            EntityEntry<Customer> addedCustomer = await _db.Customers.AddAsync(c);
            int affected = await _db.SaveChangesAsync();

            if (affected == 1)
            {
                // if customer is new, add it to cache
                return customersCache.AddOrUpdate(c.CustomerID, c, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        private Customer UpdateCache(string id, Customer c)
        {
            if (customersCache.TryGetValue(id, out Customer oldCust))
            {
                // customer was in the cache, so ensure the current cust is same as oldCust  before updating to the new one...
                if (customersCache.TryUpdate(id, c, oldCust))
                {
                    return c;
                }
            }
            return null;

        }

        public Task<IEnumerable<Customer>> RetrieveAllAsync()
        {
            // get from cache for performance
            return Task.Run<IEnumerable<Customer>>(() => customersCache.Values);
        }

        public Task<Customer> RetrieveAsync(string id)
        {
            // get from cache for performance
            return Task.Run(() => {
                customersCache.TryGetValue(id.ToUpper(), out Customer c);
                return c;
            });
        }

        public async Task<Customer> UpdateAsync(string id, Customer c)
        {
            // update in databse
            c.CustomerID = c.CustomerID.ToUpper();
            _db.Customers.Update(c);
            int affected = await _db.SaveChangesAsync();

            if (affected == 1)
            {
                // update in cache
                return UpdateCache(id.ToUpper(), c);
            }
            return null;
        }

        public async Task<bool?> DeleteAsync(string id)
        {
            // remove from databse
            Customer c = _db.Customers.Find(id.ToUpper());
            _db.Customers.Remove(c);
            int affected = await _db.SaveChangesAsync();

            if (affected == 1)
            {
                // remove from cache
                return customersCache.TryRemove(id.ToUpper(), out c);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
            Customer c = await _db.Customers.FindAsync(id.ToUpper());

            if (c == null)
            {
                // remove from cache
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
