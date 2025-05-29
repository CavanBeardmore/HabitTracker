using HabitTracker.Server.Repository;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Exceptions;
using System.Text.Json;

namespace HabitTracker.Server.Services
{
    public class HabitService : IHabitService
    {
        private readonly IHabitRepository _habitRepository;
        private readonly ILogger<HabitService> _logger;

        public HabitService(ILogger<HabitService> logger, IHabitRepository habitRepository)
        {
            _logger = logger;
            _habitRepository = habitRepository;
        }

        public Habit? GetById(int habitId, int userId)
        {
            try
            {
                _logger.LogInformation("HabitService - GetById - getting habit by id");
                Habit? habit = _habitRepository.GetById(habitId, userId, null, null);

                if (habit == null)
                {
                    throw new NotFoundException($"Habit not found of id - {habitId} for - {userId}");
                }

                _logger.LogInformation("HabitService - GetById - got habit by id");
                return habit;
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch
            {
                throw new AppException($"An error occured when getting habit of - {habitId} for {userId}");
            }
        }

        public IReadOnlyCollection<Habit?> GetAllByUserId(int userId)
        {
            try
            {
                _logger.LogInformation("HabitService - GetAllByUserId - getting all habits by user id");
                IReadOnlyCollection<Habit?> habits = _habitRepository.GetAllByUserId(userId);

                if (habits.Count == 0)
                {
                    throw new NotFoundException($"No habits found for - {userId}");
                }

                _logger.LogInformation("HabitService - GetAllByUserId - got all habits by user id");

                return habits;
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch
            {
                throw new AppException($"An error occured when getting habits for {userId}");
            }
        }

        public Habit? Add(int userId, PostHabit habit)
        {
            try
            {
                _logger.LogInformation("HabitService - Add - adding habit");
                Habit? createdHabit = _habitRepository.Add(userId, habit);

                if (habit == null)
                {
                    throw new AppException($"Failed to create habit with data - {JsonSerializer.Serialize(habit)} for - {userId}");
                }

                _logger.LogInformation("HabitService - Add - returning created habit");
                return createdHabit;
            }
            catch (AppException ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public Habit? Update(int userId, PatchHabit habit)
        {
            try
            {
                _logger.LogInformation("HabitService - Update - updating habit");
                bool success = _habitRepository.Update(userId, habit, null, null);

                if (!success)
                {
                    throw new AppException($"Failed to update habit with data for - {userId}");
                }

                return _habitRepository.GetById(habit.Id, userId, null, null);
            }
            catch (AppException ex)
            {
                throw new AppException(ex.Message);
            }
            catch
            {
                throw new AppException($"An error occured when updating habit {JsonSerializer.Serialize(habit)} for {userId}");
            }
        }

        public bool Delete(int habitId, int userId)
        {
            try
            {
                _logger.LogInformation("HabitService - Delete - deleting habit");
                return _habitRepository.Delete(habitId, userId);
            }
            catch 
            {
                throw new AppException($"An error occured when deleting habit of id {habitId} for {userId}");
            }
        }
    }
}
