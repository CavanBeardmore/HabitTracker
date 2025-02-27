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

        public HabitService(IHabitRepository habitRepository)
        {
            _habitRepository = habitRepository;
        }

        public Habit? GetById(int habitId, int userId)
        {
            try
            {
                Habit? habit = _habitRepository.GetById(habitId, userId);

                if (habit == null)
                {
                    throw new NotFoundException($"Habit not found of id - {habitId} for - {userId}");
                }

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
                IReadOnlyCollection<Habit?> habits = _habitRepository.GetAllByUserId(userId);

                if (habits.Count == 0)
                {
                    throw new NotFoundException($"No habits found for - {userId}");
                }

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
                Habit? createdHabit = _habitRepository.Add(userId, habit);

                if (habit == null)
                {
                    throw new NotFoundException($"Failed to create habit with data - {JsonSerializer.Serialize(habit)} for - {userId}");
                }

                return createdHabit;
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch
            {
                throw new AppException($"An error occured when adding habit data {JsonSerializer.Serialize(habit)} for {userId}");
            }
        }

        public bool Update(int userId, PatchHabit habit)
        {
            try
            {
                return _habitRepository.Update(userId, habit);
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
                return _habitRepository.Delete(habitId, userId);
            }
            catch 
            {
                throw new AppException($"An error occured when deleting habit of id {habitId} for {userId}");
            }
        }
    }
}
