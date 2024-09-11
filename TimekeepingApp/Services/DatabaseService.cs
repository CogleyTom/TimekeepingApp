using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimekeepingApp.Models;
using TimekeepingApp.Data;

namespace TimekeepingApp.Services
{
    public class DatabaseService
    {
        private readonly AppDbContext _context;

        public DatabaseService(AppDbContext context)
        {
            _context = context;
        }

        // Add a new timesheet
        public async Task AddTimesheetAsync(TimesheetModel timesheet)
        {
            try
            {
                // Ensure timesheet is valid and not null before adding
                if (timesheet == null) throw new ArgumentNullException(nameof(timesheet));

                _context.Timesheets.Add(timesheet);
                await _context.SaveChangesAsync(); // Save changes to persist the timesheet in the database
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding timesheet: {ex.Message}");
                // Handle or log the exception as needed
            }
        }

        // Update timesheet status
        public async Task UpdateTimesheetStatusAsync(int timesheetId, string status)
        {
            try
            {
                var timesheet = await _context.Timesheets.FindAsync(timesheetId);
                if (timesheet != null)
                {
                    timesheet.Status = status;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine($"Timesheet with ID {timesheetId} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating timesheet status: {ex.Message}");
            }
        }

        public async Task DeleteTimesheetAsync(int timesheetId)
        {
            try
            {
                var timesheet = await _context.Timesheets.FindAsync(timesheetId);
                if (timesheet != null)
                {
                    _context.Timesheets.Remove(timesheet);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting timesheet: {ex.Message}");
            }
        }

        // Retrieve all timesheets from the database
        public async Task<List<TimesheetModel>> GetAllTimesheetsAsync()
        {
            try
            {
                // Retrieve all timesheets including related user data if needed
                return await _context.Timesheets
                    .Include(t => t.User) // Ensure User data is included if needed for display
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving timesheets: {ex.Message}");
                return new List<TimesheetModel>(); // Return an empty list if an error occurs
            }
        }

        // Retrieve timesheets by user ID
        public async Task<List<TimesheetModel>> GetTimesheetsByUserIdAsync(int userId)
        {
            try
            {
                // Query the database for timesheets associated with the given user ID
                return await _context.Timesheets
                    .Where(t => t.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle exceptions and log or notify the error
                Console.WriteLine($"Error retrieving timesheets for user ID {userId}: {ex.Message}");
                return new List<TimesheetModel>(); // Return an empty list if an error occurs
            }
        }

        // Retrieve all users from the database
        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving users: {ex.Message}");
                return new List<UserModel>();
            }
        }
    }
}
